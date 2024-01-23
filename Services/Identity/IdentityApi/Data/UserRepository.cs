using IdentityApi.Models;

namespace IdentityApi.Data;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AuthenticateUser(int id, string refreshToken)
    {
        var user = _context.Users.FirstOrDefault(user => user.Id == id);
        if(user != null)
        {
            user.RefreshToken = refreshToken;
            _context.SaveChanges();
        }
    }
    public void UnauthenticateUser(int id)
    {
        var user = _context.Users.FirstOrDefault(user => user.Id == id);
        if(user != null)
        {
            user.RefreshToken = "";
            _context.SaveChanges();
        }
    }

    public User Create(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
        
        return user;
    }

    public User GetUserByEmail(string email)
    {
        return _context.Users.FirstOrDefault(user => user.Email == email);
    }

    public User GetUserById(string id)
    {
        return _context.Users.FirstOrDefault(user => user.Id == int.Parse(id));
    }



    public User? UpdateUserInfo(User updateUser)
    {
        var user = _context.Users.FirstOrDefault(user => user.Email == updateUser.Email);
        if(user != null)
        {
            user=updateUser;
            _context.SaveChanges();
            return user;
        }
        return null;
    }

    public User? UpdateUser(User updatedUser)
    {   
        var user = _context.Users.FirstOrDefault(user => user.Id == updatedUser.Id);
        
        if(user != null)
        {
            user = updatedUser;
            _context.SaveChanges();
            return user;
        }
        return null;
    }

    public bool UserExists(string email)
    {
        return _context.Users.FirstOrDefault(user => user.Email == email) != null;
    }

    public IEnumerable<User> GetAllUsers()
    {
        return _context.Users.ToList();
    }
}