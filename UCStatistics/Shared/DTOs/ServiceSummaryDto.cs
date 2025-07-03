namespace UCStatistics.Shared.DTOs
{
    public class ServiceSummaryDto
    {
        public int Level3Nr { get; set; }
        public string Level3Name { get; set; }

        public int Level2Nr { get; set; }
        public string Level2Name { get; set; }

        public int OfficeNr { get; set; }
        public string OfficeName { get; set; }

        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }

        public int TotalServices { get; set; }
        public double AvgServiceSeconds { get; set; }
    }
}
