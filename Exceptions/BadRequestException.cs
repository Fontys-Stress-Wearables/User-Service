using System.Net;

namespace User_Service.Exceptions;

public class BadRequestException : AppException
{
    public BadRequestException(string message) : base(HttpStatusCode.BadRequest, message) { }

}
