using UCStatistics.Shared.DTOs;

namespace UCStatistics.Repositories
{
    public interface IReportRepository
    {
        Task<IEnumerable<OfficeInfo>> GetOfficesAsync();
        Task<IEnumerable<SummaryDto>> GetHistoricalAsync(FilterCriteria criteria);
        Task<IEnumerable<ServiceSummaryDto>> GetServiceSummaryAsync(FilterCriteria criteria);
        Task<IEnumerable<TicketDto>> GetTicketDetailsAsync(FilterCriteria criteria);
    }
}
