using System;
using System.ComponentModel.DataAnnotations;

namespace SportsStore.Domain.Entities
{
    public class ShippingDetails
    {
        [Required(ErrorMessage = "Please enter a name")]
        public String Name { get; set; }

        [Required(ErrorMessage = "Please enter the first address line")]
        public String Line1 { get; set; }
        public String Line2 { get; set; }
        public String Line3 { get; set; }

        [Required(ErrorMessage = "Please enter a city name")]
        public String City { get; set; }

        [Required(ErrorMessage = "Please enter a state name")]
        public String State { get; set; }

        public String Zip { get; set; }

        [Required(ErrorMessage = "Please enter a country name")]
        public String Country { get; set; }

        public bool GiftWrap { get; set; }
    }
}
