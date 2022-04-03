using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;
using TheGreatFinChallenge.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheGreatFinChallenge.Models.Data;
using TheGreatFinChallenge.Xtra;
using TheGreatFinChallenge.Models.Views;

namespace TheGreatFinChallenge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TGFCContext _context;

        public HomeController(ILogger<HomeController> logger, TGFCContext context)
        {
            _logger = logger;
            _context = context;
        }


        public IActionResult Index() => View();

        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        public async Task<IActionResult> Validate(string email, string password, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var user = Queries.GetUserByEmail(_context, email);
            if (user != null)
            {
                string passwordGenerated = Hash.HashPassword(password, user.Salt);
                if (passwordGenerated == user.Password)
                {
                    var claims = new List<Claim>();
                    claims.Add(new Claim("UserId", user.UserId.ToString()));
                    claims.Add(new Claim("FirstName", user.FirstName));
                    claims.Add(new Claim("LastName", user.LastName));
                    claims.Add(new Claim("Email", user.Email));
                    if (user.Admin == true) claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    return RedirectToActionPermanent(returnUrl.Split("_")[1], returnUrl.Split("_")[0]);
                }
                else TempData["PasswordError"] = "Password did not match with this account.";
            }
            else TempData["EmailError"] = "We didn't recognize this email.";
            return View("Login");
        }


        public IActionResult Register() => View(new RegisterView(_context));


        public async Task<IActionResult> RegisterUser(string firstName, string lastName, 
            string email, string password, string passwordConfirmed, int departmentId, 
            bool gdpr, string returnUrl)
        {
            if(password != passwordConfirmed)
            {
                TempData["PasswordError"] = "Password's don't match.";
                return View("Register", new RegisterView(_context));
            }
            if (!Hash.PasswordMeetsRequirements(password))
            {
                TempData["PasswordReqError"] = "Make sure that the password meets the requirements.";
                return View("Register", new RegisterView(_context));
            }
            var user = Queries.GetUserByEmail(_context, email);
            if (user != null)
            {
                TempData["EmailError"] = "Email is already in use.";
                return View("Register", new RegisterView(_context));
            }
            var salt = Hash.GenerateSalt();
            var saltString = Hash.ConvertSaltToString(salt);
            string passwordEncrypted = Hash.HashPassword(password, salt);

            User u = new User(firstName, lastName, false, email, passwordEncrypted, saltString, gdpr, departmentId);
            _context.User.Add(u);
            await _context.SaveChangesAsync();
            return await Validate(email, password, returnUrl);
        }
    }
}