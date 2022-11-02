namespace User_Service.Dtos.PatientDto
{
    public class ReadUserDto
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public bool IsActive { get; set; }
    }
}
