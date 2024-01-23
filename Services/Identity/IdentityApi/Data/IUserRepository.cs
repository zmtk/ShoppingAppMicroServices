using IdentityApi.Models;

namespace IdentityApi.Data;

public interface IUserRepository
{
    bool UserExists(string email);
    User Create(User user);
    User GetUserByEmail(string email);
    User GetUserById(string id);
    User UpdateUser(User user);
    User UpdateUserInfo(User user);
    void AuthenticateUser(int id, string refreshToken);
    void UnauthenticateUser(int id);
    
    IEnumerable<User> GetAllUsers();
    
}