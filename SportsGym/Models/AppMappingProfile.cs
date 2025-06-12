using System.Globalization;
using AutoMapper;
using SportsGym.Models.DTO;
using SportsGym.Models.Entities;

namespace SportsGym.Models
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<Gym, GymDTO>();
            CreateMap<Booking, BookingDTO>();
            CreateMap<Trainer, TrainerDTO>();
            CreateMap<Client, ClientDTO>();
            CreateMap<Admin, AdminDTO>();
        }
    }
}