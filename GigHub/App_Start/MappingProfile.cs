using AutoMapper;
using GigHub.Dtos;
using GigHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GigHub.App_Start
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<Genre, GenreDto>();
            CreateMap<Gig, GigDto>();
            CreateMap<Notification, NotificationDto>();

            //Mapper.Initialize(cfg =>
            //{
            //    cfg.CreateMap<Genre, GenreDto>();
            //});

            //Mapper.Initialize(cfg =>
            //{
            //    cfg.CreateMap<ApplicationUser, UserDto>();
            //    cfg.CreateMap<Genre, GenreDto>();
            //    cfg.CreateMap<Gig, GigDto>();
            //    cfg.CreateMap<Notification, NotificationDto>();
            //});
        }
    }
}