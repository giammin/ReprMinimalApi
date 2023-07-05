using FluentValidation;

namespace ReprMinimalApi.Filters;

public class GenericFluentValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly IValidator<T> _validator;
    private readonly ILogger<GenericFluentValidationFilter<T>> _logger;

    public GenericFluentValidationFilter(IValidator<T> validator, ILogger<GenericFluentValidationFilter<T>> logger)
    {
        _validator = validator;
        _logger = logger;
    }
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.Arguments.SingleOrDefault(x => x?.GetType() == typeof(T)) is not T toValidate)
        {
            //if this occur there is some endpoint configuration error
            var endPoint = context.HttpContext.GetEndpoint();
            var paramType = typeof(T);
            _logger.LogError("Parameter type {type} not found in request to {endPoint}", paramType, endPoint?.DisplayName);
            throw new Exception($"Parameter type {paramType} not found in request to {endPoint?.DisplayName}");
        }
        var validationResult = await _validator.ValidateAsync(toValidate);
        if (!validationResult.IsValid)
        {
            var endPoint = context.HttpContext.GetEndpoint();
            _logger.LogWarning("Request to {endPoint} is invalid: {errors}", endPoint?.DisplayName, validationResult.ToString(", "));

            return Results.ValidationProblem(
                validationResult.ToDictionary(),
                validationResult.ToString(),
                title: "Dati inviati non corretti",
                statusCode: StatusCodes.Status400BadRequest);
        }
        return await next(context);
    }
}