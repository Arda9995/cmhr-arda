namespace Quark.CMS.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // try
        // {
        //     await _next(context);
        // }
        // catch (Exception ex)
        // {
        //     context.Response.Clear();
        //
        //     // Log the exception (optional)
        //     // var logger = context.RequestServices.GetRequiredService<ILogger<ExceptionHandlerMiddleware>>();
        //     // logger.LogError(ex, "Unhandled exception");
        //
        //     context.Response.StatusCode = ex switch
        //     {
        //         KeyNotFoundException => StatusCodes.Status404NotFound,
        //         _ => StatusCodes.Status500InternalServerError
        //     };
        //
        //     var statusCode = context.Response.StatusCode;
        //
        //     switch (statusCode)
        //     {
        //         case StatusCodes.Status404NotFound:
        //             context.Response.Redirect("/Home/Error404");
        //             break;
        //         case StatusCodes.Status500InternalServerError:
        //             context.Response.Redirect("/Home/Error500");
        //             break;
        //     }
        //
        // }
        
        await _next(context);
        switch (context.Response.StatusCode)
        {
            case 404:
                context.Response.Redirect($"/Home/Error404");
                break;
            case 500:
                context.Response.Redirect($"/Home/Error500");
                break;
        }
    }
}

public static class ExceptionHandlerMiddlewareExtensions
{
    public static WebApplication UseExceptionHandler(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();
        return app;
    }
}