using UnityEngine;

public class Product : MonoBehaviour
{
    protected ProductData productData;

    public void Init(ProductData data)
    {
        productData = data;
    }
}
