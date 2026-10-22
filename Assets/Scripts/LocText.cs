using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Pikselli fontta (Jersey 10) bulunmayan harfler için metin düzeni.
// Metin kelimelere bölünür; fontta olmayan kısımlar önceden çizilmiş atlas görsellerinden
// (Resources/LocAtlas/{dil}) gelir, gerisi normal fontla yazılır. Sağdan sola dillerde sıra ters çevrilir.
// Bölme kuralı tools/loc/build.py içindeki tokenize ile birebir aynı olmalı.
public static class LocText
{
    public const char Star = '★';
    // Atlas çizim ölçüleri (build.py): satır yüksekliği 72, büyük harf merkezi ~36 px.
    public const float AtlasLine = 72f, AtlasMid = 33f, AtlasPerFontPx = 1f / 54f;

    public enum Kind { Font, Atlas, Star }

    public struct Item
    {
        public Kind kind;
        public string text;       // Font
        public Atlas atlas;       // Atlas
        public Rect uv;
        public float w, h;        // atlas parçasının piksel boyutu
        public bool space;        // öncesinde boşluk var mı
    }

    public class Atlas
    {
        public Texture2D tex;
        public Dictionary<string, RectInt> map = new Dictionary<string, RectInt>();
        public bool Has(string s) => map.ContainsKey(s);
        public Rect UV(RectInt r) => new Rect((float)r.x / tex.width, 1f - (float)(r.y + r.height) / tex.height,
                                              (float)r.width / tex.width, (float)r.height / tex.height);
    }

    static HashSet<char> jersey;
    static readonly Dictionary<string, Atlas> atlases = new Dictionary<string, Atlas>();
    static readonly Dictionary<string, List<Item>> cache = new Dictionary<string, List<Item>>();
    static readonly List<Item> NoAtlas = new List<Item>();
    static int cacheVersion = -1;

    static HashSet<char> Jersey
    {
        get
        {
            if (jersey == null)
            {
                jersey = new HashSet<char>();
                var ta = Resources.Load<TextAsset>("Loc/_jersey");
                if (ta != null) foreach (char c in ta.text) jersey.Add(c);
            }
            return jersey;
        }
    }

    public static Atlas GetAtlas(string name)
    {
        if (atlases.TryGetValue(name, out var a)) return a;
        a = null;
        var png = Resources.Load<TextAsset>("LocAtlas/" + name);
        var idx = Resources.Load<TextAsset>("LocAtlas/" + name + "_idx");
        if (png != null && idx != null)
        {
            a = new Atlas();
            a.tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            a.tex.LoadImage(png.bytes, true);
            a.tex.filterMode = FilterMode.Bilinear;
            a.tex.wrapMode = TextureWrapMode.Clamp;
            foreach (var line in idx.text.Split('\n'))
            {
                var p = line.Split('\t');
                if (p.Length < 5) continue;
                a.map[p[4]] = new RectInt(int.Parse(p[0]), int.Parse(p[1]), int.Parse(p[2]), int.Parse(p[3]));
            }
            Resources.UnloadAsset(png);
        }
        atlases[name] = a;
        return a;
    }

    // Dil değişince eski dilin atlasını bellekten at (dil seçici isimleri hariç)
    static void Flush()
    {
        cache.Clear();
        var keep = new Dictionary<string, Atlas>();
        foreach (var kv in atlases)
        {
            if (kv.Key == Loc.Code || kv.Key == "_names") keep[kv.Key] = kv.Value;
            else if (kv.Value != null) Object.Destroy(kv.Value.tex);
        }
        atlases.Clear();
        foreach (var kv in keep) atlases[kv.Key] = kv.Value;
    }

    static bool IsA(char ch, bool full)
    {
        if (ch == Star) return false;
        return !Jersey.Contains(ch) || (full && char.IsLetter(ch));
    }

    struct Tok { public string s; public bool space, a; }

