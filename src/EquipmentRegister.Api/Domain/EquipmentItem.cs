namespace EquipmentRegister.Api.Domain;

public sealed record EquipmentItem(
    string Tag,
    string Description,
    string Discipline,
    string Area,
    decimal? PowerKw,
    decimal? WeightKg,
    string Status);

public static class Disciplines
{
    public static readonly IReadOnlyList<string> All =
    [
        "Mechanical", "Electrical", "Process", "Piping", "Instrumentation", "Structural"
    ];
}

public static class EquipmentStatuses
{
    public const string Default = "Planned";

    public static readonly IReadOnlyList<string> All =
    [
        "Planned", "Ordered", "Delivered", "Installed"
    ];
}
