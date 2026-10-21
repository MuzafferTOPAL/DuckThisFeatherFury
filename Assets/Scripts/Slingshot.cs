using System;
using UnityEngine;

// Sapan: kuşu sürükle-bırak ile fırlatır. Lastikler, deri cep ve noktalı yörünge burada.
public class Slingshot : MonoBehaviour
{
    public Transform launchPoint;
    public float maxPull = 2f;
    public float launchPower = 8f;
    public float grabRadius = 1.5f;
    public float minPullY = -3.5f;
    public float groundY = -4f;

    public Transform backTip, frontTip;
    public LineRenderer backBand, frontBand;
    public Transform pouch;

    public event Action<Bird> OnLaunched;
    public bool HasBird => current != null;

    Bird current;
    bool dragging;
    Camera cam;
    SpriteRenderer[] dots;
    Vector2 pouchPos, pouchVel;
    const int DotCount = 18;

    // ---------------------------------------------------------------- kurulum
    public static Slingshot Create(Vector2 basePos, float groundY)
    {
        var art = GameArt.I;
        var go = new GameObject("Sapan");
        go.transform.position = basePos;
        var back = MakeSprite("ArkaKol", art.slingBack, 6, go.transform);
        MakeSprite("OnKol", art.slingFront, 16, go.transform);

        var s = go.AddComponent<Slingshot>();
        s.groundY = groundY;
        s.minPullY = groundY + 0.5f;
        s.backTip = MakePoint("ArkaUc", go.transform, new Vector2(5.5f, 33f) / 16f);
        s.frontTip = MakePoint("OnUc", go.transform, new Vector2(-6.5f, 33f) / 16f);
        s.launchPoint = MakePoint("LaunchPoint", go.transform, new Vector2(0f, 31f) / 16f);
        s.pouch = MakeSprite("Cep", art.pouch, 9, go.transform).transform;
        s.backBand = MakeBand("ArkaLastik", go.transform, back.sharedMaterial, 8);
        s.frontBand = MakeBand("OnLastik", go.transform, back.sharedMaterial, 15);
        s.pouchPos = s.RestPos;
        return s;
    }

