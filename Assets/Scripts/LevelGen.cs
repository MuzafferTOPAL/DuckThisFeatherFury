using System.Collections.Generic;
using UnityEngine;

// Prosedürel level üretici. Aynı level numarası her zaman aynı yapıyı üretir (tohum = level no).
// Zorluk (D) arttıkça: daha çok modül, daha çok kat, daha fazla taş, miğferli tilki ve daha az yedek ördek.
// Modüller daha önce elle test edilmiş, sağlam duran kalıplardan oluşur (çerçeve, köprü, piramit, sığınak...).
public static class LevelGen
{
    const BlockMaterial W = BlockMaterial.Wood, S = BlockMaterial.Stone, I = BlockMaterial.Ice;
    const FoxKind K = FoxKind.Kit, N = FoxKind.Normal, HL = FoxKind.Helmet, B = FoxKind.Boss;

    class Ctx
    {
        public System.Random rnd;
        public LevelDef l;
        public float d;
        public float iceP, stoneP, helmetP, kitP;
        public int wood, stone, ice, tnt, foxes;
        public bool boss;

        public float R() => (float)rnd.NextDouble();
        public BlockMaterial Mat(bool beam = false)
        {
            float r = R();
            BlockMaterial m = r < iceP ? I : r < iceP + stoneP ? S : W;
            if (beam && m == S && R() < 0.5f) m = W;
            if (m == W) wood++; else if (m == S) stone++; else ice++;
            return m;
        }
        public FoxKind Fox()
        {
            float r = R();
            return r < helmetP ? HL : r < helmetP + kitP ? K : N;
        }
        public void PutFox(FoxKind k, float x, float y0) { l.Fox(k, x, y0); foxes++; }
    }

    // maxDucks: bu levelde izin verilen en fazla ördek (dev kale sınırı, Levels belirler; patronda +1)
    // light: önceki iki level zordu → bu level "nefes" leveli olsun
    public static LevelDef Make(int index, int world, int slot, float d, List<BirdType> avail, BirdType? intro = null,
                                int maxDucks = 7, bool light = false)
    {
        // Prosedürel üretimin ilk bölümlerinde (L41-100) yapı biraz daha hafif başlar, L100'de tam değere ulaşır:
        // el yapımı levellerden prosedürel levellere geçişte ani sıçrama olmasın.
        float ds = d * Mathf.Lerp(0.72f, 1f, Mathf.InverseLerp(40f, 100f, index));
        bool boss = slot == Levels.PerWorld - 1;
        int cap = boss ? maxDucks + 1 : maxDucks;

        // tilki dalgası: bölümün ilk leveli ve patrondan önceki 2 level hafif; her 5. level nefes leveli
        var wr = new System.Random(index * 7919 + 1013);
        float wave = boss ? 1.25f
                   : slot == 0 ? 0.55f
                   : light || slot % 5 == 0 ? 0.7f
                   : slot >= 16 ? 0.75f + (float)wr.NextDouble() * 0.25f
                   : 0.75f + (float)wr.NextDouble() * 0.55f;
        int target = Mathf.RoundToInt((1.1f + ds / 11f) * wave);
        int minMods = ds >= 36f ? 3 : ds >= 16f ? 2 : 1;
        target = Mathf.Clamp(target, minMods, ds < 4f ? 2 : 5);

        // gereken ördek sayısı sınırı aşıyorsa daha küçük bir kale kur (en kötü ihtimalle tek modül)
        LevelDef l = null;
        for (int t = target; t >= 1; t--)
        {
            l = Generate(index, world, slot, ds, avail, intro, t, cap, out int need);
            if (need <= cap) break;
        }
        // patron: her zaman bir yedek ördek daha (patrona karşı en etkili tür)
        if (boss)
        {
            var list = new List<BirdType>(l.birds);
            list.Add(avail.Contains(BirdType.Scoter) ? BirdType.Scoter : avail.Contains(BirdType.Muscovy) ? BirdType.Muscovy : BirdType.Mallard);
            l.birds = list.ToArray();
        }
        return l;
    }

