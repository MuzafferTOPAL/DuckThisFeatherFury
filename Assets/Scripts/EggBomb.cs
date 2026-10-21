using UnityEngine;

// Angut'un bıraktığı yumurta bombası: ilk çarpmada patlar.
public class EggBomb : MonoBehaviour
{
    float life = 4f;
    bool exploded;
    float damageMult = 1f;

    public static void Drop(Vector2 pos, Vector2 velocity, float damageMult = 1f)
    {
        var go = new GameObject("YumurtaBomba");
        go.transform.position = pos;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = GameArt.I.egg;
        sr.sortingOrder = 11;
        var rb = go.AddComponent<Rigidbody2D>();
        rb.mass = 3f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.linearVelocity = velocity;
        rb.angularVelocity = Random.Range(-300f, 300f);
        var cc = go.AddComponent<CircleCollider2D>();
        cc.radius = 0.22f;
        go.AddComponent<EggBomb>().damageMult = damageMult;
    }

    void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0f) Boom();
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.collider.GetComponentInParent<Bird>() != null) return;
        Boom();
    }

    void Boom()
    {
        if (exploded) return;
        exploded = true;
        Explosion.Explode(transform.position, 2.2f, 13f, 20f * damageMult, gameObject);
        Destroy(gameObject);
    }
}
