﻿using User_Service.Models;

namespace User_Service.Interfaces.IServices
{
    public interface IUserService
    {
        public void AddUser(User user);

        public IEnumerable<User> GetAll(string tenantId);

        public User GetUser(string tenantId, string id);

        public User UpdateUser(string tenantId, string patientId, string? firstName, string? lastName,
            DateTime? birthdate);
    }
}
