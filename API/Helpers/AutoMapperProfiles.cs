namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDto>()
         .ForMember(dest => dest.PhotoUrl,
         opt => opt.MapFrom(src =>
             src.Photo.FirstOrDefault(x => x.IsMain).Url))
         .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
        CreateMap<Photo, PhotoDto>();
        // CreateMap<UserGroup, MemberDto>();
        CreateMap<PostPhoto, PostPhotoDto>();
        CreateMap<MemberUpdateDto, AppUser>();
        CreateMap<RegisterDto, AppUser>();
        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(src =>
                src.Sender.Photo.FirstOrDefault(x => x.IsMain).Url))
            .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(src =>
                src.Recipient.Photo.FirstOrDefault(x => x.IsMain).Url));

        CreateMap<Post, PostDto>();

    }
}
