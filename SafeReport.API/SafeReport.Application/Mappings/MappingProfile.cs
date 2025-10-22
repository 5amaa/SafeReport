using System;
using AutoMapper;
using SafeReport.Application.DTOs;
using SafeReport.Core.Models;


namespace SafeReport.Application.Mappings
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Incident, IncidentDto>().ReverseMap();
			CreateMap<CreateIncidentDto, Incident>();
			CreateMap<Report, ReportDto>()
			   .ForMember(dest => dest.IncidentName, opt => opt.MapFrom(src => src.Incident.NameEn))
			   .ForMember(dest => dest.TimeSinceCreated,
				   opt => opt.MapFrom(src => (DateTime.UtcNow - src.CreatedDate).TotalDays >= 1
					   ? $"{(int)(DateTime.UtcNow - src.CreatedDate).TotalDays} يوم"
					   : $"{(int)(DateTime.UtcNow - src.CreatedDate).TotalHours} ساعة"));
			CreateMap<IncidentType, IncidentTypeDto>().ReverseMap();
		}
	}

}
