using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Order_Genie.Data;
using Order_Genie.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Order_Genie.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseContext _DatabaseContext = new DatabaseContext();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return Ok("OK");
        }
        public string GeneratePass(string Password)
        {
            Utility.CreatePasswordHash(Password, out byte[] passwordHash, out byte[] passwordSalt);
            return string.Join("", Enumerable.Range(0, passwordHash.Length).Select(p =>
                          $"{(char)passwordHash[p]}"));
        }
        [HttpPost]
        public async Task<IActionResult> Register(string Email, string Password, string FullName)
        {
            DatabaseContext dbContext = _DatabaseContext;
            
            
            if(dbContext.Users.FirstOrDefault(U=>U.Email==Email) is Users user) 
            {
                
                user.Updated = DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
                dbContext.Update(user);
            }
            else
            {
                
                var newUser = new Users
                {
                    Email = Email,
                    Password = GeneratePass(Password),
                    FullName = FullName,
                    Updated = DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture)
                };

                dbContext.Users.Add(newUser);
            };
            //Utility.CreatePasswordHash(Password, out byte[] passwordHash, out byte[] passwordSalt);
            await dbContext.SaveChangesAsync();

            return View("Login");
            //return Ok("OK");
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Signup()
        {
            return View();
        }
        [HttpGet("denied")]
        public IActionResult Denied()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost("login")]
        public async Task <IActionResult> Validate(string username, string password, string returnUrl)
        {
            DatabaseContext dbContext = _DatabaseContext;

            if(dbContext.Users.FirstOrDefault(U=>U.Email==username&&U.Password==GeneratePass(password)) is Users user)
            {
                //Create new instance of claim
                var claims = new List<Claim>
                {
                    //Generate Key-Value Pairs
                    new Claim("username", username),
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim(ClaimTypes.Role, "Admin")
                };
                //in the future create a model for claims
                //User identiy with claims identity 
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //Authenitcation Ticket
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);
                return Redirect("Home/Dashboard");
            }

        
            TempData["Error"] = "Error. Username or Password is invalid"; 
            return View("login");
        }
        //Authorize attribute will validate whether the user is logged in.
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            
            return Redirect("/");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
