using Dapper;
using UCStatistics.Data;
using UCStatistics.Shared.DTOs;

namespace UCStatistics.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly DapperContext _db;
        public ReportRepository(DapperContext db) => _db = db;

        public async Task<IEnumerable<OfficeInfo>> GetOfficesAsync()
        {
            const string sql = @"
            SELECT DISTINCT
                OFFICE_NR     AS OfficeNr,
                OFFICE_NAME   AS OfficeName,
                LEVEL2_NR     AS Level2Nr,
                LEVEL2_NAME   AS Level2Name,
                LEVEL3_NR     AS Level3Nr,
                LEVEL3_NAME   AS Level3Name
            FROM CUSTOMER_QUEUE_INFO_DAILY
            ORDER BY Level3Name, Level2Name, OfficeName;
            ";

            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<OfficeInfo>(sql);
        }
        public async Task<IEnumerable<SummaryDto>> GetHistoricalAsync(FilterCriteria f)
        {
            const string sql = @"
            SELECT
                OFFICE_NR    AS OfficeNr,
                OFFICE_NAME  AS OfficeName,
                LEVEL2_NR    AS Level2Nr,
                LEVEL2_NAME  AS Level2Name,
                LEVEL3_NR    AS Level3Nr,
                LEVEL3_NAME  AS Level3Name,
                COUNT(*)     AS TotalTickets,
                AVG(SERVICE_TIME)       AS AvgServiceSeconds
            FROM CUSTOMER_QUEUE_INFO_DAILY
            WHERE TICKET_DATE BETWEEN @DateFrom AND @DateTo
              AND (@Level2Nr IS NULL OR LEVEL2_NR = @Level2Nr)
              AND (@Level3Nr IS NULL OR LEVEL3_NR = @Level3Nr)
              AND (@OfficeNrs IS NULL OR OFFICE_NR IN @OfficeNrs)
            GROUP BY
                OFFICE_NR, OFFICE_NAME,
                LEVEL2_NR, LEVEL2_NAME,
                LEVEL3_NR, LEVEL3_NAME
            ORDER BY
                LEVEL3_NAME, LEVEL2_NAME, OFFICE_NAME;
        ";

            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<SummaryDto>(sql, new
            {
                f.DateFrom,
                f.DateTo,
                f.Level2Nr,
                f.Level3Nr,
                f.OfficeNrs
            });
        }

        public async Task<IEnumerable<ServiceSummaryDto>> GetServiceSummaryAsync(FilterCriteria f)
        {
            const string sql = @"
            SELECT
                LEVEL3_NR      AS Level3Nr,
                LEVEL3_NAME    AS Level3Name,
                LEVEL2_NR      AS Level2Nr,
                LEVEL2_NAME    AS Level2Name,
                OFFICE_NR      AS OfficeNr,
                OFFICE_NAME    AS OfficeName,
                SERVICETYPE_NR AS ServiceCode,
                TICKET_TEXT    AS ServiceName,
                COUNT(*)       AS TotalServices,
                AVG(SERVICE_TIME) AS AvgServiceSeconds
            FROM CUSTOMER_QUEUE_INFO_DAILY
            WHERE TICKET_DATE BETWEEN @DateFrom AND @DateTo
              AND (@Level2Nr IS NULL OR LEVEL2_NR = @Level2Nr)
              AND (@Level3Nr IS NULL OR LEVEL3_NR = @Level3Nr)
              AND (@OfficeNrs IS NULL OR OFFICE_NR IN @OfficeNrs)
            GROUP BY
                LEVEL3_NR, LEVEL3_NAME,
                LEVEL2_NR, LEVEL2_NAME,
                OFFICE_NR, OFFICE_NAME,
                SERVICETYPE_NR, TICKET_TEXT
            ORDER BY
                LEVEL3_NAME, LEVEL2_NAME, OFFICE_NAME, ServiceName;
        ";

            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<ServiceSummaryDto>(sql, new
            {
                f.DateFrom,
                f.DateTo,
                f.Level2Nr,
                f.Level3Nr,
                f.OfficeNrs
            });
        }

        public async Task<IEnumerable<TicketDto>> GetTicketDetailsAsync(FilterCriteria f)
        {
            const string sql = @"
            SELECT
                LEVEL3_NR       AS Level3Nr,
                LEVEL3_NAME     AS Level3Name,
                LEVEL2_NR       AS Level2Nr,
                LEVEL2_NAME     AS Level2Name,
                OFFICE_NR       AS OfficeNr,
                OFFICE_NAME     AS OfficeName,
                SERVICETYPE_NR  AS ServiceCode,
                TICKET_TEXT     AS ServiceName,
                TICKET          AS TicketNumber,
                TICKET_DATETIME AS EntryTime,
                END_DATETIME    AS ExitTime,
                WAITING_TIME    AS WaitSeconds,
                SERVICE_TIME    AS ServiceSeconds
            FROM CUSTOMER_QUEUE_INFO_DAILY
            WHERE TICKET_DATE BETWEEN @DateFrom AND @DateTo
              AND (@Level2Nr IS NULL OR LEVEL2_NR = @Level2Nr)
              AND (@Level3Nr IS NULL OR LEVEL3_NR = @Level3Nr)
              AND (@OfficeNrs IS NULL OR OFFICE_NR IN @OfficeNrs)
            ORDER BY
                TICKET_DATETIME;
        ";

            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<TicketDto>(sql, new
            {
                f.DateFrom,
                f.DateTo,
                f.Level2Nr,
                f.Level3Nr,
                f.OfficeNrs
            });
        }
    }
}
