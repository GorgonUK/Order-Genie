using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Order_Genie.Models
{
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Updated { get; set; }

        public string FullName { get; set; }
    }
}
