using FluentValidation;
using ReprMinimalApi.Core;

namespace ReprMinimalApi.Posts;

public class GetPostHandler
{
    public async Task<BiResult<PostEntity, FailResult>> HandleAsync<TFail>(GetPostQuery request, CancellationToken cancellationToken) where TFail : FailResult
    {
        await Task.Delay(100);
        if (request.Id>3)
        {
            return new NotFoundResult("post not found");
        }

        return new PostEntity(request.Id, nameof(PostEntity.Title), nameof(PostEntity.Title),
            nameof(PostEntity.ShortText));
    }
}

public record GetPostContract(string Title, string Text);
public record PostEntity(int Id,string Title, string Text, string? ShortText);

public record GetPostQuery(int Id);


public class GetPostQueryValidator : AbstractValidator<GetPostQuery>
{
    public GetPostQueryValidator()
    {
        RuleFor(x => x.Id).NotNull().GreaterThan(0);
    }
}