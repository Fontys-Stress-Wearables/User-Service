namespace User_Service.Models
{
    public class PatientGroup
    {
        public string Id { get; set; } = "";
        public string? GroupName { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual Organisation Organisation { get; set; }
    }
}