    static LevelDef Generate(int index, int world, int slot, float d, List<BirdType> avail, BirdType? intro, int target, int cap, out int need)
    {
        var c = new Ctx
        {
            rnd = new System.Random(index * 7919 + 1013),
            l = new LevelDef { index = index, theme = world, title = "GEN-" + (index + 1), generated = true },
            d = d,
        };
        int baseTheme = world < 10 ? world : new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 2, 4, 5, 6, 9 }[world - 10];
        bool icy = baseTheme == 2 || baseTheme == 5 || baseTheme == 6;
        c.iceP = icy ? 0.28f : 0.1f;
        c.stoneP = Mathf.Clamp(0.08f + 0.014f * d, 0.08f, 0.55f);
        c.helmetP = Mathf.Clamp(0.02f + 0.017f * d, 0f, 0.6f);
        c.kitP = d < 5f ? 0.4f : d < 15f ? 0.2f : 0.08f;
        c.boss = slot == Levels.PerWorld - 1;
        c.R();   // dalga için ayrılan sayı (eski tohum sırası korunsun)

        float maxW = 10f + Mathf.Min(2.5f, d / 10f);
        float x = 2.5f;
        float end = x;
        var placed = new List<int>();

        for (int m = 0; m < target; m++)
        {
            bool last = m == target - 1;
            int kind = (c.boss && last) ? 8 : PickModule(c, placed);
            float w = Width(kind);
            if (x + w > 2.5f + maxW)
            {
                if (c.boss && !placed.Contains(8) && x + Width(8) <= 2.5f + maxW + 1.5f) kind = 8;
                else break;
                w = Width(kind);
            }
            Build(c, kind, x);
            placed.Add(kind);
            end = x + w;
            x = end + 0.9f + c.R() * 0.8f;
        }
        if (c.boss && !placed.Contains(8))
        {   // patron mutlaka olsun
            Build(c, 8, x);
            end = x + Width(8);
        }

        // altın yuva: her bölümde ortadaki ve son level
        if (slot == 9 || slot == Levels.PerWorld - 1) c.l.Nest(end + 0.9f, -4f);

