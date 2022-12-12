using Microsoft.Identity.Web;
using User_Service.Exceptions;

namespace User_Service.Middlewares;

public class OrganisationAuthorisationMiddleware
{
    private readonly RequestDelegate _next;

    public OrganisationAuthorisationMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var tenant = context.User.GetTenantId();

        if (tenant == null)
        {
            throw new NotFoundException("tenant not found");
        }

        await _next(context);
    }
}

public static class OrganisationAuthorisationMiddlewareExtensions
{
    public static IApplicationBuilder UseOrganizationAuthorization(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<OrganisationAuthorisationMiddleware>();
    }
}
