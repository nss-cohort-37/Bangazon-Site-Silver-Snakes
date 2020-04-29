
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bangazon.Models.ProfileViewModels
{
    public class ProfileViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        public string StreetAddress { get; set; }

        [Required]
        public string PhoneNumber { get; set; }


    }
}