using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GigHub.Repositories
{
    using GigHub.Models;

    public class AttendanceRepository
    {
        private readonly ApplicationDbContext _context;

        public AttendanceRepository(ApplicationDbContext context)
        {
            this._context = context;
        }

        public IEnumerable<Attendance> GetFutureAttendances(string userId)
        {
            return this._context.Attendances
                .Where(a => a.AttendeeId == userId && a.Gig.DateTime > DateTime.Now).ToList();
        }

        public Attendance GetAttendance(int gigId, string userId)
        {
            return this._context.Attendances.SingleOrDefault(a => a.GigId == gigId && a.AttendeeId == userId);
        }
    }
}