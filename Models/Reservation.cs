using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace System_Rezerwacji.Models
{
    public class Reservation
    {
        public int ReservationID { get; set; }
        public DateTime ReservationDate { get; set; }
        public int CustomerID { get; set; }
        
        [ValidateNever]
        public Customer Customer { get; set; }
        public int ServiceID { get; set; }
        
        [ValidateNever]
        public Service Service { get; set; }
    }
}
