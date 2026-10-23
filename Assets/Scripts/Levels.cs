using System.Collections.Generic;
using UnityEngine;

// 20 bölümün tanımı. Zemin üstü y = -4, sapan x = -7.
// Yapılar yardımcı fonksiyonlarla kurulur: Frame (iki direk + üst kiriş), Beam, Col, Cube, Tnt, Boulder, Fox.
public class LevelDef
{
    public struct Piece
    {
        public BlockMaterial mat;
        public Vector2 center, size;
        public bool boulder;
        public bool nest;      // altın yuva (bonus yumurta)
    }

    public struct FoxSpawn
    {
        public FoxKind kind;
        public Vector2 bottom;
    }

    public int index;
    public int theme;
    public string title;
    public BirdType[] birds;
    public BirdType? newBird;
    public string tip;          // tanıtım kartında gösterilecek ipucu (Loc anahtarı)
    public string titleKey;     // el yapımı bölümler: Loc anahtarı (lvlN); üretilenlerde null
    public bool generated;
    public int world, slot;
    public float maxX = 11f;
    public readonly List<Piece> pieces = new List<Piece>();
    public readonly List<FoxSpawn> foxes = new List<FoxSpawn>();

    const float T = 0.5f; // blok kalınlığı

    // ---- yapı yardımcıları (hepsi alt kenardan ölçülür, üst kenarı döndürür)
    public float Block(BlockMaterial m, float cx, float y0, float w, float h)
    {
        pieces.Add(new Piece { mat = m, center = new Vector2(cx, y0 + h * 0.5f), size = new Vector2(w, h) });
        return y0 + h;
    }
    public float Col(BlockMaterial m, float x, float y0, float h) => Block(m, x, y0, T, h);
    public float Beam(BlockMaterial m, float x0, float x1, float y0) => Block(m, (x0 + x1) * 0.5f, y0, x1 - x0, T);
    public float Cube(BlockMaterial m, float x, float y0, float s) => Block(m, x, y0, s, s);
    public float Tnt(float x, float y0) => Block(BlockMaterial.Tnt, x, y0, 1f, 1f);
    public float Frame(BlockMaterial col, BlockMaterial beam, float x0, float x1, float y0, float h)
    {
        Col(col, x0 + T * 0.5f, y0, h);
        Col(col, x1 - T * 0.5f, y0, h);
        return Beam(beam, x0, x1, y0 + h);
    }
    public void Boulder(float x, float y0, float r)
    {
        pieces.Add(new Piece { mat = BlockMaterial.Stone, center = new Vector2(x, y0 + r), size = new Vector2(r * 2f, r * 2f), boulder = true });
    }
    public void Fox(FoxKind k, float x, float y0) => foxes.Add(new FoxSpawn { kind = k, bottom = new Vector2(x, y0) });
    // Altın yuva: kırılınca bir kereliğine +3 altın yumurta verir
    public void Nest(float x, float y0) =>
        pieces.Add(new Piece { mat = BlockMaterial.Wood, center = new Vector2(x, y0 + 0.4f), size = new Vector2(1.1f, 0.8f), nest = true });
}

public static class Levels
{
    // 25 bölüm (sahne) x 20 level = 500 level. Bir bölüm bitmeden sonraki açılmaz.
    public const int PerWorld = 20;
    public const int WorldCount = 25;
    public static int Worlds => WorldCount;
    public const int Count = PerWorld * WorldCount;
    public const int HandCount = 40;   // ilk 2 bölüm: elle tasarlanmış 40 level

    static List<LevelDef> all;
    public static IReadOnlyList<LevelDef> All
    {
        get
        {
            if (all == null) Build();
            return all;
        }
    }

    public static LevelDef Get(int i) => All[Mathf.Clamp(i, 0, Count - 1)];

    const BlockMaterial W = BlockMaterial.Wood, S = BlockMaterial.Stone, I = BlockMaterial.Ice;
    const BirdType D = BirdType.Mallard, SP = BirdType.Mandarin, H = BirdType.Merganser, G = BirdType.Muscovy, C = BirdType.Scoter, SH = BirdType.Shelduck;
    const FoxKind K = FoxKind.Kit, N = FoxKind.Normal, HL = FoxKind.Helmet, B = FoxKind.Boss;

    static List<LevelDef> hand;

    static LevelDef L(int theme, string title, float maxX, BirdType[] birds, BirdType? newBird = null)
    {
        var l = new LevelDef { index = hand.Count, theme = theme, title = title, maxX = maxX, birds = birds, newBird = newBird, titleKey = "lvl" + hand.Count };
        hand.Add(l);
        return l;
    }

