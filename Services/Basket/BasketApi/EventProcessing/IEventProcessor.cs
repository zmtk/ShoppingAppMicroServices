namespace BasketApi.EventrProcessing;

public interface IEventProcessor
{    
    Task ProcessEventAsync(string message);
    
}