using UnityEngine;

public class BreakablePot : MonoBehaviour
{
    public GameObject bonePrefab;
    public GameObject heartPrefab;
    public float boneDropChance = 0.5f;
    public float heartDropChance = 0.2f;

    public void Break()
    {
        float rand = Random.value;

        if (rand < heartDropChance)
            Instantiate(heartPrefab, transform.position, Quaternion.identity);
        else if (rand < boneDropChance + heartDropChance)
            Instantiate(bonePrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pellet"))
        {
            Break();
            Destroy(other.gameObject);
        }
    }
}