    // Henüz açılmamış ördek türünü o anda kullanılabilen bir türle değiştir
    static BirdType Substitute(BirdType t)
    {
        switch (t)
        {
            case BirdType.Muscovy: return BirdType.Mallard;
            case BirdType.Shelduck: return BirdType.Mandarin;
            case BirdType.Scoter: return BirdType.Merganser;
            default: return BirdType.Mallard;
        }
    }

    static void Build()
    {
        BuildHandmade();
        all = new List<LevelDef>(Count);
        int bigInWorld = 0, bigStreak = 0, hardStreak = 0;
        for (int index = 0; index < Count; index++)
        {
            int w = index / PerWorld, s = index % PerWorld;
            if (s == 0) { bigInWorld = 0; bigStreak = 0; hardStreak = 0; }
            var avail = new List<BirdType>();
            for (int t = 0; t < BirdInfo.Count; t++)
                if (BirdInfo.Available((BirdType)t, index)) avail.Add((BirdType)t);

            // bu bölümde yeni açılan tür (bölümün ilk levelinde tanıtılır)
            BirdType? intro = null;
            if (s == 0)
                for (int t = 0; t < BirdInfo.Count; t++)
                    if (BirdInfo.UnlockLevel((BirdType)t) == index && index > 0) intro = (BirdType)t;

            LevelDef l;
            if (index < HandCount)
            {
                l = hand[index];
                l.index = index;
                l.theme = w;
                bool changed = false;
                for (int k = 0; k < l.birds.Length; k++)
                    if (!avail.Contains(l.birds[k])) { l.birds[k] = Substitute(l.birds[k]); changed = true; }
                if (changed)   // yerine konan tür o yapıya daha zayıf: bir ördek fazladan
                {
                    var list = new List<BirdType>(l.birds) { avail[index % avail.Count] };
                    l.birds = list.ToArray();
                }
                // ilk bölüm (%1-%11 zorluk) çok rahat olsun: bir yedek yeşilbaş; 2. bölümde en az 4 ördek
                // test raporu: L13 (Kale Duvarı), L18 (Piramit) ve L40 (patron) duvar oluyordu → birer ördek daha
                if (index == 12 || index == 17 || index == 39)
                {
                    var list = new List<BirdType>(l.birds) { avail.Contains(BirdType.Merganser) ? BirdType.Merganser : BirdType.Mallard };
                    l.birds = list.ToArray();
                }
                if (index < PerWorld || (index < PerWorld * 2 && l.birds.Length < 4))
                {
                    var list = new List<BirdType>(l.birds) { BirdType.Mallard };
                    l.birds = list.ToArray();
                }
                if (l.newBird.HasValue && !BirdInfo.Available(l.newBird.Value, index) ) l.newBird = null;
                if (l.newBird.HasValue && BirdInfo.UnlockLevel(l.newBird.Value) > 0) l.newBird = null;
            }
            else
            {
                // dev kale (7 ördek) sınırı: bölümde en fazla 4, art arda en fazla 2; iki zor levelden sonra nefes leveli
                // bölüm açılışı ve iki zor levelden sonraki level gerçekten hafif: en fazla 5 ördeklik kale
                int maxDucks = (s == 0 || hardStreak >= 2) ? 5 : (bigInWorld >= 4 || bigStreak >= 2) ? 6 : 7;
                l = LevelGen.Make(index, w, s, LevelLoader.Difficulty(index), avail, intro, maxDucks, hardStreak >= 2);
                if (s != PerWorld - 1)
                {
                    bool big = l.birds.Length >= 7, hard = l.birds.Length >= 6;
                    bigStreak = big ? bigStreak + 1 : 0;
                    if (big) bigInWorld++;
                    hardStreak = hard ? hardStreak + 1 : 0;
                }
            }
            if (intro.HasValue) l.newBird = intro;
            l.world = w; l.slot = s;
            all.Add(l);
        }
        // Altın yuva ipucu: oyuncunun karşılaşacağı ilk yuvalı levelde gösterilir
        bool tipped = false;
        foreach (var l in all)
        {
            if (l.tip == "tip_nest") l.tip = null;
            bool hasNest = false;
            foreach (var p in l.pieces) if (p.nest) hasNest = true;
            if (hasNest && !tipped) { l.tip = "tip_nest"; tipped = true; }
        }
        all[0].tip = all[0].tip ?? "tip_pull";
        if (all[1].tip == null) all[1].tip = "tip_tap";
    }

