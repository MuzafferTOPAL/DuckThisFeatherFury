using System;
using UnityEngine;

// Fırlatılan kuş. Tür (BirdType) yeteneği belirler:
//  Ördek: kanat çırpma (2x) • Serçe: üçe bölünme • Şahin: hızlı dalış • Kaz: yere çakılma • Karga: patlama
[RequireComponent(typeof(Rigidbody2D))]
public class Bird : MonoBehaviour
{
    public enum State { Waiting, Hopping, Ready, Flying }

    public BirdType type;
    public bool isClone;

    public State CurrentState { get; private set; } = State.Waiting;
    public bool IsReady => CurrentState == State.Ready;
    public bool IsFlying => CurrentState == State.Flying;
    public bool AbilityUsed { get; private set; }
    public float GravityScale => GetComponent<Rigidbody2D>().gravityScale;
    public event Action<Bird> OnFinished;
    public float DamageMult { get; private set; } = 1f;   // dükkân: Hasar yükseltmesi
    float aimRate;                                        // dükkân: Otomatik Nişan (derece/sn)
    float flightTime, aimScan, aimSparkle;
    Transform aimTarget;
    bool noHoming;

    // Hazırda kurulan parçalar
    Transform visual;
    SpriteRenderer sr;
    Rigidbody2D rb;
    Collider2D col;
    GameArt.BirdArt art;

    int flapsLeft;
    bool finished, hasHit, aiming;
    Vector2 aimDir = Vector2.right;
    float sinceHit, stillTimer, lifeTimer, hitFaceLeft, flapAnimLeft, blinkTimer, blinkLeft, squash, autoExplodeTimer = -1f;
    float idleHopT = -1f, idleHopTimer, phase;
    Vector2 lastTrailPos;
    bool bigDot;

    // zıplama
    Vector3 hopFrom, hopTo;
    float hopT, hopDur, hopHeight;
    Action hopDone;

    // ------------------------------------------------------------------ oluşturma
    public static Bird Create(BirdType type, Vector3 pos)
    {
        var info = BirdInfo.Get(type);
        var go = new GameObject("Kus_" + type);
        go.transform.position = pos;
        var rb = go.AddComponent<Rigidbody2D>();
        rb.mass = info.mass;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        var cc = go.AddComponent<CircleCollider2D>();
        cc.radius = info.Radius;
        cc.sharedMaterial = PhysMats.Bird;

        var visual = new GameObject("Gorsel").transform;
        visual.SetParent(go.transform, false);
        var srGo = new GameObject("Sprite");
        srGo.transform.SetParent(visual, false);
        srGo.transform.localPosition = info.SpriteOffset;
        var sr = srGo.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 10;

        var b = go.AddComponent<Bird>();
        b.type = type;
        b.visual = visual;
        b.sr = sr;
        b.art = GameArt.I.Bird(type);
        b.DamageMult = Economy.DamageMult(type);
        b.aimRate = Economy.AimRate(type);
        sr.sprite = b.art.idle;
        return b;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        if (col != null) col.enabled = false;
        blinkTimer = UnityEngine.Random.Range(1f, 3f);
        idleHopTimer = UnityEngine.Random.Range(1.5f, 4f);
        phase = UnityEngine.Random.Range(0f, 10f);
    }

    // ------------------------------------------------------------------ GameManager / Slingshot
    public void HopTo(Vector3 target, float duration, float height, bool toSling, Action done = null)
    {
        hopFrom = transform.position;
        hopTo = target;
        hopT = 0f;
        hopDur = Mathf.Max(0.05f, duration);
        hopHeight = height;
        hopDone = () =>
        {
            CurrentState = toSling ? State.Ready : State.Waiting;
            done?.Invoke();
        };
        CurrentState = State.Hopping;
    }

    public void SetAiming(bool value)
    {
        aiming = value;
        if (!value) aimDir = Vector2.right;
    }

