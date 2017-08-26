using GigHub.Models;
using GigHub.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GigHub.Controllers
{
    public class GigsController : Controller
    {
        private ApplicationDbContext _context;

        public GigsController(){
            _context = new ApplicationDbContext();
        }

        [Authorize]
        public ActionResult Create()
        {
            var viewModel = new GigFormViewModel
            {
                Genres = _context.Genres.ToList(),
                Heading = "Add a Gig"
            };

            return View("GigForm", viewModel);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            var userId = User.Identity.GetUserId();

            var gig = _context.Gigs.Single(g => g.Id == id && g.ArtistId == userId);

            var viewModel = new GigFormViewModel
            {
                Genres = _context.Genres.ToList(),
                Id = gig.Id,
                Date = gig.DateTime.ToString("d MMM yyyy"),
                Time = gig.DateTime.ToString("HH:mm"),
                Genre = gig.GenreId,
                Venue = gig.Venue,
                Heading = "Edit a Gig"
            };

            return View("GigForm",viewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(GigFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Genres = _context.Genres.ToList();
                return View("GigForm", viewModel);
            }

            var gig = new Gig
            {
                ArtistId = User.Identity.GetUserId(),
                DateTime = viewModel.GetDateTime(),
                GenreId = viewModel.Genre,
                Venue = viewModel.Venue
            };
            _context.Gigs.Add(gig);
            _context.SaveChanges();
            return RedirectToAction("Mine", "Gigs");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Update(GigFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Genres = _context.Genres.ToList();
                return View("GigForm", viewModel);
            }

            var userId = User.Identity.GetUserId();

            var gig = _context.Gigs
                .Include(g => g.Attendances.Select(a => a.Attendee))
                .Single(g => g.Id == viewModel.Id && g.ArtistId == userId);

            gig.DateTime = viewModel.GetDateTime();
            gig.GenreId = viewModel.Genre;
            gig.Venue = viewModel.Venue;
           
            _context.SaveChanges();
            return RedirectToAction("Mine", "Gigs");
        }

        [Authorize]
        public ActionResult Attending()
        {
            var userId = User.Identity.GetUserId();

            var gigs = _context.Attendances
                .Where(a => a.AttendeeId == userId)
                .Select(a => a.Gig)
                .Include(g => g.Artist)
                .Include(g => g.Genre)
                .ToList();

            var attendancies = this._context.Attendances
              .Where(a => a.AttendeeId == userId && a.Gig.DateTime > DateTime.Now)
              .ToList()
              .ToLookup(a => a.GigId);

            var model = new GigsViewModel
            {
                UpcomingGigs = gigs,
                SchowActions = User.Identity.IsAuthenticated,
                Heading = "Gigs I'm coming",
                Attendancies = attendancies
            };
            return View("Gigs", model);
        }

        [Authorize]
        public ActionResult Mine()
        {
            var userId = User.Identity.GetUserId();

            var gigs = _context.Gigs
               .Where(g => g.ArtistId == userId && g.DateTime > DateTime.Now && !g.IsCanceled)
               .Include (g => g.Genre)
               .ToList();
            return View(gigs);
        }

        public ActionResult Search(GigsViewModel viewModel)
        {
            return RedirectToAction("Index", "Home", new { query = viewModel.SearchTerm });
        }

        public ActionResult Details(int id)
        {
            var gig = _context.Gigs.Include("Artist")
                .FirstOrDefault(g => g.Id == id);

            var model = new GigDetailsViewModel { Gig = gig };

            if (gig == null)
            {
                return this.HttpNotFound();
            }
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();

                model.IsAttending = this._context.Attendances.Any(a => a.GigId == id && a.AttendeeId == userId);

                model.IsFollowing = this._context.Followings.Any(a => a.FollowerId == userId && a.FolloweeId == gig.ArtistId);
               
            }
            return this.View("Details", model);
        }
    }
}