using System.Linq;
using API.DTOs;
using API.Entities;
using API.Exetentions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(destination => destination.PhotoUrl, option => option.MapFrom(source =>
                    source.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, opt=>opt.MapFrom(src =>src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>(); 
            CreateMap<MemberUpdateDto,AppUser>();
            CreateMap<registerDTO,AppUser>();
        }
    }
}