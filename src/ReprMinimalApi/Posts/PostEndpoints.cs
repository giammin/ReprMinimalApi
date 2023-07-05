using Microsoft.AspNetCore.Mvc;
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

        app.MapGet("/api/posts/{id:int}", async ([AsParameters]
                    GetPostQuery query,
                    GetPostHandler handler,
                    CancellationToken cancellationToken
                ) => (await handler.HandleAsync<FailResult>(query, cancellationToken))
                .ToOkResponse(i => new GetPostContract(i.Title, i.Text)))
            .AddEndpointFilter<GenericFluentValidationFilter<GetPostQuery>>();

        app.MapGet("/api/posts", async (
                    GetAllPostsHandler handler,
                    CancellationToken cancellationToken
                ) => (await handler.HandleAsync<FailResult>(cancellationToken))
                .ToOkResponse(p=>p.Select(i=>new GetPostContract(i.Title, i.Text)) ));

        //app.MapDelete("/api/posts/{id:int}", (
        //        CreatePostCommand command,
        //        CreatePostHandler handler,
        //        CancellationToken cancellationToken
        //    ) => handler.HandleAsync(command, cancellationToken))
        //    .AddEndpointFilter<GenericFluentValidationFilter<CreatePostCommand>>();

        //app.MapPut("/api/posts/{id:int}", (
        //        CreatePostCommand command,
        //        CreatePostHandler handler,
        //        CancellationToken cancellationToken
        //    ) => handler.HandleAsync(command, cancellationToken))
        //    .AddEndpointFilter<GenericFluentValidationFilter<CreatePostCommand>>();
        
    }
}