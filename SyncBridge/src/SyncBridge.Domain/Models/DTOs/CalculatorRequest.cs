namespace SyncBridge.Domain.Models.DTOs;

public class CalculatorRequest
{
    public double Number1 { get; set; }
    public double Number2 { get; set; }
    public string Operation { get; set; } = string.Empty; // add, subtract, multiply, divide
}

public class CalculatorResponse
{
    public double Result { get; set; }
    public string? Error { get; set; }
}
