using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; //use [NotMapped] Prevents Confirm Password From being stored in DB 
using System.Collections.Generic; //let us use List<RSVP>

namespace WeddingPlanner.Models
{
    public class User
    {
        [Key] 
        //We can now access a user it is being marked with an identifier
        public int UserId { get; set; }

        [Required(ErrorMessage = "Please provide your first name.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please provide your last name.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please provide your email.")]
        [EmailAddress(ErrorMessage = "Please providea valid email address")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please Enter A Password .")]
        [MinLength(8, ErrorMessage = "This Password Must Be At Least 8 Characters in length!")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Your Passwords Need To Match.")]
        [NotMapped] //Just so we don't store mistakes or errors in the password, only store the First Password Entry, Not Mapping for Confirmed password...
        [Compare("Password", ErrorMessage = "Passwords Need to Match")]
        public string PasswordConfirmation { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //One TO Many, User plans the weddings, Planned Weddings are created by one user. .Include for each wedding/user object..
        public List<Wedding> PlanedWeddings { get; set; }
        
        //by setting this up we are saying that Rsvp can have Rsvp's.... Many people will rsvp per wedding, and many RSVP's for all weddings..
        public List<Rsvp> Rsvps {get; set;}
    }
}