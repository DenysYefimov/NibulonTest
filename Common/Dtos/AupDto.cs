namespace Common.Dtos
{
    public class AupDto
    {
        public int Id { get; set; }
        public string Postcode { get; set; }
        public string? CityCode { get; set; }
        public string CityName { get; set; }
        public int? RegionCode { get; set; }
        public string RegionName { get; set; }
        public int? DistrictCode { get; set; }
        public string DistrictName { get; set; }
    }
}
