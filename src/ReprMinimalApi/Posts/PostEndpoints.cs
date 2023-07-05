using ReprMinimalApi.Core;
using ReprMinimalApi.Filters;

namespace ReprMinimalApi.Posts;

public static class PostEndpoints
{
    public static void MapPostEndpoints(this WebApplication app)
    {
        app.MapPost("/api/posts", async (
                CreatePostCommand command,
                CreatePostHandler handler,
                CancellationToken cancellationToken
            ) => (await handler.HandleAsync<FailResult>(command, cancellationToken))
            .ToCreatedResponse(i => new CreatedResponse<int>($"/api/posts/{i}",i)))
            .AddEndpointFilter<GenericFluentValidationFilter<CreatePostCommand>>();





        //app.MapPut("/api/posts/{id:int}", (
        //        CreatePostCommand command,
        //        CreatePostHandler handler,
        //        CancellationToken cancellationToken
        //    ) => handler.HandleAsync(command, cancellationToken))
        //    .AddEndpointFilter<GenericFluentValidationFilter<CreatePostCommand>>();

        //app.MapDelete("/api/posts/{id:int}", (
        //        CreatePostCommand command,
        //        CreatePostHandler handler,
        //        CancellationToken cancellationToken
        //    ) => handler.HandleAsync(command, cancellationToken))
        //    .AddEndpointFilter<GenericFluentValidationFilter<CreatePostCommand>>();

        //app.MapGet("/api/posts/{id:int}", (
        //        CreatePostCommand command,
        //        CreatePostHandler handler,
        //        CancellationToken cancellationToken
        //    ) => handler.HandleAsync(command, cancellationToken))
        //    .AddEndpointFilter<GenericFluentValidationFilter<CreatePostCommand>>();

        //app.MapGet("/api/posts", (
        //        CreatePostCommand command,
        //        CreatePostHandler handler,
        //        CancellationToken cancellationToken
        //    ) => handler.HandleAsync(command, cancellationToken))
        //    .AddEndpointFilter<GenericFluentValidationFilter<CreatePostCommand>>();
    }
}