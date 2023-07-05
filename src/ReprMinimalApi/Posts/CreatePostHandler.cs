using FluentValidation;
using ReprMinimalApi.Core;

namespace ReprMinimalApi.Posts;

public class CreatePostHandler
{
    private readonly ILogger<CreatePostHandler> _logger;

    public CreatePostHandler(ILogger<CreatePostHandler> logger)
    {
        _logger = logger;
    }
    public async Task<BiResult<int, FailResult>> HandleAsync<TFail>(CreatePostCommand request, CancellationToken cancellationToken) where TFail:FailResult
    {
        try
        {
            await Task.Delay(1000, cancellationToken);
            if (request.Text.Contains("BadRequestResult"))
            {
                return new BadRequestResult("text cannot have BadRequestResult");
            }
            if (request.Text.Contains("ConflictResult"))
            {
                return new ConflictResult("text cannot have BadRequestResult");
            }
            if (request.Text.Contains("Exception"))
            {
                throw new Exception("test exception");
            }
            return 1;

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error sending {request}", request);
            throw;
        }
    }
}

public record CreatePostCommand(string Title, string Text, string? ShortText);



public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(x => x.Title).NotNull().MaximumLength(30);
        RuleFor(x => x.Text).NotNull();
        RuleFor(x => x.ShortText).MaximumLength(100);
    }
}