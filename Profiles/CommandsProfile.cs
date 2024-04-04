using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;
using PlatformService.DTOs;

namespace CommandsService.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<Command, CommandReadDto>();
            CreateMap<PlatformPublishedDto, Platform>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
        }
    }
}