namespace Common.Dtos
{
    public class AupDto
    {
        public string Id { get; set; }
        public string Postcode { get; set; }
        public string? CityCode { get; set; }
        public string CityName { get; set; }
        public string? RegionCode { get; set; }
        public string RegionName { get; set; }
        public string? DistrictCode { get; set; }
        public string DistrictName { get; set; }
    }
}