    static SpriteRenderer MakeSprite(string name, Sprite sp, int order, Transform parent)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sp;
        sr.sortingOrder = order;
        return sr;
    }

    static Transform MakePoint(string name, Transform parent, Vector2 local)
    {
        var t = new GameObject(name).transform;
        t.SetParent(parent, false);
        t.localPosition = local;
        return t;
    }

    static LineRenderer MakeBand(string name, Transform parent, Material mat, int order)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var lr = go.AddComponent<LineRenderer>();
        lr.sharedMaterial = mat;
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.widthMultiplier = 0.14f;
        var c = new Color(0.24f, 0.14f, 0.09f);
        lr.startColor = c;
        lr.endColor = c;
        lr.sortingOrder = order;
        return lr;
    }

    void Awake()
    {
        cam = Camera.main;
        dots = new SpriteRenderer[DotCount];
        for (int i = 0; i < DotCount; i++)
        {
            var go = new GameObject("YorungeNoktasi");
            go.transform.SetParent(transform, false);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 30;
            go.SetActive(false);
            dots[i] = sr;
        }
    }

    Vector2 RestPos => (Vector2)launchPoint.position + new Vector2(-0.05f, -0.15f);

    public void LoadBird(Bird b)
    {
        current = b;
        b.transform.position = launchPoint.position;
    }

    public void ClearBird()
    {
        current = null;
        dragging = false;
        HideDots();
    }

    // ---------------------------------------------------------------- döngü
    void Update()
    {
        if (cam == null) cam = Camera.main;
        bool canControl = current != null && current.IsReady && !GameManager.InputBlocked &&
                          (CameraRig.Instance == null || CameraRig.Instance.IntroDone);

        if (canControl) HandleInput();
        else if (dragging) CancelDrag();

        UpdatePouch(Time.deltaTime);
        UpdateBands();
    }

    void HandleInput()
    {
        Vector2 world = ScreenToWorld(GameInput.Position);

        if (GameInput.PressedThisFrame && !GameInput.OverUI &&
            Vector2.Distance(world, current.transform.position) < grabRadius)
        {
            dragging = true;
            current.SetAiming(true);
            Sfx.Play(Sfx.Stretch, 0.8f);
        }

        if (dragging && GameInput.IsPressed)
        {
            Vector2 lp = launchPoint.position;
            Vector2 offset = Vector2.ClampMagnitude(world - lp, maxPull);
            Vector2 p = lp + offset;
            if (p.y < minPullY) { p.y = minPullY; offset = p - lp; }
            current.transform.position = p;
            current.SetAimDirection(-offset);
            ShowDots(p, -offset * launchPower, offset.magnitude / maxPull);
        }

        if (dragging && GameInput.ReleasedThisFrame)
        {
            dragging = false;
            HideDots();
            current.SetAiming(false);

            Vector2 offset = (Vector2)current.transform.position - (Vector2)launchPoint.position;
            if (offset.magnitude < 0.3f)
            {
                current.transform.position = launchPoint.position;
                return;
            }
            Vector2 velocity = -offset * launchPower;
            var launched = current;
            current = null;
            launched.Launch(velocity);
            pouchVel = velocity * 0.9f;
            OnLaunched?.Invoke(launched);
        }
    }

    // Otomatik test için: kuşu verilen hızla fırlat
    public bool AutoLaunch(Vector2 velocity)
    {
        if (current == null || !current.IsReady) return false;
        var b = current;
        current = null;
        dragging = false;
        HideDots();
        b.transform.position = (Vector2)launchPoint.position - velocity / launchPower;
        b.Launch(velocity);
        pouchVel = velocity * 0.9f;
        OnLaunched?.Invoke(b);
        return true;
    }

    void CancelDrag()
    {
        dragging = false;
        HideDots();
        if (current != null)
        {
            current.SetAiming(false);
            current.transform.position = launchPoint.position;
        }
    }

    void UpdatePouch(float dt)
    {
        bool holding = current != null && (current.IsReady || dragging);
        if (holding)
        {
            pouchPos = current.transform.position;
            pouchVel = Vector2.zero;
        }
        else
        {
            Vector2 acc = (RestPos - pouchPos) * 180f - pouchVel * 9f;
            pouchVel += acc * dt;
            pouchPos += pouchVel * dt;
        }
        Vector2 lp = launchPoint.position;
        Vector2 back = pouchPos - lp;
        Vector2 dir = back.sqrMagnitude > 0.01f ? back.normalized : Vector2.left;
        pouch.position = (Vector3)(pouchPos + (holding ? dir * 0.45f : Vector2.zero));
        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;
        pouch.rotation = Quaternion.Euler(0, 0, holding ? ang : 0f);
    }

    void UpdateBands()
    {
        Vector3 end = pouch.position;
        SetBand(backBand, backTip, end);
        SetBand(frontBand, frontTip, end);
    }

    static void SetBand(LineRenderer lr, Transform tip, Vector3 end)
    {
        if (lr == null || tip == null) return;
        lr.SetPosition(0, tip.position);
        lr.SetPosition(1, end);
        float stretch = Vector3.Distance(tip.position, end);
        lr.widthMultiplier = Mathf.Lerp(0.16f, 0.09f, Mathf.InverseLerp(0.4f, 2.6f, stretch));
    }

    void ShowDots(Vector2 start, Vector2 velocity, float power)
    {
        var art = GameArt.I;
        if (art == null || current == null) return;
        Vector2 g = Physics2D.gravity * current.GravityScale;
        int visible = Mathf.RoundToInt(Mathf.Lerp(6, DotCount, power));
        bool hitGround = false;
        for (int i = 0; i < dots.Length; i++)
        {
            float t = (i + 1) * 0.065f;
            Vector2 p = start + velocity * t + 0.5f * g * t * t;
            if (p.y <= groundY) hitGround = true;
            bool on = i < visible && !hitGround;
            dots[i].gameObject.SetActive(on);
            if (!on) continue;
            float k = (float)i / dots.Length;
            dots[i].sprite = art.fxDot;
            dots[i].transform.position = p;
            float s = Mathf.Lerp(1.1f, 0.5f, k);
            dots[i].transform.localScale = new Vector3(s, s, 1f);
            dots[i].color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0.4f, k));
        }
    }

    void HideDots()
    {
        if (dots == null) return;
        foreach (var d in dots) d.gameObject.SetActive(false);
    }

    Vector2 ScreenToWorld(Vector2 screenPos)
    {
        Vector3 p = screenPos;
        p.z = -cam.transform.position.z;
        return cam.ScreenToWorldPoint(p);
    }
}
