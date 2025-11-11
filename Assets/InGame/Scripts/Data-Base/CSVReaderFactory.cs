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
        else if(typeof(T) == typeof(FruitData))
            return (ICSVReader<T>)new FruitCSVReader();

        return null;
    }
}
