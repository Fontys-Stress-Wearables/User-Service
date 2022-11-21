using User_Service.Interfaces;

namespace User_Service.Services
{
    public class HeaderConfiguration : IHeaderConfiguration
    {
        public string GetTenantId(IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HttpContext.Request.Headers["Tenant-ID"].ToString();
        }
    }
}
