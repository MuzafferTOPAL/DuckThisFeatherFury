using System.Collections.Generic;
using UnityEngine;

// Pixel-art arayüz (IMGUI): ana menü, bölüm seçimi, bölüm tanıtım kartı, oyun içi gösterge,
// duraklatma, sonuç ekranı ve 4 farklı pixel geçiş efekti.
// Ölçüler 1080 piksel yükseklik referans alınarak her ekrana orantılı çizilir.
public class Hud : MonoBehaviour
{
    public static Hud Instance { get; private set; }

    class Pop { public Vector3 world; public string text; public Color color; public float age; public int size; }

    const float RefH = 1080f;
    static readonly Color Ink = new Color(0.1f, 0.08f, 0.12f, 1f);
    static readonly Color Cream = new Color(1f, 0.96f, 0.84f);
    static readonly Color Gold = new Color(1f, 0.82f, 0.25f);
    static readonly Color Brown = new Color(0.42f, 0.25f, 0.12f);

    readonly List<Pop> pops = new List<Pop>();
    float s = 1f, vw;
    float shownScore;
    GUIStyle label;
    Camera cam;

    void Awake()
    {
        Instance = this;
        cam = Camera.main;
    }

    public static void Popup(Vector3 world, string text, Color color, int size = 44)
    {
        if (Instance == null) return;
        Instance.pops.Add(new Pop { world = world, text = text, color = color, size = size });
    }

    // ------------------------------------------------------------------ dokunuş engeli
    static float Scale => Screen.height / RefH;
    static Rect PauseRect => new Rect(28, 24, 110, 116);
    static Rect RestartRect => new Rect(150, 24, 110, 116);
    static Rect PlusRect => new Rect(272, 24, 110, 116);

    public static bool IsOverUI(Vector2 screenPos)
    {
        var gm = GameManager.Instance;
        if (gm == null) return false;
        if (gm.State != GameManager.GState.Playing || gm.Paused) return true;
        float sc = Scale;
        Vector2 g = new Vector2(screenPos.x / sc, (Screen.height - screenPos.y) / sc);
        return Grow(PauseRect, 12).Contains(g) || Grow(RestartRect, 12).Contains(g) || Grow(PlusRect, 12).Contains(g);
    }

    static Rect Grow(Rect r, float d) => new Rect(r.x - d, r.y - d, r.width + d * 2, r.height + d * 2);

    // ------------------------------------------------------------------ döngü
    void Update()
    {
        float dt = Time.unscaledDeltaTime;
        for (int i = pops.Count - 1; i >= 0; i--)
        {
            pops[i].age += dt;
            if (pops[i].age > 1.3f) pops.RemoveAt(i);
        }
        var gm = GameManager.Instance;
        if (gm != null && picker && gm.State == GameManager.GState.Playing && !gm.Paused) picker = false;
        if (gm != null)
        {
            if (gm.Score < shownScore) shownScore = gm.Score;
            shownScore = Mathf.MoveTowards(shownScore, gm.Score, Mathf.Max(300f, (gm.Score - shownScore) * 8f) * dt);
        }
    }

    void OnGUI()
    {
        var gm = GameManager.Instance;
        var art = GameArt.I;
        if (gm == null || art == null) return;
        if (cam == null) cam = Camera.main;

        s = Scale;
        vw = Screen.width / s;
        if (label == null)
            label = new GUIStyle(GUI.skin.label) { font = art.font, alignment = TextAnchor.MiddleCenter, wordWrap = false, clipping = TextClipping.Overflow };

        DrawPopups();

        switch (gm.State)
        {
            case GameManager.GState.Splash: DrawSplash(gm, art); break;
            case GameManager.GState.Menu: DrawMenu(gm, art); break;
            case GameManager.GState.LevelSelect: DrawSelect(gm, art); break;
            case GameManager.GState.Shop: DrawShop(gm, art); break;
            case GameManager.GState.Intro: DrawIntro(gm, art); break;
            case GameManager.GState.Playing:
            case GameManager.GState.Won:
            case GameManager.GState.Lost:
                if (picker && gm.State == GameManager.GState.Won) picker = false;
                DrawHud(gm, art);
                if (gm.Paused && !picker) DrawPause(gm, art);
                if (gm.State == GameManager.GState.Lost && gm.StateTime > 0.2f && !picker) DrawEnd(gm, art, false);
                if (gm.State == GameManager.GState.Won && gm.EndShown) DrawEnd(gm, art, true);
                if (picker) DrawPicker(gm, art);
                break;
            case GameManager.GState.Transition:
                if (gm.TransT > 1f && gm.Level != null) DrawHud(gm, art);
                break;
        }

        if (gm.TransT >= 0f) DrawTransition(gm);
        if (gm.State != GameManager.GState.Playing && gm.State != GameManager.GState.Lost && gm.State != GameManager.GState.Won) picker = false;
    }

    // ================================================================== ANA MENÜ
    void DrawMenu(GameManager gm, GameArt art)
    {
        Dim(0.25f);
        if (confirmQuit) { Dim(0.4f); DrawQuitConfirm(gm, art); return; }
        float t = Time.unscaledTime;
        Text(new Rect(0, 60 + Mathf.Sin(t * 2f) * 6f, vw, 170), Loc.T("title"), 170, Gold, TextAnchor.MiddleCenter, 1f, 10);
        Text(new Rect(0, 215 + Mathf.Sin(t * 2f + 0.6f) * 4f, vw, 100), Loc.T("title2"), 92, new Color(1f, 0.55f, 0.2f), TextAnchor.MiddleCenter, 1f, 7);
        Text(new Rect(0, 312, vw, 50), Loc.T("subtitle"), 42, Cream, TextAnchor.MiddleCenter);

        // kuş geçidi (6 ördek)
        float bx = vw / 2 - 450;
        for (int i = 0; i < 6; i++)
        {
            var b = art.birds[i];
            var sp = Mathf.FloorToInt(t * 6 + i) % 2 == 0 ? b.up : b.down;
            float h = 100 + Mathf.Sin(t * 3 + i) * 10;
            SpriteTex(new Rect(bx + i * 150, 380 + Mathf.Sin(t * 2.5f + i * 1.3f) * 12f, 140, h), sp);
        }

        if (Button(new Rect(vw / 2 - 170, 535, 340, 150), art.btnGreen, null, Loc.T("play"), 80)) gm.OpenLevelSelect();
        Text(new Rect(0, 695, vw, 56), "★ " + Levels.TotalStars() + " / " + (Levels.Count * 3), 46, Gold, TextAnchor.MiddleCenter);
        if (Button(new Rect(vw / 2 - 170, 765, 340, 120), art.btnOrange, null, Loc.T("shop"), 64)) gm.OpenShop();
        EggPill(new Rect(vw - 320 - 340, 30, 340, 110));
        if (Button(new Rect(vw - 290, 24, 120, 126), Music.Muted ? art.btnGrey : art.btnGreen, null, Music.Muted ? Loc.T("music_off") : Loc.T("music_on"), 30))
        {
            Music.Muted = !Music.Muted;
            Sfx.Play(Sfx.Click);
        }

        if (Button(new Rect(vw - 160, 24, 120, 126), Sfx.Muted ? art.btnGrey : art.btnBlue, null, Sfx.Muted ? Loc.T("sound_off") : Loc.T("sound_on"), 30))
            Sfx.Muted = !Sfx.Muted;
        if (Button(new Rect(40, 24, 300, 126), art.btnOrange, null, Loc.Names[Loc.Lang], 40)) { Sfx.Play(Sfx.Click); Loc.Next(); }
        if (Button(new Rect(vw / 2 - 130, 905, 260, 100), art.btnGrey, null, Loc.T("quit"), 52)) { Sfx.Play(Sfx.Click); confirmQuit = true; }
    }

