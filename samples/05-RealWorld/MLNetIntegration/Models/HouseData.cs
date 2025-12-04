using Microsoft.ML.Data;

namespace MLNetIntegration.Models;

/// <summary>
/// Input data model for house price prediction (Regression)
/// Real-world features commonly used in housing datasets
/// </summary>
public class HouseData
{
    [LoadColumn(0)]
    public float Size { get; set; }  // Square feet

    [LoadColumn(1)]
    public float Bedrooms { get; set; }

    [LoadColumn(2)]
    public float Bathrooms { get; set; }

    [LoadColumn(3)]
    public float Age { get; set; }  // Years old

    [LoadColumn(4)]
    public float LotSize { get; set; }  // Square feet

    [LoadColumn(5)]
    public float Garage { get; set; }  // Number of cars

    [LoadColumn(6), ColumnName("Label")]
    public float Price { get; set; }  // Target: Price in $1000s
}

/// <summary>
/// Prediction output model for house price
/// </summary>
public class HousePricePrediction
{
    [ColumnName("Score")]
    public float Price { get; set; }
}
