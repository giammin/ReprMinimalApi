using FluentValidation;
using ReprMinimalApi.Core;

namespace ReprMinimalApi.Posts;

public class GetAllPostsHandler
{
    public async Task<BiResult<IEnumerable<PostEntity>, FailResult>> HandleAsync<TFail>(CancellationToken cancellationToken) where TFail : FailResult
    {
        await Task.Delay(100);
       
        return new[]
        {
            new PostEntity(1, nameof(PostEntity.Title), nameof(PostEntity.Title), nameof(PostEntity.ShortText)),
            new PostEntity(2, nameof(PostEntity.Title), nameof(PostEntity.Title), nameof(PostEntity.ShortText)),
            new PostEntity(3, nameof(PostEntity.Title), nameof(PostEntity.Title), nameof(PostEntity.ShortText)),
        };
    }
}
