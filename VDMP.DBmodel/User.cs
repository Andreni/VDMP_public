using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VDMP.DBmodel
{
    [Table("Users")]
    public class User
    {
        [Key] public string UserId { get; set; }

        [Required] public string UserName { get; set; }

        [Required] public string UserPassword { get; set; }

        public ICollection<Library> UserLibraries { get; } = new List<Library>();
    }
}