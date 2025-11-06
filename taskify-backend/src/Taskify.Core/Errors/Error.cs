namespace Taskify.Core.Errors;

public record Error
{
    public string Code { get; set; } = "";
    public string Message { get; set; } = "";
    public Error() { }
    public Error(string message, string code = "") { Message = message; Code = code; }
}