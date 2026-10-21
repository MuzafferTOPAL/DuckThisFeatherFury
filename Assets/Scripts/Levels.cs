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
}

public static class Levels
{
    public static readonly string[] WorldNames = { "GÖL KIYISI", "SONBAHAR ORMANI", "KARLI DAĞLAR", "GÜN BATIMI ÇİFTLİĞİ" };
    public const int Count = 20;

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

    static LevelDef L(int theme, string title, float maxX, BirdType[] birds, BirdType? newBird = null)
    {
        var l = new LevelDef { index = all.Count, theme = theme, title = title, maxX = maxX, birds = birds, newBird = newBird };
        all.Add(l);
        return l;
    }

    static void Build()
    {
        all = new List<LevelDef>();
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
    }

    // ---- ilerleme kaydı
    public static int Stars(int i) => PlayerPrefs.GetInt("stars_" + i, 0);
    public static int Best(int i) => PlayerPrefs.GetInt("best_" + i, 0);
    public static bool Unlocked(int i) => i == 0 || Stars(i - 1) > 0 || PlayerPrefs.GetInt("unlock_all", 0) == 1;
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
