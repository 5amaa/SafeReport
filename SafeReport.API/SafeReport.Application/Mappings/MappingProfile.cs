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
		}
	}

}
