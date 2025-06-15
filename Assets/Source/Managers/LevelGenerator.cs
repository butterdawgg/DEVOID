using UnityEngine;

[System.Serializable]
public class LevelGeneratorEntry
{
    public GameObject prefab;
    public float probability;
    public Vector2 scaleVariation;
}

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private LevelGeneratorEntry[] entries;
    [SerializeField] private float instanceDensity;
    [SerializeField] private float minRadius;
    [SerializeField] private float maxRadius;

    private void Awake()
    {
        GenerateInstances();
    }

    private void GenerateInstances()
    {
        int instanceNum = (int)(maxRadius * instanceDensity);

        for (int i = 0; i < instanceNum; i++)
        {
            Vector3 point = RandomPointInSphere() + transform.position;
            LevelGeneratorEntry entry = PickPrefab();
            if (entry != null)
            {
                Quaternion randomRotation = Random.rotation;

                GameObject obj =
                    Instantiate(entry.prefab, point, randomRotation);

                if (entry.scaleVariation.y - entry.scaleVariation.y > 0.1f)
                {
                    float randScale = Random.Range(entry.scaleVariation.x,
                    entry.scaleVariation.y);

                    obj.transform.localScale =
                        new Vector3(randScale, randScale, randScale);
                }
            }
        }
    }

    private Vector3 RandomPointInSphere()
    {
        float theta = Random.Range(0f, 2f * Mathf.PI);
        float phi = Mathf.Acos(1f - 2f * Random.Range(0f, 1f));

        float r = Mathf.Pow(
            Mathf.Pow(minRadius, 3f) + Random.Range(0f, 1f) *
            (Mathf.Pow(maxRadius, 3f) - Mathf.Pow(minRadius, 3f)),
            1f / 3f);

        float x = r * Mathf.Sin(phi) * Mathf.Cos(theta);
        float y = r * Mathf.Sin(phi) * Mathf.Sin(theta);
        float z = r * Mathf.Cos(phi);

        return new Vector3(x, y, z);
    }

    private LevelGeneratorEntry PickPrefab()
    {
        if (entries == null || entries.Length == 0) return null;

        float totalWeight = 0f;
        foreach (var entry in entries)
        {
            totalWeight += entry.probability;
        }

        float rand = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var entry in entries)
        {
            cumulative += entry.probability;
            if (rand <= cumulative)
            {
                return entry;
            }
        }

        return entries[entries.Length - 1]; // Fallback
    }
}