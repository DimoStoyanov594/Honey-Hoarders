using UnityEngine;

public class ExpPickup : MonoBehaviour
{
    public int expAmount = 2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EXPManager expManager = other.GetComponent<EXPManager>();

            if (expManager != null)
            {
                expManager.GainExperience(expAmount);
            }

            Destroy(gameObject);
        }
    }
}