        c.l.maxX = Mathf.Max(10.5f, end + 1.7f);
        c.l.birds = PickDucks(c, avail, intro, cap, out need);
        return c.l;
    }

    // modüller: 0 kule1, 1 kaideler, 2 duvar, 3 kule2, 4 kaya rafı, 5 köprü, 6 ikiz kule, 7 piramit, 8 sığınak, 9 kule3
    static float Width(int k)
    {
        switch (k)
        {
            case 0: case 3: case 9: return 2.4f;
            case 1: return 4.2f;
            case 2: return 3.8f;
            case 4: return 4.1f;
            case 5: return 7f;
            case 6: return 5.8f;
            case 7: return 8f;
            case 8: return 6f;
        }
        return 3f;
    }

    static int PickModule(Ctx c, List<int> placed)
    {
        var pool = new List<int> { 0, 1, 2 };
        if (c.d >= 2f) pool.AddRange(new[] { 3, 4 });
        if (c.d >= 4f) pool.AddRange(new[] { 5, 6 });
        if (c.d >= 9f) pool.AddRange(new[] { 7, 9 });
        if (c.d >= 14f) pool.AddRange(new[] { 8, 7, 9 });
        if (c.d >= 26f) pool.AddRange(new[] { 9, 6, 7, 5 });
        if (c.d >= 40f) pool.AddRange(new[] { 9, 8, 7 });
        if (c.d >= 6f) pool.Remove(0);   // ileride tek katlı kule nadir
        int k = pool[c.rnd.Next(pool.Count)];
        if (placed.Count > 0 && placed[placed.Count - 1] == k) k = pool[c.rnd.Next(pool.Count)];
        return k;
    }

    static void Build(Ctx c, int kind, float x)
    {
        var l = c.l;
        switch (kind)
        {
            case 0: Tower(c, x, 1); break;
            case 3: Tower(c, x, 2); break;
            case 9: Tower(c, x, 3); break;
            case 1:   // taş kaideler üstünde tilkiler
                for (int i = 0; i < 3; i++)
                {
                    float px = x + 0.55f + i * 1.55f;
                    float top = l.Cube(S, px, -4f, 1.1f); c.stone++;
                    c.PutFox(c.d < 8f ? K : c.Fox(), px, top);
                }
                break;
            case 2:   // önünde duvar, arkasında çerçeve
                l.Block(c.R() < 0.5f + c.d * 0.02f ? S : W, x + 0.5f, -4f, 1f, 2f);
                {
                    var cm = c.Mat(); var bm = c.Mat(true);
                    float t = l.Frame(cm, bm, x + 1.4f, x + 3.8f, -4f, 1.4f);
                    c.PutFox(c.Fox(), x + 2.6f, -4f);
                    if (c.R() < 0.6f) c.PutFox(c.Fox(), x + 2.6f, t);
                }
                break;
            case 4:   // kaya rafı
                {
                    l.Col(W, x + 0.25f, -4f, 2.4f); l.Col(W, x + 3.85f, -4f, 2.4f); c.wood += 2;
                    float y = l.Beam(W, x, x + 4.1f, -1.6f);
                    l.Boulder(x + 0.7f, y, 0.45f); l.Boulder(x + 3.4f, y, 0.45f);
                    c.PutFox(c.d < 6f ? K : c.Fox(), x + 2.05f, y);
                    c.PutFox(c.Fox(), x + 1.3f, -4f);
                    if (c.R() < 0.5f) { l.Tnt(x + 2.8f, -4f); c.tnt++; } else c.PutFox(c.Fox(), x + 2.8f, -4f);
                }
                break;
            case 5:   // köprü
                {
                    l.Col(S, x + 0.25f, -4f, 2.4f); l.Col(W, x + 3.5f, -4f, 2.4f); l.Col(S, x + 6.75f, -4f, 2.4f); c.stone += 2;
                    float y = l.Beam(W, x, x + 7f, -1.6f); c.wood++;
                    c.PutFox(c.Fox(), x + 1.9f, -4f);
                    if (c.R() < 0.4f) { l.Tnt(x + 5.1f, -4f); c.tnt++; } else c.PutFox(c.Fox(), x + 5.1f, -4f);
                    var cm = c.Mat(); var bm = c.Mat(true);
                    float t = l.Frame(cm, bm, x + 2.3f, x + 4.7f, y, 1.3f);
                    c.PutFox(c.Fox(), x + 3.5f, y);
                    if (c.R() < 0.4f) c.PutFox(K, x + 3.5f, t);
                }
                break;
            case 6:   // ikiz kule + köprü
                {
                    int floors = c.d >= 12f && c.R() < 0.5f ? 2 : 1;
                    float t1 = Tower(c, x, floors, false);
                    Tower(c, x + 3.4f, floors, false);
                    float y = l.Beam(c.d > 10f ? S : W, x, x + 5.8f, t1);
                    c.PutFox(c.Fox(), x + 2.9f, y);
                }
                break;
            case 7:   // piramit
                {
                    var m0 = c.d > 12f ? S : c.Mat();
                    float a = l.Frame(m0, m0, x, x + 8f, -4f, 1.4f);
                    c.PutFox(c.Fox(), x + 2f, -4f); c.PutFox(c.Fox(), x + 4f, -4f); c.PutFox(c.Fox(), x + 6f, -4f);
                    var m1 = c.Mat();
                    float b = l.Frame(m1, m1, x + 1f, x + 7f, a, 1.4f);
                    c.PutFox(c.Fox(), x + 3f, a); c.PutFox(c.Fox(), x + 5f, a);
                    var m2 = c.Mat();
                    float t = l.Frame(m2, m2, x + 2.3f, x + 5.7f, b, 1.3f);
                    c.PutFox(c.Fox(), x + 4f, b);
                    if (c.R() < 0.5f) c.PutFox(K, x + 4f, t);
                }
                break;
            case 8:   // sığınak (patron)
                {
                    float y = l.Frame(S, c.d > 8f ? S : W, x, x + 6f, -4f, 2f); c.stone += 2;
                    c.PutFox(c.boss ? B : HL, x + 3f, -4f);
                    for (int side = 0; side < 2; side++)
                    {
                        float px = side == 0 ? x + 1.3f : x + 4.7f;
                        if (c.R() < 0.6f) { l.Tnt(px, -4f); c.tnt++; }
                        else { l.Cube(c.iceP > 0.2f ? I : W, px, -4f, 0.9f); }
                    }
                    var cm = c.Mat(); var bm = c.Mat(true);
                    float t = l.Frame(cm, bm, x + 1.8f, x + 4.2f, y, 1.3f);
                    c.PutFox(c.Fox(), x + 3f, y);
                    if (c.d > 10f || c.R() < 0.5f) c.PutFox(c.Fox(), x + 3f, t);
                }
                break;
        }
    }

    // Çerçeveleri üst üste dizer, her katta bir tilki; üstteki kenarı döndürür
    static float Tower(Ctx c, float x, int floors, bool topFox = true)
    {
        float y = -4f;
        for (int f = 0; f < floors; f++)
        {
            var cm = c.Mat(); var bm = c.Mat(true);
            if (f == 0 && floors >= 3 && cm == I) cm = W;   // yüksek kulenin tabanı buz olmasın
            float t = c.l.Frame(cm, bm, x, x + 2.4f, y, f == floors - 1 ? 1.3f : 1.4f);
            c.PutFox(c.Fox(), x + 1.2f, y);
            y = t;
        }
        if (topFox && c.R() < 0.65f) c.PutFox(c.d < 6f ? K : c.Fox(), x + 1.2f, y);
        return y;
    }

    static BirdType[] PickDucks(Ctx c, List<BirdType> avail, BirdType? intro, int cap, out int need)
    {
        // yedek ördek: başta bol, sonra 1; sadece son bölümün sonlarında 0
        // ileri bölümlerde tilkiler çok dayanıklı: yedek ördek azalmaz, aksine bir tane artar
        int spare = c.d < 6f ? 2 : 1;
        if (c.foxes >= 7) spare++;
        if (c.d >= 26f) spare++;
        if (c.d >= 45f) spare++;
        // en az 4 ördek; az tilkili ama korunaklı levellerde tilki sayısı + 1 (L43/L99 gibi duvarlar olmasın)
        need = Mathf.Max(Mathf.CeilToInt(c.foxes / 2.0f) + spare, Mathf.Min(c.foxes + 1, 5), 4);
        int n = Mathf.Min(need, cap);
        var weights = new Dictionary<BirdType, float>();
        foreach (var t in avail)
        {
            float w = 1f;
            if (t == BirdType.Mandarin) w += c.ice * 0.5f;
            if (t == BirdType.Merganser) w += c.wood * 0.25f;
            if (t == BirdType.Muscovy) w += c.stone * 0.35f;
            if (t == BirdType.Scoter) w += c.tnt * 0.3f + (c.boss ? 1.5f : 0f);
            if (t == BirdType.Shelduck) w += c.foxes * 0.1f;
            if (t == BirdType.Pintail) w += c.wood * 0.15f;
            if (t == BirdType.Eider) w += c.stone * 0.2f + c.foxes * 0.05f;
            if (t == BirdType.Goldeneye) w += c.foxes * 0.12f;
            weights[t] = w;
        }
        var res = new BirdType[n];
        int start = 0;
        if (intro.HasValue) { res[0] = intro.Value; if (n > 2) res[1] = intro.Value; start = n > 2 ? 2 : 1; }
        for (int i = start; i < n; i++)
        {
            float sum = 0f; foreach (var kv in weights) sum += kv.Value;
            float r = c.R() * sum;
            BirdType pick = avail[0];
            foreach (var kv in weights) { r -= kv.Value; if (r <= 0f) { pick = kv.Key; break; } }
            res[i] = pick;
            weights[pick] *= 0.55f;   // çeşitlilik
        }
        return res;
    }
}
