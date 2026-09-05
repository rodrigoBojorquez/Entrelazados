using DatePlanner.Domain.Abstractions;
using FluentAssertions;

namespace DatePlanner.UnitTests.Abstractions;

public class ResultTests
{
    private static readonly Error FailureError = Error.Conflict("Example.Conflict", "Conflict");

    [Fact]
    public void Constructor_ShouldRejectSuccess_WithNonEmptyError()
    {
        Action act = () => new Result(true, FailureError);

        act.Should().Throw<ArgumentException>().WithParameterName("error");
    }

    [Fact]
    public void Constructor_ShouldRejectFailure_WithNoError()
    {
        Action act = () => new Result(false, Error.None);

        act.Should().Throw<ArgumentException>().WithParameterName("error");
    }

    [Fact]
    public void Success_ShouldExposeSuccessfulStateAndNoError()
    {
        Result result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_ShouldExposeFailedStateAndProvidedError()
    {
        Result result = Result.Failure(FailureError);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeSameAs(FailureError);
    }

    [Fact]
    public void Value_ShouldThrow_WhenResultFailed()
    {
        Result<string> result = Result.Failure<string>(FailureError);

        Func<string> act = () => result.Value;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ImplicitConversion_ShouldCreateSuccess_WhenValueIsNotNull()
    {
        object value = new();

        Result<object> result = value;

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(value);
    }

    [Fact]
    public void ImplicitConversion_ShouldCreateNullValueFailure_WhenValueIsNull()
    {
        string? value = null;

        Result<string> result = value;

        result.IsFailure.Should().BeTrue();
        result.Error.ShouldHaveCode("General.Null", ErrorType.Failure);
    }

    [Fact]
    public void ValidationError_ShouldContainOnlyErrorsFromFailedResults()
    {
        Error firstError = Error.Problem("First", "First error");
        Error secondError = Error.Conflict("Second", "Second error");
        Result[] results = [Result.Success(), Result.Failure(firstError), Result.Failure(secondError)];

        ValidationError validationError = ValidationError.FromResults(results);

        validationError.ShouldHaveCode("General.Validation", ErrorType.Validation);
        validationError.Errors.Should().Equal(firstError, secondError);
    }

    [Fact]
    public void ValidationFailure_ShouldCreateFailedGenericResult_WithProvidedError()
    {
        Error validationError = new ValidationError([FailureError]);

        Result<string> result = Result<string>.ValidationFailure(validationError);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeSameAs(validationError);
        Func<string> accessValue = () => result.Value;
        accessValue.Should().Throw<InvalidOperationException>();
    }

    [Theory]
    [InlineData("failure", ErrorType.Failure)]
    [InlineData("problem", ErrorType.Problem)]
    [InlineData("not-found", ErrorType.NotFound)]
    [InlineData("conflict", ErrorType.Conflict)]
    public void ErrorFactory_ShouldPreserveCodeDescriptionAndType(string factory, ErrorType expectedType)
    {
        Error error = factory switch
        {
            "failure" => Error.Failure("Example.Code", "Description"),
            "problem" => Error.Problem("Example.Code", "Description"),
            "not-found" => Error.NotFound("Example.Code", "Description"),
            "conflict" => Error.Conflict("Example.Code", "Description"),
            _ => throw new ArgumentOutOfRangeException(nameof(factory))
        };

        error.Code.Should().Be("Example.Code");
        error.Description.Should().Be("Description");
        error.Type.Should().Be(expectedType);
    }
}
