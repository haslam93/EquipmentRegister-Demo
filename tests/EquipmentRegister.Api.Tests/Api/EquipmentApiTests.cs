using System.Net;
using System.Net.Http.Json;
using System.Text;
using EquipmentRegister.Api.Domain;
using EquipmentRegister.Api.Import;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EquipmentRegister.Api.Tests.Api;

public class EquipmentApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Lists_the_register_sorted_by_tag()
    {
        var items = await _client.GetFromJsonAsync<List<EquipmentItem>>("/api/equipment");

        Assert.NotNull(items);
        Assert.Contains(items, item => item.Tag == "ML-2001");
        Assert.Equal(items.Select(item => item.Tag).Order(StringComparer.OrdinalIgnoreCase), items.Select(item => item.Tag));
    }

    [Fact]
    public async Task Filters_by_discipline_ignoring_case()
    {
        var items = await _client.GetFromJsonAsync<List<EquipmentItem>>("/api/equipment?discipline=electrical");

        Assert.NotNull(items);
        Assert.NotEmpty(items);
        Assert.All(items, item => Assert.Equal("Electrical", item.Discipline));
    }

    [Fact]
    public async Task Returns_the_configured_project_areas()
    {
        using var response = await _client.GetAsync("/api/project/areas");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = await response.Content.ReadAsStreamAsync();
        using var document = await System.Text.Json.JsonDocument.ParseAsync(json);
        var areas = document.RootElement.EnumerateArray().ToArray();

        Assert.Equal(5, areas.Length);
        Assert.Equal(("100", "Crushing"), (areas[0].GetProperty("code").GetString(), areas[0].GetProperty("name").GetString()));
        Assert.Equal(("200", "Grinding"), (areas[1].GetProperty("code").GetString(), areas[1].GetProperty("name").GetString()));
        Assert.Equal(("300", "Flotation"), (areas[2].GetProperty("code").GetString(), areas[2].GetProperty("name").GetString()));
        Assert.Equal(("400", "Tailings"), (areas[3].GetProperty("code").GetString(), areas[3].GetProperty("name").GetString()));
        Assert.Equal(("500", "Utilities"), (areas[4].GetProperty("code").GetString(), areas[4].GetProperty("name").GetString()));
    }

    [Fact]
    public async Task Imports_a_csv_file_and_returns_row_errors()
    {
        const string csv =
            "Tag,Description,Discipline,Area,PowerKw,WeightKg,Status\n" +
            "AG-9001,Test agitator,Mechanical,300,45,3200,Planned\n" +
            "bad tag,Broken row,Mechanical,300,,,\n";

        var response = await _client.PostAsync("/api/imports", CsvUpload(csv, "upload.csv"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ImportResult>();
        Assert.NotNull(result);
        Assert.Equal(1, result.Accepted);
        Assert.Equal(1, result.Rejected);
        Assert.Equal(3, Assert.Single(result.Errors).RowNumber);

        var imported = await _client.GetAsync("/api/equipment/AG-9001");
        Assert.Equal(HttpStatusCode.OK, imported.StatusCode);
    }

    [Fact]
    public async Task Refuses_files_that_are_not_csv()
    {
        var response = await _client.PostAsync("/api/imports", CsvUpload("Tag\nPU-1001", "upload.xlsx"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Renders_the_summary_report_as_html()
    {
        var response = await _client.GetAsync("/api/reports/summary");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("North Ridge Concentrator Expansion", html);
        Assert.DoesNotContain("{{", html);
    }

    private static MultipartFormDataContent CsvUpload(string csv, string fileName) =>
        new() { { new ByteArrayContent(Encoding.UTF8.GetBytes(csv)), "file", fileName } };
}
