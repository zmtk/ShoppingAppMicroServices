using IdentityApi.Data;
using IdentityApi.Dtos;
using IdentityApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Auth;

namespace IdentityApi.Controllers;

[Route("api/identity/[controller]")]
[ApiController]
public class AuthController : Controller
{
    private readonly IUserRepository _userRepository;

    public AuthController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterDto registerDto)
    {
        Console.WriteLine(registerDto.Email + registerDto.FirstName + registerDto.LastName + registerDto.Password);
        bool userExists = _userRepository.UserExists(registerDto.Email);
        if (userExists)
            return Conflict("A User with this email already exists.");

        var user = new User
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
        };

        _userRepository.Create(user);

        return Ok("success");

    }

    [HttpPost("login")]
    public IActionResult Login(LoginDto loginDto)
    {
        Console.WriteLine(loginDto.Email + loginDto.Password);

        var user = _userRepository.GetUserByEmail(loginDto.Email);

        if (user == null) return NotFound(new { message = "Invalid Credentials" });

        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            return Unauthorized(new { message = "Invalid Credentials" });

        string[] roles = user.Roles.Split(",");
        string accessToken = Authorize.GenerateToken(user.Id.ToString(), user.Email, roles);
        string refreshToken = Authorize.GenerateRefreshToken(user.Id.ToString(), user.Email, roles);

        Response.Cookies.Append("refreshtoken", refreshToken, new CookieOptions { HttpOnly = true });

        _userRepository.AuthenticateUser(user.Id, refreshToken);

        return Ok(new { accessToken });
    }

    [HttpGet("user")]
    public IActionResult User()
    {
        try
        {
            var accessToken = Request.Headers.Authorization;
            bool accessTokenExist = !string.IsNullOrWhiteSpace(accessToken);

            string? uid = accessTokenExist ? Authorize.GetUserId(accessToken) : null;
            if (uid == null)
                throw new SecurityTokenExpiredException();

            var user = _userRepository.GetUserById(uid);

            return Ok(user);


        }
        catch (SecurityTokenExpiredException _)
        {
            return Unauthorized(new { message = "Access token is expired" });
            // return RefreshAccessToken();
        }

    }

    [HttpGet("refresh")]
    public IActionResult RefreshAccessToken()
    {

        var refreshToken = Request.Cookies["refreshtoken"];
        bool refreshTokenExist = !string.IsNullOrWhiteSpace(refreshToken);
        
        try
        {
            if (!refreshTokenExist)
            {
                Console.WriteLine("RefreshTokenDoesntExist");
                throw new SecurityTokenValidationException();
            }
            var token = Authorize.VerifyRefreshToken(refreshToken);
            string uid = token.Issuer;
            var user = _userRepository.GetUserById(uid);

            if (user == null)
                throw new KeyNotFoundException();

            if (user.RefreshToken != refreshToken)
                throw new SecurityTokenValidationException();

            string[] roles = user.Roles.Split(",");
            string accessToken = Authorize.GenerateToken(user.Id.ToString(), user.Email, roles);

            return Ok(new { accessToken });
        }
        catch (SecurityTokenValidationException _)
        {
            return Unauthorized(new { message = "Refresh Token is not valid" });
        }
        catch (KeyNotFoundException _)
        {
            return NotFound(new { message = "User not found" });
        }


    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        //get user id
        try
        {
            var refreshToken = Request.Cookies["refreshtoken"];
            bool refreshTokenExist = !string.IsNullOrWhiteSpace(refreshToken);

            if (!refreshTokenExist)
                throw new SecurityTokenValidationException();

            var token = Authorize.VerifyRefreshToken(refreshToken);
            int userId = int.Parse(token.Issuer);

            Response.Cookies.Delete("accesstoken");
            Response.Cookies.Delete("refreshtoken");

            _userRepository.UnauthenticateUser(userId);

            return Ok();

        }
        catch (SecurityTokenException)
        {
            return Unauthorized();
        }
        catch (Exception _)
        {
            return BadRequest();
        }
    }

    [HttpPost("updateuser")]
    public IActionResult UpdateUserInfo(UpdateUserDto updateUserDto)
    {
        try
        {
            var accessToken = Request.Headers.Authorization;
            bool accessTokenExist = !string.IsNullOrWhiteSpace(accessToken);

            string? uid = accessTokenExist ? Authorize.GetUserId(accessToken) : null;
            if (uid == null)
                throw new SecurityTokenExpiredException();

            var user = _userRepository.GetUserById(uid);

            Console.WriteLine("--> New Acquired Creds");
            Console.WriteLine("First Name: " + updateUserDto.FirstName);
            Console.WriteLine("Last Name: " + updateUserDto.LastName);
            Console.WriteLine("Email: " + updateUserDto.Email);
            Console.WriteLine("Phone Number: " + updateUserDto.PhoneNumber);
            Console.WriteLine("Date Of Birth: " + updateUserDto.DateOfBirth);

            Console.WriteLine("");

            Console.WriteLine("--> Old Creds");
            Console.WriteLine("First Name: " + user.FirstName);
            Console.WriteLine("Last Name: " + user.LastName);
            Console.WriteLine("Email: " + user.Email);
            Console.WriteLine("Phone Number: " + user.PhoneNumber);
            Console.WriteLine("Date Of Birth: " + user.DateOfBirth);

            user.FirstName = updateUserDto.FirstName ?? user.FirstName;
            user.LastName = updateUserDto.LastName ?? user.LastName;
            user.Email = updateUserDto.Email ?? user.Email;
            user.PhoneNumber = updateUserDto.PhoneNumber ?? user.PhoneNumber;
            user.DateOfBirth = updateUserDto.DateOfBirth ?? user.DateOfBirth;

            var updatedUser = _userRepository.UpdateUser(user);

            return Ok(new { message = "updated", updatedUser });

            // return Ok();

        }
        catch (SecurityTokenExpiredException _)
        {
            return Unauthorized(new { message = "Access token is expired" });
            // return RefreshAccessToken();
        }


    }

    // Move this later
    [HttpPost("promote")]
    public IActionResult Promote(PromoteDto promoteDto)
    {
        string[] availableRoles = { "admin", "user", "editor" };
        if (!availableRoles.Contains(promoteDto.Role)) return BadRequest(new { message = "Role not available" });

        var user = _userRepository.GetUserByEmail(promoteDto.Email);
        if (user == null) return NotFound(new { message = "User Not Found" });

        var userRoles = user.Roles.Split(",");

        if (userRoles.Contains(promoteDto.Role)) return BadRequest(new { message = "User is already in group" });

        if (string.IsNullOrWhiteSpace(user.Roles))
        {
            user.Roles = promoteDto.Role;
        }
        else
        {
            user.Roles = string.Join(",", userRoles.Append(promoteDto.Role).ToArray());
        }
        // Add new role then convert to string
        var updatedUser = _userRepository.UpdateUserInfo(user);

        return Ok(new { message = "promoted", newrole = promoteDto.Role, updatedUser });
    }


    [HttpGet("users")]
    public IActionResult Users()
    {

        try
        {
            var accessToken = Request.Headers.Authorization;
            bool accessTokenExist = !string.IsNullOrWhiteSpace(accessToken);

            string? uid = accessTokenExist ? Authorize.GetUserId(accessToken) : null;

            if (uid == null)
                throw new SecurityTokenExpiredException();

            var user = _userRepository.GetUserById(uid);
            var authorizedUser = user.Roles.Split(",").Contains("admin");

            if (!authorizedUser)
                return Unauthorized(new { message = "You are not authorized." });

            var users = _userRepository.GetAllUsers();

            return Ok(users);

        }
        catch (SecurityTokenExpiredException _)
        {
            return Unauthorized(new { message = "Access token is expired" });
            // return RefreshAccessToken();
        }

    }
}