using System.Collections;
using System.IO;
using UnityEngine;

// Sadece geliştirme: oyun ekranını kare kare PNG olarak kaydeder (README GIF'leri için).
// Time.captureFramerate sabitlenir, böylece kayıt yavaş olsa da oyun zamanı düzgün akar.
public class DevRecorder : MonoBehaviour
{
    public static void Record(string name, float seconds, int fps = 20)
    {
        var go = new GameObject("DevRecorder");
        var r = go.AddComponent<DevRecorder>();
        r.StartCoroutine(r.Run(name, seconds, fps));
    }

    // Kayıt + otomatik isabetli atış (README GIF'i için)
    public static void RecordShot(string name, float seconds, float delay, string target, float abilityAt, float yOff = 0f)
    {
        Record(name, seconds);
        var go = new GameObject("DevShot");
        var r = go.AddComponent<DevRecorder>();
        r.StartCoroutine(r.Shot(delay, target, abilityAt, yOff));
    }

    IEnumerator Shot(float delay, string target, float abilityAt, float yOff)
    {
        yield return new WaitForSeconds(delay);
        var gm = GameManager.Instance;
        float guard = 5f;
        while (guard > 0f && (gm.OnSling == null || !gm.OnSling.IsReady || !gm.Sling.HasBird)) { guard -= Time.deltaTime; yield return null; }
        Vector2 start = gm.Sling.launchPoint.position;
        Vector2 tgt = new Vector2(5f, -3f);
        float best = target == "tnt" ? -999f : 999f;
        foreach (var d in FindObjectsByType<Destructible>())
        {
            if (d.Dead) continue;
            Vector2 p = d.transform.position;
            if (target == "tnt" && d.material == BlockMaterial.Tnt && p.y > best) { best = p.y; tgt = p; }
            if (target == "fox" && d.isEnemy && p.x < best) { best = p.x; tgt = p; }
        }
        tgt.y += yOff;
        float g = -Physics2D.gravity.y;
        float dx = tgt.x - start.x, dy = tgt.y - start.y;
        Vector2 vel = new Vector2(1f, 1f).normalized * 16f;
        for (float v = 10f; v <= 18.01f; v += 0.25f)
        {
            float v2 = v * v;
            float disc = v2 * v2 - g * (g * dx * dx + 2f * dy * v2);
            if (disc < 0f) continue;
            float ang = Mathf.Atan((v2 - Mathf.Sqrt(disc)) / (g * dx));
            vel = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * Mathf.Min(18f, v + 1.5f);
            // biraz fazla hız: düz atışta düşüşü telafi et, sonra açıyı yeniden hesapla
            float v3 = vel.magnitude, v4 = v3 * v3;
            float disc2 = v4 * v4 - g * (g * dx * dx + 2f * dy * v4);
            if (disc2 >= 0f)
            {
                float a2 = Mathf.Atan((v4 - Mathf.Sqrt(disc2)) / (g * dx));
                vel = new Vector2(Mathf.Cos(a2), Mathf.Sin(a2)) * v3;
            }
            break;
        }
        var bird = gm.OnSling;
        gm.AutoLaunch(vel);
        if (abilityAt > 0f)
        {
            yield return new WaitForSeconds(abilityAt);
            if (bird != null) bird.TestAbility();
        }
        Destroy(gameObject);
    }

    IEnumerator Run(string name, float seconds, int fps)
    {
        string dir = Path.Combine(Application.dataPath, "../Logs/rec_" + name);
        Directory.CreateDirectory(dir);
        foreach (var f in Directory.GetFiles(dir, "*.png")) File.Delete(f);
        Time.captureFramerate = fps;
        int frames = Mathf.RoundToInt(seconds * fps);
        for (int i = 0; i < frames; i++)
        {
            yield return new WaitForEndOfFrame();
            var tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            tex.Apply();
            File.WriteAllBytes(Path.Combine(dir, i.ToString("D4") + ".png"), tex.EncodeToPNG());
            Destroy(tex);
        }
        Time.captureFramerate = 0;
        File.WriteAllText(Path.Combine(dir, "done.txt"), frames.ToString());
        Destroy(gameObject);
    }
}
