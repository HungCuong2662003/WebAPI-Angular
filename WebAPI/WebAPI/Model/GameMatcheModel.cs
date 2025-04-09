using System.ComponentModel.DataAnnotations;

namespace WebAPI.Model
{
    public class GameMatcheModel
    {
        [Required]
        public Guid RoomId { get; set; }
    }
}
