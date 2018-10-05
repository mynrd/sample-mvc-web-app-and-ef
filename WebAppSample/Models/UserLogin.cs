using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAppSample.Models
{
    public class UserLogin
    {
        [Required]
        [DisplayName("Enter your username")]
        public string Username { get; set; }
    }
}