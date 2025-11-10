using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace System_Rezerwacji.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        [ValidateNever]
        public ICollection<Reservation> Reservations { get; set; }
    }
}
