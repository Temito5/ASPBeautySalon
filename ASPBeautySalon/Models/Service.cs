namespace ASPBeautySalon.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public Category Categories { get; set; }
        public string Description { get; set;}
        public string Image {  get; set; }
        public decimal Price { get; set; }
        public decimal PromoPercent { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DateRegOn { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
