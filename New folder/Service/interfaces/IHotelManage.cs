using finalProject.Models;

namespace finalProject.Service.interfaces
{
    public interface IHotelManage
    {
        bool Reservetion(Reservation_Management reservation);
        bool CancelReservation(int reservationId);
        bool CheckRoom(Room_Management room, Reservation_Management reservation);
        bool UpdateRoomAvailability(Room_Management room, bool isAvailable);
       
        bool addRoom(Room_Management room);
        bool removeRoom(Room_Management room);
        bool addHotel(Hotel_Management hotel);
        bool removeHotel(Hotel_Management hotel);
        IEnumerable<Reservation_Management> GetReservationsByCustomer(int customerId);
        IEnumerable<Room_Management>GetRoom_sByHotel(int hotelId);
        IEnumerable<Hotel_Management> GetAllHotels();
        IEnumerable<Customer> customers();
        IEnumerable<Room_Type_Management> GetRoomTypes();
        Room_Management GetRoomById(int roomid);
    }
}