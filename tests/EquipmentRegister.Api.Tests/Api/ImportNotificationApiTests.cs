using System.Net;
using System.Text;
using EquipmentRegister.Api.Import;
using EquipmentRegister.Api.Notifications;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EquipmentRegister.Api.Tests.Api;

public sealed class ImportNotificationApiTests
{
    [Fact]
    public async Task Notifies_when_an_import_is_complete()
    {
        var notifier = new RecordingNotifier();
        await using var factory = CreateFactory(notifier);
        using var client = factory.CreateClient();

        using var response = await client.PostAsync(
            "/api/imports",
            CsvUpload("Tag,Description,Discipline,Area,PowerKw,WeightKg,Status\nPU-5001,Email test pump,Process,500,10,20,Planned\n"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(notifier.Result);
        Assert.Equal(1, notifier.Result.Accepted);
        Assert.Equal(0, notifier.Result.Rejected);
    }

    [Fact]
    public async Task Keeps_import_successful_when_notification_delivery_fails()
    {
        var notifier = new RecordingNotifier { Failure = new InvalidOperationException("SMTP unavailable") };
        await using var factory = CreateFactory(notifier);
        using var client = factory.CreateClient();

        using var response = await client.PostAsync(
            "/api/imports",
            CsvUpload("Tag,Description,Discipline,Area,PowerKw,WeightKg,Status\nPU-5002,Email test pump,Process,500,10,20,Planned\n"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(notifier.Result);
        Assert.Equal(1, notifier.Result.Accepted);
    }

    private static WebApplicationFactory<Program> CreateFactory(RecordingNotifier notifier) =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IImportCompletionNotifier>();
                services.AddSingleton<IImportCompletionNotifier>(notifier);
            }));

    private static MultipartFormDataContent CsvUpload(string csv) =>
        new() { { new ByteArrayContent(Encoding.UTF8.GetBytes(csv)), "file", "import.csv" } };

    private sealed class RecordingNotifier : IImportCompletionNotifier
    {
        public ImportResult? Result { get; private set; }

        public Exception? Failure { get; init; }

        public Task NotifyAsync(ImportResult result, CancellationToken cancellationToken = default)
        {
            Result = result;
            return Failure is null ? Task.CompletedTask : Task.FromException(Failure);
        }
    }
}
