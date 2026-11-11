using System.Net.Http.Json;
using EquipmentRegister.Api.Domain;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EquipmentRegister.Api.Tests.Api;

public class ExportApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Returns_the_register_as_a_csv_download()
    {
        var response = await _client.GetAsync("/api/equipment/export");

        response.EnsureSuccessStatusCode();
        Assert.Equal("text/csv", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("equipment-register.csv", response.Content.Headers.ContentDisposition?.FileNameStar);
    }

    [Fact]
    public async Task Exports_a_header_and_one_row_for_every_item()
    {
        var items = await _client.GetFromJsonAsync<List<EquipmentItem>>("/api/equipment");

        var csv = await _client.GetStringAsync("/api/equipment/export");

        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        Assert.Equal("Tag,Description,Discipline,Area,PowerKw,WeightKg,Status", lines[0]);
        Assert.Equal(items!.Count + 1, lines.Length);
    }
}
