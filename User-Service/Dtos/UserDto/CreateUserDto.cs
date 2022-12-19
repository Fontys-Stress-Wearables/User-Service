namespace User_Service.Dtos.UserDto
{
    public class CreateUserDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public string Role { get; set; }
    }
}