    // ================================================================== BÖLÜM SEÇİMİ
    void DrawSelect(GameManager gm, GameArt art)
    {
        bool modal = giftT > 0f;
        GUI.enabled = !modal;
        DrawSelectBody(gm, art);
        GUI.enabled = true;
        if (modal) DrawGiftReveal(art);
    }

    void DrawSelectBody(GameManager gm, GameArt art)
    {
        Dim(0.45f);
        Rect panel = new Rect(vw / 2 - 780, 150, 1560, 860);
        Panel(panel);
        int nw = Levels.Worlds;
        int w = Mathf.Clamp(gm.SelectWorld, 0, nw - 1);
        Ribbon(new Rect(vw / 2 - 560, 70, 1120, 170), Loc.T("world") + " " + (w + 1) + ": " + Loc.World(w), 64);

        // bölüm sekmeleri (önceki bölüm bitmeden sonraki açılmaz)
        float tabW = 124, tabG = 12, tabX0 = panel.center.x - (nw * tabW + (nw - 1) * tabG) / 2;
        for (int i = 0; i < nw; i++)
        {
            bool open = Levels.Unlocked(i * Levels.PerWorld);
            Rect r = new Rect(tabX0 + i * (tabW + tabG), panel.y + 125, tabW, 100);
            var tex = i == w ? art.btnOrange : open ? art.btnBlue : art.btnGrey;
            if (Button(r, tex, open ? null : art.iconLock, open ? (i + 1).ToString() : "", 60))
            {
                Sfx.Play(Sfx.Click);
                if (open) gm.SelectWorld = i;
                else lockMsgT = Time.unscaledTime;
            }
            if (open && Economy.AnyGiftReady(i))
                Tex(new Rect(r.xMax - 40, r.y - 18 + Mathf.Sin(Time.unscaledTime * 6f) * 4f, 48, 48), art.iconGift);
        }

        // 20 level: 2 sıra x 10
        int per = Levels.PerWorld, cols = 10;
        float bw = 122, bh = 118, gx = 18;
        float gx0 = panel.center.x - (cols * bw + (cols - 1) * gx) / 2;
        for (int k = 0; k < per; k++)
        {
            int idx = w * per + k;
            bool open = Levels.Unlocked(idx);
            int row = k / cols, col = k % cols;
            Rect r = new Rect(gx0 + col * (bw + gx), panel.y + 262 + row * 228, bw, bh);
            bool boss = k == per - 1;
            var bg = !open ? art.btnGrey : boss ? art.btnOrange : art.btnGreen;
            if (Button(r, bg, open ? null : art.iconLock, open ? (idx + 1).ToString() : "", idx + 1 >= 100 ? 50 : 60) && open)
                gm.PlayLevel(idx);
            int st = Levels.Stars(idx);
            for (int j = 0; j < 3; j++)
                Tex(new Rect(r.x + 4 + j * 39, r.yMax + 8, 36, 36), j < st ? art.uiStar : art.uiStarEmpty);
        }

        int ws = 0;
        for (int k = 0; k < per; k++) ws += Levels.Stars(w * per + k);
        // ---- yıldız hediyeleri: bölüm başına 2 hediye ördek
        Text(new Rect(panel.center.x - 520, panel.yMax - 140, 360, 50), "★ " + ws + " / " + (per * 3), 46, Brown, TextAnchor.MiddleCenter, 1f, 0);
        Text(new Rect(panel.center.x - 520, panel.yMax - 92, 360, 40), Loc.T("gift_hint"), 28, new Color(0.55f, 0.4f, 0.28f), TextAnchor.MiddleCenter, 1f, 0);
        for (int g = 0; g < Economy.GiftStars.Length; g++)
        {
            Rect gr = new Rect(panel.center.x - 140 + g * 268, panel.yMax - 140, 250, 108);
            bool claimed = Economy.GiftClaimed(w, g), ready = Economy.GiftReady(w, g);
            float pulse = ready ? 1f + 0.05f * Mathf.Sin(Time.unscaledTime * 8f) : 1f;
            Rect pr = new Rect(gr.center.x - gr.width * pulse / 2, gr.center.y - gr.height * pulse / 2, gr.width * pulse, gr.height * pulse);
            bool click = Button(pr, ready ? art.btnGreen : claimed ? art.btnBlue : art.btnGrey, null, null, 0);
            if (claimed)
            {
                SpriteTex(new Rect(pr.x + 18, pr.y + 12, 90, 72), art.Bird(Economy.GiftDuck(w, g)).idle);
                Text(new Rect(pr.x + 110, pr.y, pr.width - 120, pr.height * 0.86f), "+1", 56, Color.white, TextAnchor.MiddleCenter);
            }
            else
            {
                Tex(new Rect(pr.x + 16, pr.y + 16, 64, 64), art.iconGift, ready ? 1f : 0.55f);
                Text(new Rect(pr.x + 84, pr.y, pr.width - 96, pr.height * 0.86f), ready ? Loc.T("gift_take") : Economy.GiftStars[g] + "★", ready ? 52 : 50, Color.white, TextAnchor.MiddleCenter);
            }
            if (click && ready)
            {
                giftDuck = Economy.ClaimGift(w, g);
                giftT = Time.unscaledTime;
                Sfx.Play(Sfx.Win, 0.8f, false);
            }
            else if (click) Sfx.Play(Sfx.Click);
        }


        if (lockMsgT > 0f && Time.unscaledTime - lockMsgT < 1.6f)
        {
            int need = 0;
            for (int i = 0; i < nw; i++) if (!Levels.Unlocked(i * Levels.PerWorld)) { need = i; break; }
            Text(new Rect(panel.x, panel.y + 222, panel.width, 44), string.Format(Loc.T("locked_world"), need), 38, new Color(0.85f, 0.25f, 0.15f), TextAnchor.MiddleCenter, 1f, 0);
        }

        if (Button(new Rect(panel.x + 40, panel.yMax - 150, 120, 126), art.btnBlue, art.iconMenu, null, 0)) gm.OpenMenu();
        if (Button(new Rect(panel.xMax - 380, panel.yMax - 132, 340, 104), art.btnOrange, null, Loc.T("shop"), 56)) gm.OpenShop();
        EggPill(new Rect(vw - 30 - 340, 30, 340, 110));
        Text(new Rect(30, 40, 480, 70), "★ " + Levels.TotalStars() + " / " + (Levels.Count * 3), 50, Gold, TextAnchor.MiddleLeft);
    }

