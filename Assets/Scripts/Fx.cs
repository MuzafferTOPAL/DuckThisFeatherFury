using System.Collections.Generic;
using UnityEngine;

// Pixel parçacık sistemi: kare kıymıklar, tüyler, toz bulutu, yıldızlar, patlama, uçuş izi.
// Nesneler havuzda tutulur.
public class Fx : MonoBehaviour
{
    class Particle
    {
        public SpriteRenderer sr;
        public Vector2 vel;
        public float angVel, age, life, gravity, size0, size1, drag;
        public Color color;
        public bool fade = true;
    }

    static Fx instance;
    static Fx I
    {
        get
        {
            if (instance == null) instance = new GameObject("Fx").AddComponent<Fx>();
            return instance;
        }
    }

    readonly List<Particle> live = new List<Particle>();
    readonly Stack<Particle> pool = new Stack<Particle>();
    readonly List<GameObject> trail = new List<GameObject>();

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    Particle Get()
    {
        Particle p = null;
        while (pool.Count > 0 && p == null)
        {
            p = pool.Pop();
            if (p.sr == null) p = null;
        }
        if (p == null)
        {
            var go = new GameObject("p");
            go.transform.SetParent(transform, false);
            p = new Particle { sr = go.AddComponent<SpriteRenderer>() };
        }
        p.sr.gameObject.SetActive(true);
        return p;
    }

    public static void Spawn(Sprite sprite, Vector2 pos, Vector2 vel, float size0, float size1, float life,
                             Color color, float gravity = 1f, float angVel = 0f, int order = 25, float drag = 0f, bool randomRot = true)
    {
        if (sprite == null) return;
        var fx = I;
        var p = fx.Get();
        p.sr.sprite = sprite;
        p.sr.sortingOrder = order;
        p.sr.color = color;
        p.sr.transform.position = pos;
        p.sr.transform.rotation = randomRot ? Quaternion.Euler(0, 0, Random.Range(0, 4) * 90f) : Quaternion.identity;
        p.sr.transform.localScale = new Vector3(size0, size0, 1);
        p.vel = vel;
        p.angVel = angVel;
        p.age = 0f;
        p.life = Mathf.Max(0.05f, life);
        p.gravity = gravity;
        p.size0 = size0;
        p.size1 = size1;
        p.drag = drag;
        p.color = color;
        fx.live.Add(p);
    }

    static GameArt Art => GameArt.I;

    // Malzemeye göre renkli kare kıymıklar
    public static void Debris(Vector2 pos, int count, BlockMaterial m, float speed)
    {
        if (Art == null) return;
        Color[] cols;
        switch (m)
        {
            case BlockMaterial.Stone: cols = new[] { C(0x9aa1aa), C(0x6f7680), C(0xc3c9d0) }; break;
            case BlockMaterial.Ice: cols = new[] { C(0xa8e4ff), C(0xffffff), C(0x6fc0e8) }; break;
            case BlockMaterial.Tnt: cols = new[] { C(0xd63c2e), C(0x4a1410), C(0xfff4d6) }; break;
            case BlockMaterial.Fox: cols = new[] { C(0xe8772e), C(0xfff3e2), C(0xb5531c) }; break;
            default: cols = new[] { C(0xc88a4a), C(0x8a5426), C(0xe8b070) }; break;
        }
        for (int i = 0; i < count; i++)
        {
            Vector2 dir = Random.insideUnitCircle.normalized * Random.Range(0.35f, 1f) * speed;
            dir.y += speed * 0.4f;
            float s = Random.Range(1f, 2.2f);
            Spawn(Art.fxPixel, pos + Random.insideUnitCircle * 0.3f, dir, s, s * 0.6f, Random.Range(0.6f, 1.1f),
                  cols[Random.Range(0, cols.Length)], 1f, Random.Range(-360f, 360f), 25);
        }
    }

    public static void Feathers(Vector2 pos, int count, Color color)
    {
        if (Art == null) return;
        for (int i = 0; i < count; i++)
        {
            Vector2 v = Random.insideUnitCircle * 2.5f + Vector2.up * 1.2f;
            Spawn(Art.fxFeather, pos, v, 1f, 0.8f, Random.Range(0.9f, 1.4f), color, 0.15f, Random.Range(-200f, 200f), 26, 2f, false);
        }
    }

