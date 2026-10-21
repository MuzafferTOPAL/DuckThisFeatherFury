using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

// Otomatik bölüm testi (sadece geliştirme için).
// Editörde "Ördek Oyunu > Bölümleri Otomatik Test Et" menüsüyle çalışır.
// Her bölüm için: (1) atış yapmadan yapı sağlam duruyor mu, (2) nişanlı atışlarla kazanılabiliyor mu.
// Sonuçlar Logs/autotest.txt dosyasına yazılır.
public class AutoTester : MonoBehaviour
{
    public int trials = 4;
    public float speed = 4f;
    public int fromLevel = 0;
    public int toLevel = Levels.Count - 1;

    readonly StringBuilder log = new StringBuilder();
    string path;

    void Start()
    {
        fromLevel = Mathf.Clamp(PlayerPrefs.GetInt("autotest_from", 1) - 1, 0, Levels.Count - 1);
        toLevel = Mathf.Clamp(PlayerPrefs.GetInt("autotest_to", Levels.Count) - 1, fromLevel, Levels.Count - 1);
        path = Path.Combine(Application.dataPath, "../Logs/autotest.txt");
        StartCoroutine(Run());
    }

    void Line(string s)
    {
        Debug.Log("[AutoTest] " + s);
        log.AppendLine(s);
        try { File.WriteAllText(path, log.ToString()); } catch { }
    }

    IEnumerator Run()
    {
        yield return null;
        // oyuncunun ilerlemesini koru
        var savedStars = new int[Levels.Count];
        var savedBest = new int[Levels.Count];
        for (int k = 0; k < Levels.Count; k++) { savedStars[k] = Levels.Stars(k); savedBest[k] = Levels.Best(k); }
        int savedEggs = Economy.Eggs;
        var gm = GameManager.Instance;
        Line("=== Otomatik test başladı " + System.DateTime.Now + " ===");
        int totalWins = 0, totalTrials = 0;

        for (int i = fromLevel; i <= toLevel; i++)
        {
            // 1) sağlamlık
            gm.DebugStart(i, false);
            Time.timeScale = speed;
            yield return null;
            yield return null;
            int foxes0 = gm.EnemiesLeft;
            int pieces0 = CountPieces();
            yield return new WaitForSeconds(4f);
            int foxes1 = gm.EnemiesLeft;
            int pieces1 = CountPieces();
            string stab = (foxes1 == foxes0 && pieces1 == pieces0) ? "SAĞLAM" : $"SORUN (tilki {foxes0}->{foxes1}, blok {pieces0}->{pieces1})";

            // 2) oynanış
            int wins = 0, bestStars = 0, bestScore = 0, minShots = 99;
            for (int t = 0; t < trials; t++)
            {
                gm.DebugStart(i, true);
                Time.timeScale = speed;
                int shots = 0;
                float timeout = 120f;
                while (timeout > 0f && gm.State == GameManager.GState.Playing)
                {
                    if (gm.OnSling != null && gm.OnSling.IsReady && gm.Sling.HasBird)
                    {
                        Vector2 target;
                        Vector2 v = AimAtFox(gm, t, out target);
                        var bird = gm.OnSling;
                        if (gm.AutoLaunch(v))
                        {
                            shots++;
                            yield return SmartAbility(bird, target);
                        }
                    }
                    timeout -= Time.unscaledDeltaTime;
                    yield return null;
                }
                while (gm.State == GameManager.GState.Won && !gm.EndShown) yield return null;
                bool won = gm.State == GameManager.GState.Won;
                if (won)
                {
                    wins++;
                    bestStars = Mathf.Max(bestStars, gm.Stars);
                    bestScore = Mathf.Max(bestScore, gm.Score);
                    minShots = Mathf.Min(minShots, shots);
                }
                totalTrials++;
            }
            totalWins += wins;
            var def = Levels.Get(i);
            Line($"Bölüm {i + 1,2} {def.title,-18} | {stab,-30} | kazanma {wins}/{trials} | en iyi {bestStars}★ {bestScore} | min atış {(minShots == 99 ? "-" : minShots.ToString())}/{def.birds.Length} | 2★≥{gm.TwoStar} 3★≥{gm.ThreeStar}");
        }
        Time.timeScale = 1f;
        Line($"=== Bitti: toplam {totalWins}/{totalTrials} kazanma ===");
        for (int k = 0; k < Levels.Count; k++)
        {
            PlayerPrefs.SetInt("stars_" + k, savedStars[k]);
            PlayerPrefs.SetInt("best_" + k, savedBest[k]);
        }
        Economy.Eggs = savedEggs;
        PlayerPrefs.Save();
        gm.OpenMenu();
        Destroy(this);
    }

    static int CountPieces()
    {
        int n = 0;
        foreach (var d in FindObjectsByType<Destructible>())
            if (!d.isEnemy && !d.Dead) n++;
        return n;
    }

    // Yeteneği doğru anda kullan (gerçek oyuncu gibi)
    static IEnumerator SmartAbility(Bird bird, Vector2 target)
    {
        float t = 0f;
        while (bird != null && bird.IsFlying && !bird.AbilityUsed && t < 6f)
        {
            Vector2 p = bird.transform.position;
            float dx = target.x - p.x;
            bool use = false;
            switch (bird.type)
            {
                case BirdType.Mallard: use = t > 0.5f && p.y < target.y + 0.5f && dx > 2.5f; break;
                case BirdType.Mandarin: use = dx < 3.5f; break;
                case BirdType.Merganser: use = dx < 6f; break;
                case BirdType.Muscovy: use = Mathf.Abs(dx) < 0.6f; break;
                case BirdType.Scoter: use = Vector2.Distance(p, target) < 1.4f; break;
                case BirdType.Shelduck: use = Mathf.Abs(dx) < 0.9f; break;
            }
            if (use) { bird.TestAbility(); if (bird.type != BirdType.Mallard) break; t += 0.3f; }
            t += Time.deltaTime;
            yield return null;
        }
    }

    // Rastgele bir tilkiye balistik nişan (biraz gürültüyle)
    static Vector2 AimAtFox(GameManager gm, int trial, out Vector2 aimed)
    {
        var foxes = FindObjectsByType<Fox>();
        Vector2 start = gm.Sling.launchPoint.position;
        Vector2 target = new Vector2(5f, -3f);
        if (foxes.Length > 0)
        {
            var f = foxes[Random.Range(0, foxes.Length)];
            target = f.transform.position;
        }
        target += Random.insideUnitCircle * 0.5f;
        aimed = target;
        float g = -Physics2D.gravity.y;
        float dx = target.x - start.x, dy = target.y - start.y;
        bool high = Random.value < (trial % 2 == 1 ? 0.7f : 0.3f);
        for (float v = Random.Range(12f, 15f); v <= 16.01f; v += 0.5f)
        {
            float v2 = v * v;
            float disc = v2 * v2 - g * (g * dx * dx + 2f * dy * v2);
            if (disc < 0f) continue;
            float root = Mathf.Sqrt(disc);
            float ang = Mathf.Atan((v2 + (high ? root : -root)) / (g * dx));
            return new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * v;
        }
        return new Vector2(1f, 1f).normalized * 16f;
    }
}
