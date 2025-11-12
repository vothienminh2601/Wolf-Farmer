using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SeedCSVReader : ICSVReader<SeedData>
{
    public UniTask<Dictionary<string, SeedData>> LoadDataFromCSV(TextAsset csvFile)
    {
        Dictionary<string, SeedData> seeds = new();

        if (csvFile == null)
        {
            Debug.LogWarning("CSV file not assigned.");
            return UniTask.FromResult(seeds);
        }

        string[] lines = csvFile.text.Split('\n');
        if (lines.Length <= 1)
        {
            Debug.LogWarning("CSV file is empty or missing header.");
            return UniTask.FromResult(seeds);
        }

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] cols = line.Split('\t'); // dùng tab nếu file xuất từ Excel
            if (cols.Length < 8)
            {
                // fallback: nếu CSV dùng dấu phẩy
                cols = line.Split(',');
            }

            if (cols.Length < 8)
            {
                Debug.LogWarning($"Line skipped (invalid format): {line}");
                continue;
            }

            // Gán dữ liệu
            SeedData data = new SeedData();
            data.id = cols[0].Trim();
            data.name = cols[1].Trim();

            int.TryParse(cols[2], out data.baseValue);
            float.TryParse(cols[3], out data.stageDuration);
            float.TryParse(cols[4], out data.fruitInterval);
            int.TryParse(cols[5], out data.maxFruitCount);

            data.iconAddress = cols[6].Trim();

            // crop_steps (ngăn cách bằng ;)
            string[] steps = cols[7].Split(';');
            data.cropStepAddresses = new List<string>();
            foreach (string s in steps)
            {
                string path = s.Trim();
                if (!string.IsNullOrEmpty(path))
                    data.cropStepAddresses.Add(path);
            }

            data.fruitId = cols[8].Trim();

            if (!seeds.ContainsKey(data.id))
                seeds.Add(data.id, data);
            else
                Debug.LogWarning($"Duplicate seed ID '{data.id}' found. Skipped line {i + 1}.");
        }

        Debug.Log($"Loaded {seeds.Count} seeds from CSV.");
        return UniTask.FromResult(seeds);
    }
}
