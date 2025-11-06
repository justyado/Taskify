using FluentValidation;

namespace Taskify.Application.Features.Task.Commands.Create;

public class CreateTaskValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Название задачи не может быть пустым.")
            .MaximumLength(200)
            .WithMessage("Название задачи не должно превышать 200 символов.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Описание задачи не должно превышать 1000 символов.");
    }
}