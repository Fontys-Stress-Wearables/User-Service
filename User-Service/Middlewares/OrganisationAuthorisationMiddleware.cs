using Microsoft.Identity.Web;
using User_Service.Exceptions;

namespace User_Service.Middlewares;

public class OrganisationAuthorisationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public OrganisationAuthorisationMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _configuration = config;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var tenant = context.User.GetTenantId();

        if (tenant == null)
        {
            throw new NotFoundException("tenant not found");
        }

        if (tenant != _configuration["tenant"])
        {
            throw new UnauthorizedException("tenant not found");
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
