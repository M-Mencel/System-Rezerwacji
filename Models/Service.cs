using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace System_Rezerwacji.Models
{
    public class Service
    {
        public int ServiceID { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public TimeSpan Duration { get; set; }
        [ValidateNever]
        public ICollection<Reservation> Reservations { get; set; }

    }
}
