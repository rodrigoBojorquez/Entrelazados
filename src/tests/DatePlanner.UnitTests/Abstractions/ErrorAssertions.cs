using DatePlanner.Domain.Abstractions;
using FluentAssertions;

namespace DatePlanner.UnitTests.Abstractions;

internal static class ErrorAssertions
{
    internal static void ShouldHaveCode(this Error error, string expectedCode, ErrorType expectedType)
    {
        error.Code.Should().Be(expectedCode);
        error.Type.Should().Be(expectedType);
    }
}
