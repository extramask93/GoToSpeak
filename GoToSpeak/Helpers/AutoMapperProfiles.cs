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
            CreateMap<MessageForCreationDto,Message>().ReverseMap();
            CreateMap<Message, MessageToReturnDto>();
        }
    }
}