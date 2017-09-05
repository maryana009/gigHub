using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GigHub.Repositories
{
    using GigHub.Models;

    public class FollowingRepository
    {
        private readonly ApplicationDbContext _context;

        public FollowingRepository(ApplicationDbContext context)
        {
            this._context = context;
        }

        public Following GetFollowing(string userId, string artistId)
        {
            return this._context.Followings.SingleOrDefault(a => a.FollowerId == userId && a.FolloweeId == artistId);
        }
         
    }
}