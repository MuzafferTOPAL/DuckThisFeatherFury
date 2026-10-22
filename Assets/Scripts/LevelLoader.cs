using System.Collections.Generic;
using UnityEngine;

// Bir LevelDef'ten sahneyi kurar: tema arka planı, zemin, süsler, bloklar ve tilkiler.
// Her bölüm değişiminde eski bölüm silinip yenisi kurulur (sahne yeniden yüklenmez).
public static class LevelLoader
{
    public const float GroundY = -4f;
    public const float SlingX = -7f;

    static GameObject levelRoot, backgroundRoot;
    static int builtTheme = -1;

    public static int EnemyCount { get; private set; }
    public static int PotentialScore { get; private set; }

    // Zorluk yüzdesi (oyuncuya gösterilen): level 1'de %1, level 15'te %15, level 100'de %75,
    // sonra yavaşlayarak level 500'de ~%98. Hiç düşmez; tilki SAYISI level level dalgalanır (LevelGen),
    // ama dayanıklılık, taş oranı, miğferli tilki ve yapı karmaşıklığı sürekli artar.
    public static float Percent(int index)
    {
        float n = index + 1;
        if (n <= 15f) return n;
        if (n <= 100f) return 15f + (n - 15f) * 60f / 85f;
        return 75f + 25f * (1f - Mathf.Exp(-(n - 100f) / 150f));
    }
    // İç zorluk değeri D (0..60): yapı üreticisi ve dayanıklılık çarpanları bunu kullanır
    public static float Difficulty(int index) => Percent(index) * 0.6f;
    public static int Tier(int index) => Mathf.Clamp(index / Levels.PerWorld, 0, Levels.WorldCount - 1);
    public static float FoxHealthMult(int index) => 1f + 0.02f * Difficulty(index);
    public static float BlockHealthMult(int index) => 1f + 0.01f * Difficulty(index);
    static float blockMul = 1f;

    public static void Clear()
    {
        if (levelRoot != null) Object.Destroy(levelRoot);
        levelRoot = null;
        Fx.ClearAll();
    }

    public static void Build(LevelDef def, Camera cam)
    {
        Clear();
        BuildBackground(def.theme, cam);

        levelRoot = new GameObject("Bolum_" + (def.index + 1));
        EnemyCount = 0;
        PotentialScore = 0;
        blockMul = BlockHealthMult(def.index);
        float foxMul = FoxHealthMult(def.index);

        int nestNo = 0;
        foreach (var p in def.pieces)
        {
            if (p.nest) { PotentialScore += CreateNest(p, levelRoot.transform, "nest_" + def.index + "_" + nestNo++); continue; }
            PotentialScore += CreatePiece(p, levelRoot.transform);
        }

        foreach (var f in def.foxes)
        {
            var fox = Fox.Create(f.kind, f.bottom);
            fox.transform.SetParent(levelRoot.transform, true);
            var fd = fox.GetComponent<Destructible>();
            fd.ResetHealth(fd.MaxHealth * foxMul);
            EnemyCount++;
        }
    }

    // ------------------------------------------------------------------ altın yuva
    static int CreateNest(LevelDef.Piece p, Transform parent, string key)
    {
        bool collected = PlayerPrefs.GetInt(key, 0) == 1;
        var go = new GameObject("AltinYuva");
        go.transform.SetParent(parent, false);
        go.transform.position = new Vector3(p.center.x, p.center.y - 0.4f + 14f / 32f, 0f);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = collected ? GameArt.I.nestEmpty : GameArt.I.nestGold;
        sr.sortingOrder = 5;
        var rb = go.AddComponent<Rigidbody2D>();
        rb.useAutoMass = true;
        var d = go.AddComponent<Destructible>();
        d.material = BlockMaterial.Wood;
        d.minImpact = 1.5f;
        var bc = go.AddComponent<BoxCollider2D>();
        bc.size = new Vector2(1.05f, 0.8f);
        bc.offset = new Vector2(0f, -0.04f);
        bc.sharedMaterial = PhysMats.Wood;
        bc.density = 0.8f;
        d.ResetHealth(3f);
        d.scoreValue = 1000;
        if (!collected) { d.bonusEggs = Economy.NestReward; d.bonusKey = key; }
        return d.scoreValue;
    }

