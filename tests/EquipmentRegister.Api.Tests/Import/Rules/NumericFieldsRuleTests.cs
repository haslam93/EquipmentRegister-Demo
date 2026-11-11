using EquipmentRegister.Api.Import;
using EquipmentRegister.Api.Import.Rules;

namespace EquipmentRegister.Api.Tests.Import.Rules;

public class NumericFieldsRuleTests
{
    private readonly NumericFieldsRule _rule = new();

    [Theory]
    [InlineData("", "")]
    [InlineData("0", "12")]
    [InlineData("2.2", "85.5")]
    public void Accepts_blank_and_non_negative_numbers(string powerKw, string weightKg)
    {
        var row = Rows.Row(powerKw: powerKw, weightKg: weightKg);

        Assert.Empty(_rule.Validate(row, Rows.Context(row)));
    }

    [Theory]
    [InlineData("1,5")]
    [InlineData("1,250")]
    [InlineData("75 kW")]
    [InlineData("abc")]
    public void Rejects_values_that_are_not_plain_decimal_numbers(string powerKw)
    {
        var row = Rows.Row(powerKw: powerKw);

        var error = Assert.Single(_rule.Validate(row, Rows.Context(row)));

        Assert.Equal("R004", error.RuleId);
        Assert.Equal(ImportColumns.PowerKw, error.Column);
    }

    [Fact]
    public void Rejects_negative_weight()
    {
        var row = Rows.Row(weightKg: "-150");

        var error = Assert.Single(_rule.Validate(row, Rows.Context(row)));

        Assert.Equal(ImportColumns.WeightKg, error.Column);
        Assert.Contains("negative", error.Message);
    }
}
