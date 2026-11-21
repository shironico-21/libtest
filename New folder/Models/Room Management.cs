namespace finalProject.Models
{
    public class Room_Management
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int Capacity { get; set; }
        
        public enum RoomStatus
        {
            Available = 0,
            Reserved = 1,     
            Maintenance = 2
        }
        public int RoomTypeId { get; set; }
        public Room_Type_Management RoomType { get; set; } = null!;
        public int HotelId { get; set; }
        public Hotel_Management Hotel { get; set; } = null!;
        public RoomStatus Status { get; internal set; }
    }
}
