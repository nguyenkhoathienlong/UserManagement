using Data.Models;
using Microsoft.OpenApi.Models;
using Service.Mapper;
using UserManagement.Extensions;
using UserManagement.Helpers;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.
builder.Services.ConfigurePostgreSqlServer(builder.Configuration.GetSection("DbSetup").Get<DbSetupModel>());
builder.Services.AddAutoMapper(typeof(MapperProfiles));
builder.Services.AddCors();
builder.Services.ConfigureJWTToken(builder.Configuration.GetSection("JWT").Get<JwtModel>());
builder.Services.AddBusinessServices();

builder.Services.AddControllers(op =>
{
    op.Filters.Add(new ResultManipulator());
});

//builder.Services.Configure<JwtModel>(builder.Configuration.GetSection("JWT"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "UserManagement API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers();

app.Run();
