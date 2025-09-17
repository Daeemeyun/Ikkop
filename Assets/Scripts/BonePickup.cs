using UnityEngine;

public class BonePickup : MonoBehaviour
{
    public int amount = 1;
    public AudioClip pickupSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // work on this function when possible - currencyManager
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.AddBones(amount);

                //pickup sound
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }

                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("CurrencyManager instance not found!");
            }
        }
    }
}
