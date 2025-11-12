using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class AnimalCSVReader : ICSVReader<AnimalData>
{
    public UniTask<Dictionary<string, AnimalData>> LoadDataFromCSV(TextAsset csvFile)
    {
        Dictionary<string, AnimalData> animals = new();

        if (csvFile == null)
        {
            Debug.LogWarning("CSV file not assigned for AnimalCSVReader.");
            return UniTask.FromResult(animals);
        }

        string[] lines = csvFile.text.Split('\n');
        if (lines.Length <= 1)
        {
            Debug.LogWarning("Animal CSV file is empty or missing header.");
            return UniTask.FromResult(animals);
        }

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Hỗ trợ tab hoặc dấu phẩy
            string[] cols = line.Split('\t');
            if (cols.Length < 8)
                cols = line.Split(',');

            if (cols.Length < 8)
            {
                Debug.LogWarning($"Line skipped (invalid format): {line}");
                continue;
            }

            // Cấu trúc CSV:
            // id, name, grow_duration, product_interval, max_product_count, icon_path, prefab_path, product_id

            AnimalData data = new AnimalData();
            data.id = cols[0].Trim();
            data.name = cols[1].Trim();
            int.TryParse(cols[2], out data.baseValue);
            float.TryParse(cols[3], out data.growDuration);
            float.TryParse(cols[4], out data.productInterval);
            int.TryParse(cols[5], out data.maxProductCount);

            data.iconAddress = cols[6].Trim();
            data.prefabAddress = cols[7].Trim();
            data.productId = cols[8].Trim();

            if (!animals.ContainsKey(data.id))
                animals.Add(data.id, data);
            else
                Debug.LogWarning($"Duplicate Animal ID '{data.id}' found. Skipped line {i + 1}.");
        }

        Debug.Log($"✅ Loaded {animals.Count} animals from CSV.");
        return UniTask.FromResult(animals);
    }
}
