using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public enum eWorkerState
{
    Idle,
    Moving,
    Harvesting,
    Planting,
    Returning
}

[RequireComponent(typeof(NavMeshAgent))]
public class Worker : MonoBehaviour
{
    [Header("Worker Settings")]
    [SerializeField] private float harvestRange = 2.5f;
    [SerializeField] private float actionDelay = 1.0f;   // Thời gian thực hiện mỗi hành động

    [SerializeField] private NavMeshAgent agent;
    private eWorkerState state = eWorkerState.Idle;
    private Coroutine currentRoutine;
    private Vector3 lastTarget;

    void Start()
    {
        GoIdle();
    }

    void Update()
    {
        if (state == eWorkerState.Moving && !agent.pathPending && agent.remainingDistance <= harvestRange)
        {
            
            if (currentRoutine == null)
                currentRoutine = StartCoroutine(PerformActionAtDestination());
        }
    }

    private IEnumerator PerformActionAtDestination()
    {
        Debug.Log(2);
        yield return new WaitForSeconds(actionDelay);

        switch (state)
        {
            case eWorkerState.Harvesting:
                CollectNearestProduct();
                break;

            case eWorkerState.Planting:
                PlantSeedAtPlot();
                break;
        }

        yield return new WaitForSeconds(0.5f);
        FindNextTask();
    }

    // ============================================================
    // 🔹 TÌM VIỆC
    // ============================================================
    public void FindNextTask()
    {
        Debug.Log("Find Task");
        var readyProduct = ProductManager.Instance.GetNearestProduct(transform.position);
        if (readyProduct != null)
        {
            MoveTo(readyProduct.transform.position, eWorkerState.Harvesting);
            return;
        }
        var emptyPlot = FarmManager.Instance.GetNearestEmptyPlot(transform.position);
        if (emptyPlot != null)
        {
            MoveTo(emptyPlot.transform.position, eWorkerState.Planting);
            return;
        }

        // 3️⃣ Không có việc → trở về kho
        ReturnToWarehouse();
    }

    // ============================================================
    // 🧺 HÀNH ĐỘNG
    // ============================================================
    private void CollectNearestProduct()
    {
        var nearest = ProductManager.Instance.GetNearestProduct(transform.position);
        if (nearest == null) return;


        ResourceManager.Instance.AddProduct(nearest.productData.id, 1);
        Destroy(nearest.gameObject);
        Debug.Log("Collect");
    }

    private void PlantSeedAtPlot()
    {
        var plot = FarmManager.Instance.GetNearestEmptyPlot(transform.position);
        if (plot == null) return;

        // Chọn loại hạt đầu tiên mà người chơi có
        var seeds = ResourceManager.Instance.GetAllSeeds();
        if (seeds.Count == 0) return;

        var firstSeed = DataManager.GetSeedById(seeds[0].id);
        if (firstSeed == null || seeds[0].quantity <= 0)
        {
            Debug.LogWarning("No seeds available for planting!");
            return;
        }

        CultivationManager.Instance.RegisterCropPlot(plot, firstSeed);
        plot.Purpose = ePlotPurpose.Farming;
        ResourceManager.Instance.UseSeed(firstSeed.id, 1);

        Debug.Log($"{name} planted {firstSeed.name} on plot {plot.name}");
    }

    // ============================================================
    // 🚶 DI CHUYỂN
    // ============================================================
    private void MoveTo(Vector3 target, eWorkerState newState)
    {
        if (agent == null || !agent.isOnNavMesh)
        {
            Debug.LogWarning($"{name}: NavMeshAgent is not on NavMesh. Skipping MoveTo.");
            return;
        }

        state = newState;
        agent.isStopped = false;
        agent.SetDestination(target);
        lastTarget = target;
        currentRoutine = null;
    }

    private void ReturnToWarehouse()
    {
        // if (warehousePoint == null)
        // {
        //     Debug.LogWarning("Warehouse point not set!");
        //     return;
        // }

        if (agent != null && agent.isOnNavMesh)
            MoveTo(new Vector3(0, 4, 0), eWorkerState.Returning);
        else
            Debug.LogWarning($"{name}: cannot return, agent not on NavMesh!");

        StartCoroutine(WaitAndIdle());
    }

    private IEnumerator WaitAndIdle()
    {
        yield return new WaitForSeconds(3f);
        GoIdle();
    }

    private void GoIdle()
    {
        state = eWorkerState.Idle;

        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = true;

        Debug.Log($"{name} is idle at warehouse.");
        Invoke(nameof(FindNextTask), 2f);
    }

}
