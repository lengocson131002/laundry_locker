using LockerService.API;
using LockerService.API.Filters;
using LockerService.API.Middlewares;
using LockerService.Application;
using LockerService.Application.Common.Exceptions;
using LockerService.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    // Swagger doc
    opt.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "AT Locker Api",
        Version = "v1"
    });

    //Security Definition
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    // Filter security requirement
    opt.OperationFilter<AuthorizationOperationFilter>();
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin",
        b =>
        {
            b.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.ConfigureApiServices(builder.Configuration);

builder.Services.ConfigureApplicationServices(builder.Configuration);

builder.Services.ConfigureInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowOrigin");
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<LoggingMiddleware>();

app.MapControllers();

app.Run();