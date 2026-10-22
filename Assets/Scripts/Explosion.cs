using UnityEngine;

// Patlama: yarıçap içindeki cisimleri dışarı savurur ve uzaklığa göre hasar verir.
// Karga yeteneği ve TNT sandıkları kullanır.
public static class Explosion
{
    public static void Explode(Vector2 center, float radius, float force, float damage, GameObject ignore = null, bool shockwave = false)
    {
        if (shockwave) Fx.Puff(center, radius * 0.6f, 14, new Color(1f, 1f, 1f, 0.7f));
        else { Fx.Boom(center, radius); Sfx.Play(Sfx.Boom); }
        if (CameraRig.Instance != null) CameraRig.Instance.Shake(0.45f);

        var hits = Physics2D.OverlapCircleAll(center, radius);
        var done = new System.Collections.Generic.HashSet<GameObject>();
        for (int i = 0; i < hits.Length; i++)
        {
            var c = hits[i];
            if (c == null) continue;
            var go = c.attachedRigidbody != null ? c.attachedRigidbody.gameObject : c.gameObject;
            if (go == ignore || !done.Add(go)) continue;

            Vector2 p = c.ClosestPoint(center);
            Vector2 dir = p - center;
            float dist = dir.magnitude;
            float k = 1f - Mathf.Clamp01(dist / radius);
            if (dir.sqrMagnitude < 0.0001f) dir = Vector2.up;
            dir.Normalize();

            var rb = c.attachedRigidbody;
            if (rb != null && rb.bodyType == RigidbodyType2D.Dynamic)
                rb.AddForce((dir + Vector2.up * 0.3f) * force * (0.35f + k) * Mathf.Max(0.5f, rb.mass), ForceMode2D.Impulse);

            var d = go.GetComponent<Destructible>();
            if (d != null && !d.Dead) d.TakeDamage(damage * (0.25f + k), p, 10f);
        }
    }
}
