namespace Db.Models
{
    public class District
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public ICollection<City> Cities { get; set; }
        public ICollection<Aup> Aups { get; set; }
    }
}
