namespace UCStatistics.Shared.DTOs
{
    public class SummaryDto
    {
        public int OfficeNr { get; set; }
        public string OfficeName { get; set; }
        public int Level2Nr { get; set; }
        public string Level2Name { get; set; }
        public int Level3Nr { get; set; }
        public string Level3Name { get; set; }
        public int TotalTickets { get; set; }
        public double AvgServiceSeconds { get; set; }

    }
}
