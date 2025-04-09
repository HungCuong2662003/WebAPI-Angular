using System.ComponentModel.DataAnnotations;

namespace WebAPI.Model
{
    public class RoomPlayerModel
    {
        [Required]
        public Guid RoomId { get; set; }

        [Required]
        public string UserID { get; set; }
    }
}
