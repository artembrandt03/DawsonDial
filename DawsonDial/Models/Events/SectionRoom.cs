using DawsonDial.Models.Rooms;

namespace DawsonDial.Models.Events
{
    public class SectionRoom
    {
        public Guid SectionRoomId { get; set; }
        public Guid SectionId { get; set; }
        public Guid RoomId { get; set; }
        public string RoomKey { get; set; } = string.Empty;

        public virtual Section Section { get; set; } = null!;
        public virtual Room Room { get; set; } = null!;
    }

}
