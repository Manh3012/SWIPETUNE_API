using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Runtime.InteropServices;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Models
{
    public class Account : IdentityUser<Guid>
    {
        [Key]
        public override Guid Id { get; set; }
        public DateTime DOB { get; set; }
        public string? Gender { get;set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime Verified_At { get; set; }
        public bool isDeleted { get; set; }
        public DateTime Created_At { get; set; }

        public bool Status { get; set; }

        public virtual ICollection<Playlist>? Playlists { get; set;
        }

    }
}
