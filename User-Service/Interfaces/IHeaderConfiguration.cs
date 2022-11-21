namespace User_Service.Interfaces
{
    public interface IHeaderConfiguration
    {
        string GetTenantId(IHttpContextAccessor httpContextAccessor);
    }
}
