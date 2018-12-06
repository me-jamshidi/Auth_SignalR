using System.Collections.Generic;
using System.Security.Claims;
using Auth_SignalR.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Auth_SignalR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        List<User> _users;
        public UsersController()
        {
            _users = new List<User> () 
            {
                new User()
                {
                    Id= 1,
                    Username = "mehran",
                    Password = "123456"
                },
                new User()
                {
                    Id= 2,
                    Username = "test",
                    Password = "test"
                }
            };
        }

        [HttpPost("login")]
        public User Login([FromForm]string username, [FromForm]string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var user = _users.Find(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "User")
                };

                var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    //AllowRefresh = <bool>,
                    // Refreshing the authentication session should be allowed.

                    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.

                    //IsPersistent = true,
                    // Whether the authentication session is persisted across 
                    // multiple requests. Required when setting the 
                    // ExpireTimeSpan option of CookieAuthenticationOptions 
                    // set with AddCookie. Also required when setting 
                    // ExpiresUtc.

                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.

                    //RedirectUri = <string>
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.
                };

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                                        new ClaimsPrincipal(claimsIdentity), 
                                        authProperties
                                        ).Wait();
                
                user.Password = "";
                return user;
            }

            return null;
        }

        [HttpPost("logout")]
        public void Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
        }
        
    }
}