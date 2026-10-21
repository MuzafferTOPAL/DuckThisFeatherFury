using UnityEngine;

// Gökyüzü degradesini her ekran oranında tüm ekranı kaplayacak şekilde ölçekler.
[RequireComponent(typeof(SpriteRenderer))]
public class SkyFit : MonoBehaviour
{
    Camera cam;
    SpriteRenderer sr;

    void Start()
    {
        cam = GetComponentInParent<Camera>();
        sr = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (cam == null || sr == null || sr.sprite == null) return;
        Vector2 spriteSize = sr.sprite.bounds.size;
        float h = cam.orthographicSize * 2f + 1f;
        float w = h * cam.aspect + 1f;
        transform.localScale = new Vector3(w / spriteSize.x, h / spriteSize.y, 1f);
        transform.localPosition = new Vector3(0f, 0f, 20f);
    }
}
