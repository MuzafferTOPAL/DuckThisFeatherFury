using System;
using UnityEngine;

public enum BlockMaterial { Wood, Stone, Ice, Tnt, Fox }

// Hasar alıp kırılabilen her şey: tahta, taş, buz, TNT ve tilkiler.
// Hasar = çarpma hızı x karşı cismin ağırlık çarpanı x kuşun malzeme çarpanı.
[RequireComponent(typeof(Rigidbody2D))]
public class Destructible : MonoBehaviour
{
    public BlockMaterial material = BlockMaterial.Wood;
    public float health = 14f;
    public float minImpact = 2f;
    public bool isEnemy;
    public int scoreValue = 500;
    public SpriteRenderer crackRenderer;
    public int bonusEggs;          // altın yuva: kırılınca verilecek yumurta (bir kereliğine)
    public string bonusKey;

    public float MaxHealth { get; private set; }
    public float HealthPercent => MaxHealth > 0f ? Mathf.Clamp01(health / MaxHealth) : 0f;
    public bool Dead => dead;
    public event Action<float> OnDamaged;

    float spawnGrace = 0.8f;
    float chipCooldown;
    bool dead;

    void Awake()
    {
        MaxHealth = health;
    }

    public void ResetHealth(float h)
    {
        health = h;
        MaxHealth = h;
    }

    void Update()
    {
        if (spawnGrace > 0f) spawnGrace -= Time.deltaTime;
        chipCooldown -= Time.deltaTime;
        if (!dead && (transform.position.y < -15f || Mathf.Abs(transform.position.x) > 70f)) Die(false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (dead || spawnGrace > 0f) return;
        float impact = collision.relativeVelocity.magnitude;
        if (impact < minImpact) return;

        float otherMass = collision.rigidbody != null ? collision.rigidbody.mass : 3f;
        float massFactor = Mathf.Clamp(Mathf.Sqrt(otherMass / 2f), 0.4f, 1.8f);
        float birdFactor = 1f;
        var bird = collision.collider.GetComponentInParent<Bird>();
        if (bird != null) birdFactor = BirdInfo.DamageVs(bird.type, material) * bird.DamageMult;

        TakeDamage(impact * massFactor * birdFactor, collision.GetContact(0).point, impact);
    }

    public void TakeDamage(float amount, Vector2 point, float impact)
    {
        if (dead || amount <= 0f) return;
        health -= amount;
        OnDamaged?.Invoke(amount);

        if (health <= 0f)
        {
            Die(true);
            return;
        }

        UpdateCracks();
        if (impact > 3f && chipCooldown <= 0f)
        {
            chipCooldown = 0.15f;
            Fx.Debris(point, 4, material, 2.5f);
            if (material != BlockMaterial.Fox) Sfx.Play(Sfx.Thud, Mathf.Clamp01(impact / 12f) * 0.7f);
        }
    }

    void UpdateCracks()
    {
        var art = GameArt.I;
        if (crackRenderer == null || art == null) return;
        float p = HealthPercent;
        if (p < 0.34f) { crackRenderer.sprite = art.crack2; crackRenderer.enabled = true; }
        else if (p < 0.67f) { crackRenderer.sprite = art.crack1; crackRenderer.enabled = true; }
    }

    public void Die(bool effects)
    {
        if (dead) return;
        dead = true;
        Vector2 pos = transform.position;
        var rig = CameraRig.Instance;

        if (effects)
        {
            float area = 1f;
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null) area = Mathf.Clamp(sr.bounds.size.x * sr.bounds.size.y, 0.3f, 3f);
            Fx.Debris(pos, Mathf.RoundToInt(6 + area * 5), material, 4f);

            switch (material)
            {
                case BlockMaterial.Wood:
                    Fx.Puff(pos, 0.7f, 4, new Color(0.95f, 0.85f, 0.7f, 0.85f));
                    Sfx.Play(Sfx.WoodBreak);
                    break;
                case BlockMaterial.Stone:
                    Fx.Puff(pos, 0.9f, 5, new Color(0.85f, 0.85f, 0.85f, 0.9f));
                    Sfx.Play(Sfx.StoneBreak);
                    if (rig != null) rig.Shake(0.12f);
                    break;
                case BlockMaterial.Ice:
                    Fx.Puff(pos, 0.5f, 3, new Color(0.85f, 0.95f, 1f, 0.8f));
                    Sfx.Play(Sfx.IceBreak);
                    break;
                case BlockMaterial.Tnt:
                    Explosion.Explode(pos, 2.6f, 15f, 22f, gameObject);
                    break;
                case BlockMaterial.Fox:
                    Fx.Puff(pos, 1.3f * transform.localScale.x, 9, new Color(0.95f, 0.9f, 0.85f, 0.95f));
                    Fx.Stars(pos, 6);
                    Sfx.Play(Sfx.FoxPop);
                    if (rig != null) rig.Shake(0.18f);
                    break;
            }
        }

        if (GameManager.Instance != null)
        {
            if (effects) GameManager.Instance.AddScore(scoreValue, pos);
            if (effects && !isEnemy) GameManager.Instance.BlockDestroyed(scoreValue);
            if (effects && bonusEggs > 0) GameManager.Instance.NestBroken(bonusKey, bonusEggs, pos);
            if (isEnemy) GameManager.Instance.EnemyDefeated();
        }
        Destroy(gameObject);
    }
}
