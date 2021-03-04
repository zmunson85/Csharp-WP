using System;
using System.Linq; // allows us to access query methods(FirstOrDefault)
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace WeddingPlanner.Controllers
{
    public class UsersController : Controller
    {

        //DB setup
        private readonly MyContext _context;

        //dependency injection
        public UsersController(MyContext myContext)
        {
            _context = myContext;
        }

        //for each route this controller is to handle:
        [HttpGet]       //type of request
        [Route("")]     //associated route string (exclude the leading /)
        public IActionResult LoginReg()
        {
            return View();
        }

        [HttpPost("users")]
        public IActionResult Register(User userToCreate)
        {

            if (!ModelState.IsValid)
            {
                return View("LoginReg");
            }

            var existingUser = _context
                .Users
                .FirstOrDefault(user => user.Email == userToCreate.Email);


            if (existingUser != null) //if there is a existing user
            {
                ModelState.AddModelError("Email", "Email already registered");
                return View("LoginReg");
            }

            //hash the password
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            userToCreate.Password = Hasher.HashPassword(userToCreate, userToCreate.Password);

            //Save the user to the DB
            _context.Add(userToCreate);
            _context.SaveChanges();

            //Save the user ID to session
            HttpContext.Session.SetInt32("UserId", userToCreate.UserId);

            // note that we'are sending the user to a different controller
            return RedirectToAction("Dashboard", "Home");


        }

        [HttpPost("users/login")]
        public IActionResult Login(LoginUser userToLogin)
        {
            if (ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var foundUser = _context.Users.FirstOrDefault(user => user.Email == userToLogin.LoginEmail);

                // If no user exists with provided email
                if (foundUser == null)
                {
                    // Add an error to ModelState and return to View!
                    Console.WriteLine("User wasn't found");
                    ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                    return View("LoginReg");
                }

                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();

                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(userToLogin, foundUser.Password, userToLogin.LoginPassword);

                // result can be compared to 0 for failure
                if (result == 0)
                {
                    //if an email exist // handle failure (this should be similar to how "existing email" is handled)
                    //If there are errors in ModelState It will Kick Back To Login/RegPage
                    Console.WriteLine("Password does not match");
                    ModelState.AddModelError("LoginPassword", "Invalid Email/Password");
                    return View("LoginReg");
                }

                //put the user ID into session
                HttpContext.Session.SetInt32("UserId", foundUser.UserId);

            }
            //redirect 
            return RedirectToAction("Dashboard", "Home");
        }

        [HttpGet("users/logout")]
        public IActionResult Logout()
        {   
            //clear the seesion
            HttpContext.Session.Clear();
            return RedirectToAction("LoginReg");
        }

    }
}
