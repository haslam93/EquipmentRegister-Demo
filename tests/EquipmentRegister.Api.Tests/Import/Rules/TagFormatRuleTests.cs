using EquipmentRegister.Api.Import;
using EquipmentRegister.Api.Import.Rules;

namespace EquipmentRegister.Api.Tests.Import.Rules;

public class TagFormatRuleTests
{
    private readonly TagFormatRule _rule = new();

    [Theory]
    [InlineData("PU-1001")]
    [InlineData("PU-2010A")]
    [InlineData("FIT-3005")]
    public void Accepts_well_formed_tags(string tag)
    {
        var row = Rows.Row(tag: tag);

        Assert.Empty(_rule.Validate(row, Rows.Context(row)));
    }

    [Theory]
    [InlineData("pu-1001")]
    [InlineData("PU1001")]
    [InlineData("PU-101")]
    [InlineData("PUMP-1001")]
    [InlineData("PU-1001AB")]
    public void Rejects_malformed_tags(string tag)
    {
        var row = Rows.Row(rowNumber: 7, tag: tag);

        var error = Assert.Single(_rule.Validate(row, Rows.Context(row)));

        Assert.Equal("R002", error.RuleId);
        Assert.Equal(ImportColumns.Tag, error.Column);
        Assert.Equal(7, error.RowNumber);
        Assert.Contains(tag, error.Message);
    }

    [Fact]
    public void Leaves_a_missing_tag_to_the_required_fields_rule()
    {
        var row = Rows.Row(tag: "");

        Assert.Empty(_rule.Validate(row, Rows.Context(row)));
    }
}
