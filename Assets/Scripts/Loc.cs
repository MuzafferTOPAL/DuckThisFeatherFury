using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Çoklu dil desteği: dünyada en çok konuşulan 30 dil.
// Metinler Resources/Loc/{kod}.json içinde; Jersey 10 pikselli fontunda olmayan yazılar
// (Kiril, Çince, Japonca, Korece, Hint dilleri, Tayca, Arapça...) önceden çizilmiş
// Resources/LocAtlas/{kod} görsellerinden çizilir (bkz. LocText).
public static class Loc
{
    public static readonly string[] Codes =
    {
        "en", "tr", "de", "es", "fr", "pt", "it", "pl", "id", "fil", "sw", "ha", "vi", "ru", "uk",
        "zh-Hans", "zh-Hant", "ja", "ko", "hi", "bn", "mr", "te", "ta", "gu", "pa", "th", "ar", "ur", "fa"
    };
    public static readonly string[] Names =
    {
        "ENGLISH", "TÜRKÇE", "DEUTSCH", "ESPAÑOL", "FRANÇAIS", "PORTUGUÊS", "ITALIANO", "POLSKI", "BAHASA INDONESIA",
        "FILIPINO", "KISWAHILI", "HAUSA", "TIẾNG VIỆT", "РУССКИЙ", "УКРАЇНСЬКА", "简体中文", "繁體中文", "日本語", "한국어",
        "हिन्दी", "বাংলা", "मराठी", "తెలుగు", "தமிழ்", "ગુજરાતી", "ਪੰਜਾਬੀ", "ไทย", "العربية", "اردو", "فارسی"
    };

    static readonly HashSet<string> fullAtlas = new HashSet<string> { "ru", "uk", "vi", "ha" };
    static readonly HashSet<string> rtl = new HashSet<string> { "ar", "ur", "fa" };

    static string code;
    static Dictionary<string, string> table, fallback;
    public static int Version { get; private set; }   // dil değişince artar (önbellekler için)

    public static string Code
    {
        get
        {
            if (code == null)
            {
                string c = PlayerPrefs.GetString("lang_code", "");
                if (string.IsNullOrEmpty(c) && PlayerPrefs.HasKey("lang"))
                {
                    string[] old = { "tr", "en", "de", "es", "fr", "pt", "it" };
                    c = old[Mathf.Clamp(PlayerPrefs.GetInt("lang"), 0, old.Length - 1)];
                }
                if (IndexOf(c) < 0) c = Detect();
                Set(c, false);
            }
            return code;
        }
    }

    public static int Lang => IndexOf(Code);
    public static bool FullAtlasFor(string c) => fullAtlas.Contains(c);
    public static bool RtlFor(string c) => rtl.Contains(c);

    public static int IndexOf(string c)
    {
        for (int i = 0; i < Codes.Length; i++) if (Codes[i] == c) return i;
        return -1;
    }

    public static void Set(string c, bool save = true)
    {
        if (IndexOf(c) < 0) c = "en";
        code = c;
        table = LoadTable(c);
        if (fallback == null) fallback = c == "en" ? table : LoadTable("en");
        Version++;
        if (save) { PlayerPrefs.SetString("lang_code", c); PlayerPrefs.Save(); }
    }

    public static string T(string key)
    {
        if (table == null) { var _ = Code; }
        if (table.TryGetValue(key, out var v)) return v;
        if (fallback != null && fallback.TryGetValue(key, out v)) return v;
        return key;
    }

    public static string World(int i) => T("w" + i);
    public static string LevelTitle(int i)
    {
        var d = Levels.Get(i);
        if (!string.IsNullOrEmpty(d.titleKey)) return T(d.titleKey);
        return World(d.world) + " " + (d.slot + 1);
    }
    public static string BirdName(BirdType t) => T("bird" + (int)t);
    public static string BirdAbility(BirdType t) => T("abil" + (int)t);

    // Yüzde yazımı dile göre: Türkçe "%15", Fransızca "15 %", diğerleri "15%"
    public static string Pct(int n)
    {
        switch (Code)
        {
            case "tr": return "%" + n;
            case "fr": return n + " %";
            default: return n + "%";
        }
    }

    // ------------------------------------------------------------------ cihaz dili
    static string Detect()
    {
        string lang = "", script = "", country = "";
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var cls = new AndroidJavaClass("java.util.Locale"))
            using (var loc = cls.CallStatic<AndroidJavaObject>("getDefault"))
            {
                lang = loc.Call<string>("getLanguage") ?? "";
                script = loc.Call<string>("getScript") ?? "";
                country = loc.Call<string>("getCountry") ?? "";
            }
        }
        catch { }
#endif
        if (string.IsNullOrEmpty(lang)) lang = FromSystemLanguage(Application.systemLanguage);
        lang = lang.ToLowerInvariant();
        switch (lang)
        {
            case "zh":
                if (script == "Hant" || country == "TW" || country == "HK" || country == "MO") return "zh-Hant";
                return "zh-Hans";
            case "zh-hant": return "zh-Hant";
            case "zh-hans": return "zh-Hans";
            case "in": return "id";
            case "tl": return "fil";
        }
        return IndexOf(lang) >= 0 ? lang : "en";
    }

    static string FromSystemLanguage(SystemLanguage l)
    {
        switch (l)
        {
            case SystemLanguage.Turkish: return "tr";
            case SystemLanguage.German: return "de";
            case SystemLanguage.Spanish: return "es";
            case SystemLanguage.French: return "fr";
            case SystemLanguage.Portuguese: return "pt";
            case SystemLanguage.Italian: return "it";
            case SystemLanguage.Polish: return "pl";
            case SystemLanguage.Indonesian: return "id";
            case SystemLanguage.Vietnamese: return "vi";
            case SystemLanguage.Russian: return "ru";
            case SystemLanguage.Ukrainian: return "uk";
            case SystemLanguage.ChineseTraditional: return "zh-Hant";
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseSimplified: return "zh-Hans";
            case SystemLanguage.Japanese: return "ja";
            case SystemLanguage.Korean: return "ko";
            case SystemLanguage.Thai: return "th";
            case SystemLanguage.Arabic: return "ar";
            default: return "en";
        }
    }

    // ------------------------------------------------------------------ JSON (düz {"anahtar":"değer"})
    static Dictionary<string, string> LoadTable(string c)
    {
        var d = new Dictionary<string, string>();
        var ta = Resources.Load<TextAsset>("Loc/" + c);
        if (ta == null) { Debug.LogWarning("Loc: missing " + c); return d; }
        string s = ta.text;
        int i = 0;
        while (true)
        {
            string k = ReadStr(s, ref i);
            if (k == null) break;
            string v = ReadStr(s, ref i);
            if (v == null) break;
            d[k] = v;
        }
        return d;
    }

    static string ReadStr(string s, ref int i)
    {
        while (i < s.Length && s[i] != '"') i++;
        if (i >= s.Length) return null;
        i++;
        var sb = new StringBuilder();
        while (i < s.Length)
        {
            char ch = s[i++];
            if (ch == '"') return sb.ToString();
            if (ch == '\\' && i < s.Length)
            {
                char e = s[i++];
                switch (e)
                {
                    case 'n': sb.Append('\n'); break;
                    case 't': sb.Append('\t'); break;
                    case 'u': sb.Append((char)System.Convert.ToInt32(s.Substring(i, 4), 16)); i += 4; break;
                    default: sb.Append(e); break;
                }
            }
            else sb.Append(ch);
        }
        return null;
    }
}
