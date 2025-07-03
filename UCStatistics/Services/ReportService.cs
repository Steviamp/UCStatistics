using UCStatistics.Repositories;
using UCStatistics.Shared.DTOs;

namespace UCStatistics.Services
{
    public class ReportService
    {
        private readonly IReportRepository _repository;

        public ReportService(IReportRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<OfficeInfo>> GetOfficesAsync()
             => _repository.GetOfficesAsync();
        public Task<IEnumerable<SummaryDto>> GetHistoricalAsync(FilterCriteria criteria)
        {
            return _repository.GetHistoricalAsync(criteria);
        }

        public Task<IEnumerable<ServiceSummaryDto>> GetServiceSummaryAsync(FilterCriteria criteria)
        {
            return _repository.GetServiceSummaryAsync(criteria);
        }

        public Task<IEnumerable<TicketDto>> GetTicketDetailsAsync(FilterCriteria criteria)
        {
            return _repository.GetTicketDetailsAsync(criteria);
        }
    }
}
