using Dapper;
using UCStatistics.Data;
using UCStatistics.Shared.DTOs;

namespace UCStatistics.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly DapperContext _db;

        public ReportRepository(DapperContext db)
            => _db = db;


        public async Task<IEnumerable<OfficeInfo>> GetOfficesAsync()
        {
            const string sql = @"
      SELECT DISTINCT
        OFFICE_NR    AS OfficeNr,
        OFFICE_NAME  AS OfficeName,
        Level2_NR    AS Level2Nr,
        Level2_NAME  AS Level2Name,
        Level3_NR    AS Level3Nr,
        Level3_NAME  AS Level3Name
      FROM Customer_Queue_info_Daily
      ORDER BY Level3_NAME, Level2_NAME, OfficeName;
    ";

            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<OfficeInfo>(sql);
        }

        public async Task<IEnumerable<SummaryDto>> GetHistoricalAsync(FilterCriteria f)
        {
            var sql = @"
        SELECT
            OFFICE_NR    AS OfficeNr,
            OFFICE_NAME  AS OfficeName,
            Level2_NR    AS Level2Nr,
            Level2_NAME  AS Level2Name,
            Level3_NR    AS Level3Nr,
            Level3_NAME  AS Level3Name,
            COUNT(*)     AS TotalTickets,                                    -- πλήθος εισιτηρίων
            AVG(DATEDIFF(SECOND, EntryTime, ExitTime)) AS AvgServiceSeconds  -- μέσος χρόνος εξυπηρέτησης σε δευτ.
        FROM Customer_Queue_info_Daily
        WHERE [Date]      BETWEEN @DateFrom AND @DateTo
          AND [Time]      BETWEEN @TimeFrom AND @TimeTo
          AND (@Level2Nr IS NULL OR Level2_NR = @Level2Nr)
          AND (@Level3Nr IS NULL OR Level3_NR = @Level3Nr)
          AND (
              @OfficeNrs IS NULL 
              OR Office_NR IN @OfficeNrs
          )
        GROUP BY
            OFFICE_NR,
            OFFICE_NAME,
            Level2_NR,
            Level2_NAME,
            Level3_NR,
            Level3_NAME
        ORDER BY
            Level3_NAME,
            Level2_NAME,
            OfficeName;
    ";

            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<SummaryDto>(sql, new
            {
                f.DateFrom,
                f.DateTo,
                f.TimeFrom,
                f.TimeTo,
                f.Level2Nr,
                f.Level3Nr,
                f.OfficeNrs
            });
        }

        public async Task<IEnumerable<ServiceSummaryDto>> GetServiceSummaryAsync(FilterCriteria f)
        {
            const string sql = @"
        SELECT
            Level3_NR    AS Level3Nr,
            Level3_NAME  AS Level3Name,
            Level2_NR    AS Level2Nr,
            Level2_NAME  AS Level2Name,
            OFFICE_NR    AS OfficeNr,
            OFFICE_NAME  AS OfficeName,
            Service_Code AS ServiceCode,
            Service_Name AS ServiceName,
            COUNT(*)     AS TotalServices,
            AVG(DATEDIFF(SECOND, EntryTime, ExitTime)) AS AvgServiceSeconds
        FROM Customer_Queue_info_Daily
        WHERE [Date]      BETWEEN @DateFrom AND @DateTo
          AND [Time]      BETWEEN @TimeFrom AND @TimeTo
          AND (@Level2Nr IS NULL OR Level2_NR = @Level2Nr)
          AND (@Level3Nr IS NULL OR Level3_NR = @Level3Nr)
          AND (@OfficeNrs IS NULL OR OFFICE_NR IN @OfficeNrs)
        GROUP BY
            Level3_NR, Level3_NAME,
            Level2_NR, Level2_NAME,
            OFFICE_NR, OFFICE_NAME,
            Service_Code, Service_Name
        ORDER BY
            Level3_NAME, Level2_NAME, OfficeName, ServiceName;
    ";

            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<ServiceSummaryDto>(sql, f);
        }
        public async Task<IEnumerable<TicketDto>> GetTicketDetailsAsync(FilterCriteria f)
        {
            var sql = @"
                SELECT
                    Level3_NR    AS Level3Nr,
                    Level3_NAME  AS Level3Name,
                    Level2_NR    AS Level2Nr,
                    Level2_NAME  AS Level2Name,
                    OFFICE_NR    AS OfficeNr,
                    OFFICE_NAME  AS OfficeName,
                    Service_Code AS ServiceCode,
                    Service_Name AS ServiceName,
                    Ticket_Number   AS TicketNumber,
                    EntryTime,
                    ExitTime,
                    DATEDIFF(SECOND, DATEADD(SECOND, -DATEDIFF(SECOND, EntryTime, ExitTime), EntryTime), EntryTime) AS WaitSeconds,
                    DATEDIFF(SECOND, EntryTime, ExitTime) AS ServiceSeconds
                FROM Customer_Queue_info_Daily
                WHERE [Date] BETWEEN @DateFrom AND @DateTo
                  AND [Time] BETWEEN @TimeFrom AND @TimeTo
                  AND (@Level2Nr IS NULL OR Level2_NR = @Level2Nr)
                  AND (@Level3Nr IS NULL OR Level3_NR = @Level3Nr)
                  AND (@OfficeNrs IS NULL OR OFFICE_NR IN @OfficeNrs)
                ORDER BY EntryTime;
            ";

            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<TicketDto>(sql, new
            {
                f.DateFrom,
                f.DateTo,
                f.TimeFrom,
                f.TimeTo,
                f.Level2Nr,
                f.Level3Nr,
                f.OfficeNrs
            });
        }
    }
}
