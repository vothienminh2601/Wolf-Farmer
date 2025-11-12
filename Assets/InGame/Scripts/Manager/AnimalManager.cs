using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : Singleton<AnimalManager>
{
    private readonly List<AnimalUnit> activeAnimals = new();
    [SerializeField] private float updateInterval = 1f;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            TickAnimals();
        }
    }

    private void TickAnimals()
    {
        foreach (var animal in activeAnimals)
            animal.Tick(updateInterval);
    }

    public void AddAnimal(AnimalData data, Plot plot)
    {
        if (data == null || plot == null) return;

        var newAnimal = new AnimalUnit(data, plot);
        activeAnimals.Add(newAnimal);

        // Tạo model trong chuồng
        data.LoadPrefab(prefab =>
        {
            if (prefab != null)
                Object.Instantiate(prefab, plot.transform.position + new Vector3(0, 5, 0), Quaternion.identity, plot.transform);
        });

        Debug.Log($"Added new {data.name} to the farm!");
    }

    public void RemoveAnimal(AnimalUnit unit)
    {
        if (unit == null) return;
        activeAnimals.Remove(unit);
    }

    public AnimalUnit GetAnimalData(Plot plot)
    {
        return activeAnimals.Find(p => p.plot == plot);
    }

    public List<AnimalUnit> GetAllAnimals() => activeAnimals;
}
