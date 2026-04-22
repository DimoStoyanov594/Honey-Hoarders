using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeedX = -0.02f;
    [SerializeField] private float scrollSpeedY = -0.02f;

    private Material materialInstance;
    private Vector2 offset;

    void Start()
    {
        Image img = GetComponent<Image>();

        // Create a unique instance so you don't affect other UI
        materialInstance = new Material(img.material);
        img.material = materialInstance;

        offset = Vector2.zero;
    }

    void Update()
    {
        offset.x += scrollSpeedX * Time.deltaTime;
        offset.y += scrollSpeedY * Time.deltaTime;

        materialInstance.mainTextureOffset = offset;
    }
}