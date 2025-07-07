namespace UCStatistics.Shared.DTOs
{
    public class ServiceSummaryDto
    {
        public int Level3Nr { get; set; }
        public string Level3Name { get; set; } = string.Empty;

        public int Level2Nr { get; set; }
        public string Level2Name { get; set; } = string.Empty;

        public int OfficeNr { get; set; }
        public string OfficeName { get; set; } = string.Empty;

        public int ServiceCode { get; set; }
        public string ServiceName { get; set; } = string.Empty;

        public int IncomingCustomers { get; set; }
        public int UnattendedCustomers { get; set; }
        public int ServedCustomers { get; set; }
        public int GoldenClients { get; set; }
        public int DigitalTickets { get; set; }

        public TimeSpan AvgWaitingTime { get; set; }
        public TimeSpan AvgServiceTime { get; set; }
        public TimeSpan AvgCustomerTime { get; set; }

        public double ObjectiveWaitingPercent { get; set; }
        public double ObjectiveServicePercent { get; set; }

        public TimeSpan MaxWaitingTime { get; set; }
        public TimeSpan MaxServiceTime { get; set; }
    }
}
