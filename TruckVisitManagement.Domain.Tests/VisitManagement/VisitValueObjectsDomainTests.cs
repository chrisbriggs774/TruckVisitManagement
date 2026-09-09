using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Domain.Tests.VisitManagement;

public class VisitValueObjectsDomainTests
{
    [Fact]
    public void VisitId_From_ShouldThrow_WhenGuidIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => VisitId.From(Guid.Empty));
    }

    [Fact]
    public void VisitId_New_ShouldCreateNonEmptyValue()
    {
        var visitId = VisitId.New();

        Assert.NotEqual(Guid.Empty, visitId.Value);
    }

    [Theory]
    [InlineData(" ab 12 cd ", "AB12CD")]
    [InlineData("yx22abc", "YX22ABC")]
    public void LicensePlate_ShouldNormalize(string input, string expected)
    {
        var value = new LicensePlate(input);

        Assert.Equal(expected, value.Value);
    }

    [Theory]
    [InlineData(" ab 123 ", "AB123")]
    [InlineData("unit 001", "UNIT001")]
    public void TruckUnitNumber_ShouldNormalize(string input, string expected)
    {
        var value = new TruckUnitNumber(input);

        Assert.Equal(expected, value.Value);
    }

    [Theory]
    [InlineData(" tr 55 ", "TR55")]
    [InlineData("tlr-1", "TLR-1")]
    public void TrailerNumber_ShouldNormalize(string input, string expected)
    {
        var value = new TrailerNumber(input);

        Assert.Equal(expected, value.Value);
    }

    [Theory]
    [InlineData(" xy 77 zz ", "XY77ZZ")]
    [InlineData("reg-1", "REG-1")]
    public void TrailerRegistration_ShouldNormalize(string input, string expected)
    {
        var value = new TrailerRegistration(input);

        Assert.Equal(expected, value.Value);
    }

    [Theory]
    [InlineData("  terminal-1  ", "TERMINAL-1")]
    [InlineData("rotterdam", "ROTTERDAM")]
    public void TerminalId_ShouldNormalize(string input, string expected)
    {
        var value = new TerminalId(input);

        Assert.Equal(expected, value.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NormalizedValueObjects_ShouldRejectBlankValues(string input)
    {
        Assert.Throws<ArgumentException>(() => new LicensePlate(input));
        Assert.Throws<ArgumentException>(() => new TruckUnitNumber(input));
        Assert.Throws<ArgumentException>(() => new TrailerNumber(input));
        Assert.Throws<ArgumentException>(() => new TrailerRegistration(input));
        Assert.Throws<ArgumentException>(() => new TerminalId(input));
    }

    [Fact]
    public void ValueObjects_ShouldUseValueEquality()
    {
        var left = new LicensePlate("ab 123");
        var right = new LicensePlate("AB123");

        Assert.Equal(left, right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }
}
