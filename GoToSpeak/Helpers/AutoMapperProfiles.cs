using AutoMapper;
using GoToSpeak.Dtos;
using GoToSpeak.Models;

namespace GoToSpeak.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>();
            CreateMap<Log,LogToReturnDto>().ForMember(dest => dest.User, opts => opts.MapFrom(src => src.User.UserName));
            CreateMap<MessageForCreationDto,Message>().ReverseMap();
            CreateMap<Message, MessageToReturnDto>();
            CreateMap<MessageForCreationDto, MessageToReturnDto>();
            CreateMap<User,PhotoForCreationDto>();
            CreateMap<User,PhotoToReturnDto>();
            CreateMap<Room, RoomToReturn>();
            CreateMap<RoomToReturn, Room>();
            CreateMap<UserForRegisterDto, User>();
        }
    }
}