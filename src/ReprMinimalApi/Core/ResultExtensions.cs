using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace ReprMinimalApi.Core;

public static class ResultExtensions
{
    public const string ProblemDetailsExtensionsException = "exception";
    public static IResult ToResponse<TSuccess, TFail>(this BiResult<TSuccess, TFail> result,
        Func<TSuccess, IResult> resultConverterDelegate)
        where TFail : FailResult
    {
        return result.Match(
            success: resultConverterDelegate,
            fail: failResult => failResult switch
            {
                NotFoundResult notFoundResult => Results.NotFound(notFoundResult),
                ConflictResult conflictResult => Results.Conflict(conflictResult.ToProblemDetails()),
                ExceptionResult exceptionResult => Results.Problem(exceptionResult.ToProblemDetails()),
                BadRequestResult badRequestResult => Results.Problem(badRequestResult.ToProblemDetails()),
                FailedValidationResult failedValidationResult => Results.Problem(failedValidationResult.ToProblemDetails()),
                _ => throw new ArgumentOutOfRangeException(nameof(failResult), failResult, "FailResultNotHandled")
            });
    }
    public static IResult ToOkResponse<TSuccess, TContract, TFail>(this BiResult<TSuccess, TFail> result,
        Func<TSuccess, TContract> mapper)
        where TFail : FailResult
    {
        return result.ToResponse(success => Results.Ok(mapper(success)));
    }
    public static IResult ToCreatedResponse<TSuccess, TContract, TFail>(this BiResult<TSuccess, TFail> result,
        Func<TSuccess, CreatedResponse<TContract>> mapper)
        where TFail : FailResult
    {
        return result.ToResponse(success =>
        {
            var (url, contract) = mapper(success);
            return Results.Created(url, contract);
        });
    }

    public static IResult ToNoContentResponse<TFail>(this BiResult<object, TFail> result)
        where TFail : FailResult
    {
        return result.ToResponse(_ => Results.NoContent());
    }

    public static ProblemDetails ToProblemDetails(this ConflictResult conflictResult) =>
        new()
        {
            Title = conflictResult.Title,
            Status = (int)HttpStatusCode.Conflict,
            Detail = conflictResult.Details
        };
    public static ProblemDetails ToProblemDetails(this BadRequestResult badRequestResult)
    {
        var details = new ProblemDetails
        {
            Title = badRequestResult.Title,
            Status = (int)HttpStatusCode.BadRequest,
            Detail = badRequestResult.Details
        };
        foreach (var item in badRequestResult.ToDictionary())
        {
            details.Extensions.Add(item.Key, item.Value);
        }
        return details;
    }
    public static HttpValidationProblemDetails ToProblemDetails(this FailedValidationResult failedValidationResult)
    {
        var details = new HttpValidationProblemDetails(failedValidationResult.ToDictionary())
        {
            Title = failedValidationResult.Title,
            Status = (int)HttpStatusCode.BadRequest,
            Detail = failedValidationResult.Details
        };
        return details;
    }
    public static ProblemDetails ToProblemDetails(this ExceptionResult exceptionResult)
    {
        var rtn = exceptionResult.Exception != null ? exceptionResult.Exception.ToProblemDetails() : new ProblemDetails();

        rtn.Title = exceptionResult.Title;
        if (!string.IsNullOrWhiteSpace(exceptionResult.Details))
        {
            rtn.Detail = exceptionResult.Details;
        }
        return rtn;
    }
    public static ProblemDetails ToProblemDetails(this Exception exception)
    {
        var jsonException = new ExceptionJsonReturn(exception);
        var problemDetails = new ProblemDetails
        {
            Title = $"Unhandled Exception of type {jsonException.Type}",
            Status = (int)HttpStatusCode.InternalServerError,
            Detail = jsonException.Message
        };

        problemDetails.Extensions.Add(ProblemDetailsExtensionsException, jsonException);
        return problemDetails;
    }
}
public record CreatedResponse<T>(string Url, T Contract);