    static List<Tok> Tokenize(string text, bool full)
    {
        var toks = new List<Tok>();
        var cur = new StringBuilder();
        bool space = false, prev = false;
        foreach (char raw in text)
        {
            if (raw == ' ' || raw == '\n' || raw == Star)
            {
                if (cur.Length > 0) { toks.Add(new Tok { s = cur.ToString(), space = space, a = prev }); cur.Clear(); }
                if (raw == Star)
                {
                    toks.Add(new Tok { s = "★", space = true, a = false });
                    space = true;
                }
                else space = true;
                continue;
            }
            bool a = IsA(raw, full);
            if (cur.Length > 0 && a != prev)
            {
                toks.Add(new Tok { s = cur.ToString(), space = space, a = prev });
                cur.Clear(); space = false;
            }
            cur.Append(raw); prev = a;
        }
        if (cur.Length > 0) toks.Add(new Tok { s = cur.ToString(), space = space, a = prev });
        if (toks.Count > 0) { var t = toks[0]; t.space = false; toks[0] = t; }
        return toks;
    }

    /// <summary>Metnin çizim parçaları; tamamı fontla yazılabiliyorsa null (hızlı yol).</summary>
    public static List<Item> Layout(string text, string lang = null)
    {
        if (string.IsNullOrEmpty(text)) return null;
        if (cacheVersion != Loc.Version) { cacheVersion = Loc.Version; Flush(); }
        lang = lang ?? Loc.Code;
        string key = lang + "\u0001" + text;
        if (cache.TryGetValue(key, out var res)) return res == NoAtlas ? null : res;

        string atlasLang = lang;
        bool full, rtl;
        if (lang.StartsWith("_names:"))
        {
            string c = lang.Substring(7);
            atlasLang = "_names"; full = Loc.FullAtlasFor(c); rtl = Loc.RtlFor(c);
        }
        else { full = Loc.FullAtlasFor(lang); rtl = Loc.RtlFor(lang); }

        var toks = Tokenize(text, full);
        bool any = false;
        foreach (var t in toks) if (t.a || t.s == "★") { any = true; break; }
        if (!any) { if (cache.Count > 2000) cache.Clear(); cache[key] = NoAtlas; return null; }

        var atlas = GetAtlas(atlasLang);
        var items = new List<Item>();
        var font = new StringBuilder();
        bool fontSpace = false;
        void FlushFont()
        {
            if (font.Length == 0) return;
            items.Add(new Item { kind = Kind.Font, text = font.ToString(), space = fontSpace });
            font.Clear();
        }
        void AddFont(string s, bool sp)
        {
            if (font.Length == 0) { fontSpace = sp; font.Append(s); }
            else { if (sp) font.Append(' '); font.Append(s); }
        }

        int i = 0;
        while (i < toks.Count)
        {
            var t = toks[i];
            if (t.s == "★" && !t.a)
            {
                FlushFont();
                items.Add(new Item { kind = Kind.Star, space = t.space });
                i++; continue;
            }
            if (!t.a || atlas == null) { AddFont(t.s, t.space); i++; continue; }
            // en uzun eşleşen atlas dizisi (boşluksuz bitişik parçalar dahil)
            int end = i;
            while (end + 1 < toks.Count && toks[end + 1].a) end++;
            bool found = false;
            for (int j = end; j >= i; j--)
            {
                var sb = new StringBuilder(toks[i].s);
                for (int k = i + 1; k <= j; k++) { if (toks[k].space) sb.Append(' '); sb.Append(toks[k].s); }
                string s = sb.ToString();
                if (atlas.map.TryGetValue(s, out var r))
                {
                    FlushFont();
                    items.Add(new Item { kind = Kind.Atlas, atlas = atlas, uv = atlas.UV(r), w = r.width, h = r.height, space = t.space });
                    i = j + 1; found = true; break;
                }
            }
            if (!found) { AddFont(t.s, t.space); i++; }
        }
        FlushFont();

        if (rtl && items.Count > 1)
        {
            var rev = new List<Item>(items.Count);
            for (int k = items.Count - 1; k >= 0; k--)
            {
                var it = items[k];
                it.space = k + 1 < items.Count && items[k + 1].space;
                rev.Add(it);
            }
            items = rev;
        }
        if (cache.Count > 2000) cache.Clear();
        cache[key] = items;
        return items;
    }
}