    // ------------------------------------------------------------------ bloklar
    static int CreatePiece(LevelDef.Piece p, Transform parent)
    {
        var art = GameArt.I;
        var go = new GameObject(p.boulder ? "Kaya" : p.mat.ToString());
        go.transform.SetParent(parent, false);
        go.transform.position = p.center;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 3;

        var rb = go.AddComponent<Rigidbody2D>();
        rb.useAutoMass = true;

        var d = go.AddComponent<Destructible>();
        d.material = p.mat;
        d.minImpact = 2f;

        if (p.boulder)
        {
            sr.sprite = art.boulder;
            float s = p.size.x / 1f;          // boulder sprite 16px = 1 birim
            go.transform.localScale = new Vector3(s, s, 1f);
            var cc = go.AddComponent<CircleCollider2D>();
            cc.radius = 0.47f;
            cc.sharedMaterial = PhysMats.Stone;
            cc.density = 3f;
            d.ResetHealth(30f * blockMul);
            d.scoreValue = 800;
            return d.scoreValue;
        }

        bool vertical = p.size.y > p.size.x && p.mat != BlockMaterial.Tnt;
        float len = vertical ? p.size.y : p.size.x;
        float thick = vertical ? p.size.x : p.size.y;
        if (vertical) go.transform.rotation = Quaternion.Euler(0, 0, 90f);

        if (p.mat == BlockMaterial.Tnt)
        {
            sr.sprite = art.tnt;
            sr.drawMode = SpriteDrawMode.Simple;
            go.transform.localScale = new Vector3(p.size.x, p.size.y, 1f);
            var bc0 = go.AddComponent<BoxCollider2D>();
            bc0.size = Vector2.one;
            bc0.sharedMaterial = PhysMats.Wood;
            bc0.density = 0.8f;
            d.ResetHealth(4f);
            d.scoreValue = 600;
            return d.scoreValue;
        }

        sr.sprite = p.mat == BlockMaterial.Stone ? art.stone : p.mat == BlockMaterial.Ice ? art.ice : art.wood;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.tileMode = SpriteTileMode.Continuous;
        sr.size = new Vector2(len, thick);

        var bc = go.AddComponent<BoxCollider2D>();
        bc.size = sr.size;
        bc.sharedMaterial = PhysMats.For(p.mat);
        bc.density = p.mat == BlockMaterial.Stone ? 3f : p.mat == BlockMaterial.Ice ? 0.9f : 1.4f;

        // çatlak katmanı
        var crackGo = new GameObject("Catlak");
        crackGo.transform.SetParent(go.transform, false);
        var cr = crackGo.AddComponent<SpriteRenderer>();
        cr.sprite = art.crack1;
        cr.drawMode = SpriteDrawMode.Tiled;
        cr.size = sr.size;
        cr.sortingOrder = 4;
        cr.enabled = false;
        cr.color = p.mat == BlockMaterial.Ice ? new Color(1f, 1f, 1f, 0.95f)
                 : p.mat == BlockMaterial.Stone ? new Color(0.15f, 0.17f, 0.2f, 0.9f)
                 : new Color(0.25f, 0.13f, 0.05f, 0.9f);
        d.crackRenderer = cr;

        switch (p.mat)
        {
            case BlockMaterial.Stone: d.ResetHealth(30f * blockMul); d.scoreValue = 800; break;
            case BlockMaterial.Ice: d.ResetHealth(6f * blockMul); d.scoreValue = 300; break;
            default: d.ResetHealth(14f * blockMul); d.scoreValue = 500; break;
        }
        return d.scoreValue;
    }

