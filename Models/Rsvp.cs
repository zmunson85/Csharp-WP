using System;
using System.ComponentModel.DataAnnotations;


namespace WeddingPlanner.Models
{
    public class Rsvp
    {  
        public int RsvpId { get; set; } //new modelId
        public int? UserId { get; set; }
        public int WeddingId { get; set; }
        public User User { get; set; }
        public Wedding Wedding { get; set; }

    }

}