using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public Transform[] tiles;
    public Transform cam;
    [Range(0f, 1f)] public float parallaxEffectX = 0.1f;
    [Range(0f, 1f)] public float parallaxEffectY = 0.02f;
    public float maxDriftY = 0.25f;

    private float tileWidth;
    private float[] offsets;
    private float camStartY;

    void Start()
    {
        tileWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        camStartY = cam.position.y;
        offsets = new float[] { 0, tileWidth, -tileWidth };

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].position = new Vector3(
                cam.position.x + offsets[i],
                tiles[i].position.y,
                tiles[i].position.z
            );
        }
    }

    void LateUpdate()
    {
        float camX = cam.position.x;

        // Y parallax
        float camDeltaY = cam.position.y - camStartY;
        float driftY = Mathf.Clamp(camDeltaY * parallaxEffectY, -maxDriftY, maxDriftY);
        float targetY = cam.position.y - driftY;

        for (int i = 0; i < tiles.Length; i++)
        {
            float targetX = camX * parallaxEffectX + offsets[i];
            tiles[i].position = new Vector3(targetX, targetY, tiles[i].position.z);

            float diff = tiles[i].position.x - camX;
            if (diff > tileWidth) offsets[i] -= tileWidth * 3;
            else if (diff < -tileWidth) offsets[i] += tileWidth * 3;
        }
    }
}