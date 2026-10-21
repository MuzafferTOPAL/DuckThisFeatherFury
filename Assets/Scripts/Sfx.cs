using UnityEngine;

// Basit ses çalar: chiptune efektleri, rastgele hafif perde farkı, sessize alma.
public class Sfx : MonoBehaviour
{
    static Sfx instance;
    AudioSource[] sources;
    int next;

    public static bool Muted
    {
        get => PlayerPrefs.GetInt("muted", 0) == 1;
        set { PlayerPrefs.SetInt("muted", value ? 1 : 0); PlayerPrefs.Save(); }
    }

    static Sfx I
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("Sfx");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<Sfx>();
                instance.sources = new AudioSource[8];
                for (int i = 0; i < instance.sources.Length; i++)
                {
                    instance.sources[i] = go.AddComponent<AudioSource>();
                    instance.sources[i].playOnAwake = false;
                }
            }
            return instance;
        }
    }

    static GameArt A => GameArt.I;
    public static AudioClip Launch => A != null ? A.sLaunch : null;
    public static AudioClip Flap => A != null ? A.sFlap : null;
    public static AudioClip Pop => A != null ? A.sPop : null;
    public static AudioClip Dash => A != null ? A.sDash : null;
    public static AudioClip Hit => A != null ? A.sHit : null;
    public static AudioClip Thud => A != null ? A.sThud : null;
    public static AudioClip WoodBreak => A != null ? A.sWood : null;
    public static AudioClip StoneBreak => A != null ? A.sStone : null;
    public static AudioClip IceBreak => A != null ? A.sIce : null;
    public static AudioClip Boom => A != null ? A.sBoom : null;
    public static AudioClip FoxPop => A != null ? A.sFox : null;
    public static AudioClip Click => A != null ? A.sClick : null;
    public static AudioClip Star => A != null ? A.sStar : null;
    public static AudioClip Stretch => A != null ? A.sStretch : null;
    public static AudioClip Win => A != null ? A.sWin : null;
    public static AudioClip Lose => A != null ? A.sLose : null;
    public static AudioClip Whoosh => A != null ? A.sWhoosh : null;

    public static void Play(AudioClip clip, float volume = 1f, bool varyPitch = true)
    {
        if (clip == null || Muted || volume <= 0.01f) return;
        var s = I.sources[I.next];
        I.next = (I.next + 1) % I.sources.Length;
        s.pitch = varyPitch ? Random.Range(0.92f, 1.08f) : 1f;
        s.PlayOneShot(clip, Mathf.Clamp01(volume));
    }
}
