using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ProductCSVReader : ICSVReader<ProductData>
{
    public UniTask<Dictionary<string, ProductData>> LoadDataFromCSV(TextAsset csvFile)
    {
        Dictionary<string, ProductData> fruits = new();

        if (csvFile == null)
        {
            Debug.LogWarning("CSV file not assigned.");
            return UniTask.FromResult(fruits);
        }

        string[] lines = csvFile.text.Split('\n');
        if (lines.Length <= 1)
        {
            Debug.LogWarning("CSV file is empty or missing header.");
            return UniTask.FromResult(fruits);
        }

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] cols = line.Split('\t');
            if (cols.Length < 6)
            {
                cols = line.Split(',');
            }

            if (cols.Length < 6)
            {
                Debug.LogWarning($"Line skipped (invalid format): {line}");
                continue;
            }

            ProductData data = new ProductData
            {
                id = cols[0].Trim(),
                name = cols[1].Trim()
            };

            int.TryParse(cols[2], out data.value);
            data.iconAddress = cols[3].Trim();
            data.prefabAddress = cols[4].Trim();
            data.description = cols[5].Trim();

            if (!fruits.ContainsKey(data.id))
                fruits.Add(data.id, data);
            else
                Debug.LogWarning($"Duplicate fruit ID '{data.id}' found, skipped.");
        }

        Debug.Log($"Loaded {fruits.Count} fruits from CSV.");
        return UniTask.FromResult(fruits);
    }
}
