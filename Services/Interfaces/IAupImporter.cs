namespace Services.Interfaces
{
    public interface IAupImporter
    {
        Task ImportDataAsync(Stream excelData);
    }
}
