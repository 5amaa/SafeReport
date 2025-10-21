using AutoMapper;
using SafeReport.Application.Common;
using SafeReport.Application.DTOs;
using SafeReport.Application.ISevices;
using SafeReport.Core.Interfaces;
using SafeReport.Core.Models;

public class IncidentService(IIncidentRepository incidentRepository, IMapper mapper) : IIncidentService
{
	private readonly IIncidentRepository _incidentRepository = incidentRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<Response<IEnumerable<IncidentDto>>> GetAllAsync(PaginationFilter filter)
	{
		try
		{
			var items = await _incidentRepository.GetPagedAsync(filter.PageNumber, filter.PageSize);
			var result = _mapper.Map<IEnumerable<IncidentDto>>(items);
			return Response<IEnumerable<IncidentDto>>.SuccessResponse(result, "Fetched incidents successfully.");
		}
		catch (Exception ex)
		{
			return Response<IEnumerable<IncidentDto>>.FailResponse($"Error: {ex.Message}");
		}
	}

	public async Task<Response<IncidentDto?>> GetByIdAsync(int id)
	{
		try
		{
			var incident = await _incidentRepository.GetByIdAsync(id);
			if (incident == null || incident.IsDeleted)
				return Response<IncidentDto?>.FailResponse("Incident not found.");

			var dto = _mapper.Map<IncidentDto>(incident);
			return Response<IncidentDto?>.SuccessResponse(dto, "Incident found.");
		}
		catch (Exception ex)
		{
			return Response<IncidentDto?>.FailResponse($"Error: {ex.Message}");
		}
	}

	public async Task<Response<IncidentDto>> CreateAsync(CreateIncidentDto dto)
	{
		try
		{
			var incident = _mapper.Map<Incident>(dto);
			await _incidentRepository.AddAsync(incident);
			await _incidentRepository.SaveChangesAsync();
			var incidentDto = _mapper.Map<IncidentDto>(incident);
			return Response<IncidentDto>.SuccessResponse(incidentDto, "Incident created successfully.");
		}
		catch (Exception ex)
		{
			return Response<IncidentDto>.FailResponse($"Creation failed: {ex.Message}");
		}
	}

	public async Task<Response<bool>> UpdateAsync(int id, CreateIncidentDto dto)
	{
		try
		{
			var incident = await _incidentRepository.GetByIdAsync(id);
			if (incident == null || incident.IsDeleted)
				return Response<bool>.FailResponse("Incident not found.");

			_mapper.Map(dto, incident);

			_incidentRepository.Update(incident);
			await _incidentRepository.SaveChangesAsync();

			return Response<bool>.SuccessResponse(true, "Incident updated successfully.");
		}
		catch (Exception ex)
		{
			return Response<bool>.FailResponse($"Update failed: {ex.Message}");
		}
	}

	public async Task<Response<bool>> SoftDeleteAsync(int id)
	{
		try
		{
			var incident = await _incidentRepository.GetByIdAsync(id);
			if (incident == null || incident.IsDeleted)
				return Response<bool>.FailResponse("Incident not found.");

			_incidentRepository.SoftDelete(incident);
			await _incidentRepository.SaveChangesAsync();

			return Response<bool>.SuccessResponse(true, "Incident deleted successfully.");
		}
		catch (Exception ex)
		{
			return Response<bool>.FailResponse($"Delete failed: {ex.Message}");
		}
	}
}
