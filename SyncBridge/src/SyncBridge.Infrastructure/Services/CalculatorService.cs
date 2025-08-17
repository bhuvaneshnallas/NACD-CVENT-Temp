using SyncBridge.Domain.Interfaces;
using SyncBridge.Domain.Models.DTOs;

namespace SyncBridge.Infrastructure.Services;

public class CalculatorService : ICalculatorService
{
    public CalculatorResponse Calculate(CalculatorRequest request)
    {
        var response = new CalculatorResponse();
        try
        {
            switch (request.Operation.ToLower())
            {
                case "add":
                    response.Result = request.Number1 + request.Number2;
                    break;
                case "subtract":
                    response.Result = request.Number1 - request.Number2;
                    break;
                case "multiply":
                    response.Result = request.Number1 * request.Number2;
                    break;
                case "divide":
                    if (request.Number2 == 0)
                    {
                        response.Error = "Division by zero is not allowed.";
                    }
                    else
                    {
                        response.Result = request.Number1 / request.Number2;
                    }
                    break;
                default:
                    response.Error = $"Unknown operation: {request.Operation}";
                    break;
            }
        }
        catch (Exception ex)
        {
            response.Error = ex.Message;
        }
        return response;
    }
}
