namespace Mobile_Server_Dioxide.Services.TickerServices
{
    public interface ITickerService
    {
        Task<List<string>?> GetTickersAsync();
    }
}
