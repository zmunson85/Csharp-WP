using System;
using System.ComponentModel.DataAnnotations;//let us use DataType
using System.ComponentModel.DataAnnotations.Schema; //only used to avoid storing the confirm password in the DB


namespace WeddingPlanner.Models
{
    public class LoginUser
    {
        public string LoginEmail {get; set;}
        
        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }
    }
}