using AutoMapper;
using TaskManagement.API.Data.Entities.Tables;
using TaskManagement.API.Interfaces.Mapping;

namespace TaskManagement.API.Models.DTOs
{
    public class CreateTaskDto : IMapFrom<TbTask>
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required DateTime DueDate { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateProjection<CreateTaskDto, TbTask>();
            profile.CreateMap<CreateTaskDto, TbTask>()
                .ForMember(d => d.CreatedAt, opt => opt.MapFrom(c => DateTime.Now))
                .ForMember(d => d.UpdatedAt, opt => opt.MapFrom(c => DateTime.Now));
        }
    }
}
