using User_Service.Exceptions;
using User_Service.Interfaces;
using User_Service.Interfaces.IServices;
using User_Service.Models;

namespace User_Service.Services
{
    public class UserService : IUserService
    {
        // IUnitofWork implements IUserRepository
        private readonly IUnitOfWork _unitOfWork;
        //private readonly INatsService _natsService;

        public UserService(IUnitOfWork _unitOfWork)
        {
            this._unitOfWork = _unitOfWork;
        }
        
        public void AddUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.FirstName))
            {
                throw new BadRequestException("First name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(user.LastName))
            {
                throw new BadRequestException("Last name cannot be empty.");
            }

            if (user.Birthdate > DateTime.Now)
            {
                throw new BadRequestException("Birthdate cannot be after the current date.");
            }
            _unitOfWork.Users.Add(user);
            _unitOfWork.Complete();
            //_natsService.Publish("user-created", user.TenantId, user);
            //_natsService.Publish("th-logs", "", $"User created with ID: '{user.Id}.'");
        }

        public IEnumerable<User> GetAll(string tenantId)
        {
            return _unitOfWork.Users.GetAllByTenant(tenantId);
        }

        public User GetUser(string tenantId, string id)
        {
            var user = _unitOfWork.Users.GetByIdAndTenant(tenantId, id);

            if (user == null)
            {
                throw new NotFoundException($"User with id '{id}' doesn't exist.");
            }

            return user;
        }

        public User UpdateUser(string tenantId, string patientId, string? firstName, string? lastName, DateTime? birthdate)
        {
            var user = GetUser(tenantId, patientId);

            if (birthdate != null && birthdate > DateTime.Now)
            {
                throw new BadRequestException("Birthdate cannot be after the current date.");
            }

            user.FirstName = firstName ?? user.FirstName;
            user.LastName = lastName ?? user.LastName;
            user.Birthdate = birthdate ?? user.Birthdate;

            _unitOfWork.Users.UpdateUser(user);
            //_natsService.Publish("patient-updated", user.TenantId, user);
            //_natsService.Publish("th-logs", "", $"User updated with ID: '{user.Id}.'");

            _unitOfWork.Complete();
            return user;
        }
    }
}
