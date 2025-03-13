using AutoMapper;
using Common.Dtos;
using Db.Models;

namespace Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Aup, AupDto>();
        }
    }
}
