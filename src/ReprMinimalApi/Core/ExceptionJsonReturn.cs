namespace ReprMinimalApi.Core;


public record ExceptionJsonReturn
{
    public string Message { get; set; }
    public string? Source { get; set; }
    public string? StackTrace { get; set; }
    public string Type { get; set; }
    public ExceptionJsonReturn? BaseException { get; set; }
    public ExceptionJsonReturn(Exception exception)
    {
        Message = exception.Message;
        Source = exception.Source;
        StackTrace = exception.StackTrace;
        Type = exception.GetType().Name;
        if (exception.InnerException != null)
        {
            BaseException = new ExceptionJsonReturn(exception.GetBaseException());
        }
    }
}