    float lockMsgT = -1f;
    float giftT = -1f;
    BirdType giftDuck;

    // Hediye açılış penceresi
    void DrawGiftReveal(GameArt art)
    {
        float t = Time.unscaledTime - giftT;
        Dim(0.6f * Mathf.Clamp01(t / 0.2f));
        float k = EaseBack(Mathf.Clamp01(t / 0.35f));
        Rect panel = Centered(820 * k, 600 * k, 20);
        Panel(panel);
        if (k < 1f) return;
        Ribbon(new Rect(vw / 2 - 400, panel.y - 70, 800, 150), Loc.T("gift_title"), 64);
        var b = art.Bird(giftDuck);
        var sp = Mathf.FloorToInt(Time.unscaledTime * 6f) % 2 == 0 ? b.up : b.down;
        float bob = Mathf.Sin(Time.unscaledTime * 4f) * 8f;
        SpriteTex(new Rect(panel.center.x - 150, panel.y + 110 + bob, 300, 230), sp);
        Text(new Rect(panel.x, panel.y + 350, panel.width, 70), Loc.BirdName(giftDuck), 60, Brown, TextAnchor.MiddleCenter, 1f, 0);
        Text(new Rect(panel.x, panel.y + 418, panel.width, 50), "+1 " + Loc.T("extra") + "  (" + Loc.T("owned") + ": " + Economy.Extra(giftDuck) + ")", 36, new Color(0.3f, 0.55f, 0.2f), TextAnchor.MiddleCenter, 1f, 0);
        if (t > 0.6f && Button(new Rect(panel.center.x - 130, panel.yMax - 110, 260, 104), art.btnGreen, null, Loc.T("ok"), 56))
        {
            Sfx.Play(Sfx.Click);
            giftT = -1f;
        }
    }

    // ================================================================== TANITIM KARTI
    void DrawIntro(GameManager gm, GameArt art)
    {
        var lvl = gm.Level;
        if (lvl == null) return;
        Dim(0.35f);
        float k = EaseBack(Mathf.Clamp01(gm.StateTime / 0.35f));
        bool hasNew = lvl.newBird.HasValue;
        bool tierUp = lvl.index > 0 && lvl.index % Levels.PerWorld == 0;
        bool hasTip = !string.IsNullOrEmpty(lvl.tip);
        float ph = (hasNew ? 720 : 470) + (tierUp ? 70 : 0) + (hasTip ? 110 : 0);
        Rect panel = Centered(1100 * k, ph * k, 30);
        Panel(panel);
        if (k < 1f) return;

        Ribbon(new Rect(vw / 2 - 460, panel.y - 70, 920, 150), Loc.T("level") + " " + (lvl.index + 1), 72);
        Text(new Rect(panel.x, panel.y + 105, panel.width, 80), Loc.LevelTitle(lvl.index), 78, Brown, TextAnchor.MiddleCenter, 1f, 0);
        Text(new Rect(panel.x, panel.y + 180, panel.width, 50), Loc.World(lvl.theme), 40, new Color(0.6f, 0.45f, 0.3f), TextAnchor.MiddleCenter, 1f, 0);

        float y = panel.y + 250;
        if (hasNew)
        {
            var bt = lvl.newBird.Value;
            Text(new Rect(panel.x, y, panel.width, 60), Loc.T("new_duck"), 60, new Color(0.85f, 0.3f, 0.15f), TextAnchor.MiddleCenter, 1f, 0);
            var b = art.Bird(bt);
            var sp = Mathf.FloorToInt(Time.unscaledTime * 5f) % 2 == 0 ? b.up : b.down;
            SpriteTex(new Rect(panel.x + 90, y + 80, 260, 200), sp);
            Text(new Rect(panel.x + 400, y + 90, panel.width - 460, 70), Loc.BirdName(bt), 64, Brown, TextAnchor.MiddleLeft, 1f, 0);
            Text(new Rect(panel.x + 400, y + 170, panel.width - 460, 60), Loc.BirdAbility(bt), 38, new Color(0.3f, 0.2f, 0.12f), TextAnchor.MiddleLeft, 1f, 0);
            y += 300;
        }
        else
        {
            // bu bölümdeki kuşlar
            float bx = panel.center.x - lvl.birds.Length * 75;
            for (int i = 0; i < lvl.birds.Length; i++)
                SpriteTex(new Rect(bx + i * 150, y + 10, 130, 100), art.Bird(lvl.birds[i]).idle);
            y += 130;
        }

        if (tierUp)
        {
            int tier = LevelLoader.Tier(lvl.index) + 1;
            int pct = Mathf.RoundToInt((LevelLoader.FoxHealthMult(lvl.index) - 1f) * 100f);
            Text(new Rect(panel.x + 30, y + 4, panel.width - 60, 56), string.Format(Loc.T("tier_up"), tier, pct), 40, new Color(0.8f, 0.25f, 0.15f), TextAnchor.MiddleCenter, 1f, 0);
            y += 70;
        }
        if (hasTip)
        {
            SpriteTex(new Rect(panel.x + 70, y + 8, 120, 92), art.nestGold);
            Text(new Rect(panel.x + 210, y + 10, panel.width - 260, 90), Loc.T(lvl.tip), 38, new Color(0.3f, 0.2f, 0.12f), TextAnchor.MiddleLeft, 1f, 0);
            y += 110;
        }

        float blink = 0.55f + 0.45f * Mathf.Sin(Time.unscaledTime * 5f);
        Text(new Rect(panel.x, y + 10, panel.width, 60), Loc.T("tap_start"), 46, new Color(0.25f, 0.55f, 0.2f), TextAnchor.MiddleCenter, blink, 0);

        if (gm.StateTime > 0.5f && Event.current.type == EventType.MouseUp)
        {
            Sfx.Play(Sfx.Click);
            gm.BeginPlay(true);
            Event.current.Use();
        }
    }

