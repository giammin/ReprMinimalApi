using FluentValidation;

namespace ReprMinimalApi;

public class CreatePostHandler
{
    private readonly ILogger<CreatePostHandler> _logger;

    public CreatePostHandler(ILogger<CreatePostHandler> logger)
    {
        _logger = logger;
    }
    public Task<CreatePostCommand> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        try
        {
            //todo
            return Task.FromResult(request);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error sending {request}", request);
            throw;
        }
    }
}

public record CreatePostCommand(string DisplayName, string Email, string? LinkedIn, string City, string Passions,
    string Strengths, string? Work, string Future, string Learning, string Results, string Errors, string Updating,
    string Worth, string? Choice, string Thoughts);



public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(x => x.DisplayName).NotNull();
        RuleFor(x => x.Email).EmailAddress().NotNull();
        RuleFor(x => x.City).NotNull();
        RuleFor(x => x.Passions).NotNull();
        RuleFor(x => x.Strengths).NotNull();
        RuleFor(x => x.Future).NotNull();
        RuleFor(x => x.Learning).NotNull();
        RuleFor(x => x.Results).NotNull();
        RuleFor(x => x.Errors).NotNull();
        RuleFor(x => x.Updating).NotNull();
        RuleFor(x => x.Worth).NotNull();
        RuleFor(x => x.Thoughts).NotNull();
    }
}