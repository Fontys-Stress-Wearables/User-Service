namespace User_Service.Models
{
    public class Organisation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual List<User> Users { get; set; }
        public virtual List<PatientGroup> PatientGroups { get; set; }
    }
}