    // ================================================================== OYUN İÇİ
    void DrawHud(GameManager gm, GameArt art)
    {
        bool playing = gm.State == GameManager.GState.Playing;
        if (playing)
        {
            if (Button(PauseRect, art.btnBlue, art.iconPause, null, 0) && !gm.Paused) { Sfx.Play(Sfx.Click); gm.SetPaused(true); }
            if (Button(RestartRect, art.btnOrange, art.iconRestart, null, 0)) gm.Restart();
            if (!gm.Paused)
            {
                if (Button(PlusRect, art.btnGreen, art.iconPlus, null, 0)) OpenPicker();
                int ex = Economy.TotalExtras();
                if (ex > 0) Text(new Rect(PlusRect.xMax - 50, PlusRect.y - 6, 60, 50), ex.ToString(), 40, Gold, TextAnchor.MiddleCenter);
            }
        }

        Rect pill = new Rect(vw - 30 - 440, 26, 440, 110);
        Nine(pill, art.pill, 3, 6f);
        Text(new Rect(pill.x + 30, pill.y, 160, pill.height), Loc.T("score"), 40, Gold, TextAnchor.MiddleLeft);
        Text(new Rect(pill.x, pill.y, pill.width - 30, pill.height), Mathf.RoundToInt(shownScore).ToString("N0"), 70, Color.white, TextAnchor.MiddleRight);
        Text(new Rect(pill.x, pill.yMax + 2, pill.width - 30, 44), Loc.T("best") + "  " + Levels.Best(gm.LevelIndex).ToString("N0"), 34, Cream, TextAnchor.MiddleRight);

        if (gm.Level != null)
        {
            Text(new Rect(vw / 2 - 400, 22, 800, 70), Loc.T("level") + " " + (gm.Level.index + 1) + " - " + Loc.LevelTitle(gm.Level.index), 54, Color.white, TextAnchor.MiddleCenter);
            Text(new Rect(vw / 2 - 400, 82, 800, 50), Loc.T("foxes") + ": " + gm.EnemiesLeft, 40, new Color(1f, 0.7f, 0.4f), TextAnchor.MiddleCenter);
        }

        // kalan kuşlar (sol üstte küçük ikonlar)
        float x = 34;
        if (gm.OnSling != null) { SpriteTex(new Rect(x, 150, 64, 50), art.Bird(gm.OnSling.type).idle); x += 72; }
        foreach (var b in gm.Queue)
        {
            if (b == null) continue;
            SpriteTex(new Rect(x, 150, 64, 50), art.Bird(b.type).idle);
            x += 72;
        }
    }

    void DrawPause(GameManager gm, GameArt art)
    {
        Dim(0.55f);
        if (confirmQuit) { DrawQuitConfirm(gm, art); return; }
        Rect panel = Centered(820, 820, 40);
        Panel(panel);
        Ribbon(new Rect(vw / 2 - 400, panel.y - 70, 800, 150), Loc.T("paused"), 66);
        if (gm.Level != null)
            Text(new Rect(panel.x, panel.y + 96, panel.width, 50), Loc.T("level") + " " + (gm.Level.index + 1) + " - " + Loc.LevelTitle(gm.Level.index), 40, Brown, TextAnchor.MiddleCenter, 1f, 0);

        var items = new (Texture2D bg, Texture2D icon, string label)[]
        {
            (art.btnGreen, art.iconPlay, Loc.T("resume")),
            (art.btnOrange, art.iconRestart, Loc.T("restart_level")),
            (art.btnBlue, art.iconMenu, Loc.T("levels")),
            (art.btnBlue, art.iconHome, Loc.T("main_menu")),
            (art.btnGrey, art.iconExit, Loc.T("quit")),
        };
        float bw = 600, bh = 104, gap = botGap;
        float y0 = panel.y + 160;
        for (int i = 0; i < items.Length; i++)
        {
            Rect r = new Rect(panel.center.x - bw / 2, y0 + i * (bh + gap), bw, bh);
            bool clicked = Button(r, items[i].bg, null, null, 0);
            Tex(new Rect(r.x + 26, r.y + 18, 60, 60), items[i].icon);
            Text(new Rect(r.x + 100, r.y - 4, r.width - 120, r.height * 0.9f), items[i].label, 48, Color.white, TextAnchor.MiddleCenter);
            if (!clicked) continue;
            switch (i)
            {
                case 0: Sfx.Play(Sfx.Click); gm.SetPaused(false); break;
                case 1: gm.Restart(); break;
                case 2: gm.BackToSelect(); break;
                case 3: gm.GoToMainMenu(); break;
                case 4: Sfx.Play(Sfx.Click); confirmQuit = true; break;
            }
        }
    }

    const float botGap = 22f;

    // ================================================================== ÇIKIŞ ONAYI
    bool confirmQuit;

    public void AskQuit() { confirmQuit = true; }

    void DrawQuitConfirm(GameManager gm, GameArt art)
    {
        Rect panel = Centered(820, 420, 30);
        Panel(panel);
        Ribbon(new Rect(vw / 2 - 360, panel.y - 70, 720, 150), Loc.T("quit"), 66);
        Text(new Rect(panel.x, panel.y + 110, panel.width, 70), Loc.T("quit_q"), 48, Brown, TextAnchor.MiddleCenter, 1f, 0);
        Rect yes = new Rect(panel.center.x - 290, panel.y + 220, 260, 120);
        Rect no = new Rect(panel.center.x + 30, panel.y + 220, 260, 120);
        if (Button(yes, art.btnOrange, null, Loc.T("yes"), 60)) { confirmQuit = false; gm.QuitGame(); }
        if (Button(no, art.btnGreen, null, Loc.T("no"), 60)) { Sfx.Play(Sfx.Click); confirmQuit = false; }
    }

