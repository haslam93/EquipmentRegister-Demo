using EquipmentRegister.Api.Domain;

namespace EquipmentRegister.Api.Data;

public static class SeedData
{
    public static readonly IReadOnlyList<EquipmentItem> Items =
    [
        new("CR-1001", "Primary gyratory crusher", "Mechanical", "100", 450m, 185000m, "Installed"),
        new("FE-1002", "Apron feeder", "Mechanical", "100", 75m, 42000m, "Installed"),
        new("CV-1010", "Crushed ore conveyor", "Mechanical", "100", 250m, 96000m, "Delivered"),
        new("ML-2001", "SAG mill", "Mechanical", "200", 12000m, 890000m, "Delivered"),
        new("ML-2002", "Ball mill", "Mechanical", "200", 8500m, 640000m, "Ordered"),
        new("PU-2010A", "Cyclone feed pump (duty)", "Process", "200", 900m, 14500m, "Ordered"),
        new("PU-2010B", "Cyclone feed pump (standby)", "Process", "200", 900m, 14500m, "Ordered"),
        new("FL-3001", "Rougher flotation cell bank", "Process", "300", 320m, 58000m, "Planned"),
        new("TH-4001", "Tailings thickener", "Process", "400", 55m, 210000m, "Planned"),
        new("TX-5001", "Main step-down transformer", "Electrical", "500", null, 74000m, "Ordered"),
        new("SG-5002", "13.8 kV switchgear line-up", "Electrical", "500", null, 21000m, "Planned"),
        new("FIT-3005", "Flotation air flow transmitter", "Instrumentation", "300", null, 12m, "Planned"),
    ];
}
