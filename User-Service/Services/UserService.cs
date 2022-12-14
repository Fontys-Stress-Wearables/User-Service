using Azure.Identity;
using Microsoft.Graph;
using System.Net.Http.Headers;
using User_Service.Exceptions;
using User_Service.Interfaces;
using User_Service.Interfaces.IServices;
using User = User_Service.Models.User;

namespace User_Service.Services
{
    public class UserService : IUserService
    {
        // the unit of work service and interface implement the IuserRepo
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        //private readonly INatsService _natsService;

        // testing sake
        public UserService(IUnitOfWork _unitOfWork, IConfiguration configuration)
        {
            this._unitOfWork = _unitOfWork;
            _configuration = configuration;

        }
        //public UserService(IUnitOfWork _unitOfWork, INatsService _natsService)
        //{
        //    this._unitOfWork = _unitOfWork;
        //    this._natsService = _natsService;
        //}

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
            user.IsActive = true;
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

            if (user == null || user.Role != "Patient")
            {
                return null;
                throw new NotFoundException($"User with id '{id}' doesn't exist.");
            }

            return user;
        }

        public async Task<User> GetCaregiver(string tenantId, string id)
        {
            // if this caregiver is in our database 
            var caregiver = _unitOfWork.Users.GetByIdAndTenant(tenantId, id);

            if (caregiver != null && caregiver.Role == "Caregiver")
            {
                return caregiver;
            }

            // if the user(caregiver) isnt in our database
            // fetch user from azure
            var newCaregivers = await FetchCaregiversFromAzureGraph(tenantId);

            caregiver = newCaregivers.FirstOrDefault(c => c.Id == id);

            if (caregiver == null)
            {
                throw new NotFoundException($"Caregiver with id '{id}' doesn't exist.");
            }
            return caregiver;
        }

        public async Task<ICollection<User>> FetchCaregiversFromAzureGraph(string tenantId)
        {
            var credential = new ChainedTokenCredential(
                new ClientSecretCredential(tenantId, _configuration["AzureAD:ClientID"],
                _configuration["AzureAD:ClientSecret"]));

            var token = await credential.GetTokenAsync(
                new Azure.Core.TokenRequestContext(
                    new[] { "https://graph.microsoft.com/.default" }));

            var accessToken = token.Token;
            var graphServiceClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(requestMessage =>
                {
                    requestMessage
                        .Headers
                        .Authorization = new AuthenticationHeaderValue("bearer", accessToken);
                    return Task.CompletedTask;
                }));

            var users = await graphServiceClient.Users.Request().GetAsync();
            var caregivers = users.Select(x => new User {Id = x.Id, Role = "Caregiver"}).ToList();

            _unitOfWork.Users.UpdateCaregiverByTenant(caregivers, tenantId);
            _unitOfWork.Complete();

            return caregivers;
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

            _unitOfWork.Users.UpdateUserInDB(user);
            //_natsService.Publish("patient-updated", user.TenantId, user);
            //_natsService.Publish("th-logs", "", $"User updated with ID: '{user.Id}.'");

            _unitOfWork.Complete();
            return user;
        }
    }
}
