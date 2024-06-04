namespace UrlShortener.Domain.Repositories
{
    public interface IVistorCounterRespository
    {
        Task AddAsync(string code, int count);

        Task<double> TotalAsync(string code);
    }
}