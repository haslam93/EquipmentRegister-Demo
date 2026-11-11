using EquipmentRegister.Api.Data;
using EquipmentRegister.Api.Import;
using EquipmentRegister.Api.Import.Rules;
using EquipmentRegister.Api.Reports;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.Configure<ProjectOptions>(builder.Configuration.GetSection("Project"));

builder.Services.AddSingleton<IEquipmentRepository>(_ => new InMemoryEquipmentRepository(SeedData.Items));
builder.Services.AddSingleton<ReportBuilder>();

// Import validation rules run in registration order. Add new rules here.
builder.Services.AddSingleton<IRowRule, RequiredFieldsRule>();
builder.Services.AddSingleton<IRowRule, TagFormatRule>();
builder.Services.AddSingleton<IRowRule, DisciplineRule>();
builder.Services.AddSingleton<IRowRule, NumericFieldsRule>();
builder.Services.AddSingleton<IRowRule, StatusRule>();
builder.Services.AddSingleton<EquipmentImportService>();

var app = builder.Build();

app.MapControllers();
app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

app.Run();

public partial class Program;
