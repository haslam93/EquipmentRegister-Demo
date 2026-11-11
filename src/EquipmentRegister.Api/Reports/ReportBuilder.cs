using System.Globalization;
using System.Net;
using System.Text;
using EquipmentRegister.Api.Data;
using EquipmentRegister.Api.Domain;
using Microsoft.Extensions.Options;

namespace EquipmentRegister.Api.Reports;

public sealed class ProjectOptions
{
    public string Name { get; set; } = "Unnamed project";

    public string Number { get; set; } = "N/A";
}

/// <summary>
/// Fills the HTML report templates in Reports/Templates. Templates must follow
/// docs/report-style-guide.md; `node tools/check-report-style.mjs` enforces it.
/// </summary>
public sealed class ReportBuilder(
    IEquipmentRepository repository,
    IWebHostEnvironment environment,
    IOptions<ProjectOptions> project,
    TimeProvider timeProvider)
{
    public async Task<string> BuildSummaryAsync(CancellationToken cancellationToken = default)
    {
        var items = await repository.GetAllAsync(cancellationToken);

        var disciplineRows = items
            .GroupBy(item => item.Discipline)
            .OrderBy(group => group.Key, StringComparer.Ordinal)
            .Select(group => Row(
                Text(group.Key),
                Number(group.Count()),
                Number(group.Sum(item => item.PowerKw ?? 0)),
                Number(group.Sum(item => item.WeightKg ?? 0))));

        var statusRows = EquipmentStatuses.All
            .Select(status => Row(Text(status), Number(items.Count(item => item.Status == status))));

        return await FillAsync("equipment-summary.html", cancellationToken, new()
        {
            ["totalCount"] = items.Count.ToString(CultureInfo.InvariantCulture),
            ["disciplineRows"] = string.Concat(disciplineRows),
            ["statusRows"] = string.Concat(statusRows),
        });
    }

    public async Task<string> BuildAreaLoadListAsync(CancellationToken cancellationToken = default)
    {
        var items = await repository.GetAllAsync(cancellationToken);

        var areaRows = items
            .GroupBy(item => item.Area)
            .OrderBy(group => group.Key, StringComparer.Ordinal)
            .Select(group => Row(
                Text(group.Key),
                Number(group.Count()),
                Number(group.Sum(item => item.PowerKw ?? 0))));

        return await FillAsync("area-load-list.html", cancellationToken, new()
        {
            ["areaRows"] = string.Concat(areaRows),
            ["totalPowerKw"] = items.Sum(item => item.PowerKw ?? 0).ToString("N0", CultureInfo.InvariantCulture),
        });
    }

    private async Task<string> FillAsync(string templateName, CancellationToken cancellationToken, Dictionary<string, string> values)
    {
        var path = Path.Combine(environment.ContentRootPath, "Reports", "Templates", templateName);
        var html = new StringBuilder(await File.ReadAllTextAsync(path, cancellationToken));

        values["projectName"] = WebUtility.HtmlEncode(project.Value.Name);
        values["projectNumber"] = WebUtility.HtmlEncode(project.Value.Number);
        values["generatedOn"] = timeProvider.GetUtcNow().ToString("yyyy-MM-dd HH:mm 'UTC'", CultureInfo.InvariantCulture);

        foreach (var (key, value) in values)
        {
            html.Replace("{{" + key + "}}", value);
        }

        return html.ToString();
    }

    private static string Row(params string[] cells) => $"<tr>{string.Concat(cells)}</tr>\n";

    private static string Text(string value) => $"<td>{WebUtility.HtmlEncode(value)}</td>";

    private static string Number(decimal value) =>
        $"<td class=\"number\">{value.ToString("N0", CultureInfo.InvariantCulture)}</td>";
}
