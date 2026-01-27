namespace ASPBeautySalon.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateRegOn { get; set; }
        public ICollection<Service> Services { get; set; }
    }
}
