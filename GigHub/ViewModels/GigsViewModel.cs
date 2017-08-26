using GigHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GigHub.ViewModels
{
    public class GigsViewModel
    {
        public IEnumerable<Gig> UpcomingGigs { get; set; }

        public bool SchowActions { get; set; }

        public string Heading { get; set; }

        public string SearchTerm { get; set; }

        public ILookup<int, Attendance> Attendancies { get; set; }

    }
}