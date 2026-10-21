using UnityEngine;

// Kamera: ekran oranına göre bölümü sığdırır, açılışta kaleden sapana kayar,
// uçan kuşu kadrajda tutar ve çarpışmalarda sarsılır. Her bölümde Configure ile ayarlanır.
[RequireComponent(typeof(Camera))]
public class CameraRig : MonoBehaviour
{
    public static CameraRig Instance { get; private set; }

    public float minX = -11.5f, maxX = 10f, groundY = -4f, bottomMargin = 1.4f, minSize = 5.2f, maxSize = 10f;
    public Vector2 introFocus = new Vector2(6f, -1f);
    public float introZoom = 0.62f, introDuration = 2.4f;

    [HideInInspector] public Transform follow;

    Camera cam;
    float introT = 1f;
    float shake;
    Vector3 curPos, posVel;
    float curSize, sizeVel;
    bool frozen;

    public bool IntroDone => introT >= 1f;
    public Camera Cam => cam;

    void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        BaseView(out curPos, out curSize);
    }

    public void Configure(float levelMinX, float levelMaxX, Vector2 focus, bool playIntro)
    {
        minX = levelMinX;
        maxX = levelMaxX;
        introFocus = focus;
        follow = null;
        introT = playIntro ? 0f : 1f;
        BaseView(out curPos, out curSize);
        posVel = Vector3.zero;
        sizeVel = 0f;
        frozen = playIntro;   // StartIntro çağrılana kadar kaleye bakarak bekle
        if (playIntro) SetIntroStart();
        Apply(Vector3.zero);
    }

    public void StartIntro()
    {
        frozen = false;
        introT = 0f;
    }

    public void SkipIntro()
    {
        frozen = false;
        introT = 1f;
    }

    public void Shake(float amount)
    {
        shake = Mathf.Min(0.7f, Mathf.Max(shake, amount));
    }

    void BaseView(out Vector3 pos, out float size)
    {
        float width = maxX - minX;
        size = Mathf.Clamp(width * 0.5f / cam.aspect, minSize, maxSize);
        pos = new Vector3((minX + maxX) * 0.5f, groundY - bottomMargin + size, -10f);
    }

    void SetIntroStart()
    {
        BaseView(out _, out float baseSize);
        float zs = baseSize * introZoom;
        curPos = new Vector3(introFocus.x, Mathf.Max(introFocus.y, groundY - bottomMargin + zs), -10f);
        curSize = zs;
    }

    void LateUpdate()
    {
        BaseView(out Vector3 targetPos, out float targetSize);

        if (frozen)
        {
            SetIntroStart();
        }
        else if (introT < 1f)
        {
            if (GameInput.PressedThisFrame && introT > 0.15f) introT = 1f;
            introT = Mathf.Min(1f, introT + Time.unscaledDeltaTime / introDuration);
            float hold = 0.3f;
            float k = Mathf.Clamp01((introT - hold) / (1f - hold));
            k = k * k * (3f - 2f * k);
            float zs = targetSize * introZoom;
            Vector3 zp = new Vector3(introFocus.x, Mathf.Max(introFocus.y, groundY - bottomMargin + zs), -10f);
            curPos = Vector3.Lerp(zp, targetPos, k);
            curSize = Mathf.Lerp(zs, targetSize, k);
            posVel = Vector3.zero;
            sizeVel = 0f;
        }
        else
        {
            if (follow != null)
            {
                Vector2 p = follow.position;
                float top = targetPos.y + targetSize;
                float needTop = p.y + 1.4f;
                if (needTop > top)
                {
                    float extra = Mathf.Min(needTop - top, targetSize);
                    targetSize += extra * 0.5f;
                    targetPos.y += extra * 0.5f;
                }
                float halfW = targetSize * cam.aspect;
                if (p.x + 1.5f > targetPos.x + halfW) targetPos.x = p.x + 1.5f - halfW;
                if (p.x - 1.5f < targetPos.x - halfW) targetPos.x = p.x - 1.5f + halfW;
            }
            curPos = Vector3.SmoothDamp(curPos, targetPos, ref posVel, 0.35f);
            curSize = Mathf.SmoothDamp(curSize, targetSize, ref sizeVel, 0.35f);
        }

        Vector3 offset = Vector3.zero;
        if (shake > 0f)
        {
            offset = (Vector3)(Random.insideUnitCircle * shake);
            shake = Mathf.MoveTowards(shake, 0f, Time.deltaTime * 1.5f);
        }
        Apply(offset);
    }

    void Apply(Vector3 offset)
    {
        transform.position = curPos + offset;
        cam.orthographicSize = curSize;
    }
}
