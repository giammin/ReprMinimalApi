namespace ReprMinimalApi.Core;

public record FailResult(string Title, string? Details = null);

public record FailedValidationResult(string Title, string? Details = null, IEnumerable<ResultErrorDetail>? Errors = null) : FailResult(Title, Details)
{
    public IEnumerable<ResultErrorDetail> Errors { get; init; } = Errors ?? Array.Empty<ResultErrorDetail>();
    public IDictionary<string, string[]> ToDictionary()
    {
        return Errors
            .GroupBy(x => x.Key)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.Details).ToArray()
            );
    }
}

public record ResultErrorDetail(string Key, string Details);
//{
//    public ResultErrorDetail(string key, string details)
//    {
//        Key = key;
//        Details = details;
//    }

//    public string Key { get; set; }
//    public string Details { get; set; }
//}

public record BadRequestResult(string Title, string? Details = null, IEnumerable<ResultErrorDetail>? Errors = null) : FailedValidationResult(Title, Details, Errors);
public record NotFoundResult(string Title, string? Details = null) : FailResult(Title, Details);
public record ConflictResult(string Title, string? Details = null) : FailResult(Title, Details);
public record ExceptionResult(string Title, string? Details = null, Exception? Exception = null) : FailResult(Title, Details);