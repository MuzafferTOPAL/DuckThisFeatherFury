using UnityEngine;

public enum FoxKind { Kit, Normal, Helmet, Boss }

// Düşman tilki: göz kırpar, kuş havadayken korkar, hasar alınca morarır, sallanır.
// Çeşitler: yavru (küçük, zayıf), normal, miğferli (dayanıklı), patron (büyük, çok dayanıklı).
[RequireComponent(typeof(Destructible))]
public class Fox : MonoBehaviour
{
    public FoxKind kind = FoxKind.Normal;

    SpriteRenderer sr;
    Transform visual;
    GameArt.FoxArt art;
    Destructible destructible;
    float blinkTimer, blinkLeft, wobble, phase;
    bool hurt;

    public const float Radius = 0.55f;
    static readonly Vector2 SpriteOffset = new Vector2(1f, 2f) / 16f;

    public static float Scale(FoxKind k) => k == FoxKind.Kit ? 0.7f : k == FoxKind.Boss ? 1.5f : 1f;

    public static Fox Create(FoxKind kind, Vector2 bottomCenter)
    {
        float s = Scale(kind);
        var go = new GameObject("Tilki_" + kind);
        go.transform.position = bottomCenter + Vector2.up * Radius * s;
        go.transform.localScale = new Vector3(s, s, 1f);

        var rb = go.AddComponent<Rigidbody2D>();
        rb.mass = kind == FoxKind.Boss ? 4f : kind == FoxKind.Kit ? 0.6f : 1.2f;
        var cc = go.AddComponent<CircleCollider2D>();
        cc.radius = Radius;
        cc.sharedMaterial = PhysMats.Fox;

        var visual = new GameObject("Gorsel").transform;
        visual.SetParent(go.transform, false);
        var srGo = new GameObject("Sprite");
        srGo.transform.SetParent(visual, false);
        srGo.transform.localPosition = SpriteOffset;
        var sr = srGo.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 7;

        var d = go.AddComponent<Destructible>();
        d.material = BlockMaterial.Fox;
        d.isEnemy = true;
        switch (kind)
        {
            case FoxKind.Kit: d.ResetHealth(3f); d.scoreValue = 3000; break;
            case FoxKind.Helmet: d.ResetHealth(12f); d.scoreValue = 7000; break;
            case FoxKind.Boss: d.ResetHealth(28f); d.scoreValue = 15000; break;
            default: d.ResetHealth(6f); d.scoreValue = 5000; break;
        }

        var f = go.AddComponent<Fox>();
        f.kind = kind;
        f.sr = sr;
        f.visual = visual;
        f.art = (kind == FoxKind.Helmet || kind == FoxKind.Boss) ? GameArt.I.foxHelmet : GameArt.I.fox;
        sr.sprite = f.art.idle;
        return f;
    }

    void Awake()
    {
        destructible = GetComponent<Destructible>();
        destructible.OnDamaged += _ => { hurt = true; wobble = 1f; };
        blinkTimer = Random.Range(1f, 4f);
        phase = Random.Range(0f, 10f);
    }

    void Update()
    {
        float dt = Time.deltaTime;
        blinkTimer -= dt;
        blinkLeft -= dt;
        if (blinkTimer <= 0f)
        {
            blinkLeft = 0.14f;
            blinkTimer = Random.Range(2f, 5f);
        }

        if (art != null && sr != null)
        {
            Sprite s;
            if (hurt) s = art.hurt;
            else if (BirdIsNear()) s = art.scared;
            else if (blinkLeft > 0f) s = art.blink;
            else s = art.idle;
            sr.sprite = s;
        }

        if (visual != null)
        {
            float sx = 1f, sy = 1f;
            if (wobble > 0f)
            {
                wobble = Mathf.MoveTowards(wobble, 0f, dt * 2.5f);
                float k = Mathf.Sin(wobble * 25f) * wobble * 0.18f;
                sx += k; sy -= k;
            }
            else
            {
                float b = Mathf.Sin(Time.time * 2.2f + phase) * 0.03f;
                sx -= b; sy += b;
            }
            visual.localScale = new Vector3(sx, sy, 1f);
        }
    }

    bool BirdIsNear()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.LeadBird == null) return false;
        return Vector2.Distance(gm.LeadBird.position, transform.position) < 7f;
    }
}
