namespace UCStatistics.Shared.DTOs
{
    public class TicketDto
    {
        public int Level3Nr { get; set; }
        public string Level3Name { get; set; }

        public int Level2Nr { get; set; }
        public string Level2Name { get; set; }

        public int OfficeNr { get; set; }
        public string OfficeName { get; set; }

        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }

        public string TicketNumber { get; set; }

        public DateTime EntryTime { get; set; }
        public DateTime ExitTime { get; set; }

        public int WaitSeconds { get; set; }
        public int ServiceSeconds { get; set; }
    }
}
