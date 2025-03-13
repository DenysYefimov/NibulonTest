namespace Db.Models
{
    public class City
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int? DistrictCode { get; set; }
        public District? District { get; set; }
        public int? RegionCode { get; set; }
        public Region? Region { get; set; }
        public ICollection<Aup> Aups { get; set; }
    }
}
