namespace UCStatistics.Shared.DTOs
{
    public class FilterCriteria
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public TimeSpan TimeFrom { get; set; }
        public TimeSpan TimeTo { get; set; }

        public int? Level2Nr { get; set; }
        public int? Level3Nr { get; set; }

        public List<int>? OfficeNrs { get; set; }
    }
}