    public static void Puff(Vector2 pos, float size, int count = 6, Color? tint = null)
    {
        if (Art == null) return;
        Color c = tint ?? new Color(1f, 1f, 1f, 0.9f);
        for (int i = 0; i < count; i++)
        {
            Vector2 v = Random.insideUnitCircle * size * 1.8f;
            float s = size * Random.Range(1.2f, 2f);
            Spawn(Art.fxPuff, pos + Random.insideUnitCircle * size * 0.3f, v, s * 0.4f, s, Random.Range(0.4f, 0.7f),
                  c, -0.05f, 0f, 28, 4f, false);
        }
    }

    public static void Stars(Vector2 pos, int count)
    {
        if (Art == null) return;
        for (int i = 0; i < count; i++)
        {
            Vector2 v = Random.insideUnitCircle.normalized * Random.Range(1.5f, 3.5f) + Vector2.up * 2f;
            Spawn(Art.fxStar, pos, v, 1f, 0.6f, Random.Range(0.6f, 1f), Color.white, 0.6f, Random.Range(-180f, 180f), 30, 0f, false);
        }
    }

    public static void Boom(Vector2 pos, float radius)
    {
        if (Art == null) return;
        Spawn(Art.fxBoom, pos, Vector2.zero, radius * 0.6f, radius * 1.4f, 0.35f, Color.white, 0f, 0f, 31, 0f, false);
        for (int i = 0; i < 10; i++)
        {
            Vector2 v = Random.insideUnitCircle.normalized * Random.Range(3f, 7f);
            Spawn(Art.fxPixel, pos, v, 2f, 1f, Random.Range(0.4f, 0.8f), i % 2 == 0 ? C(0xffd23f) : C(0xff8a2a), 0.5f, 0f, 30);
        }
        Puff(pos, radius * 0.6f, 8, new Color(0.35f, 0.32f, 0.35f, 0.85f));
    }

    public static void TrailDot(Vector2 pos, float size)
    {
        if (Art == null) return;
        var go = new GameObject("iz");
        go.transform.SetParent(I.transform, false);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(size, size, 1);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Art.fxDot;
        sr.color = new Color(1, 1, 1, 0.85f);
        sr.sortingOrder = 4;
        I.trail.Add(go);
    }

    public static void ClearTrail()
    {
        if (instance == null) return;
        foreach (var go in instance.trail)
            if (go != null) Destroy(go);
        instance.trail.Clear();
    }

    // Bölüm değişince her şeyi temizle
    public static void ClearAll()
    {
        if (instance == null) return;
        ClearTrail();
        foreach (var p in instance.live)
            if (p.sr != null) { p.sr.gameObject.SetActive(false); instance.pool.Push(p); }
        instance.live.Clear();
    }

    static Color C(int hex) => new Color(((hex >> 16) & 255) / 255f, ((hex >> 8) & 255) / 255f, (hex & 255) / 255f, 1f);

    void Update()
    {
        float dt = Time.deltaTime;
        for (int i = live.Count - 1; i >= 0; i--)
        {
            var p = live[i];
            if (p.sr == null) { live.RemoveAt(i); continue; }
            p.age += dt;
            if (p.age >= p.life)
            {
                p.sr.gameObject.SetActive(false);
                pool.Push(p);
                live.RemoveAt(i);
                continue;
            }
            float t = p.age / p.life;
            p.vel += Physics2D.gravity * p.gravity * dt;
            p.vel *= 1f / (1f + p.drag * dt);
            var tr = p.sr.transform;
            tr.position += (Vector3)(p.vel * dt);
            if (p.angVel != 0f) tr.Rotate(0, 0, p.angVel * dt);
            float s = Mathf.Lerp(p.size0, p.size1, t);
            tr.localScale = new Vector3(s, s, 1);
            Color c = p.color;
            if (t > 0.6f) c.a *= 1f - (t - 0.6f) / 0.4f;
            p.sr.color = c;
        }
    }
}
