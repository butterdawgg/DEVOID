using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    [SerializeField] private Player[] playerPrefabs;
    [SerializeField] private bool overrideSerializedValue;
    [SerializeField] private int overrideIndex;

    private static PlayerInitializer instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (overrideSerializedValue)
        {
            Instantiate(playerPrefabs[overrideIndex].gameObject,
                Vector3.zero, Quaternion.identity);
        }
        else
        {
            Instantiate(playerPrefabs[SerializeManager.GetPlayerClassID()].gameObject,
                Vector3.zero, Quaternion.identity);
        }
    }
}