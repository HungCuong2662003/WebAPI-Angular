using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Data
{
    public class RoomPlayers
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid RoomId { get; set; }

        [Required]
        public string UserID { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("RoomId")]
        public virtual Rooms Room { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
    }
}
