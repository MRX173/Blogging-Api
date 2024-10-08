using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Application;
using Application.Services;
using Domain;
using Domain.Entities;
using FluentValidation;
using Infrastracture;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Nest;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System
            .Text
            .Json
            .Serialization
            .ReferenceHandler
            .Preserve;
        options.JsonSerializerOptions.MaxDepth = 64;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Blogging App", Version = "v1" });

    // Configure Swagger to use the token
    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Please enter token in format: Bearer {token}",
        }
    );

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                Array.Empty<string>()
            },
        }
    );
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddDomainServices();
builder.Services.AddControllers();
/*builder.Services.AddSingleton<IElasticClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
        .DefaultIndex("posts")
        .DisableDirectStreaming()
        .PrettyJson();
    return new ElasticClient(settings);
});*/
builder
    .Services.AddIdentity<User, Role>(options => { options.SignIn.RequireConfirmedAccount = true; })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// builder.Services
//     .AddFluentEmail(builder.Configuration["Email:SenderEmail"]
//         , builder.Configuration["Email:Sender"])
//     .AddSmtpSender(builder.Configuration["Email:Host"]
//         , builder.Configuration.GetValue<int>("Email:Port>"));
builder
    .Services.AddFluentEmail("mohamedsamir177@outlook.com")
    .AddSmtpSender(
        new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new System.Net.NetworkCredential(
                "mohamedsamir177@outlook.com",
                "??"
            ),
            EnableSsl = true,
        }
    );
builder.Services.AddScoped<JwtService>();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
if (Directory.Exists(Path.Combine(builder.Environment.ContentRootPath, "wwwroot")))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
        RequestPath = "/static"
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
