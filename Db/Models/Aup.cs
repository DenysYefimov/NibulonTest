namespace Db.Models
{
    public class Aup
    {
        public int Id { get; set; }
        public string Postcode { get; set; }
        public string? CityCode { get; set; }
        public City? City { get; set; }
        public string CityName { get; set; }
        public int? RegionCode { get; set; }
        public Region? Region { get; set; }
        public string RegionName { get; set; }
        public int? DistrictCode { get; set; }
        public District? District { get; set; }
        public string DistrictName { get; set; }
    }
}
