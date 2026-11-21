using finalProject.Dto;
using finalProject.Models;

namespace finalProject.Service.interfaces
{
    public interface IAutthContract
    {
       
        bool Register(AuthUserDtos authUser);
        User login(loginDtos login);
        bool logout();

    }
}