    void DrawEnd(GameManager gm, GameArt art, bool won)
    {
        float appear = Mathf.Clamp01(gm.StateTime / 0.35f);
        Dim(0.6f * appear);
        float k = EaseBack(appear);
        Rect panel = Centered(900 * k, 740 * k, 40);
        Panel(panel);
        if (appear < 1f) return;
        Ribbon(new Rect(vw / 2 - 450, panel.y - 80, 900, 170), won ? Loc.T("level_done") : Loc.T("foxes_won"), 70);

        float[] xs = { -240f, 0f, 240f };
        float[] ys = { 110f, 80f, 110f };
        float[] sz = { 190f, 230f, 190f };
        for (int i = 0; i < 3; i++)
        {
            float cx = panel.center.x + xs[i], cy = panel.y + ys[i] + sz[i] * 0.5f;
            Tex(new Rect(cx - sz[i] / 2, cy - sz[i] / 2, sz[i], sz[i]), art.uiStarEmpty);
            if (won && i < gm.Stars)
            {
                float st = (gm.StateTime - 0.45f - i * 0.4f) / 0.3f;
                if (st > 0f)
                {
                    if (st < 0.1f && !starPlayed[i]) { starPlayed[i] = true; Sfx.Play(Sfx.Star, 1f, false); }
                    float sc = st < 1f ? EaseBack(st) : 1f;
                    Tex(new Rect(cx - sz[i] * sc / 2, cy - sz[i] * sc / 2, sz[i] * sc, sz[i] * sc), art.uiStar);
                }
            }
            else starPlayed[i] = false;
        }

        Text(new Rect(panel.x, panel.y + 330, panel.width, 50), Loc.T("score"), 42, Brown, TextAnchor.MiddleCenter, 1f, 0);
        Text(new Rect(panel.x, panel.y + 375, panel.width, 100), gm.Score.ToString("N0"), 96, Gold, TextAnchor.MiddleCenter, 1f, 6);
        if (won)
            Text(new Rect(panel.x, panel.y + 470, panel.width, 50), gm.NewRecord ? Loc.T("new_record") : Loc.T("best") + "  " + Levels.Best(gm.LevelIndex).ToString("N0"),
                 40, gm.NewRecord ? new Color(0.9f, 0.3f, 0.2f) : Brown, TextAnchor.MiddleCenter, 1f, 0);

        if (won && gm.Level != null && Economy.AnyGiftReady(gm.Level.world))
        {
            Tex(new Rect(panel.center.x - 250, panel.y + 588, 44, 44), art.iconGift);
            Text(new Rect(panel.center.x - 200, panel.y + 584, 460, 50), Loc.T("gift_ready"), 38, new Color(0.85f, 0.35f, 0.1f), TextAnchor.MiddleLeft, 1f, 0);
        }

        // yıkım oranı + kazanılan altın yumurta
        float ly = panel.y + 528;
        Text(new Rect(panel.x, ly, panel.width / 2 - 10, 60), Loc.T("destruction") + " " + (Loc.Lang == 0 ? "%" + Mathf.RoundToInt(gm.Destruction * 100f) : Mathf.RoundToInt(gm.Destruction * 100f) + "%"), 44, Brown, TextAnchor.MiddleRight, 1f, 0);
        Tex(new Rect(panel.center.x + 20, ly, 56, 60), art.uiEgg);
        Text(new Rect(panel.center.x + 86, ly, panel.width / 2 - 100, 60), "+" + gm.EggsEarned + (gm.FirstClear ? "  (" + Loc.T("first_clear") + ")" : ""), 48, Gold, TextAnchor.MiddleLeft, 1f, 4);

        if (gm.StateTime < 0.8f) return;
        float bs = 150, by = panel.yMax - bs * 0.6f;
        if (won)
        {
            if (Button(new Rect(panel.center.x - bs * 1.5f - 40, by, bs, bs * 1.05f), art.btnBlue, art.iconMenu, null, 0)) gm.BackToSelect();
            if (Button(new Rect(panel.center.x - bs * 0.5f, by, bs, bs * 1.05f), art.btnOrange, art.iconRestart, null, 0)) gm.Restart();
            if (Button(new Rect(panel.center.x + bs * 0.5f + 40, by, bs, bs * 1.05f), art.btnGreen, art.iconNext, null, 0)) gm.NextLevel();
        }
        else
        {
            float x0 = panel.center.x - (bs * 2 + 320 + 60) / 2;
            if (Button(new Rect(x0, by, bs, bs * 1.05f), art.btnBlue, art.iconMenu, null, 0)) gm.BackToSelect();
            if (Button(new Rect(x0 + bs + 30, by, bs, bs * 1.05f), art.btnOrange, art.iconRestart, null, 0)) gm.Restart();
            if (Button(new Rect(x0 + bs * 2 + 60, by, 320, bs * 1.05f), art.btnGreen, null, "+ " + Loc.T("continue"), 46)) OpenPicker();
        }
    }

    readonly bool[] starPlayed = new bool[3];

    // ================================================================== AÇILIŞ (DuckThisGames)
    void DrawSplash(GameManager gm, GameArt art)
    {
        float t = gm.StateTime;
        const float Dur = 2.8f;
        if (t >= Dur) { gm.FinishSplash(); return; }
        float bgA = t > Dur - 0.4f ? (Dur - t) / 0.4f : 1f;
        float a = Mathf.Clamp01(t / 0.45f) * (t > Dur - 0.6f ? Mathf.Clamp01((Dur - 0.2f - t) / 0.4f) : 1f);

        var old = GUI.color;
        GUI.color = new Color(0.08f, 0.06f, 0.11f, bgA);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);

        // zıplayan, kanat çırpan ördek
        var b = art.Bird(BirdType.Mallard);
        var sp = Mathf.FloorToInt(Time.unscaledTime * 7f) % 2 == 0 ? b.up : b.down;
        float bob = Mathf.Sin(Time.unscaledTime * 4f) * 10f;
        GUI.color = new Color(1f, 1f, 1f, a);
        SpriteTex(new Rect(vw / 2 - 150, 230 + bob, 300, 230), sp);
        GUI.color = old;

