using EquipmentRegister.Api.Data;
using EquipmentRegister.Api.Import;
using EquipmentRegister.Api.Import.Rules;
using EquipmentRegister.Api.Reports;
using Microsoft.Extensions.Options;

namespace EquipmentRegister.Api.Tests.Import;

public class EquipmentImportServiceTests
{
    private const string Header = "Tag,Description,Discipline,Area,PowerKw,WeightKg,Status\n";

    private readonly InMemoryEquipmentRepository _repository = new([]);
    private readonly EquipmentImportService _service;

    public EquipmentImportServiceTests()
    {
        var project = Options.Create(new ProjectOptions
        {
            Areas =
            [
                new("100", "Crushing"),
                new("200", "Grinding"),
                new("300", "Flotation"),
                new("400", "Tailings"),
                new("500", "Utilities"),
            ],
        });
        IRowRule[] rules =
        [
            new RequiredFieldsRule(),
            new TagFormatRule(),
            new DisciplineRule(),
            new NumericFieldsRule(),
            new StatusRule(),
            new AreaRule(project),
        ];
        _service = new EquipmentImportService(rules, _repository);
    }

    [Fact]
    public async Task Adds_valid_rows_to_the_register()
    {
        var csv = Header +
            "PU-1001A,Slurry pump,process,100,75.5,1250,ordered\n" +
            "TK-1002,Pump box,Process,100,,8900,\n";

        var result = await _service.ImportAsync("pumps.csv", csv);

        Assert.Equal(2, result.TotalRows);
        Assert.Equal(2, result.Accepted);
        Assert.Empty(result.Errors);

        var pump = await _repository.FindByTagAsync("PU-1001A");
        Assert.NotNull(pump);
        Assert.Equal("Process", pump.Discipline);
        Assert.Equal(75.5m, pump.PowerKw);
        Assert.Equal("Ordered", pump.Status);

        var tank = await _repository.FindByTagAsync("TK-1002");
        Assert.NotNull(tank);
        Assert.Null(tank.PowerKw);
        Assert.Equal("Planned", tank.Status);
    }

    [Fact]
    public async Task Rejects_invalid_rows_and_reports_their_spreadsheet_row_number()
    {
        var csv = Header +
            "PU-1001A,Slurry pump,Process,100,75,1250,Planned\n" +
            "pu-1002,Reagent pump,Chemical,100,\"1,5\",85,Planned\n" +
            "TK-1003,Pump box,Process,100,,8900,Planned\n";

        var result = await _service.ImportAsync("mixed.csv", csv);

        Assert.Equal(3, result.TotalRows);
        Assert.Equal(2, result.Accepted);
        Assert.Equal(1, result.Rejected);
        Assert.Equal(["R002", "R003", "R004"], result.Errors.Select(error => error.RuleId));
        Assert.All(result.Errors, error => Assert.Equal(3, error.RowNumber));
        Assert.Null(await _repository.FindByTagAsync("pu-1002"));
    }

    [Fact]
    public async Task Updates_an_item_that_is_already_in_the_register()
    {
        await _service.ImportAsync("first.csv", Header + "PU-1001A,Slurry pump,Process,100,75,1250,Planned\n");

        var result = await _service.ImportAsync("second.csv", Header + "PU-1001A,Slurry pump,Process,100,90,1300,Ordered\n");

        Assert.Equal(1, result.Accepted);
        var pump = await _repository.FindByTagAsync("PU-1001A");
        Assert.Equal(90m, pump!.PowerKw);
        Assert.Equal("Ordered", pump.Status);
    }

    [Fact]
    public async Task Rejects_the_whole_file_when_a_required_column_is_missing()
    {
        var result = await _service.ImportAsync("no-area.csv", "Tag,Description,Discipline\nPU-1001A,Slurry pump,Process\n");

        Assert.Equal(0, result.Accepted);
        Assert.Equal(1, result.Rejected);
        var error = Assert.Single(result.Errors);
        Assert.Equal(EquipmentImportService.FileRuleId, error.RuleId);
        Assert.Contains("Area", error.Message);
    }

    [Fact]
    public async Task Rejects_rows_with_an_area_not_in_the_project()
    {
        var result = await _service.ImportAsync(
            "unknown-area.csv",
            Header + "PU-1001A,Slurry pump,Process,900,75,1250,Planned\n");

        Assert.Equal(0, result.Accepted);
        Assert.Equal(1, result.Rejected);
        Assert.Equal(
            new RowError(2, "R007", "Area", "Area '900' is not an area of this project. Use one of: 100, 200, 300, 400, 500."),
            Assert.Single(result.Errors));
    }
}
