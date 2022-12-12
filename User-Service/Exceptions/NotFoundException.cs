using System.Net;

namespace User_Service.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(HttpStatusCode.NotFound, message) { }
}
