using EquipmentRegister.Api.Import;
using EquipmentRegister.Api.Import.Rules;
using EquipmentRegister.Api.Reports;
using Microsoft.Extensions.Options;

namespace EquipmentRegister.Api.Tests.Import.Rules;

public class AreaRuleTests
{
    [Fact]
    public void Rejects_areas_not_in_the_project_using_configured_order()
    {
        var rule = CreateRule(new("300", "Flotation"), new("100", "Crushing"));
        var row = Rows.Row(rowNumber: 7, area: " 200 ");

        var error = Assert.Single(rule.Validate(row, Rows.Context(row)));

        Assert.Equal("R007", error.RuleId);
        Assert.Equal(ImportColumns.Area, error.Column);
        Assert.Equal(7, error.RowNumber);
        Assert.Equal("Area '200' is not an area of this project. Use one of: 300, 100.", error.Message);
    }

    [Fact]
    public void Accepts_a_configured_area_after_trimming()
    {
        var rule = CreateRule(new("100", "Crushing"));
        var row = Rows.Row(area: " 100 ");

        Assert.Empty(rule.Validate(row, Rows.Context(row)));
    }

    [Fact]
    public void Leaves_a_blank_area_to_the_required_fields_rule()
    {
        var rule = CreateRule(new("100", "Crushing"));
        var row = Rows.Row(area: " ");

        Assert.Empty(rule.Validate(row, Rows.Context(row)));
    }

    [Fact]
    public void Does_not_report_areas_when_none_are_configured()
    {
        var rule = CreateRule();
        var row = Rows.Row(area: "200");

        Assert.Empty(rule.Validate(row, Rows.Context(row)));
    }

    private static AreaRule CreateRule(params ProjectArea[] areas) =>
        new(Options.Create(new ProjectOptions { Areas = [.. areas] }));
}
