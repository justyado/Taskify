using FluentValidation;

namespace Taskify.Application.Features.Task.Commands.Update;

public class UpdateTaskValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskValidator()
    {
        RuleFor(command => command.Title)
            .MaximumLength(200);
        
        RuleFor(command => command.Description)
            .MaximumLength(2000);

        RuleFor(command => command.Status)
            .IsInEnum();
    }
}