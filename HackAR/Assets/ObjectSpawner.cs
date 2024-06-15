using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    public List<GameObject> prefabsToSpawn;
    public float spawnForce = 10f;
    public Vector3 spawnDirection = Vector3.back;
    public float spawnRate = 1f; // objects per second
    public float objectLifetime = 5f; // seconds before objects get destroyed
    public Vector3 rotationOffset;

    public GameObject persistentObject; // Reference to the persistent object

    private MeshFilter persistentMeshFilter;
    private Renderer persistentRenderer;

    private float spawnTimer = 0f;

    void Start()
    {
        // Get the MeshFilter and Renderer of the persistent object
        persistentMeshFilter = persistentObject.GetComponent<MeshFilter>();
        persistentRenderer = persistentObject.GetComponent<Renderer>();
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= 1f / spawnRate)
        {
            SpawnPrefab();
            spawnTimer = 0f;
        }
        spawnDirection = this.transform.forward;
    }

    void SpawnPrefab()
    {
        if (prefabsToSpawn.Count == 0)
        {
            Debug.LogError("No prefabs to spawn!");
            return;
        }

        int randomIndex = Random.Range(0, prefabsToSpawn.Count);
        GameObject prefab = prefabsToSpawn[randomIndex];

        GameObject newObject = Instantiate(prefab, transform.position, transform.rotation);
        newObject.transform.Rotate(rotationOffset);
        Rigidbody rb = newObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(spawnDirection.normalized * spawnForce, ForceMode.Impulse);
        }

        // Assign MeshFilter and Renderer of persistent object to the spawned prefab
        if (persistentMeshFilter != null && persistentRenderer != null)
        {
            MeshFilter objMeshFilter = newObject.GetComponent<MeshFilter>();
            Renderer objRenderer = newObject.GetComponent<Renderer>();

            if (objMeshFilter != null && objRenderer != null)
            {
                objMeshFilter.mesh = persistentMeshFilter.mesh;
                objRenderer.sharedMaterials = persistentRenderer.sharedMaterials;
            }
            else
            {
                Debug.LogWarning("MeshFilter or Renderer component not found on the spawned prefab.");
            }
        }
        else
        {
            Debug.LogWarning("MeshFilter or Renderer component not found on the persistent object.");
        }

        StartCoroutine(DeleteObjectAfterTime(newObject, objectLifetime));
    }

    IEnumerator DeleteObjectAfterTime(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }
}
