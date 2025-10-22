using SafeReport.Web.DTOs;
using SafeReport.Web.Interfaces;
using System.Collections.Generic;
using System.Net.Http.Json;


namespace SafeReport.Web.Services;

public class ReportService: IReportService
{
    private readonly HttpClient _httpClient;

    public ReportService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Response<PagedResultDto>> GetAllReportsAsync(ReportFilterDto filter)
    {
        var response = await _httpClient.GetFromJsonAsync<Response<IEnumerable<PagedResultDto>>>("api/Report/GetAll");

        //if (response != null && response.Data != null)
        //{
        //    var responseList = response.Data.
        //        .Select(it => Response<IncidentType>.SuccessResponse( ))
        //        .ToList();

        //    return responseList;
        //}

        return Response<PagedResultDto>.SuccessResponse(null, response.Message);
    }


    public async Task<List<Response<IncidentType>>> GetAllIncidentsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<Response<IEnumerable<IncidentDto>>>("api/Incident");

            if (response != null && response.Data != null)
            {
                var incidentTypes = response.Data.Select(d => new IncidentType
                {
                    Id = d.Id,
                    Name = d.NameEn 
                });
                var responseList = incidentTypes
                    .Select(it => Response<IncidentType>.SuccessResponse(it))
                    .ToList();

                return responseList;
            }

            return new List<Response<IncidentType>>
            {
               Response<IncidentType>.SuccessResponse(null, "No incidents found")
            };

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching incidents: {ex.Message}");
            return new List<Response<IncidentType>>
                {
                    Response<IncidentType>.FailResponse("Failed to fetch incidents.")
                };
        }
    }

        public async Task<bool> DeleteReportAsync(int id)
    {
        await Task.Delay(200); // delay وهمي
        return true; // كأن الحذف تم بنجاح
    }

    public async Task PrintReportAsync(int id)
    {
        await Task.Delay(300); // delay وهمي
        Console.WriteLine($"Printing report #{id}...");
    }
}
