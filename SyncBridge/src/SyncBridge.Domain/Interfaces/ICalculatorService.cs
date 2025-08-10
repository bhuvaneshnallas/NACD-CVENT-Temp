using SyncBridge.Domain.Models.DTOs;

namespace SyncBridge.Domain.Interfaces
{
    public interface ICalculatorService
    {
        CalculatorResponse Calculate(CalculatorRequest request);
    }
}