    public void SetAimDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude > 0.001f) aimDir = dir.normalized;
    }

    public void Launch(Vector2 velocity, bool silent = false)
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        if (col != null) col.enabled = true;
        rb.linearVelocity = velocity;
        rb.angularVelocity = 0f;
        CurrentState = State.Flying;
        aiming = false;
        flapsLeft = type == BirdType.Mallard ? 2 : 0;
        squash = 0.3f;
        flapAnimLeft = 0.4f;
        lastTrailPos = transform.position;
        if (!silent)
        {
            Fx.ClearTrail();
            Fx.Puff(transform.position, 0.5f, 4);
            Sfx.Play(Sfx.Launch, 0.8f);
            Sfx.Play(Sfx.Voice(type), 0.9f);
        }
    }

    // ------------------------------------------------------------------ döngü
    void Update()
    {
        float dt = Time.deltaTime;
        switch (CurrentState)
        {
            case State.Waiting:
                idleHopTimer -= dt;
                if (idleHopTimer <= 0f && idleHopT < 0f)
                {
                    idleHopT = 0f;
                    idleHopTimer = UnityEngine.Random.Range(1.5f, 4f);
                }
                break;
            case State.Hopping: UpdateHop(dt); break;
            case State.Flying: UpdateFlying(dt); break;
        }
        UpdateSprite(dt);
        UpdateVisual(dt);
    }

    void UpdateHop(float dt)
    {
        hopT += dt / hopDur;
        float t = Mathf.Clamp01(hopT);
        Vector3 p = Vector3.Lerp(hopFrom, hopTo, t);
        p.y += Mathf.Sin(t * Mathf.PI) * hopHeight;
        transform.position = p;
        if (t >= 1f)
        {
            squash = -0.2f;
            var cb = hopDone;
            hopDone = null;
            cb?.Invoke();
        }
    }

    void UpdateFlying(float dt)
    {
        if (finished) return;

        if (!hasHit && GameInput.PressedThisFrame && !GameInput.OverUI && !GameManager.InputBlocked)
            UseAbility();

        // uçuş izi
        if (!hasHit && Vector2.Distance(lastTrailPos, transform.position) > 0.45f)
        {
            lastTrailPos = transform.position;
            bigDot = !bigDot;
            Fx.TrailDot(transform.position, bigDot ? 1f : 0.65f);
        }

        if (autoExplodeTimer > 0f)
        {
            autoExplodeTimer -= dt;
            if (autoExplodeTimer <= 0f) { CrowExplode(); return; }
        }

        // Bitiş: çarptıktan sonra yavaşladıysa hemen, en geç 3.5 sn sonra; ıskalayıp bölüm dışına çıktıysa anında
        lifeTimer += dt;
        if (hasHit) sinceHit += dt;
        float speed = rb.linearVelocity.magnitude;
        if (speed < (hasHit ? 0.9f : 0.25f)) stillTimer += dt;
        else stillTimer = 0f;

        float maxX = GameManager.Instance != null && GameManager.Instance.Level != null ? GameManager.Instance.Level.maxX : 20f;
        Vector2 p = transform.position;
        bool outOfLevel = p.x > maxX + 4f || p.x < -16f || p.y < -8f;

        if (stillTimer > (hasHit ? 0.5f : 1f) || sinceHit > 3.5f || lifeTimer > 9f || outOfLevel)
            Finish();
    }

    void UseAbility()
    {
        switch (type)
        {
            case BirdType.Mallard:
                if (flapsLeft <= 0) return;
                flapsLeft--;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 6.5f);
                flapAnimLeft = 0.45f;
                squash = 0.2f;
                Fx.Puff((Vector2)transform.position + Vector2.down * 0.45f, 0.45f, 4);
                Fx.Feathers(transform.position, 3, BirdInfo.Get(type).featherColor);
                Sfx.Play(Sfx.Ability(type));
                break;

            case BirdType.Mandarin:
                if (AbilityUsed) return;
                AbilityUsed = true;
                Vector2 v = rb.linearVelocity;
                for (int i = -1; i <= 1; i += 2)
                {
                    var clone = Create(BirdType.Mandarin, transform.position + (Vector3)(Vector2.Perpendicular(v.normalized) * 0.35f * i));
                    clone.isClone = true;
                    clone.AbilityUsed = true;
                    clone.Launch(Quaternion.Euler(0, 0, 9f * i) * v, true);
                    if (GameManager.Instance != null) GameManager.Instance.RegisterExtraBird(clone);
                }
                Fx.Puff(transform.position, 0.6f, 5);
                Sfx.Play(Sfx.Ability(type));
                break;

            case BirdType.Merganser:
                if (AbilityUsed) return;
                AbilityUsed = true;
                Vector2 hv = rb.linearVelocity;
                if (hv.sqrMagnitude < 0.1f) hv = Vector2.right;
                rb.linearVelocity = hv.normalized * Mathf.Max(hv.magnitude * 1.9f, 21f);
                squash = 0.35f;
                Fx.Puff(transform.position, 0.7f, 6);
                Sfx.Play(Sfx.Ability(type));
                break;

            case BirdType.Muscovy:
                if (AbilityUsed) return;
                AbilityUsed = true;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.25f, -22f);
                rb.mass *= 2.5f;
                noHoming = true;
                squash = -0.3f;
                Fx.Puff(transform.position, 0.7f, 6);
                Sfx.Play(Sfx.Ability(type));
                break;

            case BirdType.Scoter:
                if (AbilityUsed) return;
                CrowExplode();
                break;

            case BirdType.Shelduck:
                if (AbilityUsed) return;
                AbilityUsed = true;
                Vector2 sv = rb.linearVelocity;
                EggBomb.Drop((Vector2)transform.position + Vector2.down * 0.45f, new Vector2(sv.x * 0.3f, -9f), DamageMult);
                rb.linearVelocity = new Vector2(sv.x * 1.1f + 1.5f, 9.5f);
                flapAnimLeft = 0.5f;
                squash = 0.3f;
                Fx.Puff((Vector2)transform.position + Vector2.down * 0.4f, 0.5f, 5);
                Sfx.Play(Sfx.Ability(type));
                break;
        }
    }

    public void TestAbility()
    {
        if (CurrentState == State.Flying && !hasHit) UseAbility();
    }

    void CrowExplode()
    {
        if (finished) return;
        AbilityUsed = true;
        Sfx.Play(Sfx.Ability(type), 0.7f);
        Explosion.Explode(transform.position, 3f, 16f, 28f * DamageMult, gameObject);
        FinishSilently();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (CurrentState != State.Flying) return;
        float impact = collision.relativeVelocity.magnitude;

        if (!hasHit)
        {
            hasHit = true;
            var rig = CameraRig.Instance;
            float shake = Mathf.Clamp(impact * 0.012f, 0.03f, 0.25f);
            if (type == BirdType.Muscovy && AbilityUsed) shake = 0.45f;
            if (rig != null) rig.Shake(shake);
            if (type == BirdType.Scoter && !AbilityUsed) autoExplodeTimer = 0.8f;
            if (!isClone && impact > 2f) Sfx.Play(Sfx.DuckHit(type), 0.75f);
        }

        if (impact > 2.5f)
        {
            hitFaceLeft = 0.7f;
            squash = -Mathf.Clamp(impact * 0.025f, 0.1f, 0.35f);
            Fx.Feathers(collision.GetContact(0).point, Mathf.Clamp((int)(impact * 0.5f), 2, 7), BirdInfo.Get(type).featherColor);
            Sfx.Play(Sfx.Hit, Mathf.Clamp01(impact / 15f));
        }
    }

    // ------------------------------------------------------------------ görsel
    // ------------------------------------------------------------------ otomatik nişan
    // Uçuşun 0.25. saniyesinden sonra, ilk çarpmaya kadar, önündeki en yakın tilkiye doğru
    // hız vektörünü yavaşça döndürür (yükseltme seviyesine göre 25 / 45 / 70 derece/sn).
    void FixedUpdate()
    {
        if (aimRate <= 0f || CurrentState != State.Flying || hasHit || finished || noHoming) return;
        float dt = Time.fixedDeltaTime;
        flightTime += dt;
        if (flightTime < 0.25f) return;

        aimScan -= dt;
        if (aimScan <= 0f || aimTarget == null)
        {
            aimScan = 0.2f;
            aimTarget = FindTarget();
        }
        if (aimTarget == null) return;

        Vector2 v = rb.linearVelocity;
        float speed = v.magnitude;
        if (speed < 1f) return;
        Vector2 to = (Vector2)aimTarget.position - (Vector2)transform.position;
        float cur = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        float want = Mathf.Atan2(to.y, to.x) * Mathf.Rad2Deg;
        float ang = Mathf.MoveTowardsAngle(cur, want, aimRate * dt);
        rb.linearVelocity = new Vector2(Mathf.Cos(ang * Mathf.Deg2Rad), Mathf.Sin(ang * Mathf.Deg2Rad)) * speed;

        aimSparkle -= dt;
        if (aimSparkle <= 0f && Mathf.Abs(Mathf.DeltaAngle(cur, want)) > 2f)
        {
            aimSparkle = 0.12f;
            Fx.Stars(transform.position, 1);
        }
    }

    Transform FindTarget()
    {
        Vector2 p = transform.position;
        Transform best = null;
        float bestD = 13f;
        foreach (var f in FindObjectsByType<Fox>())
        {
            if (f == null) continue;
            Vector2 fp = f.transform.position;
            if (fp.x < p.x + 0.5f) continue;           // sadece öndekiler
            float d = Vector2.Distance(p, fp);
            if (d < bestD) { bestD = d; best = f.transform; }
        }
        return best;
    }

    void UpdateSprite(float dt)
    {
        if (art == null || sr == null) return;
        hitFaceLeft -= dt;
        flapAnimLeft -= dt;
        blinkTimer -= dt;
        blinkLeft -= dt;
        if (blinkTimer <= 0f)
        {
            blinkLeft = 0.13f;
            blinkTimer = UnityEngine.Random.Range(2f, 4.5f);
        }

        Sprite s;
        if (hitFaceLeft > 0f) s = art.hit;
        else if (CurrentState == State.Flying && !hasHit)
        {
            float rate = flapAnimLeft > 0f ? 0.06f : 0.16f;
            s = Mathf.FloorToInt((Time.time + phase) / rate) % 2 == 0 ? art.up : art.down;
        }
        else if (CurrentState == State.Hopping) s = art.up;
        else if (blinkLeft > 0f) s = art.blink;
        else s = art.idle;
        sr.sprite = s;
    }

    void UpdateVisual(float dt)
    {
        if (visual == null) return;
        if (CurrentState == State.Flying && !hasHit)
        {
            Vector2 v = rb.linearVelocity;
            if (v.sqrMagnitude > 0.5f)
            {
                float a = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
                bool back = Mathf.Abs(a) > 90f;
                sr.flipY = back;
                visual.rotation = Quaternion.Euler(0, 0, a);
            }
        }
        else if (aiming)
        {
            float a = Mathf.Clamp(Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg, -60f, 75f);
            visual.localRotation = Quaternion.Slerp(visual.localRotation, Quaternion.Euler(0, 0, a), dt * 15f);
        }
        else
        {
            sr.flipY = false;
            visual.localRotation = Quaternion.Slerp(visual.localRotation, Quaternion.identity, dt * 8f);
        }

        squash = Mathf.MoveTowards(squash, 0f, dt * 1.6f);
        float wobble = squash * Mathf.Cos(Time.time * 30f);
        float sx = 1f + wobble, sy = 1f - wobble;

        float hopY = 0f;
        if (idleHopT >= 0f)
        {
            idleHopT += dt / 0.35f;
            hopY = Mathf.Sin(Mathf.Clamp01(idleHopT) * Mathf.PI) * 0.25f;
            if (idleHopT >= 1f) { idleHopT = -1f; squash = -0.12f; }
        }
        visual.localScale = new Vector3(sx, sy, 1f);
        visual.localPosition = new Vector3(0f, hopY, 0f);
    }

    // ------------------------------------------------------------------ bitiş
    void Finish()
    {
        if (finished) return;
        Fx.Puff(transform.position, 1.0f, 7);
        Fx.Feathers(transform.position, 5, BirdInfo.Get(type).featherColor);
        FinishSilently();
    }

    void FinishSilently()
    {
        if (finished) return;
        finished = true;
        OnFinished?.Invoke(this);
        Destroy(gameObject);
    }

    public void PopForBonus()
    {
        Fx.Puff(transform.position, 0.9f, 6);
        Fx.Stars(transform.position, 5);
        Destroy(gameObject);
    }
}
