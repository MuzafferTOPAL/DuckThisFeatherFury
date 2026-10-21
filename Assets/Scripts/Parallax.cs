using UnityEngine;

// Arka plan katmanlarını kamerayla farklı hızlarda kaydırır (derinlik hissi).
// driftSpeed > 0 ise (bulutlar gibi) kendi kendine kayar ve wrapWidth içinde döner.
public class Parallax : MonoBehaviour
{
    [Range(0f, 1f)] public float factor = 0.5f;
    public float driftSpeed;
    public float wrapWidth;
    public float verticalFactor;

    Transform cam;
    Vector3 start;
    float drift;
    bool started;

    void Start() => Init();

    void Init()
    {
        if (started) return;
        started = true;
        cam = Camera.main != null ? Camera.main.transform : null;
        start = transform.position;
    }

    void LateUpdate()
    {
        if (!started) Init();
        if (cam == null) return;
        drift += driftSpeed * Time.deltaTime;
        float x = start.x + cam.position.x * factor + drift;
        if (wrapWidth > 0f)
        {
            float rel = x - cam.position.x;
            rel = Mathf.Repeat(rel + wrapWidth * 0.5f, wrapWidth) - wrapWidth * 0.5f;
            x = cam.position.x + rel;
        }
        transform.position = new Vector3(x, start.y + cam.position.y * verticalFactor, start.z);
    }
}
