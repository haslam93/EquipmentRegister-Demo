using EquipmentRegister.Api.Reports;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentRegister.Api.Controllers;

[ApiController]
[Route("api/reports")]
public sealed class ReportsController(ReportBuilder reports) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<ContentResult> Summary(CancellationToken cancellationToken) =>
        Content(await reports.BuildSummaryAsync(cancellationToken), "text/html");

    [HttpGet("area-load-list")]
    public async Task<ContentResult> AreaLoadList(CancellationToken cancellationToken) =>
        Content(await reports.BuildAreaLoadListAsync(cancellationToken), "text/html");
}
