using finalProject.Dto;
using finalProject.Models;
using finalProject.Repositry.interfaces;
using finalProject.Service.interfaces;

namespace finalProject.Service.classes
{
    public class AuthServiece : IAutthContract
    {
        private readonly IUserContract _userRepo;
        public AuthServiece(IUserContract userRepo)
        {
            _userRepo = userRepo;
        }

        public User login(loginDtos login)
        {
            var user = _userRepo.UserExists(login.email);
            if (user == null)
            {
                return null;
            }
            if (user.Password != login.Password)
            {
                return null;
            }
            return user; 
        }

        public bool logout()
        {
           
            return true; 
        }

        public bool Register(AuthUserDtos authUser)
        {
           var user = new Models.User
            {
                Name = authUser.Name,
                Email = authUser.email,
                Password = authUser.Password
            };
            var existingUser = _userRepo.UserExists(user.Email);
            if (existingUser != null)
            {
                return false;
            }

            var result = _userRepo.Add(user);
            return result > 0;
        }
    }
}
