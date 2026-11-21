namespace finalProject.Models
{
    public class Reservation_Management
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public int HotelId { get; set; }
        public Hotel_Management Hotel { get; set; } = null!;
        public int RoomId { get; set; }
        public Room_Management Room { get; set; } = null!;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal totalPrice { get; set; }
        public enum ReservationStatus
        {
            Pending = 0,
            Confirmed = 1,
            Cancelled = 2
        }
        public ReservationStatus Status { get; set; }
    }
}
