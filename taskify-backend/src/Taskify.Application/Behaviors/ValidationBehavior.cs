using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Taskify.Application.Common;
using Taskify.Core.Errors;

namespace Taskify.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TResponse : Result, IResultFactory<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            _logger.LogDebug("No validators found for {RequestType}", typeof(TRequest).Name);
            return await next();
        }

        _logger.LogDebug("Validating {RequestType} with {ValidatorCount} validator(s)", typeof(TRequest).Name, _validators.Count());

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count == 0)
        {
            _logger.LogDebug("Validation passed for {RequestType}", typeof(TRequest).Name);
            return await next();
        }

        _logger.LogWarning("Validation failed for {RequestType} with {ErrorCount} error(s): {Errors}",
            typeof(TRequest).Name,
            failures.Count,
            string.Join(", ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}")));

        var errors = failures
            .Select(f => new Error(f.ErrorMessage, $"Validation.{f.PropertyName}"))
            .ToArray();

        return TResponse.Fail(errors);
    }
}
