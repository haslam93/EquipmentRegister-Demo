using EquipmentRegister.Api.Import;

namespace EquipmentRegister.Api.Tests.Import;

public class CsvParserTests
{
    [Fact]
    public void Splits_lines_and_fields()
    {
        var lines = CsvParser.Parse("Tag,Description\r\nPU-1001,Pump\r\n");

        Assert.Equal(2, lines.Count);
        Assert.Equal(["Tag", "Description"], lines[0].Fields);
        Assert.Equal(["PU-1001", "Pump"], lines[1].Fields);
    }

    [Fact]
    public void Keeps_commas_and_quotes_inside_quoted_fields()
    {
        var lines = CsvParser.Parse("Tag,Description\nPU-1001,\"Pump, 6\"\" discharge\"");

        Assert.Equal(["PU-1001", "Pump, 6\" discharge"], lines[1].Fields);
    }

    [Fact]
    public void Skips_blank_lines_but_keeps_spreadsheet_line_numbers()
    {
        var lines = CsvParser.Parse("Tag\nPU-1001\n\nPU-1002\n");

        Assert.Equal([1, 2, 4], lines.Select(line => line.LineNumber));
    }
}