        Text(new Rect(0, 480, vw, 150), "DUCKTHIS", 150, Gold, TextAnchor.MiddleCenter, a, 9);
        Text(new Rect(0, 610, vw, 90), "GAMES", 84, Cream, TextAnchor.MiddleCenter, a, 6);
        Text(new Rect(0, 760, vw, 60), Loc.T("dev_by"), 44, new Color(0.75f, 0.72f, 0.8f), TextAnchor.MiddleCenter, a, 0);

        if (t > 0.5f && Event.current.type == EventType.MouseUp) { gm.FinishSplash(); Event.current.Use(); }
    }

    // ================================================================== DÜKKAN
    int shopSel;
    float shopMsgT = -1f;

    void DrawShop(GameManager gm, GameArt art)
    {
        Dim(0.5f);
        Rect panel = new Rect(vw / 2 - 760, 150, 1520, 820);
        Panel(panel);
        Ribbon(new Rect(vw / 2 - 420, 70, 840, 170), Loc.T("shop"), 70);
        EggPill(new Rect(vw - 30 - 340, 30, 340, 110));

        // ördek sekmeleri
        float tw = 200, tg = 20, tx0 = panel.center.x - (tw * 6 + tg * 5) / 2;
        for (int i = 0; i < 6; i++)
        {
            var bt = (BirdType)i;
            bool open = Economy.TypeUnlocked(bt);
            Rect r = new Rect(tx0 + i * (tw + tg), panel.y + 135, tw, 150);
            var bg = i == shopSel ? art.btnOrange : open ? art.btnBlue : art.btnGrey;
            if (Button(r, bg, null, null, 0)) { shopSel = i; Sfx.Play(Sfx.Click); }
            var old = GUI.color;
            if (!open) GUI.color = new Color(0.15f, 0.15f, 0.2f, 0.85f);
            SpriteTex(new Rect(r.x + 30, r.y + 18, r.width - 60, r.height - 50), art.Bird(bt).idle);
            GUI.color = old;
            if (!open) Tex(new Rect(r.center.x - 30, r.y + 40, 60, 60), art.iconLock);
        }

        var t = (BirdType)Mathf.Clamp(shopSel, 0, 5);
        bool unlocked = Economy.TypeUnlocked(t);
        float y0 = panel.y + 320;

        // sol: büyük ördek + isim
        {
            var b = art.Bird(t);
            var sp = Mathf.FloorToInt(Time.unscaledTime * 5f) % 2 == 0 ? b.up : b.down;
            var old = GUI.color;
            if (!unlocked) GUI.color = new Color(0.15f, 0.15f, 0.2f, 0.85f);
            SpriteTex(new Rect(panel.x + 70, y0 + 10 + Mathf.Sin(Time.unscaledTime * 3f) * 6f, 330, 250), sp);
            GUI.color = old;
            Text(new Rect(panel.x + 30, y0 + 275, 410, 64), Loc.BirdName(t), 54, Brown, TextAnchor.MiddleCenter, 1f, 0);
        }

        float rx = panel.x + 470, rw = panel.xMax - 50 - rx;
        if (!unlocked)
        {
            Text(new Rect(rx, y0 + 120, rw, 80), string.Format(Loc.T("unlock_at"), Economy.IntroLevel(t) + 1), 56, Brown, TextAnchor.MiddleCenter, 1f, 0);
        }
        else
        {
            for (int row = 0; row < 3; row++)
            {
                float y = y0 + row * 150;
                string lbl = row == 0 ? Loc.T("damage") : row == 1 ? Loc.T("aim") : Loc.T("extra");
                string desc = row == 0 ? Loc.T("damage_d") : row == 1 ? Loc.T("aim_d") : Loc.T("extra_d");
                Text(new Rect(rx, y, 440, 60), lbl, 52, Brown, TextAnchor.MiddleLeft, 1f, 0);
                Text(new Rect(rx, y + 58, 440, 44), desc, 30, new Color(0.55f, 0.4f, 0.28f), TextAnchor.MiddleLeft, 1f, 0);

                if (row < 2)
                {
                    int lvl = row == 0 ? Economy.DamageLevel(t) : Economy.AimLevel(t);
                    int max = row == 0 ? Economy.MaxDamage : Economy.MaxAim;
                    for (int k = 0; k < max; k++)
                        Pip(new Rect(rx + 460 + k * 58, y + 30, 48, 48), k < lvl, row == 0 ? new Color(0.95f, 0.35f, 0.2f) : new Color(0.3f, 0.75f, 1f));
                }
                else
                    Text(new Rect(rx + 460, y + 10, 270, 80), Loc.T("owned") + ": " + Economy.Extra(t), 48, Brown, TextAnchor.MiddleLeft, 1f, 0);

                int cost = row == 0 ? Economy.DamageCost(t) : row == 1 ? Economy.AimCost(t) : Economy.ExtraCost(t);
                Rect br = new Rect(rx + rw - 250, y + 2, 250, 110);
                if (PriceButton(br, cost))
                {
                    bool ok = row == 0 ? Economy.BuyDamage(t) : row == 1 ? Economy.BuyAim(t) : Economy.BuyExtra(t);
                    if (ok) Sfx.Play(Sfx.Star);
                    else { Sfx.Play(Sfx.Click); shopMsgT = Time.unscaledTime; }
                }
            }
        }

        if (shopMsgT > 0f && Time.unscaledTime - shopMsgT < 1.4f)
            Text(new Rect(panel.x, panel.yMax - 120, panel.width, 70), Loc.T("no_eggs"), 52, new Color(0.95f, 0.3f, 0.2f), TextAnchor.MiddleCenter, 1f, 4);

        if (Button(new Rect(panel.x + 40, panel.yMax - 150, 120, 126), art.btnBlue, art.iconMenu, null, 0)) gm.CloseShop();
    }

    void Pip(Rect r, bool on, Color c)
    {
        var old = GUI.color;
        GUI.color = Ink;
        GUI.DrawTexture(Px(r), Texture2D.whiteTexture);
        GUI.color = on ? c : new Color(0.75f, 0.66f, 0.55f);
        GUI.DrawTexture(Px(new Rect(r.x + 6, r.y + 6, r.width - 12, r.height - 12)), Texture2D.whiteTexture);
        GUI.color = old;
    }

    // Yumurta ikonlu fiyat butonu. cost < 0 ise MAKS. Tıklanınca true (para yetmese de; çağıran kontrol eder).
    bool PriceButton(Rect r, int cost)
    {
        var art = GameArt.I;
        if (cost < 0)
        {
            Button(r, art.btnGrey, null, Loc.T("max"), 52);
            return false;
        }
        bool afford = Economy.Eggs >= cost;
        bool clicked = Button(r, afford ? art.btnGreen : art.btnGrey, null, null, 0);
        float isz = r.height * 0.5f;
        Tex(new Rect(r.x + r.width * 0.14f, r.y + r.height * 0.43f - isz / 2, isz * 0.9f, isz), art.uiEgg);
        Text(new Rect(r.x + r.width * 0.14f + isz, r.y, r.width * 0.86f - isz - 16, r.height * 0.86f), cost.ToString(), 56, Color.white, TextAnchor.MiddleCenter);
        return clicked;
    }

    void EggPill(Rect r)
    {
        var art = GameArt.I;
        Nine(r, art.pill, 3, 6f);
        Tex(new Rect(r.x + 22, r.y + r.height * 0.18f, r.height * 0.58f, r.height * 0.64f), art.uiEgg);
        Text(new Rect(r.x + 30 + r.height * 0.6f, r.y, r.width - 60 - r.height * 0.6f, r.height), Economy.Eggs.ToString("N0"), 64, Gold, TextAnchor.MiddleRight);
    }

    // ================================================================== EKSTRA ÖRDEK SEÇİCİ
    bool picker;

    public void OpenPicker()
    {
        var gm = GameManager.Instance;
        if (gm == null || !gm.CanAddExtra) return;
        Sfx.Play(Sfx.Click);
        picker = true;
        if (gm.State == GameManager.GState.Playing) gm.SetPaused(true);
    }

    void ClosePicker(GameManager gm)
    {
        picker = false;
        if (gm.Paused) gm.SetPaused(false);
    }

    void DrawPicker(GameManager gm, GameArt art)
    {
        Dim(0.55f);
        Rect panel = Centered(1320, 660, 30);
        Panel(panel);
        Ribbon(new Rect(vw / 2 - 420, panel.y - 70, 840, 150), Loc.T("add_duck"), 64);
        EggPill(new Rect(panel.xMax - 370, panel.yMax - 132, 330, 100));

        var types = new List<BirdType>();
        for (int i = 0; i < 6; i++) if (Economy.TypeUnlocked((BirdType)i)) types.Add((BirdType)i);
        float cw = 196, cg = 16, x0 = panel.center.x - (types.Count * cw + (types.Count - 1) * cg) / 2;
        for (int i = 0; i < types.Count; i++)
        {
            var t = types[i];
            float x = x0 + i * (cw + cg), y = panel.y + 110;
            var b = art.Bird(t);
            SpriteTex(new Rect(x + 18, y, cw - 36, 130), Mathf.FloorToInt(Time.unscaledTime * 4f + i) % 2 == 0 ? b.up : b.idle);
            Text(new Rect(x - 6, y + 140, cw + 12, 44), Loc.BirdName(t), 32, Brown, TextAnchor.MiddleCenter, 1f, 0);
            int own = Economy.Extra(t);
            Text(new Rect(x, y + 184, cw, 44), Loc.T("owned") + ": " + own, 34, new Color(0.55f, 0.4f, 0.28f), TextAnchor.MiddleCenter, 1f, 0);
            Rect br = new Rect(x, y + 238, cw, 104);
            bool clicked = own > 0 ? Button(br, art.btnGreen, null, Loc.T("use"), 46) : PriceButton(br, Economy.ExtraCost(t));
            if (clicked)
            {
                if (own <= 0 && Economy.Eggs < Economy.ExtraCost(t)) { Sfx.Play(Sfx.Click); shopMsgT = Time.unscaledTime; continue; }
                bool wasPaused = gm.Paused;
                if (wasPaused) gm.SetPaused(false);
                if (gm.UseExtraBird(t)) { picker = false; }
                else if (wasPaused) gm.SetPaused(true);
            }
        }

        if (shopMsgT > 0f && Time.unscaledTime - shopMsgT < 1.4f)
            Text(new Rect(panel.x, panel.y + 465, panel.width, 60), Loc.T("no_eggs"), 48, new Color(0.95f, 0.3f, 0.2f), TextAnchor.MiddleCenter, 1f, 4);

        if (picker && Button(new Rect(panel.center.x - 80, panel.yMax - 140, 160, 126), art.btnOrange, null, "X", 72)) { Sfx.Play(Sfx.Click); ClosePicker(gm); }
    }

    // ================================================================== GEÇİŞ EFEKTLERİ
    void DrawTransition(GameManager gm)
    {
        float cell = Mathf.Max(24f, 72f * s);
        int cols = Mathf.CeilToInt(Screen.width / cell) + 1;
        int rows = Mathf.CeilToInt(Screen.height / cell) + 1;
        float tt = gm.TransT;
        bool covering = tt < 1f;
        float p = covering ? tt : tt - 1f;
        var old = GUI.color;
        GUI.color = gm.TransColor;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                float th;
                switch (gm.TransStyle)
                {
                    case 0: th = Hash(x, y); break;                                                           // pixel dağılma
                    case 1:
                        float dx = (x + 0.5f) / cols - 0.5f, dy = (y + 0.5f) / rows - 0.5f;
                        th = 1f - Mathf.Clamp01(Mathf.Sqrt(dx * dx * 1.6f + dy * dy) * 1.45f); break;         // iris
                    case 2: th = (y % 2 == 0) ? (float)x / cols : 1f - (float)x / cols; break;                 // jaluzi
                    default: th = (float)(x + y) / (cols + rows); break;                                       // çapraz
                }
                th *= 0.8f;
                float k = covering ? Mathf.Clamp01((p - th) * 5f) : 1f - Mathf.Clamp01((p - th) * 5f);
                if (k <= 0f) continue;
                float sz = cell * k;
                GUI.DrawTexture(new Rect(x * cell + (cell - sz) / 2, y * cell + (cell - sz) / 2, sz + 1, sz + 1), Texture2D.whiteTexture);
            }
        }
        GUI.color = old;
    }

    static float Hash(int x, int y)
    {
        uint h = (uint)(x * 374761393 + y * 668265263);
        h = (h ^ (h >> 13)) * 1274126177;
        return (h & 0xffff) / 65535f;
    }

    // ================================================================== uçan puanlar
    void DrawPopups()
    {
        if (cam == null) return;
        foreach (var p in pops)
        {
            Vector3 sp = cam.WorldToScreenPoint(p.world + Vector3.up * (p.age * 0.9f));
            float t = p.age / 1.3f;
            float pop = t < 0.12f ? Mathf.Lerp(0.3f, 1.25f, t / 0.12f) : Mathf.Lerp(1.25f, 1f, Mathf.Clamp01((t - 0.12f) / 0.15f));
            Color c = p.color;
            float a = t > 0.7f ? 1f - (t - 0.7f) / 0.3f : 1f;
            float gx = sp.x / s, gy = (Screen.height - sp.y) / s;
            Text(new Rect(gx - 200, gy - 50, 400, 100), p.text, Mathf.RoundToInt(p.size * pop), c, TextAnchor.MiddleCenter, a);
        }
    }

    // ================================================================== çizim yardımcıları
    Rect Px(Rect r) => new Rect(r.x * s, r.y * s, r.width * s, r.height * s);
    Rect Centered(float w, float h, float yOff) => new Rect(vw / 2 - w / 2, RefH / 2 - h / 2 + yOff, w, h);

    void Tex(Rect r, Texture2D t, float alpha = 1f)
    {
        if (t == null) return;
        var old = GUI.color;
        GUI.color = new Color(1, 1, 1, alpha);
        GUI.DrawTexture(Px(r), t, ScaleMode.ScaleToFit);
        GUI.color = old;
    }

    void SpriteTex(Rect r, Sprite sp)
    {
        if (sp == null) return;
        GUI.DrawTexture(Px(r), sp.texture, ScaleMode.ScaleToFit);
    }

    // 9 parçalı çizim: köşeler pixel ölçeğinde kalır
    void Nine(Rect r, Texture2D t, int b, float pixel)
    {
        if (t == null) return;
        Rect p = Px(r);
        float bp = b * pixel * s;
        float tw = t.width, th = t.height;
        float[] xs = { p.x, p.x + bp, p.xMax - bp, p.xMax };
        float[] ys = { p.y, p.y + bp, p.yMax - bp, p.yMax };
        // orta parçanın UV'si yarım piksel içeri alınır (kenarlarda ince çizgi oluşmasın)
        float[] us = { 0.1f / tw, (b + 0.5f) / tw, 1 - (b + 0.5f) / tw, 1 - 0.1f / tw };
        float[] vs = { 1 - 0.1f / th, 1 - (b + 0.5f) / th, (b + 0.5f) / th, 0.1f / th };   // GUI y aşağı, UV y yukarı
        for (int j = 0; j < 3; j++)
            for (int i = 0; i < 3; i++)
            {
                var dst = new Rect(xs[i], ys[j], xs[i + 1] - xs[i], ys[j + 1] - ys[j]);
                if (dst.width <= 0 || dst.height <= 0) continue;
                var uv = new Rect(us[i], vs[j + 1], us[i + 1] - us[i], vs[j] - vs[j + 1]);
                GUI.DrawTextureWithTexCoords(dst, t, uv);
            }
    }

    void Panel(Rect r) => Nine(r, GameArt.I.uiPanel, 7, 7f);

    void Dim(float a)
    {
        var old = GUI.color;
        GUI.color = new Color(0.05f, 0.04f, 0.1f, a);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = old;
    }

    void Ribbon(Rect r, string text, int size)
    {
        Nine(r, GameArt.I.uiRibbon, 9, 8f);
        Text(new Rect(r.x, r.y - r.height * 0.08f, r.width, r.height * 0.85f), text, size, Color.white, TextAnchor.MiddleCenter);
    }

    bool Button(Rect r, Texture2D bg, Texture2D icon, string text, int size)
    {
        Rect p = Px(r);
        bool down = p.Contains(Event.current.mousePosition) && GameInput.IsPressed;
        Rect d = down ? new Rect(r.x, r.y + r.height * 0.05f, r.width, r.height * 0.95f) : r;
        Nine(d, bg, 4, Mathf.Min(d.width / 20f, d.height / 21f));
        if (icon != null)
        {
            float isz = Mathf.Min(d.width, d.height) * 0.55f;
            Tex(new Rect(d.center.x - isz / 2, d.y + d.height * 0.43f - isz / 2, isz, isz), icon);
        }
        if (!string.IsNullOrEmpty(text))
            Text(new Rect(d.x, d.y - d.height * 0.05f, d.width, d.height * 0.9f), text, size, Color.white, TextAnchor.MiddleCenter);
        return GUI.Button(p, GUIContent.none, GUIStyle.none);
    }

    void Text(Rect r, string text, int size, Color color, TextAnchor anchor, float alpha = 1f, int outline = 4)
    {
        // Yazı boyutları 4 piksellik adımlara yuvarlanır: her karede farklı boyut istemek
        // dinamik font atlasını sürekli yeniden kurdurup harflerin kaybolmasına yol açıyordu.
        label.fontSize = QuantSize(size * s);
        label.alignment = anchor;
        Rect p = Px(r);
        if (p.width > 4f)
        {
            float w = label.CalcSize(new GUIContent(text)).x;
            if (w > p.width) label.fontSize = QuantSize(label.fontSize * p.width / w, true);
        }
        label.font.RequestCharactersInTexture(text, label.fontSize);
        if (outline > 0)
        {
            float o = Mathf.Max(1f, outline * s);
            label.normal.textColor = new Color(Ink.r, Ink.g, Ink.b, alpha);
            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                    if (i != 0 || j != 0)
                        GUI.Label(new Rect(p.x + i * o, p.y + j * o, p.width, p.height), text, label);
            GUI.Label(new Rect(p.x, p.y + o * 2f, p.width, p.height), text, label);
        }
        color.a = alpha;
        label.normal.textColor = color;
        GUI.Label(p, text, label);
    }

    static int QuantSize(float px, bool down = false)
    {
        int v = Mathf.Max(1, down ? Mathf.FloorToInt(px) : Mathf.RoundToInt(px));
        if (v <= 16) return v;
        return down ? (v / 4) * 4 : Mathf.RoundToInt(v / 4f) * 4;
    }

    static float EaseBack(float t)
    {
        t = Mathf.Clamp01(t);
        const float c1 = 1.70158f, c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
}
