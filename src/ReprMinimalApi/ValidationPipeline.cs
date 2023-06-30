using FluentValidation;

namespace ReprMinimalApi;

public class ValidationPipeline
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationPipeline(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public async Task<TResponse> Exec<TRequest, TResponse>(TRequest request, Func<TRequest,CancellationToken, Task<TResponse>> next,
        CancellationToken cancellationToken)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<TRequest>>();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorDictionary = validationResult.ToDictionary();

            //return Results.ValidationProblem(
            //    errorDictionary,
            //    string.Join(", ", errorDictionary.Values),
            //    title: "Dati inviati non corretti",
            //    statusCode: StatusCodes.Status400BadRequest);
        }
        return await next(request, cancellationToken);
    }
}