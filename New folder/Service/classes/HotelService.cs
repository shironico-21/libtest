using finalProject.Models;
using finalProject.Repositry.interfaces;
using finalProject.Service.interfaces;
using System;
using System.Collections.Generic;
using static finalProject.Models.Reservation_Management;
using static finalProject.Models.Room_Management;

namespace finalProject.Service.classes
{
    public class HotelService : IHotelManage
    {
        private readonly Ihotelrepo _hotelRepo;
        private readonly IRoomRepo _roomRepo;
        private readonly IResrvationRepo _reservationRepo;
        private readonly IRoomTypeRepo _roomtypeRepo;
        private readonly ICustomerRepo _customerRepo;
        private readonly IUserContract _userRepo;

        public HotelService(Ihotelrepo hotelRepo, IUserContract userRepo, IRoomRepo roomRepo, IResrvationRepo reservationRepo, IRoomTypeRepo roomtypeRepo, ICustomerRepo customerRepo)
        {
            _hotelRepo = hotelRepo;
            _roomRepo = roomRepo;
            _reservationRepo = reservationRepo;
            _roomtypeRepo = roomtypeRepo;
            _customerRepo = customerRepo;
            _userRepo = userRepo;
        }

        // --- CORE BUSINESS LOGIC: CREATING A RESERVATION ---
        public bool Reservetion(Reservation_Management reservation)
        {
            // 1. Get the USER ID that we passed from the Controller
            int loggedInUserId = reservation.CustomerId;
            var user = _userRepo.GetbyId(loggedInUserId);

            if (user == null)
            {
                Console.WriteLine("DEBUG: Failed because User is null.");
                return false; // User not found
            }

            // 2. Check if this user is already in the Customers table by matching their Email
            var allCustomers = _customerRepo.GetAll();
            var existingCustomer = allCustomers.FirstOrDefault(c => c.Email == user.Email);

            if (existingCustomer != null)
            {
                
                reservation.CustomerId = existingCustomer.Id;
            }
            else
            {
                
                var newCustomer = new Customer
                {
                    NationalID = user.Email,
                    Name = user.Name,
                    Email = user.Email
                };

                _customerRepo.add(newCustomer);

               
                reservation.CustomerId = newCustomer.Id;
            }
            // 1. Extra Validation: Date logic
            if (reservation.CheckOutDate <= reservation.CheckInDate) return false;
            if (reservation.CheckInDate.Date < DateTime.Today) return false;

            // Fetch the room and its type from the database
            var room = _roomRepo.GetbyId(reservation.RoomId);
            if (room == null) return false;
            reservation.HotelId = room.HotelId;

            var roomType = _roomtypeRepo.GetbyId(room.RoomTypeId);
            if (roomType == null) return false;

            // 2. Business Rule: Room cannot be reserved if currently unavailable
            if (room.Status != RoomStatus.Available)
            {
                return false;
            }

            // 3. Business Rule: Number of guests should not exceed capacity
            if (reservation.NumberOfGuests > room.Capacity)
            {
                return false;
            }

            // 4. Business Rule: Total price calculated automatically
            int totalNights = (reservation.CheckOutDate - reservation.CheckInDate).Days;
            reservation.totalPrice = totalNights * roomType.Price;

            // 5. Business Rule: Update reservation and room status
            reservation.Status = ReservationStatus.Confirmed;
            room.Status = RoomStatus.Reserved; // Or RoomStatus.Reserved depending on your enum

            // Save changes via repositories
            _roomRepo.update(room);
            
            _reservationRepo.add(reservation);

            return true;
        }

        // --- CORE BUSINESS LOGIC: CANCELLING A RESERVATION ---
        public bool CancelReservation(int reservationId)
        {
            var reservation = _reservationRepo.GetbyId(reservationId);
            if (reservation == null || reservation.Status == ReservationStatus.Cancelled)
                return false;

            var room = _roomRepo.GetbyId(reservation.RoomId);
            if (room != null)
            {
                // Update room back to available
                room.Status = RoomStatus.Available;
                _roomRepo.update(room);
            }

            // Update reservation status
            reservation.Status = ReservationStatus.Cancelled;
            _reservationRepo.update(reservation);

            return true;
        }

        // --- CORE BUSINESS LOGIC: CHECKING A ROOM BEFORE BOOKING ---
        public bool CheckRoom(Room_Management room, Reservation_Management reservation)
        {
            if (room == null || reservation == null) return false;

            // Check status and capacity rules without saving to the database
            bool isAvailable = room.Status == RoomStatus.Available;
            bool fitsCapacity = reservation.NumberOfGuests <= room.Capacity;
            bool validDates = reservation.CheckOutDate > reservation.CheckInDate && reservation.CheckInDate >= DateTime.Today;

            return isAvailable && fitsCapacity && validDates;
        }

        // --- STANDARD CRUD OPERATIONS ---

        public bool addHotel(Hotel_Management hotel)
        {
            if (hotel == null) return false;
            _hotelRepo.add(hotel);
            return true;
        }

        public bool addRoom(Room_Management room)
        {
            if (room == null) return false;
            _roomRepo.add(room);
            return true;
        }

        public bool removeHotel(Hotel_Management hotel)
        {
            if (hotel == null) return false;
            _hotelRepo.delete(hotel); // Assuming Delete takes an ID
            return true;
        }

        public bool removeRoom(Room_Management room)
        {
            if (room == null) return false;
            _roomRepo.delete(room);
            return true;
        }

        public IEnumerable<Hotel_Management> GetAllHotels()
        {
            return _hotelRepo.GetAll();
        }

        public IEnumerable<Customer> customers()
        {
            return _customerRepo.GetAll();
        }

        

        public IEnumerable<Reservation_Management> GetReservationsByCustomer(int loggedInUserId)
        {
            var user = _userRepo.GetbyId(loggedInUserId);
            if (user == null) return Enumerable.Empty<Reservation_Management>();

            // 2. Find the corresponding Customer record by matching the email
            var customer = _customerRepo.GetAll().FirstOrDefault(c => c.Email == user.Email);
            if (customer == null) return Enumerable.Empty<Reservation_Management>();

            // 3. Return reservations matching the actual Customer ID (not the User ID)
            return _reservationRepo.GetAll().Where(r => r.CustomerId == customer.Id);
        }

        public IEnumerable<Room_Management> GetRoom_sByHotel(int hotelId)
        {
           return _roomRepo.GetAll().Where(r => r.HotelId == hotelId);
        }

        public bool UpdateRoomAvailability(Room_Management room, bool isAvailable)
        {
            if (room == null) return false;

            room.Status = isAvailable ? RoomStatus.Available : RoomStatus.Maintenance;
            _roomRepo.update(room);
            return true;
        }
        public IEnumerable<Room_Type_Management> GetRoomTypes()
        {
            return _roomtypeRepo.GetAll();
        }
        public Room_Management GetRoomById(int roomid)
        {
            return _roomRepo.GetbyId(roomid);
        }



    }
}