using System.Text.Json;
using AutoTrade.Core.DTOs;
using Microsoft.AspNetCore.Diagnostics;

namespace AutoTrade.API.Middlewares;

public static class UseCustomExceptionHandler
{
    public static void UseCustomException(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(config =>
        {
            config.Run(async context =>
            {
                context.Response.ContentType = "application/json";

                var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (exceptionFeature != null)
                {
                    var ex = exceptionFeature.Error;

                    // Hata tipine göre HTTP StatusCode belirleme
                    int statusCode = ex switch
                    {
                        ArgumentException => 400,
                        KeyNotFoundException => 404,
                        _ => 500
                    };

                    context.Response.StatusCode = statusCode;

                    var response = CustomResponseDto<string>.Fail(statusCode, ex.Message);

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                }
            });
        });
    }
}