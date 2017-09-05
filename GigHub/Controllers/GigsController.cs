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
    using GigHub.Repositories;

    public class GigsController : Controller
    {
        private ApplicationDbContext _context;

        private readonly UnitOfWork _unitOfWork;

        public GigsController()
        {
            _context = new ApplicationDbContext();
            this._unitOfWork = new UnitOfWork(_context);
        }

        [Authorize]
        public ActionResult Create()
        {
            var viewModel = new GigFormViewModel
            {
                Genres = this._unitOfWork.Genres.GetGenres(),
                Heading = "Add a Gig"
            };

            return View("GigForm", viewModel);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            var userId = User.Identity.GetUserId();

            var gig = _unitOfWork.Gigs.GetGig(id);

            if (gig == null)
                return this.HttpNotFound();

            if (gig.ArtistId != userId)
                return new HttpUnauthorizedResult();

            var viewModel = new GigFormViewModel
            {
                Genres = this._unitOfWork.Genres.GetGenres(),
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
            this._unitOfWork.Complete();
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

            var gig = this._unitOfWork.Gigs.GetGigWithAttendees(viewModel.Id);

            if (gig == null) return this.HttpNotFound();

            if (gig.ArtistId != userId)
                return new HttpUnauthorizedResult(); 

            gig.DateTime = viewModel.GetDateTime();
            gig.GenreId = viewModel.Genre;
            gig.Venue = viewModel.Venue;
           
            this._unitOfWork.Complete();
            return RedirectToAction("Mine", "Gigs");
        }

        [Authorize]
        public ActionResult Attending()
        {
            var userId = User.Identity.GetUserId();

            var model = new GigsViewModel
                            {
                                UpcomingGigs = _unitOfWork.Gigs.GetGigsUserAttending(userId),
                                SchowActions = User.Identity.IsAuthenticated,
                                Heading = "Gigs I'm coming",
                                Attendancies = _unitOfWork.Attendances.GetFutureAttendances(userId).ToLookup(a => a.GigId)
                            };
       
            return View("Gigs", model);
        }

        [Authorize]
        public ActionResult Mine()
        {
            var userId = User.Identity.GetUserId();

            var gigs =this._unitOfWork.Gigs.GetUpcomingGigsByArtist(userId)
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

                model.IsAttending = this._unitOfWork.Attendances.GetAttendance(id, userId) != null;

                model.IsFollowing = this._unitOfWork.Followings.GetFollowing(userId, gig.ArtistId) != null;
               
            }
            return this.View("Details", model);
        }
    }
}