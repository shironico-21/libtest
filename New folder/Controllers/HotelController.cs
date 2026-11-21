using finalProject.Models;
using finalProject.Service.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace finalProject.Controllers
{
    // Restricts access to logged-in users by default
    [Authorize]
    public class HotelController : Controller
    {
        private readonly IHotelManage _hotelService;

        public HotelController(IHotelManage hotelService)
        {
            _hotelService = hotelService;
        }

        // --- PUBLIC / GENERAL USER VIEWS ---

        // Allow unauthenticated users to see the list of hotels, if desired
        [AllowAnonymous]
        public IActionResult Index()
        {
            var hotels = _hotelService.GetAllHotels();
            return View(hotels);
        }

        [AllowAnonymous]
        public IActionResult Rooms(int hotelId)
        {
            var rooms = _hotelService.GetRoom_sByHotel(hotelId);
            ViewBag.HotelId = hotelId;
            return View(rooms);
        }

        // --- USER RESERVATION ACTIONS ---

        [HttpGet]
        public IActionResult Book(int roomId)
        {
            var room = _hotelService.GetRoomById(roomId);

            if (room == null)
            {
                return NotFound();
            }

            var reservation = new Reservation_Management { RoomId = roomId , HotelId = room.HotelId };
            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Book(Reservation_Management reservation)
        {
            
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            
            Console.WriteLine($"DEBUG CONTROLLER: Found Claim ID = '{userIdString}'");

           
            if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int userId))
            {
                reservation.CustomerId = userId;
            }
            else
            {
  
               
                ModelState.AddModelError("", "Session error: Could not identify your user account. Please log out and log in again.");
                return View(reservation);
            }

          
            if (_hotelService.Reservetion(reservation))
            {
                return RedirectToAction("Payment", new { reservationId = reservation.Id }); 
            }

            ModelState.AddModelError("", "Booking failed. Please check your dates and capacity.");
            return View(reservation);
        }

        public IActionResult MyReservations()
        {
            var customerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(customerIdStr, out int customerId))
            {
                var reservations = _hotelService.GetReservationsByCustomer(customerId);
                return View(reservations);
            }
            return RedirectToAction("Login", "Auth");
        }

        [HttpPost]
        public IActionResult CancelBooking(int reservationId)
        {
            _hotelService.CancelReservation(reservationId);
            return RedirectToAction(nameof(MyReservations));
        }

        [HttpGet]
        public IActionResult Payment(int reservationId)
        {
            if (reservationId == 0)
            {
                return BadRequest("Invalid Reservation ID. The reservation may not have saved correctly.");
            }
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int customerId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var reservation = _hotelService.GetReservationsByCustomer(customerId)
                .FirstOrDefault(r => r.Id == reservationId);

            if (reservation == null)
            {
               
                return NotFound("Reservation not found for this user.");
            }

            return View(reservation);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessPayment(int reservationId)
        {
            // Update reservation status or perform payment gateway logic here

            TempData["SuccessMessage"] = "Payment successful! Your reservation is fully confirmed.";
            return RedirectToAction(nameof(MyReservations));
        }

        // --- ADMIN ONLY ACTIONS ---


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult CreateHotel()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateHotel(Hotel_Management hotel)
        {
            if (_hotelService.addHotel(hotel))
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Failed to add hotel.");
            return View(hotel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult DeleteHotel(int id)
        {
            
            var hotel = new Hotel_Management { Id = id };
            _hotelService.removeHotel(hotel);
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult CreateRoom(int hotelId)
        {
            
            var roomTypes = _hotelService.GetRoomTypes();

           
            ViewBag.RoomTypesList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(roomTypes, "Id", "Type");

            var room = new Room_Management { HotelId = hotelId };
            return View(room);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateRoom(Room_Management room)
        {
           
            if (_hotelService.addRoom(room))
            {
                return RedirectToAction(nameof(Rooms), new { hotelId = room.HotelId });
            }

            ModelState.AddModelError("", "Failed to add room.");
            return View(room);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult DeleteRoom(int id, int hotelId)
        {
            var room = new Room_Management { Id = id };

            
            _hotelService.removeRoom(room);

          
            return RedirectToAction(nameof(Rooms), new { hotelId = hotelId });
        }
    }
}