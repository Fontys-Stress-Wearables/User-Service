namespace User_Service.Models
{
    public class User
    {
        public string Id { get; set; }
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
        public DateTime Birthdate { get; set; }
        public bool IsActive { get; set; }
        public virtual Organisation Organisation { get; set; }
        public string Role { get; set; }
        public virtual List<PatientGroup> PatientGroups { get; set; }
    }
}
