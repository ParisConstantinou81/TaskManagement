using AutoMapper;
using TaskManagement.API.Data.Entities.Tables;
using TaskManagement.API.Enums;
using TaskManagement.API.Interfaces.Mapping;

namespace TaskManagement.API.Models.DTOs
{
    public class UpdateTaskDto : IMapFrom<TbTask>
    {
        public required int Status { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateProjection<UpdateTaskDto, TbTask>();
            profile.CreateMap<UpdateTaskDto, TbTask>()
                .ForMember(d => d.Status, opt => opt.MapFrom(c => Enum.GetName(typeof(TaskStatusEn), c.Status)));
        }
    }
}
