using FluentAssertions;
using Taskify.Application.Features.Task.Commands.Create;

namespace Taskify.Tests.Application.Validators;

public class CreateTaskValidatorTests
{
    private readonly CreateTaskValidator _validator;

    public CreateTaskValidatorTests()
    {
        _validator = new CreateTaskValidator();
    }

    [Fact]
    public async System.Threading.Tasks.Task Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new CreateTaskCommand("Valid Title", "Valid Description");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task Validate_EmptyTitle_FailsValidation()
    {
        // Arrange
        var command = new CreateTaskCommand("", "Valid Description");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors.First().PropertyName.Should().Be("Title");
        result.Errors.First().ErrorMessage.Should().Be("Название задачи не может быть пустым.");
    }

    [Fact]
    public async System.Threading.Tasks.Task Validate_NullTitle_FailsValidation()
    {
        // Arrange
        var command = new CreateTaskCommand(null!, "Valid Description");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
        result.Errors.First(e => e.PropertyName == "Title").ErrorMessage
            .Should().Be("Название задачи не может быть пустым.");
    }

    [Fact]
    public async System.Threading.Tasks.Task Validate_WhitespaceTitle_FailsValidation()
    {
        // Arrange
        var command = new CreateTaskCommand("   ", "Valid Description");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors.First().PropertyName.Should().Be("Title");
        result.Errors.First().ErrorMessage.Should().Be("Название задачи не может быть пустым.");
    }

    [Fact]
    public async System.Threading.Tasks.Task Validate_TitleExceedsMaxLength_FailsValidation()
    {
        // Arrange
        var longTitle = new string('a', 201);
        var command = new CreateTaskCommand(longTitle, "Valid Description");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors.First().PropertyName.Should().Be("Title");
        result.Errors.First().ErrorMessage.Should().Be("Название задачи не должно превышать 200 символов.");
    }

    [Fact]
    public async System.Threading.Tasks.Task Validate_TitleExactlyMaxLength_PassesValidation()
    {
        // Arrange
        var title = new string('a', 200);
        var command = new CreateTaskCommand(title, "Valid Description");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task Validate_TitleJustBelowMaxLength_PassesValidation()
    {
        // Arrange
        var title = new string('a', 199);
        var command = new CreateTaskCommand(title, "Valid Description");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task Validate_DescriptionExceedsMaxLength_FailsValidation()
    {
        // Arrange
        var longDescription = new string('a', 1001);
        var command = new CreateTaskCommand("Valid Title", longDescription);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors.First().PropertyName.Should().Be("Description");
        result.Errors.First().ErrorMessage.Should().Be("Описание задачи не должно превышать 1000 символов.");
    }

    [Fact]
    public async System.Threading.Tasks.Task Validate_DescriptionExactlyMaxLength_PassesValidation()
    {
        // Arrange
        var description = new string('a', 1000);
        var command = new CreateTaskCommand("Valid Title", description);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task Validate_DescriptionJustBelowMaxLength_PassesValidation()
    {
        // Arrange
        var description = new string('a', 999);
        var command = new CreateTaskCommand("Valid Title", description);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task Validate_EmptyDescription_PassesValidation()
    {
        // Arrange
        var command = new CreateTaskCommand("Valid Title", "");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task Validate_NullDescription_PassesValidation()
    {
        // Arrange
        var command = new CreateTaskCommand("Valid Title", null!);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task Validate_MinimalValidCommand_PassesValidation()
    {
        // Arrange
        var command = new CreateTaskCommand("T", "");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task Validate_MultipleTitleErrors_ReturnsAllErrors()
    {
        // Arrange
        var longTitle = new string('a', 201);
        var command = new CreateTaskCommand("", "Valid Description");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public async System.Threading.Tasks.Task Validate_BothTitleAndDescriptionExceedMaxLength_FailsValidation()
    {
        // Arrange
        var longTitle = new string('a', 201);
        var longDescription = new string('b', 1001);
        var command = new CreateTaskCommand(longTitle, longDescription);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public async System.Threading.Tasks.Task Validate_TitleWithSpecialCharacters_PassesValidation()
    {
        // Arrange
        var command = new CreateTaskCommand("Task #1: Important! @Test", "Description");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task Validate_TitleWithUnicode_PassesValidation()
    {
        // Arrange
        var command = new CreateTaskCommand("Задача №1: Тест 🎯", "Описание задачи");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}
