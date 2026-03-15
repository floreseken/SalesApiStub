using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace SalesApiStub.Transformers;

/// <summary>
/// Registers the OAuth2 Client Credentials security scheme in the OpenAPI document components.
/// </summary>
internal sealed class SecurityDocumentTransformer : IOpenApiDocumentTransformer
{
    private readonly IConfiguration _config;

    public SecurityDocumentTransformer(IConfiguration config) => _config = config;

    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var issuer = _config["OAuth:Issuer"] ?? "https://localhost:7065";

        document.Info.Description =
            """
            A stub Sales API returning products currently **in stock** with their prices.

            ## Authentication

            This API uses the **OAuth 2.0 Client Credentials** flow (RFC 6749 §4.4).

            Obtain a token by calling `POST /oauth/token`, then click **Authorize** above
            and paste the `access_token` value, or let Scalar handle it automatically.

            **Demo credentials**

            | Field           | Value         |
            |-----------------|---------------|
            | `client_id`     | demo-client   |
            | `client_secret` | demo-secret   |
            | `scope`         | sales:read    |
            """;

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["oauth2"] = new OpenApiSecurityScheme
        {
            Type  = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                ClientCredentials = new OpenApiOAuthFlow
                {
                    TokenUrl = new Uri($"{issuer}/oauth/token"),
                    Scopes   = new Dictionary<string, string>
                    {
                        { "sales:read", "Read access to sales inventory and pricing data" }
                    }
                }
            }
        };

        return Task.CompletedTask;
    }
}

/// <summary>
/// Adds an OAuth2 security requirement to every endpoint decorated with <c>[Authorize]</c>.
/// </summary>
internal sealed class AuthorizeOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var requiresAuth = context.Description.ActionDescriptor.EndpointMetadata
            .OfType<IAuthorizeData>()
            .Any();

        if (requiresAuth)
        {
            operation.Security ??= [];
            var requirement = new OpenApiSecurityRequirement();
            requirement[new OpenApiSecuritySchemeReference("oauth2", context.Document, null!)] =
                ["sales:read"];
            operation.Security.Add(requirement);
        }

        return Task.CompletedTask;
    }
}
