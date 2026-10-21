using UnityEngine;

// Arka plan müziği: menü ve dünya parçaları arasında yumuşak geçiş, ayrı aç/kapa ayarı.
public class Music : MonoBehaviour
{
    public enum Track { None, Menu, WorldA, WorldB }

    static Music instance;
    AudioSource a, b;          // çapraz geçiş için iki kaynak
    AudioSource current;
    Track playing = Track.None;
    float duck = 1f;           // duraklatmada sesi kıs

    const float BaseVolume = 0.32f;

    public static bool Muted
    {
        get => PlayerPrefs.GetInt("music_muted", 0) == 1;
        set
        {
            PlayerPrefs.SetInt("music_muted", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    static Music I
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("Muzik");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<Music>();
                instance.a = instance.MakeSource(go);
                instance.b = instance.MakeSource(go);
            }
            return instance;
        }
    }

    AudioSource MakeSource(GameObject go)
    {
        var s = go.AddComponent<AudioSource>();
        s.loop = true;
        s.playOnAwake = false;
        s.volume = 0f;
        s.ignoreListenerPause = true;
        return s;
    }

    public static void Play(Track t) => I.SetTrack(t);
    public static void SetDuck(bool d) => I.duck = d ? 0.35f : 1f;

    static AudioClip Clip(Track t)
    {
        var art = GameArt.I;
        if (art == null) return null;
        switch (t)
        {
            case Track.Menu: return art.musicMenu;
            case Track.WorldA: return art.musicWorld;
            case Track.WorldB: return art.musicWorld2;
            default: return null;
        }
    }

    void SetTrack(Track t)
    {
        if (t == playing) return;
        playing = t;
        var clip = Clip(t);
        var next = current == a ? b : a;
        if (clip != null)
        {
            next.clip = clip;
            next.volume = 0f;
            next.Play();
        }
        current = clip != null ? next : null;
    }

    void Update()
    {
        float target = Muted ? 0f : BaseVolume * duck;
        float dt = Time.unscaledDeltaTime;
        foreach (var s in new[] { a, b })
        {
            if (s == null) continue;
            float v = s == current ? target : 0f;
            s.volume = Mathf.MoveTowards(s.volume, v, dt * (s == current ? 0.5f : 0.8f));
            if (s != current && s.isPlaying && s.volume <= 0.001f) s.Stop();
        }
    }
}
