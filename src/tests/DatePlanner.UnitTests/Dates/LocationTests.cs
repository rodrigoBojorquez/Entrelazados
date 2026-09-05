using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Dates;
using FluentAssertions;
using DatePlanner.UnitTests.Abstractions;

namespace DatePlanner.UnitTests.Dates;

public class LocationTests
{
    [Theory]
    [InlineData("-90", "-180")]
    [InlineData("-89.999", "-179.999")]
    [InlineData("0", "0")]
    [InlineData("89.999", "179.999")]
    [InlineData("90", "180")]
    public void Create_ShouldAcceptCoordinates_AtAndWithinBoundaries(string latitudeText, string longitudeText)
    {
        decimal latitude = decimal.Parse(latitudeText, System.Globalization.CultureInfo.InvariantCulture);
        decimal longitude = decimal.Parse(longitudeText, System.Globalization.CultureInfo.InvariantCulture);

        Result<Location> result = Location.Create(latitude, longitude);

        result.IsSuccess.Should().BeTrue();
        result.Value.Latitude.Should().Be(latitude);
        result.Value.Longitude.Should().Be(longitude);
    }

    [Theory]
    [InlineData("-90.001")]
    [InlineData("90.001")]
    public void Create_ShouldRejectLatitude_ImmediatelyOutsideBoundary(string latitudeText)
    {
        decimal latitude = decimal.Parse(latitudeText, System.Globalization.CultureInfo.InvariantCulture);

        Result<Location> result = Location.Create(latitude, 0m);

        result.IsFailure.Should().BeTrue();
        result.Error.ShouldHaveCode("Location.InvalidLatitudeRange", ErrorType.Problem);
    }

    [Theory]
    [InlineData("-180.001")]
    [InlineData("180.001")]
    public void Create_ShouldRejectLongitude_ImmediatelyOutsideBoundary(string longitudeText)
    {
        decimal longitude = decimal.Parse(longitudeText, System.Globalization.CultureInfo.InvariantCulture);

        Result<Location> result = Location.Create(0m, longitude);

        result.IsFailure.Should().BeTrue();
        result.Error.ShouldHaveCode("Location.InvalidLongitudeRange", ErrorType.Problem);
    }

    [Fact]
    public void ValueEquality_ShouldTreatSameCoordinatesAsSameLocation()
    {
        Location first = Location.Create(21.1619m, -86.8515m).Value;
        Location second = Location.Create(21.1619m, -86.8515m).Value;

        second.Should().Be(first);
    }
}