    // ------------------------------------------------------------------ arka plan
    static void BuildBackground(int themeIndex, Camera cam)
    {
        var art = GameArt.I;
        var th = art.themes[Mathf.Clamp(themeIndex, 0, art.themes.Length - 1)];

        // gökyüzü kameranın çocuğu olarak kalıcı
        var sky = cam.transform.Find("Gokyuzu");
        if (sky == null)
        {
            var go = new GameObject("Gokyuzu");
            go.transform.SetParent(cam.transform, false);
            var ssr = go.AddComponent<SpriteRenderer>();
            ssr.sortingOrder = -100;
            go.AddComponent<SkyFit>();
            sky = go.transform;
        }
        sky.GetComponent<SpriteRenderer>().sprite = th.sky;

        if (builtTheme == themeIndex && backgroundRoot != null) return;
        if (backgroundRoot != null) Object.Destroy(backgroundRoot);
        builtTheme = themeIndex;
        backgroundRoot = new GameObject("ArkaPlan_" + themeIndex);
        var root = backgroundRoot.transform;

        // güneş / ay
        Para(Spr("Gunes", th.sun, -95, new Vector2(4.5f, 4.6f), root), 0.92f, 0f, 0f, 0.6f);
        if (th.stars)
        {
            var rnd = new System.Random(7);
            for (int i = 0; i < 28; i++)
            {
                var s = Spr("Yildiz", art.starPx, -97, new Vector2((float)rnd.NextDouble() * 40f - 20f, 1f + (float)rnd.NextDouble() * 9f), root);
                Para(s, 0.95f, 0f, 0f, 0.7f);
            }
        }
        // bulutlar
        var clouds = new (bool a, float x, float y, float speed)[]
        {
            (true, -8f, 4.2f, 0.18f), (false, 0f, 3.4f, 0.12f), (true, 7f, 5.2f, 0.25f), (false, 14f, 4.6f, 0.15f), (true, -16f, 3f, 0.2f)
        };
        foreach (var c in clouds)
            Para(Spr("Bulut", c.a ? th.cloudA : th.cloudB, -92, new Vector2(c.x, c.y), root), 0.85f, c.speed, 48f, 0.5f);

        // uzak dağlar ve yakın tepeler (yan yana kopyalar)
        var far = new GameObject("Uzak").transform; far.SetParent(root, false);
        for (int i = -3; i <= 3; i++) Spr("Dag", th.far, -80, new Vector2(i * 20f, GroundY + 0.6f), far);
        Para(far, 0.6f, 0f, 0f, 0.2f);

        var near = new GameObject("Yakin").transform; near.SetParent(root, false);
        for (int i = -3; i <= 3; i++) Spr("Tepe", th.near, -70, new Vector2(i * 20f, GroundY - 0.4f), near);
        Para(near, 0.35f, 0f, 0f, 0.08f);

        // göl şeridi
        var water = Spr("Gol", th.water, -50, new Vector2(0f, GroundY + 0.38f), root);
        water.drawMode = SpriteDrawMode.Tiled;
        water.size = new Vector2(140f, 0.75f);

        // zemin
        var ground = Spr("Zemin", th.ground, -10, new Vector2(0f, GroundY - 0.81f), root);
        ground.drawMode = SpriteDrawMode.Tiled;
        ground.size = new Vector2(140f, 2f);
        var dirt = Spr("Toprak", th.dirt, -11, new Vector2(0f, GroundY - 1.81f - 15f), root);
        dirt.transform.localScale = new Vector3(140f / 0.25f, 30f / 0.25f, 1f);
        var gc = new GameObject("ZeminCollider").AddComponent<BoxCollider2D>();
        gc.transform.SetParent(root, false);
        gc.transform.position = new Vector3(0f, GroundY - 5f, 0f);
        gc.size = new Vector2(160f, 10f);
        gc.sharedMaterial = PhysMats.Ground;

        // süsler (zemine oturur)
        Spr("Sus0", th.decor0, -6, new Vector2(-13.2f, GroundY), root);
        Spr("Sus1", th.decor1, -7, new Vector2(-4.6f, GroundY), root);
        Spr("Cali", th.bush, -6, new Vector2(-2.8f, GroundY), root);
        Spr("Sus2", th.decor2, -6, new Vector2(0.4f, GroundY), root);
        Spr("Cali2", th.bush, -6, new Vector2(15.8f, GroundY), root);
        Spr("Sus0b", th.decor0, -6, new Vector2(17.5f, GroundY), root);
    }

    static SpriteRenderer Spr(string name, Sprite s, int order, Vector2 pos, Transform parent)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.position = pos;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = s;
        sr.sortingOrder = order;
        return sr;
    }

    static void Para(Component c, float factor, float drift, float wrap, float vertical)
    {
        var p = c.gameObject.AddComponent<Parallax>();
        p.factor = factor;
        p.driftSpeed = drift;
        p.wrapWidth = wrap;
        p.verticalFactor = vertical;
    }
}
