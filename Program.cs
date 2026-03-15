using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SalesApiStub.Services;
using SalesApiStub.Transformers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers ───────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── JWT / OAuth 2.0 Bearer authentication ────────────────────────────────────
var oauthSection = builder.Configuration.GetSection("OAuth");

var secretKey = oauthSection["SecretKey"]
    ?? throw new InvalidOperationException("OAuth:SecretKey is required in configuration.");
var issuer = oauthSection["Issuer"]
    ?? throw new InvalidOperationException("OAuth:Issuer is required in configuration.");
var audience = oauthSection["Audience"]
    ?? throw new InvalidOperationException("OAuth:Audience is required in configuration.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidIssuer              = issuer,
            ValidateAudience         = true,
            ValidAudience            = audience,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew                = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

// ── OpenAPI document generation ───────────────────────────────────────────────
builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer<SecurityDocumentTransformer>();
    options.AddOperationTransformer<AuthorizeOperationTransformer>();
});

// ── Application services ──────────────────────────────────────────────────────
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<OrderService>();

// ── Build and configure middleware pipeline ───────────────────────────────────
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Serves the raw OpenAPI JSON at /openapi/v1.json
    app.MapOpenApi();

    // Scalar API reference UI at /docs
    app.MapScalarApiReference("/docs", options =>
    {
        options.WithTitle("Sales API")
               .WithTheme(ScalarTheme.Kepler)
               .AddPreferredSecuritySchemes("oauth2")
               .AddOAuth2Authentication("oauth2", scheme =>
                    scheme.WithFlows(flows =>
                        flows.WithClientCredentials(cc =>
                            cc.WithClientId("demo-client")
                              .WithClientSecret("demo-secret"))));
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
