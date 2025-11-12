using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : Singleton<AnimalManager>
{
    [SerializeField] private List<AnimalUnit> activeAnimals = new();
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

        GameObject animal;
        data.LoadPrefab(prefab =>
        {
            if (prefab != null)
            {
                animal = Instantiate(prefab, plot.transform.position + new Vector3(0, 5, 0), Quaternion.identity, plot.transform);
                plot.SetEntity(animal);
            }
        });

        Debug.Log($"Added new {data.name} to the farm!");
    }

    public void RemoveAnimal(AnimalUnit unit)
    {
        if (unit == null) return;
        activeAnimals.Remove(unit);
    }

    public void RemoveAnimalByPlot(Plot plot)
    {
        if (plot == null) return;

        for (int i = activeAnimals.Count - 1; i >= 0; i--)
        {
            AnimalUnit unit = activeAnimals[i];
            if (unit == null || unit.plot == null) continue;

            if (unit.plot == plot)
            {
                plot.RemoveEntity();
                activeAnimals.RemoveAt(i);

                Debug.Log($"🐮 Removed animal from plot: {plot.name}");
            }
        }
    }

    public AnimalUnit GetAnimalData(Plot plot)
    {
        return activeAnimals.Find(p => p.plot == plot);
    }

    public List<AnimalUnit> GetAllAnimals() => activeAnimals;
}
