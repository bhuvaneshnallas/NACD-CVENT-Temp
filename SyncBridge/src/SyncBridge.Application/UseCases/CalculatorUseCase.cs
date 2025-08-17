using SyncBridge.Domain.Interfaces;
using SyncBridge.Domain.Models.DTOs;

namespace SyncBridge.Application.UseCases;

public class CalculatorUseCase(ICalculatorService calculatorService)
{
    private readonly ICalculatorService _calculatorService = calculatorService;

    public CalculatorResponse Execute(CalculatorRequest request)
    {
        return _calculatorService.Calculate(request);
    }
}
