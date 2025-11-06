using Taskify.Core.Errors;

namespace Taskify.Application.Common;

public interface IResultFactory<TResponse>
{
    static abstract TResponse Fail(params Error[] errors);
}

public record Result : IResultFactory<Result>
{
    public bool IsSuccess { get; internal set; }
    public bool IsFailed => !IsSuccess;
    public List<Error> Errors { get; internal set; } = new();

    public static Result Ok() => new() { IsSuccess = true };
    public static Result Fail(params Error[] errors) => new() { IsSuccess = false, Errors = errors.ToList() };
}

public record Result<T> : Result, IResultFactory<Result<T>>
{
    public T Value { get; private set; }

    public static Result<T> Ok(T value) => new() { IsSuccess = true, Value = value };
    public new static Result<T> Fail(params Error[] errors) => new() { IsSuccess = false, Errors = errors.ToList() };
}