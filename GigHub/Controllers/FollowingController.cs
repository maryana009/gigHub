using GigHub.Dtos;
using GigHub.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace GigHub.Controllers
{
    public class FollowingController : ApiController
    {
        private ApplicationDbContext _context;

        public FollowingController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpPost]
        public IHttpActionResult Follow(FollowingDto dto)
        {
            var userId = User.Identity.GetUserId();

            if (_context.Followings.Any(f => f.FollowerId == userId && f.FolloweeId == dto.FolloweeId))
                return BadRequest("Following already exists");

            var follow = new Following
            {
                FolloweeId = dto.FolloweeId,
                FollowerId = userId
            };
            _context.Followings.Add(follow);
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete]
        public IHttpActionResult Unfollow(string id)
        {
            var userId = User.Identity.GetUserId();

            var following = this._context.Followings
                .SingleOrDefault(f => f.FolloweeId == id && f.FollowerId == userId);

            if (following == null)
            {
                return this.NotFound();
            }
           
            _context.Followings.Remove(following);
            _context.SaveChanges();
            return Ok(id);
        }
    }
}