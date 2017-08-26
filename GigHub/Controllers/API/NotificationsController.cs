using GigHub.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using GigHub.Dtos;
using AutoMapper;

namespace GigHub.Controllers.API
{
    [Authorize]
    public class NotificationsController : ApiController
    {
        private ApplicationDbContext _context;

        public NotificationsController()
        {
            _context = new ApplicationDbContext();
        }

        public IEnumerable<NotificationDto> GetNewNotifications()
        {
            var userId = User.Identity.GetUserId();

            var notifications = _context.UserNotifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .Select(un => un.Notification)
                .Include(n => n.Gig.Artist)
                .ToList();

            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<ApplicationUser, UserDto>();
            //    cfg.CreateMap<Genre, GenreDto>();
            //    cfg.CreateMap<Gig, GigDto>();
            //    cfg.CreateMap<Notification, NotificationDto>();
            //});

            //IMapper mapper = config.CreateMapper();, mapper.Map

            var dto = notifications.Select(Mapper.Map<Notification, NotificationDto>).ToList();
            return dto;

        }

        [Route("api/genre")]
        public IEnumerable<GenreDto> GetGenres()
        {
            var userId = User.Identity.GetUserId();

            var genres = _context.Genres
                .ToList();

            //Mapper.Initialize(cfg =>
            //{
            //    cfg.CreateMap<Genre, GenreDto>();
            //});

            var dto = genres.Select(Mapper.Map<Genre, GenreDto>).ToList();
            return dto;
        }

        [HttpPost]
        public IHttpActionResult MarkAsRead()
        {
            var userId = User.Identity.GetUserId();

            var notifications = _context.UserNotifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToList();

            notifications.ForEach(n => n.Read());

            _context.SaveChanges();

            return Ok();

        }
    }
}
