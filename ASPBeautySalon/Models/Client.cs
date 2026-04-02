using Microsoft.AspNetCore.Identity;

namespace ASPBeautySalon.Models
{
    public class Client:IdentityUser
    { 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateRegOn { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