    // Level düzeni değişti (200 → 500 level, yeni zorluk eğrisi): eski level ilerlemesi bir kez sıfırlanır.
    // Altın yumurta (yeni ekonomiye göre x10), yükseltmeler ve ekstra ördekler korunur.
    public static void MigrateIfNeeded()
    {
        int v = PlayerPrefs.GetInt("level_layout", 1);
        if (v >= 3) return;
        for (int i = 0; i < 200; i++)
        {
            PlayerPrefs.DeleteKey("stars_" + i);
            PlayerPrefs.DeleteKey("best_" + i);
            for (int n = 0; n < 4; n++) PlayerPrefs.DeleteKey("nest_" + i + "_" + n);
        }
        for (int w = 0; w < 10; w++) for (int k = 0; k < 2; k++) PlayerPrefs.DeleteKey("gift_" + w + "_" + k);
        PlayerPrefs.DeleteKey("last_level");
        if (PlayerPrefs.GetInt("econ_v2", 0) == 0)
        {
            PlayerPrefs.SetInt("eggs", PlayerPrefs.GetInt("eggs", 0) * 10);
            PlayerPrefs.SetInt("econ_v2", 1);
        }
        PlayerPrefs.SetInt("level_layout", 3);
        PlayerPrefs.Save();
    }

