using UnityEngine;

[System.Serializable]
public class AnimalUnit
{
    public AnimalData data;
    public Plot plot;
    public float growTimer;
    public float productTimer;
    public int productCount;
    public bool IsAdult => growTimer >= data.growDuration;
    public bool IsExhausted => productCount >= data.maxProductCount;
    public AnimalUnit(AnimalData animalData, Plot plot)
    {
        this.plot = plot;
        data = animalData;
        growTimer = 0f;
        productTimer = 0f;
        productCount = 0;
    }

    public void Tick(float deltaTime)
    {
        if (IsExhausted) return;

        if (!IsAdult)
        {
            growTimer += deltaTime;
            if (IsAdult)
                Debug.Log($"{data.name} has grown up!");
        }
        else
        {
            productTimer += deltaTime;
            if (productTimer >= data.productInterval)
            {
                productTimer = 0f;
                Produce();
            }
        }
    }

    private void Produce()
    {
        productCount++;
        var productData = DataManager.GetProductById(data.productId);
        if (productData == null) return;
        
        Vector3 spawnPos = plot.transform.position + new Vector3(Random.Range(-1, 1), 3, Random.Range(-1, 1));
        ProductManager.Instance.SpawnProduct(productData, spawnPos, plot);
        Debug.Log($"{data.name} produced {data.productId}");
    }
}
