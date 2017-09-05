using GigHub.Models;
using GigHub.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;

namespace GigHub.Controllers
{
    using GigHub.Repositories;

    using Microsoft.AspNet.Identity;

    public class HomeController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UnitOfWork _unitOfWork;


        public HomeController(UnitOfWork unitOfWork)
        {
            _context = new ApplicationDbContext();
            this._unitOfWork = new UnitOfWork(_context);
        }

    public ActionResult Index(string query=null)
        {
            var gigs = _context.Gigs
                .Include(g => g.Artist)
                .Include(g => g.Genre)
                .Where(g => g.DateTime > DateTime.Now && !g.IsCanceled);
                

            if (!String.IsNullOrWhiteSpace(query))
            {
                gigs = gigs.Where(g =>
                        g.Artist.Name.Contains(query) ||
                        g.Genre.Name.Contains(query) ||
                        g.Venue.Contains(query));
            }

            var userId = User.Identity.GetUserId();

            var attendancies = _unitOfWork.Attendances
                .GetFutureAttendances(userId)
                .ToLookup(a => a.GigId);

            var viewModel = new GigsViewModel
            {
                SchowActions = User.Identity.IsAuthenticated,
                UpcomingGigs = gigs,
                Heading = "Upcoming Gigs",
                SearchTerm = query,
                Attendancies = attendancies
            };
            return View("Gigs", viewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}