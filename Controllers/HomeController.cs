using System;
using System.Linq; //allows us to access query methods(FirstOrDefault)
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;//allow us to use include
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyContext _context;

        public HomeController(MyContext myContext)
        {
            _context = myContext;
        }


        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            //get the UserId from session and store in userId
            int? userId = HttpContext.Session.GetInt32("UserId"); //? meaning we are not sure if there is a userId in session

            if (userId == null)
            {
                //no user present
                return RedirectToAction("LoginReg", "Users");
            }

            //put the userId in ViewBag to use in cshtml
            ViewBag.User = _context
            .Users
            .Find(userId);

            //display the weddings from view bag
            ViewBag.Wedding = _context
            .Weddings
            .Include(wedding => wedding.PlanedBy)
            .Include(wedding => wedding.Rsvps)
            .ToList();

            return View();
        }

        [HttpGet("weddings/new")]
        public IActionResult NewWeddingPage()
        {
            int? userId = HttpContext.Session.GetInt32("UserId"); //get the logged in user

            if (userId == null)
            {
                return RedirectToAction("LoginReg", "Users"); //action: go to LoginReg page, controller: UsersController//
            }

            ViewBag.User = _context
                .Users
                .Find(userId);

            return View();
        }

        [HttpPost("weddings")]
        public IActionResult CreateWedding(Wedding weddingToCreate)
        {

            //validate the date to make sure it is in the future
            if (weddingToCreate.Date < DateTime.Now)
            {
                ModelState.AddModelError("Date", "Please enter a future date!");
            }

            //is the info 
            if (ModelState.IsValid)
            {

                //get userid for the wedding created by user in session
                weddingToCreate.UserId = HttpContext.Session.GetInt32("UserId").GetValueOrDefault();
                //establishing a place to set up the api call, using the var address
                var address= weddingToCreate.WeddingAddress;//the wedding being passed in is going to be stored in as a string for the address.
                                               //SET UP THE MAP API WITH COHORT HELP ---SH---
                weddingToCreate.WeddingAddress="https://www.google.com/maps/embed/v1/place?key=AIzaSyAnyw7HwxXYNpxsoplJ5h9N6YP3J0l6ohA&q=" + address; 
                //we have linked the api address in and stored it as the wedding address.
                //add and save changes to the DB
                _context.Add(weddingToCreate);
                _context.SaveChanges();

                // after we add the weddingToCreate in the _context and save the change, then the WeddingId will get created
                // to send to the wedding page
                return Redirect($"/weddings/{weddingToCreate.WeddingId}");
            }

            int? userId = HttpContext.Session.GetInt32("UserId").GetValueOrDefault();
            
            ViewBag.User = _context
                .Users
                .Find(userId);

            return View("NewWeddingPage");

        }

        //After successful create the wedding, the wedding detail goes to /weddings/{weddingToCreate.WeddingId}
        [HttpGet("weddings/{id}")]
        public IActionResult WeddingPage(int id)
        {
            //get the wedding to display
            ViewBag.Wedding = _context
                .Weddings
                //one to many relationship
                .Include(wedding => wedding.PlanedBy)
                //rsvps show up first on the page
                .Include(wedding => wedding.Rsvps)
                //users will shop up after we show the rsvps
                .ThenInclude(rsvp => rsvp.User)
                .FirstOrDefault(wedding => wedding.WeddingId == id); //take the wedding, whose WeddingId == id

            //go to the wedding display page
            return View();
        }

        // User in session add a RSVP to the wedding
        [HttpPost("weddings/{id}/rsvps")]
        public IActionResult AddRsvp(int id)
        {
            var rsvpToAdd = new Rsvp();

            // Rsvp model has 3 fields(RsvpId, UserId, WeddingId)

            rsvpToAdd.UserId = HttpContext.Session.GetInt32("UserId").GetValueOrDefault(); // add UserId
            rsvpToAdd.WeddingId = id; //add WeddingId

            _context.Add(rsvpToAdd);
            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        // User in session UN-RSVP to the wedding
        [HttpPost("weddings/{id}/rsvps/delete")]
        public IActionResult RemoveRsvp(int id)
        {
            //get the loggedin userId
            var userId = HttpContext.Session.GetInt32("UserId").GetValueOrDefault();
            //get the Rsvp that need to remove
            var rsvpToRemove = _context
                .Rsvps
            //get the first like where the wedding id matches the id we took in the form, and the user id in session matches the user id
                .FirstOrDefault(rsvp => rsvp.WeddingId == id && rsvp.UserId == userId);

            _context.Rsvps.Remove(rsvpToRemove);
            _context.SaveChanges();

            return RedirectToAction("Dashboard");
            }
        
        // User who posted can delete
        [HttpPost("weddings/{id}/delete")]
        public IActionResult DeleteWedding(int id)
        {
            //get the wedding that need to remove
            var weddingToDelete = _context
                .Weddings
            //get the first like where the movie id matches the id we took in the form, and the user id in session matches the user id
                .Find(id);

            _context.Weddings.Remove(weddingToDelete);
            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }
    }
}
