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

        public async Task<IEnumerable<SummaryDto>> GetHistoricalAsync(FilterCriteria criteria)
        {
            const int digitalServiceTypeNr = 42;         // Κωδικός ψηφιακού εισιτηρίου
            const int objectiveWaitingSeconds = 300;     // Στόχος αναμονής (σε sec)
            const int objectiveServiceSeconds = 600;     // Στόχος εξυπηρέτησης (σε sec)

            const string sql = @"
                SELECT
                    LEVEL3_NR                               AS Level3Nr,
                    LEVEL3_NAME                             AS Level3Name,
                    LEVEL2_NR                               AS Level2Nr,
                    LEVEL2_NAME                             AS Level2Name,
                    OFFICE_NR                               AS OfficeNr,
                    OFFICE_NAME                             AS OfficeName,
                    COUNT(*)                                AS IncomingCustomers,
                    SUM(CASE WHEN LOSTTICKET = 1 THEN 1 ELSE 0 END) AS UnattendedCustomers,
                    SUM(CASE WHEN LOSTTICKET = 0 THEN 1 ELSE 0 END) AS ServedCustomers,
                    SUM(CASE WHEN ICU = 'Y' THEN 1 ELSE 0 END)       AS GoldenClients,
                    SUM(CASE WHEN SERVICETYPE_NR = @DigitalServiceTypeNr THEN 1 ELSE 0 END) AS DigitalTickets,
                    CONVERT(time, DATEADD(SECOND, AVG(WAITING_TIME), 0))          AS AvgWaitingTime,
                    CONVERT(time, DATEADD(SECOND, AVG(SERVICE_TIME), 0))          AS AvgServiceTime,
                    CONVERT(time, DATEADD(SECOND, AVG(WAITING_TIME + SERVICE_TIME), 0)) AS AvgCustomerTime,
                    AVG(CASE WHEN WAITING_TIME <= @ObjectiveWaitingSeconds THEN 1.0 ELSE 0 END) * 100   AS ObjectiveWaitingPercent,
                    AVG(CASE WHEN SERVICE_TIME <= @ObjectiveServiceSeconds THEN 1.0 ELSE 0 END) * 100   AS ObjectiveServicePercent,
                    CONVERT(time, DATEADD(SECOND, MAX(WAITING_TIME), 0))          AS MaxWaitingTime,
                    CONVERT(time, DATEADD(SECOND, MAX(SERVICE_TIME), 0))          AS MaxServiceTime
                FROM CUSTOMER_QUEUE_INFO_DAILY
                WHERE
                    TICKET_DATE BETWEEN @DateFrom AND @DateTo
                    AND (@Level2Nr IS NULL OR LEVEL2_NR = @Level2Nr)
                    AND (@Level3Nr IS NULL OR LEVEL3_NR = @Level3Nr)
                    AND (@OfficeNrs IS NULL OR OFFICE_NR IN @OfficeNrs)
                GROUP BY
                    LEVEL3_NR, LEVEL3_NAME,
                    LEVEL2_NR, LEVEL2_NAME,
                    OFFICE_NR, OFFICE_NAME
                ORDER BY
                    LEVEL3_NAME, LEVEL2_NAME, OFFICE_NAME;";

            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<SummaryDto>(sql, new
            {
                criteria.DateFrom,
                criteria.DateTo,
                criteria.Level2Nr,
                criteria.Level3Nr,
                OfficeNrs = criteria.OfficeNrs,
                DigitalServiceTypeNr = digitalServiceTypeNr,
                ObjectiveWaitingSeconds = objectiveWaitingSeconds,
                ObjectiveServiceSeconds = objectiveServiceSeconds
            });
        }

        public async Task<IEnumerable<ServiceSummaryDto>> GetServiceSummaryAsync(FilterCriteria criteria)
        {
            // Branches Summary Statistics with Services
            const int digitalServiceTypeNr = 42;         // Κωδικός ψηφιακού εισιτηρίου
            const int objectiveWaitingSeconds = 300;     // Στόχος αναμονής (σε sec)
            const int objectiveServiceSeconds = 600;     // Στόχος εξυπηρέτησης (σε sec)

            const string sql = @"
                SELECT
                    LEVEL3_NR        AS Level3Nr,
                    LEVEL3_NAME      AS Level3Name,
                    LEVEL2_NR        AS Level2Nr,
                    LEVEL2_NAME      AS Level2Name,
                    OFFICE_NR        AS OfficeNr,
                    OFFICE_NAME      AS OfficeName,
                    SERVICETYPE_NR   AS ServiceCode,
                    TICKET_TEXT      AS ServiceName,
                    COUNT(*)         AS IncomingCustomers,
                    SUM(CASE WHEN LOSTTICKET = 1 THEN 1 ELSE 0 END) AS UnattendedCustomers,
                    SUM(CASE WHEN LOSTTICKET = 0 THEN 1 ELSE 0 END) AS ServedCustomers,
                    SUM(CASE WHEN ICU = 'Y' THEN 1 ELSE 0 END)       AS GoldenClients,
                    SUM(CASE WHEN SERVICETYPE_NR = @DigitalServiceTypeNr THEN 1 ELSE 0 END) AS DigitalTickets,
                    CONVERT(time, DATEADD(SECOND, AVG(WAITING_TIME), 0))          AS AvgWaitingTime,
                    CONVERT(time, DATEADD(SECOND, AVG(SERVICE_TIME), 0))          AS AvgServiceTime,
                    CONVERT(time, DATEADD(SECOND, AVG(WAITING_TIME + SERVICE_TIME), 0)) AS AvgCustomerTime,
                    AVG(CASE WHEN WAITING_TIME <= @ObjectiveWaitingSeconds THEN 1.0 ELSE 0 END) * 100   AS ObjectiveWaitingPercent,
                    AVG(CASE WHEN SERVICE_TIME <= @ObjectiveServiceSeconds THEN 1.0 ELSE 0 END) * 100   AS ObjectiveServicePercent,
                    CONVERT(time, DATEADD(SECOND, MAX(WAITING_TIME), 0))          AS MaxWaitingTime,
                    CONVERT(time, DATEADD(SECOND, MAX(SERVICE_TIME), 0))          AS MaxServiceTime
                FROM CUSTOMER_QUEUE_INFO_DAILY
                WHERE
                    TICKET_DATE BETWEEN @DateFrom AND @DateTo
                    AND (@Level2Nr IS NULL OR LEVEL2_NR = @Level2Nr)
                    AND (@Level3Nr IS NULL OR LEVEL3_NR = @Level3Nr)
                    AND (@OfficeNrs IS NULL OR OFFICE_NR IN @OfficeNrs)
                GROUP BY
                    LEVEL3_NR, LEVEL3_NAME,
                    LEVEL2_NR, LEVEL2_NAME,
                    OFFICE_NR, OFFICE_NAME,
                    SERVICETYPE_NR, TICKET_TEXT
                ORDER BY
                    LEVEL3_NAME, LEVEL2_NAME, OFFICE_NAME, ServiceName;";

            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<ServiceSummaryDto>(sql, new
            {
                criteria.DateFrom,
                criteria.DateTo,
                criteria.Level2Nr,
                criteria.Level3Nr,
                OfficeNrs = criteria.OfficeNrs,
                DigitalServiceTypeNr = digitalServiceTypeNr,
                ObjectiveWaitingSeconds = objectiveWaitingSeconds,
                ObjectiveServiceSeconds = objectiveServiceSeconds
            });
        }

        public async Task<IEnumerable<TicketDto>> GetTicketDetailsAsync(FilterCriteria criteria)
        {
            // Detail per ticket – unchanged
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
                WHERE
                    TICKET_DATE BETWEEN @DateFrom AND @DateTo
                    AND (@Level2Nr IS NULL OR LEVEL2_NR = @Level2Nr)
                    AND (@Level3Nr IS NULL OR LEVEL3_NR = @Level3Nr)
                    AND (@OfficeNrs IS NULL OR OFFICE_NR IN @OfficeNrs)
                ORDER BY
                    TICKET_DATETIME;";

            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<TicketDto>(sql, new
            {
                criteria.DateFrom,
                criteria.DateTo,
                criteria.Level2Nr,
                criteria.Level3Nr,
                OfficeNrs = criteria.OfficeNrs
            });
        }

    }
}
