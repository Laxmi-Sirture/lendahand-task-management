using AutoMapper;
using LendAHand.Application.DTOs;
using LendAHand.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeDTO>();
            CreateMap<CreateEmployeeDTO, Employee>();
            CreateMap<UpdateEmployeeDTO, Employee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<TaskItem, TaskDTO>()
                .ForMember(dest => dest.Priority,
                    opt => opt.MapFrom(src => src.Priority.ToString()))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.AssignedEmployeeName,
                    opt => opt.MapFrom(src => src.AssignedEmployee != null
                        ? src.AssignedEmployee.Name : string.Empty));

            CreateMap<Notification, NotificationDTO>();
            CreateMap<FileUpload, FileUploadDTO>();
        }
    }
}
