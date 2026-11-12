using EquipmentRegister.Api.Import;
using EquipmentRegister.Api.Import.Rules;

namespace EquipmentRegister.Api.Tests.Import.Rules;

public class DescriptionLengthRuleTests
{
    private readonly DescriptionLengthRule _rule = new();

    [Theory]
    [InlineData(121)]
    [InlineData(200)]
    public void Rejects_descriptions_over_120_characters_after_trimming(int length)
    {
        var row = Rows.Row(rowNumber: 7, description: $"  {new string('a', length)} \t");

        var error = Assert.Single(_rule.Validate(row, Rows.Context(row)));

        Assert.Equal("R008", _rule.Id);
        Assert.Equal("R008", error.RuleId);
        Assert.Equal(ImportColumns.Description, error.Column);
        Assert.Equal(7, error.RowNumber);
        Assert.Equal($"Description is {length} characters long. Shorten it to 120 characters or fewer.", error.Message);
    }

    [Fact]
    public void Counts_length_as_utf16_code_units()
    {
        var row = Rows.Row(description: new string('a', 119) + "\U0001F600");

        var error = Assert.Single(_rule.Validate(row, Rows.Context(row)));

        Assert.Equal("Description is 121 characters long. Shorten it to 120 characters or fewer.", error.Message);
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(119, false)]
    [InlineData(120, false)]
    [InlineData(120, true)]
    public void Accepts_descriptions_up_to_120_characters_after_trimming(int length, bool padded)
    {
        var description = new string('a', length);
        var row = Rows.Row(description: padded ? $" \t{description} \t" : description);

        Assert.Empty(_rule.Validate(row, Rows.Context(row)));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" \t ")]
    public void Leaves_blank_descriptions_to_the_required_fields_rule(string description)
    {
        var row = Rows.Row(description: description);

        Assert.Empty(_rule.Validate(row, Rows.Context(row)));
    }
}