    static void BuildHandmade()
    {
        hand = new List<LevelDef>();
        float y, t, a, b;
        LevelDef l;

        // ======================= DÜNYA 1: GÖL KIYISI
        l = L(0, "İLK ATIŞ", 9.5f, new[] { D, D, D }, D);
        t = l.Frame(W, W, 4f, 6.4f, -4f, 1.4f);
        l.Fox(N, 5.2f, -4f); l.Fox(N, 5.2f, t);

        l = L(0, "İKİZ KULELER", 12f, new[] { D, D, D, D });
        a = l.Frame(W, W, 2.5f, 4.9f, -4f, 1.4f); l.Fox(N, 3.7f, -4f); l.Fox(K, 3.7f, a);
        b = l.Frame(W, W, 7f, 9.4f, -4f, 1.4f); l.Fox(N, 8.2f, -4f); l.Fox(N, 8.2f, b);

        l = L(0, "TAŞ TEMEL", 10.5f, new[] { D, D, D });
        l.Cube(S, 3.5f, -4f, 1.2f); l.Cube(S, 6.9f, -4f, 1.2f); l.Fox(N, 5.2f, -4f);
        y = l.Beam(W, 2.9f, 7.5f, -2.8f);
        t = l.Frame(W, W, 4f, 6.4f, y, 1.4f); l.Fox(N, 5.2f, y); l.Fox(N, 5.2f, t);

        l = L(0, "BUZ SARAYI", 12.5f, new[] { SP, SP, D }, SP);
        a = l.Frame(I, I, 3.5f, 6.1f, -4f, 1.4f); l.Fox(N, 4.8f, -4f);
        t = l.Frame(I, I, 3.5f, 6.1f, a, 1.4f); l.Fox(N, 4.8f, a); l.Fox(K, 4.8f, t);
        b = l.Frame(I, I, 7.5f, 9.9f, -4f, 1.4f); l.Fox(K, 8.7f, -4f); l.Cube(I, 8.7f, b, 0.8f);

        l = L(0, "GÖL KALESİ", 11.5f, new[] { D, SP, D, SP });
        y = l.Frame(S, W, 3f, 8.4f, -4f, 1.9f);
        l.Fox(B, 5.7f, -4f); l.Tnt(4.1f, -4f); l.Tnt(7.4f, -4f);
        a = l.Frame(I, I, 3.4f, 5.6f, y, 1.3f); l.Fox(N, 4.5f, y);
        l.Frame(I, I, 5.8f, 8.0f, y, 1.3f); l.Fox(N, 6.9f, y);
        t = l.Beam(W, 3.4f, 8.0f, a); l.Fox(HL, 5.7f, t);
        l.Nest(10.2f, -4f);

        // ======================= DÜNYA 2: SONBAHAR ORMANI
        l = L(1, "ŞAHİN DALIŞI", 12.5f, new[] { H, H, D }, H);
        l.Block(W, 3.2f, -4f, 1f, 2.2f);
        t = l.Frame(W, W, 5f, 7.4f, -4f, 1.4f); l.Fox(N, 6.2f, -4f); l.Fox(N, 6.2f, t);
        l.Block(W, 9f, -4f, 1f, 2.5f); l.Fox(K, 10.5f, -4f);

        l = L(1, "ODUN KULESİ", 10.5f, new[] { H, D, SP });
        y = -4f;
        a = l.Frame(W, W, 5f, 7.4f, y, 1.3f); l.Fox(N, 6.2f, y);
        b = l.Frame(W, W, 5f, 7.4f, a, 1.3f); l.Fox(N, 6.2f, a);
        t = l.Frame(W, W, 5f, 7.4f, b, 1.3f); l.Fox(K, 6.2f, b); l.Fox(K, 6.2f, t);

        l = L(1, "PATLAYAN SANDIK", 11f, new[] { D, H, D });
        l.Col(S, 3.3f, -4f, 1.5f); l.Col(S, 8.7f, -4f, 1.5f);
        y = l.Beam(W, 3f, 9f, -2.5f);
        l.Tnt(4.5f, -4f); l.Tnt(7.5f, -4f); l.Fox(N, 6f, -4f);
        l.Fox(N, 4.5f, y); l.Tnt(6f, y); l.Fox(N, 7.5f, y);

        l = L(1, "AĞAÇ EVLERİ", 13f, new[] { H, SP, H });
        l.Col(S, 2.8f, -4f, 2.5f); l.Col(S, 5.2f, -4f, 2.5f);
        y = l.Beam(W, 2.5f, 5.5f, -1.5f);
        l.Frame(W, W, 2.8f, 5.2f, y, 1.3f); l.Fox(N, 4f, y);
        l.Col(S, 8f, -4f, 3.5f); l.Col(S, 10.4f, -4f, 3.5f);
        y = l.Beam(W, 7.7f, 10.7f, -0.5f);
        t = l.Frame(I, I, 8f, 10.4f, y, 1.3f); l.Fox(N, 9.2f, y); l.Fox(K, 9.2f, t);
        l.Fox(K, 6.6f, -4f);

        l = L(1, "TİLKİ İNİ", 11.5f, new[] { H, D, SP, H });
        y = l.Frame(S, W, 3f, 9f, -4f, 2f);
        l.Fox(B, 6f, -4f); l.Tnt(4.1f, -4f); l.Tnt(7.9f, -4f);
        l.Frame(W, W, 3.4f, 5.8f, y, 1.3f); l.Fox(HL, 4.6f, y);
        a = l.Frame(W, W, 6.2f, 8.6f, y, 1.3f); l.Fox(HL, 7.4f, y);
        t = l.Beam(W, 3.4f, 8.6f, a); l.Fox(N, 6f, t);
        l.Nest(10.3f, -4f);

        // ======================= DÜNYA 3: KARLI DAĞLAR
        l = L(2, "KAZ GELİYOR", 11f, new[] { G, G, D }, G);
        a = l.Frame(S, W, 4f, 6.6f, -4f, 1.4f); l.Fox(N, 5.3f, -4f); l.Fox(N, 5.3f, a);
        y = l.Cube(S, 9f, -4f, 1f); l.Fox(K, 9f, y);

        l = L(2, "BUZ KULELERİ", 12.5f, new[] { SP, G, SP });
        y = l.Beam(S, 2.6f, 10.3f, -4f);
        a = l.Frame(I, I, 3f, 5.4f, y, 1.3f); l.Fox(K, 4.2f, y);
        l.Frame(I, I, 3f, 5.4f, a, 1.3f); l.Fox(K, 4.2f, a);
        b = l.Frame(I, I, 7.5f, 9.9f, y, 1.3f); l.Fox(N, 8.7f, y);
        t = l.Frame(I, I, 7.5f, 9.9f, b, 1.3f); l.Fox(N, 8.7f, b); l.Fox(K, 8.7f, t);

        l = L(2, "KALE DUVARI", 12.5f, new[] { SH, SH, G, SH }, SH);
        l.Block(S, 3.5f, -4f, 1f, 1.4f);
        t = l.Frame(W, W, 5.2f, 7.6f, -4f, 1.4f); l.Fox(N, 6.4f, -4f); l.Fox(N, 6.4f, t);
        l.Block(S, 9f, -4f, 1f, 2f); l.Fox(HL, 10.6f, -4f);

        l = L(2, "KAYAN KAYALAR", 13f, new[] { D, G, H });
        l.Col(W, 3.2f, -4f, 2.5f); l.Col(W, 6.8f, -4f, 2.5f);
        y = l.Beam(W, 2.9f, 7.1f, -1.5f);
        l.Boulder(3.8f, y, 0.45f); l.Boulder(6.2f, y, 0.45f); l.Fox(K, 5f, y);
        l.Fox(N, 4.2f, -4f); l.Fox(N, 5.8f, -4f);
        a = l.Frame(I, I, 8.6f, 11f, -4f, 1.4f); l.Fox(K, 9.8f, -4f); l.Boulder(9.8f, a, 0.45f);

        l = L(2, "BUZ KALESİ", 12f, new[] { G, SP, H, G });
        y = l.Frame(S, I, 3f, 9.4f, -4f, 2f);
        l.Fox(B, 6.2f, -4f); l.Cube(I, 4.2f, -4f, 1f); l.Cube(I, 8.1f, -4f, 1f);
        l.Frame(I, I, 3.4f, 6f, y, 1.3f); l.Fox(HL, 4.7f, y);
        a = l.Frame(I, I, 6.4f, 9f, y, 1.3f); l.Fox(HL, 7.7f, y);
        b = l.Beam(S, 3.4f, 9f, a);
        t = l.Frame(W, W, 5f, 7.4f, b, 1.3f); l.Fox(N, 6.2f, b);
        l.Nest(10.7f, -4f);

        // ======================= DÜNYA 4: GÜN BATIMI ÇİFTLİĞİ
        l = L(3, "KARGA BOMBASI", 11.5f, new[] { C, C, D }, C);
        l.Frame(W, W, 4f, 6.4f, -4f, 1.3f); l.Fox(N, 5.2f, -4f);
        a = l.Frame(W, W, 6.6f, 9f, -4f, 1.3f); l.Fox(N, 7.8f, -4f);
        t = l.Beam(S, 4f, 9f, a); l.Fox(N, 5.2f, t); l.Tnt(7.8f, t);

        l = L(3, "AHIR", 11.5f, new[] { C, H, SP });
        y = l.Frame(W, W, 3f, 9f, -4f, 1.8f);
        l.Fox(N, 4.6f, -4f); l.Tnt(6f, -4f); l.Fox(N, 7.4f, -4f);
        t = l.Frame(W, W, 3.6f, 8.4f, y, 1.4f); l.Fox(HL, 6f, y); l.Fox(K, 6f, t);

        l = L(3, "PİRAMİT", 13f, new[] { G, C, SH });
        a = l.Frame(S, S, 2.5f, 10.5f, -4f, 1.4f); l.Fox(N, 4.5f, -4f); l.Fox(HL, 6.5f, -4f); l.Fox(N, 8.5f, -4f);
        b = l.Frame(W, W, 3.5f, 9.5f, a, 1.4f); l.Fox(N, 5.5f, a); l.Fox(N, 7.5f, a);
        t = l.Frame(I, I, 4.8f, 8.2f, b, 1.3f); l.Fox(K, 6.5f, b); l.Fox(N, 6.5f, t);

        l = L(3, "ÜÇ KALE", 15f, new[] { D, SP, H, C, G });
        l.Frame(W, W, 1.5f, 3.9f, -4f, 1.3f); l.Fox(N, 2.7f, -4f);
        a = l.Frame(I, I, 5.5f, 7.9f, -4f, 1.3f); l.Fox(N, 6.7f, -4f);
        l.Frame(I, I, 5.5f, 7.9f, a, 1.3f); l.Fox(K, 6.7f, a);
        l.Cube(S, 10f, -4f, 1.2f); l.Cube(S, 12.4f, -4f, 1.2f); l.Fox(N, 11.2f, -4f);
        y = l.Beam(S, 9.4f, 13f, -2.8f); t = l.Tnt(11.2f, y); l.Fox(HL, 11.2f, t);

        l = L(3, "TİLKİ KRAL", 14f, new[] { C, G, SH, H, SP });
        y = l.Frame(S, S, 3f, 11f, -4f, 2f);
        l.Fox(B, 7f, -4f); l.Tnt(4.2f, -4f); l.Tnt(9.8f, -4f); l.Fox(K, 5.4f, -4f); l.Fox(K, 8.6f, -4f);
        l.Frame(W, W, 3.5f, 7f, y, 1.4f); l.Fox(HL, 5.25f, y);
        a = l.Frame(W, W, 7.2f, 10.5f, y, 1.4f); l.Fox(HL, 8.85f, y);
        b = l.Beam(S, 3.5f, 10.5f, a);
        t = l.Frame(I, I, 5.2f, 8.8f, b, 1.3f); l.Fox(N, 7f, b); l.Fox(N, 7f, t);
        l.Nest(12.4f, -4f);

        // ======================= DÜNYA 5: ÇÖL KANYONU  (kademe 2 başlar: tilkiler ve bloklar bir kademe daha sağlam)
        l = L(4, "KUM FIRTINASI", 12f, new[] { D, SP, H, D }); l.tip = "tip_nest";
        a = l.Frame(W, W, 3f, 5.4f, -4f, 1.4f); l.Fox(N, 4.2f, -4f);
        t = l.Frame(W, W, 3f, 5.4f, a, 1.3f); l.Fox(K, 4.2f, a); l.Nest(4.2f, t);
        b = l.Frame(W, W, 8f, 10.4f, -4f, 1.4f); l.Fox(N, 9.2f, -4f); l.Fox(K, 9.2f, b);

        l = L(4, "KAKTÜS KULESİ", 11f, new[] { H, SP, D });
        a = l.Frame(W, W, 5f, 7.4f, -4f, 1.4f); l.Fox(N, 6.2f, -4f);
        b = l.Frame(W, W, 5f, 7.4f, a, 1.4f); l.Fox(N, 6.2f, a);
        t = l.Frame(I, I, 5f, 7.4f, b, 1.3f); l.Fox(K, 6.2f, b); l.Fox(N, 6.2f, t);
        l.Tnt(4.2f, -4f); l.Tnt(8.2f, -4f);

        l = L(4, "VAHA", 12.5f, new[] { G, D, SP, G });
        l.Cube(S, 3.4f, -4f, 1.2f); l.Cube(S, 9.6f, -4f, 1.2f);
        y = l.Beam(S, 2.8f, 10.2f, -2.8f);
        l.Fox(N, 5f, -4f); l.Fox(HL, 7.4f, -4f);
        l.Boulder(3.9f, y, 0.45f); l.Boulder(9.1f, y, 0.45f);
        t = l.Frame(I, I, 5.4f, 7.8f, y, 1.3f); l.Fox(N, 6.6f, y); l.Fox(K, 6.6f, t);

        l = L(4, "KANYON KÖPRÜSÜ", 13f, new[] { SH, H, C });
        l.Col(S, 3.5f, -4f, 2.6f); l.Col(W, 7f, -4f, 2.6f); l.Col(S, 10.5f, -4f, 2.6f);
        y = l.Beam(W, 3.25f, 10.75f, -1.4f);
        l.Fox(N, 5.2f, -4f); l.Fox(N, 8.8f, -4f);
        l.Frame(W, W, 4f, 6.4f, y, 1.3f); l.Fox(HL, 5.2f, y);
        l.Frame(W, W, 7.6f, 10f, y, 1.3f); l.Fox(N, 8.8f, y);
        l.Nest(7f, y);

        l = L(4, "FİRAVUN TİLKİ", 13f, new[] { G, C, SH, SP });
        y = l.Frame(S, S, 3f, 11f, -4f, 2f);
        l.Fox(B, 7f, -4f); l.Tnt(4.3f, -4f); l.Tnt(9.7f, -4f); l.Fox(K, 5.6f, -4f); l.Fox(K, 8.4f, -4f);
        a = l.Frame(S, W, 4f, 10f, y, 1.4f); l.Fox(HL, 5.5f, y); l.Fox(HL, 8.5f, y); l.Cube(I, 7f, y, 1f);
        t = l.Frame(W, W, 5.8f, 8.2f, a, 1.3f); l.Fox(N, 7f, a); l.Nest(7f, t);

        // ======================= DÜNYA 6: TROPİK SAHİL
        l = L(5, "KUMDAN KALE", 11.5f, new[] { SP, D, H, SP });
        a = l.Frame(I, I, 4f, 6.4f, -4f, 1.3f); l.Fox(N, 5.2f, -4f); l.Fox(K, 5.2f, a);
        b = l.Frame(W, I, 7f, 9.4f, -4f, 1.3f); l.Fox(N, 8.2f, -4f); l.Cube(I, 8.2f, b, 0.8f);
        l.Nest(10.6f, -4f);

        l = L(5, "HİNDİSTAN CEVİZİ", 13f, new[] { D, G, H, SP });
        l.Col(W, 3.2f, -4f, 2.4f); l.Col(W, 6.8f, -4f, 2.4f);
        y = l.Beam(W, 2.9f, 7.1f, -1.6f);
        l.Boulder(3.8f, y, 0.45f); l.Boulder(6.2f, y, 0.45f); l.Fox(N, 5f, y);
        l.Fox(N, 4.2f, -4f); l.Fox(HL, 5.8f, -4f);
        l.Col(W, 9f, -4f, 1.6f); l.Col(W, 11.6f, -4f, 1.6f);
        b = l.Beam(W, 8.7f, 11.9f, -2.4f); l.Boulder(10.6f, b, 0.45f); l.Nest(9.5f, b);
        l.Fox(N, 10.3f, -4f);

        l = L(5, "DENİZ FENERİ", 11.5f, new[] { H, G, C });
        a = l.Frame(S, W, 5f, 7.4f, -4f, 1.4f); l.Fox(N, 6.2f, -4f);
        b = l.Frame(W, W, 5f, 7.4f, a, 1.4f); l.Fox(HL, 6.2f, a);
        t = l.Frame(I, I, 5f, 7.4f, b, 1.3f); l.Fox(K, 6.2f, b); l.Fox(N, 6.2f, t);
        l.Frame(W, W, 9f, 11.4f, -4f, 1.3f); l.Fox(N, 10.2f, -4f);

        l = L(5, "KORSAN GEMİSİ", 14f, new[] { C, SH, SP, D });
        y = l.Beam(W, 2.5f, 12.5f, -4f);
        a = l.Frame(W, W, 3f, 6f, y, 1.4f); l.Fox(N, 4.5f, y); l.Tnt(6.8f, y);
        l.Frame(W, W, 7.6f, 10.6f, y, 1.4f); l.Fox(HL, 9.1f, y); l.Tnt(11.6f, y);
        b = l.Beam(W, 3f, 10.6f, a);
        t = l.Frame(W, W, 5.6f, 8f, b, 1.4f); l.Fox(N, 6.8f, b); l.Nest(6.8f, t);

        l = L(5, "TİLKİ ADASI", 14f, new[] { G, C, SH, H });
        a = l.Frame(S, W, 2.5f, 5.5f, -4f, 1.4f); l.Fox(HL, 4f, -4f);
        l.Frame(W, W, 2.9f, 5.1f, a, 1.3f); l.Fox(N, 4f, a);
        y = l.Frame(S, S, 7f, 12.4f, -4f, 2f);
        l.Fox(B, 9.7f, -4f); l.Tnt(8.05f, -4f); l.Tnt(11.35f, -4f);
        b = l.Frame(W, W, 7.4f, 9.6f, y, 1.3f); l.Fox(HL, 8.5f, y);
        l.Frame(W, W, 9.8f, 12f, y, 1.3f); l.Fox(HL, 10.9f, y);
        t = l.Beam(S, 7.4f, 12f, b); l.Fox(N, 9.7f, t);

        // ======================= DÜNYA 7: KİRAZ VADİSİ  (kademe 3)
        l = L(6, "PAGODA", 11.5f, new[] { SP, H, SH });
        a = l.Frame(W, W, 4f, 8.4f, -4f, 1.4f); l.Fox(N, 5.2f, -4f); l.Fox(N, 7.2f, -4f);
        b = l.Frame(W, W, 4.6f, 7.8f, a, 1.3f); l.Fox(N, 6.2f, a);
        t = l.Frame(I, I, 5.1f, 7.3f, b, 1.2f); l.Fox(K, 6.2f, b); l.Fox(N, 6.2f, t);

        l = L(6, "BAMBU ORMANI", 13f, new[] { H, H, SP, C });
        a = l.Frame(W, W, 2.8f, 5.2f, -4f, 2.2f); l.Fox(N, 4f, -4f); l.Fox(K, 4f, a);
        l.Frame(W, W, 5.6f, 8f, -4f, 2.2f); l.Fox(N, 6.8f, -4f); l.Nest(6.8f, a);
        l.Frame(W, W, 8.4f, 10.8f, -4f, 2.2f); l.Fox(HL, 9.6f, -4f); l.Fox(N, 9.6f, a);

        l = L(6, "TAŞ BAHÇE", 12.5f, new[] { G, SP, C });
        y = l.Beam(S, 3f, 10f, -4f);
        l.Cube(S, 3.6f, y, 1.1f); a = l.Cube(S, 9.4f, y, 1.1f);
        l.Fox(HL, 5.4f, y); l.Fox(N, 7.6f, y);
        b = l.Beam(I, 3f, 10f, a);
        l.Frame(I, I, 4.4f, 6.8f, b, 1.3f); l.Fox(N, 5.6f, b);
        l.Frame(I, I, 7f, 9.4f, b, 1.3f); l.Fox(N, 8.2f, b);
        l.Boulder(3.6f, b, 0.45f);

        l = L(6, "ÇAY EVİ", 13f, new[] { SH, C, G, SP, D });
        a = l.Frame(W, W, 3f, 7f, -4f, 1.5f); l.Fox(N, 4.1f, -4f); l.Tnt(5.6f, -4f);
        t = l.Frame(W, W, 3.4f, 6.6f, a, 1.3f); l.Fox(HL, 5f, a); l.Nest(5f, t);
        l.Cube(S, 9f, -4f, 1f); l.Cube(S, 11.4f, -4f, 1f);
        y = l.Beam(W, 8.5f, 11.9f, -3f); l.Fox(N, 10.2f, -4f); l.Fox(N, 10.2f, y);

        l = L(6, "ŞOGUN TİLKİ", 14f, new[] { C, G, H, SP, SH });
        y = l.Frame(S, S, 3f, 11f, -4f, 2f);
        l.Fox(B, 7f, -4f); l.Cube(I, 4.3f, -4f, 1f); l.Cube(I, 9.7f, -4f, 1f); l.Fox(K, 5.5f, -4f); l.Fox(K, 8.5f, -4f);
        l.Frame(W, W, 3.4f, 6.8f, y, 1.4f); l.Fox(HL, 5.1f, y);
        a = l.Frame(W, W, 7.2f, 10.6f, y, 1.4f); l.Fox(HL, 8.9f, y);
        b = l.Beam(S, 3.4f, 10.6f, a);
        t = l.Frame(I, I, 5.6f, 8.4f, b, 1.3f); l.Fox(N, 7f, b); l.Fox(N, 7f, t);

        // ======================= DÜNYA 8: ATEŞ DAĞI  (kademe 4, final)
        l = L(7, "KÜL YOLU", 12f, new[] { C, H, SP, D });
        a = l.Frame(S, W, 3.5f, 5.9f, -4f, 1.4f); l.Fox(N, 4.7f, -4f); l.Fox(HL, 4.7f, a);
        b = l.Frame(W, W, 7.5f, 9.9f, -4f, 1.4f); l.Fox(N, 8.7f, -4f); l.Tnt(8.7f, b);
        l.Nest(11f, -4f);

        l = L(7, "OBSİDYEN KULE", 11.5f, new[] { G, C, G, SH });
        a = l.Frame(S, S, 5f, 7.4f, -4f, 1.4f); l.Fox(HL, 6.2f, -4f);
        b = l.Frame(S, S, 5f, 7.4f, a, 1.4f); l.Fox(HL, 6.2f, a);
        t = l.Frame(S, W, 5f, 7.4f, b, 1.3f); l.Fox(N, 6.2f, b); l.Fox(N, 6.2f, t);
        y = l.Cube(S, 3.6f, -4f, 1.2f); l.Fox(K, 3.6f, y);
        y = l.Cube(S, 8.8f, -4f, 1.2f); l.Fox(K, 8.8f, y);

        l = L(7, "LAV KÖPRÜSÜ", 13.5f, new[] { SH, C, H });
        l.Col(S, 3.5f, -4f, 2.4f); l.Col(S, 7f, -4f, 2.4f); l.Col(S, 10.5f, -4f, 2.4f);
        y = l.Beam(W, 3.25f, 10.75f, -1.6f);
        l.Fox(N, 5.25f, -4f); l.Fox(N, 8.2f, -4f); l.Tnt(9.6f, -4f);
        l.Boulder(4.2f, y, 0.45f);
        l.Frame(W, W, 5f, 7.4f, y, 1.3f); l.Fox(HL, 6.2f, y);
        l.Frame(I, I, 8f, 10.4f, y, 1.3f); l.Fox(N, 9.2f, y);

        l = L(7, "EJDERHA MAĞARASI", 14f, new[] { SP, C, G, SH, H });
        y = l.Frame(S, W, 2.5f, 8.5f, -4f, 1.8f);
        l.Fox(N, 3.8f, -4f); l.Fox(HL, 5.5f, -4f); l.Fox(N, 7.2f, -4f);
        a = l.Frame(I, I, 3f, 8f, y, 1.3f); l.Fox(N, 4.4f, y); l.Tnt(5.5f, y); l.Fox(N, 6.6f, y);
        l.Fox(K, 5.5f, a);
        b = l.Frame(S, W, 10f, 12.4f, -4f, 1.4f); l.Fox(HL, 11.2f, -4f); l.Nest(11.2f, b);

        l = L(7, "TİLKİ İMPARATOR", 15f, new[] { C, G, SH, H, SP, D });
        a = l.Frame(W, W, 2f, 4.4f, -4f, 1.4f); l.Fox(HL, 3.2f, -4f); l.Fox(N, 3.2f, a);
        y = l.Frame(S, S, 6f, 13f, -4f, 2f);
        l.Fox(B, 8.4f, -4f); l.Fox(B, 11f, -4f); l.Tnt(6.95f, -4f);
        l.Frame(W, W, 6.4f, 9.4f, y, 1.4f); l.Fox(HL, 7.9f, y);
        b = l.Frame(W, W, 9.6f, 12.6f, y, 1.4f); l.Fox(HL, 11.1f, y);
        t = l.Beam(S, 6.4f, 12.6f, b);
        a = l.Frame(I, I, 8.3f, 10.7f, t, 1.3f); l.Fox(N, 9.5f, t); l.Fox(K, 9.5f, a); l.Nest(7.2f, t);
    }

    // ---- ilerleme kaydı
    public static int Stars(int i) => PlayerPrefs.GetInt("stars_" + i, 0);
    public static int Best(int i) => PlayerPrefs.GetInt("best_" + i, 0);
    public static bool Unlocked(int i) => i == 0 || Stars(i - 1) > 0 || PlayerPrefs.GetInt("unlock_all", 0) == 1;
    public static IEnumerable<string> NestKeys()
    {
        foreach (var l in All)
        {
            int n = 0;
            foreach (var p in l.pieces) if (p.nest) yield return "nest_" + l.index + "_" + n++;
        }
    }

    public static int TotalStars()
    {
        int s = 0;
        for (int i = 0; i < Count; i++) s += Stars(i);
        return s;
    }
    public static void Save(int i, int stars, int score)
    {
        if (stars > Stars(i)) PlayerPrefs.SetInt("stars_" + i, stars);
        if (score > Best(i)) PlayerPrefs.SetInt("best_" + i, score);
        PlayerPrefs.Save();
    }
}
