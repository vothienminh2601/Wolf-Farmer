using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ICSVReader<T>
{
    UniTask<Dictionary<string, T>> LoadDataFromCSV(TextAsset csvFile);
}

public enum eCSVDataType
{
    Mineral,
    Asteroid,

    // Add more here in future
}

public class CSVReaderFactory
{
    public static ICSVReader<T> CreateReader<T>()
    {
        if (typeof(T) == typeof(SeedData))
            return (ICSVReader<T>)new SeedCSVReader();
        else if (typeof(T) == typeof(ProductData))
            return (ICSVReader<T>)new ProductCSVReader();
        else if(typeof(T) == typeof(AnimalData))
            return (ICSVReader<T>)new AnimalCSVReader();

        return null;
    }
}
