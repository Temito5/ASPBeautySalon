using Microsoft.EntityFrameworkCore.Query;

namespace ASPBeautySalon.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public Service Services { get; set; }
        public string ClientId { get; set; }
        public Client Clients { get; set; }
        public DateTime DateReservation { get; set; }
        public DateTime DateRegOn { get; set; }
    }
}
