using UnityEngine;

public class CompanionBullet : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;
    private CompanionAI owner;
    private float timer;

    public void Init(CompanionAI companionAI)
    {
        owner = companionAI;
        timer = lifetime;
    }

    private void OnEnable()
    {
        timer = lifetime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            Deactivate();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Deactivate();
    }

    private void Deactivate()
    {
        if (owner != null)
            owner.ReturnToPool(gameObject);
        else
            gameObject.SetActive(false);
    }
}