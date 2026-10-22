using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Oyunun beyni: ana menü, bölüm seçimi, geçiş efektleri, bölüm tanıtımı, kuş sırası,
// puan, kazanma/kaybetme ve ilerleme kaydı.
public class GameManager : MonoBehaviour
{
    public enum GState { Splash, Menu, LevelSelect, Shop, Transition, Intro, Playing, Won, Lost }

    public static GameManager Instance { get; private set; }
    public static bool InputBlocked => Instance == null || Instance.State != GState.Playing || Instance.Paused;

    public GState State { get; private set; } = GState.Splash;
    public bool Paused { get; private set; }
    public int LevelIndex { get; private set; }
    public LevelDef Level { get; private set; }
    public int Score { get; private set; }
    public int Stars { get; private set; }
    public int EnemiesLeft { get; private set; }
    public int TwoStar { get; private set; }
    public int ThreeStar { get; private set; }
    public bool NewRecord { get; private set; }
    public int SelectWorld { get; set; }

    // ekonomi
    public int DestroyedScore { get; private set; }
    public float Destruction => LevelLoader.PotentialScore > 0 ? Mathf.Clamp01(DestroyedScore / (float)LevelLoader.PotentialScore) : 0f;
    public int EggsEarned { get; private set; }
    public float DamageHp { get; private set; }          // bu levelde verilen iç hasar
    public int DamagePoints => Mathf.RoundToInt(DamageHp * Economy.DamagePointsPerHp);
    public bool FirstClear { get; private set; }
    GState shopReturn = GState.Menu;

    // geçiş efekti (Hud çizer)
    public float TransT { get; private set; } = -1f;   // 0..1 kapanış, 1..2 açılış
    public int TransStyle { get; private set; }
    public Color TransColor { get; private set; } = Color.black;

    public Transform LeadBird
    {
        get
        {
            foreach (var b in active) if (b != null) return b.transform;
            return null;
        }
    }

    public IReadOnlyList<Bird> Queue => queue;
    public Bird OnSling => onSling;
    public Slingshot Sling => sling;
    public float StateTime { get; private set; }

    readonly List<Bird> queue = new List<Bird>();
    readonly List<Bird> active = new List<Bird>();
    Bird onSling;
    Slingshot sling;
    CameraRig rig;
    Camera cam;
    bool loseCheckRunning;
    Action onCovered, onRevealed;
    int levelToken;   // eski bölümden kalan coroutine'leri iptal etmek için


    // Performans: yüksek çözünürlüklü telefonlarda (1440p ve üstü) pixel-art oyunu tam çözünürlükte çizmek
    // gereksiz GPU yükü. Kısa kenar en fazla 1080 (düşük RAM'li cihazlarda 720) piksele indirilir.
    static void LimitResolution()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        int w = Screen.width, h = Screen.height;
        int shortSide = Mathf.Min(w, h);
        bool lowEnd = SystemInfo.systemMemorySize > 0 && SystemInfo.systemMemorySize < 3000 || SystemInfo.processorCount <= 4;
        int max = lowEnd ? 720 : 1080;
        if (shortSide > max)
        {
            float k = (float)max / shortSide;
            Screen.SetResolution(Mathf.RoundToInt(w * k), Mathf.RoundToInt(h * k), true);
        }
#endif
    }

    void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        LimitResolution();
        Time.timeScale = 1f;
        PlayerProfile.Ensure();
        Levels.MigrateIfNeeded();
        cam = Camera.main;
        rig = cam.GetComponent<CameraRig>();
    }

    void Start()
    {
        sling = Slingshot.Create(new Vector2(LevelLoader.SlingX, LevelLoader.GroundY - 0.05f), LevelLoader.GroundY);
        sling.OnLaunched += OnLaunched;
        LevelIndex = Mathf.Clamp(PlayerPrefs.GetInt("last_level", 0), 0, Levels.Count - 1);
        SelectWorld = LevelIndex / Levels.PerWorld;
        LoadLevel(LevelIndex, false);
        SetState(GState.Splash);

        if (PlayerPrefs.GetInt("autotest", 0) == 1)
        {
            PlayerPrefs.SetInt("autotest", 0);
            SetState(GState.Menu);
            gameObject.AddComponent<AutoTester>();
        }
    }

    void SetState(GState s)
    {
        State = s;
        StateTime = 0f;
    }

    // ================================================================== menü akışı (Hud çağırır)
    public void OpenLevelSelect() { Sfx.Play(Sfx.Click); SetState(GState.LevelSelect); }
    public void OpenMenu() { Sfx.Play(Sfx.Click); SetPaused(false); SetState(GState.Menu); }
    public void FinishSplash() { if (State == GState.Splash) SetState(GState.Menu); }

    public void OpenShop()
    {
        if (State != GState.Menu && State != GState.LevelSelect) return;
        Sfx.Play(Sfx.Click);
        shopReturn = State;
        SetState(GState.Shop);
    }

    public void CloseShop() { Sfx.Play(Sfx.Click); SetState(shopReturn); }

    public void PlayLevel(int index)
    {
        if (!Levels.Unlocked(index)) return;
        Sfx.Play(Sfx.Click);
        SetPaused(false);
        StartTransition(index, () => { LoadLevel(index, true); SetState(GState.Intro); }, null);
    }

    public void Restart()
    {
        Sfx.Play(Sfx.Click);
        SetPaused(false);
        StartTransition(LevelIndex, () => { LoadLevel(LevelIndex, false); }, () => BeginPlay(false));
        SetState(GState.Transition);
    }

    public void NextLevel()
    {
        Sfx.Play(Sfx.Click);
        if (LevelIndex + 1 >= Levels.Count) { SelectWorld = Levels.Worlds - 1; OpenLevelSelect(); return; }
        PlayLevel(LevelIndex + 1);
    }

    public void BackToSelect()
    {
        Sfx.Play(Sfx.Click);
        SetPaused(false);
        SelectWorld = LevelIndex / Levels.PerWorld;
        StartTransition(LevelIndex, () => { LoadLevel(LevelIndex, false); ClearBirds(); }, () => SetState(GState.LevelSelect));
    }

    // Duraklatma menüsünden ana menüye dön
    public void GoToMainMenu()
    {
        Sfx.Play(Sfx.Click);
        SetPaused(false);
        SelectWorld = LevelIndex / Levels.PerWorld;
        StartTransition(LevelIndex, () => { LoadLevel(LevelIndex, false); }, () => SetState(GState.Menu));
    }

    // Oyundan çık (Android'de uygulamayı kapatır, editörde Play modunu durdurur)
    public void QuitGame()
    {
        Sfx.Play(Sfx.Click);
        PlayerPrefs.Save();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SetPaused(bool p)
    {
        if (p && State != GState.Playing) return;
        Paused = p;
        Time.timeScale = p ? 0f : 1f;
    }

    // Tanıtım kartına dokununca
    public void BeginPlay(bool cameraIntro = true)
    {
        SetState(GState.Playing);
        if (cameraIntro) rig.StartIntro(); else rig.SkipIntro();
        StartCoroutine(FirstBirdAfterIntro(levelToken));
    }

    // ================================================================== geçiş
    void StartTransition(int targetLevel, Action covered, Action revealed)
    {
        var th = GameArt.I.themes[Levels.Get(targetLevel).theme];
        TransColor = th.transitionColor;
        TransStyle = targetLevel % 4;
        TransT = 0f;
        onCovered = covered;
        onRevealed = revealed;
        Sfx.Play(Sfx.Whoosh, 0.8f, false);
        SetState(GState.Transition);
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;
        StateTime += dt;

        if (TransT >= 0f)
        {
            float before = TransT;
            TransT += dt / 0.55f;
            if (before < 1f && TransT >= 1f)
            {
                var c = onCovered; onCovered = null;
                c?.Invoke();
                TransT = 1f + 0.0001f;
            }
            if (TransT >= 2f)
            {
                TransT = -1f;
                var r = onRevealed; onRevealed = null;
                r?.Invoke();
                if (State == GState.Transition) SetState(GState.Intro);
            }
        }

        UpdateMusic();

#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (State == GState.Playing) SetPaused(!Paused);
            else if (State == GState.LevelSelect) OpenMenu();
            else if (State == GState.Shop) CloseShop();
            else if (State == GState.Splash) FinishSplash();
            else if (State == GState.Menu && Hud.Instance != null) Hud.Instance.AskQuit();
        }
#endif
    }

    // ================================================================== müzik
    void UpdateMusic()
    {
        switch (State)
        {
            case GState.Splash: Music.Play(Music.Track.None); break;
            case GState.Menu:
            case GState.LevelSelect:
            case GState.Shop: Music.Play(Music.Track.Menu); break;
            case GState.Transition: break;   // geçiş bitince değişir
            default:
                Music.Play(LevelLoader.Tier(LevelIndex) < 12 ? Music.Track.WorldA : Music.Track.WorldB);
                break;
        }
        Music.SetDuck(Paused || State == GState.Lost);
    }

    // ================================================================== bölüm kurma
    void LoadLevel(int index, bool cameraIntro)
    {
        levelToken++;
        StopAllCoroutines();
        loseCheckRunning = false;
        LevelIndex = index;
        Level = Levels.Get(index);
        PlayerPrefs.SetInt("last_level", index);

        ClearBirds();
        LevelLoader.Build(Level, cam);
        EnemiesLeft = LevelLoader.EnemyCount;
        Score = 0;
        Stars = 0;
        NewRecord = false;
        DestroyedScore = 0;
        EggsEarned = 0;
        DamageHp = 0f;
        NestEggs = 0;
        FirstClear = false;

        // yıldız eşikleri: tilki puanları + yıkım + kullanılmayan kuş bonusu
        int enemyScore = 0;
        foreach (var f in Level.foxes)
            enemyScore += f.kind == FoxKind.Kit ? 3000 : f.kind == FoxKind.Helmet ? 7000 : f.kind == FoxKind.Boss ? 15000 : 5000;
        int n = Level.birds.Length;
        TwoStar = Round(enemyScore + LevelLoader.PotentialScore * 0.25f + 10000 * Mathf.Max(0, n - 3));
        ThreeStar = Round(enemyScore + LevelLoader.PotentialScore * 0.45f + 10000 * Mathf.Max(1, n - 2));

        // kuş sırası
        float x = LevelLoader.SlingX - 2.1f;
        for (int i = 0; i < Level.birds.Length; i++)
        {
            var t = Level.birds[i];
            float r = BirdInfo.Get(t).Radius;
            if (i > 0) x -= 0.25f + r;
            var b = Bird.Create(t, new Vector3(x, LevelLoader.GroundY + 0.19f + BirdInfo.Get(t).StandHeight, 0f));
            b.OnFinished += OnBirdFinished;
            queue.Add(b);
            x -= r + 0.35f;
        }

        Vector2 focus = new Vector2(Level.maxX - 4.5f, -1f);
        rig.Configure(-11.5f, Level.maxX, focus, cameraIntro);
    }

    static int Round(float v) => Mathf.RoundToInt(v / 500f) * 500;

    void ClearBirds()
    {
        foreach (var b in queue) if (b != null) Destroy(b.gameObject);
        foreach (var b in active) if (b != null) Destroy(b.gameObject);
        if (onSling != null) Destroy(onSling.gameObject);
        queue.Clear();
        active.Clear();
        onSling = null;
        if (sling != null) sling.ClearBird();
    }

    Vector3 QueuePos(int i)
    {
        float x = LevelLoader.SlingX - 2.1f;
        for (int k = 0; k <= i && k < queue.Count; k++)
        {
            float r = BirdInfo.Get(queue[k].type).Radius;
            if (k > 0) x -= 0.25f + r;
            if (k == i) return new Vector3(x, LevelLoader.GroundY + 0.19f + BirdInfo.Get(queue[k].type).StandHeight, 0f);
            x -= r + 0.35f;
        }
        return new Vector3(x, LevelLoader.GroundY + 0.6f, 0f);
    }

    IEnumerator FirstBirdAfterIntro(int token)
    {
        while (rig != null && !rig.IntroDone) yield return null;
        yield return new WaitForSeconds(0.15f);
        if (token == levelToken) LoadNextBird();
    }

    void LoadNextBird()
    {
        if (State != GState.Playing || onSling != null) return;
        if (queue.Count == 0)
        {
            if (active.Count == 0 && !loseCheckRunning) StartCoroutine(CheckLose(levelToken));
            return;
        }
        Bird next = queue[0];
        queue.RemoveAt(0);
        onSling = next;
        next.HopTo(sling.launchPoint.position, 0.38f, 1.2f, true, () => { if (onSling == next) sling.LoadBird(next); });
        for (int i = 0; i < queue.Count; i++)
            queue[i].HopTo(QueuePos(i), 0.35f, 0.35f, false);
    }

    // ================================================================== kuş olayları
    void OnLaunched(Bird b)
    {
        onSling = null;
        active.Add(b);
        rig.follow = b.transform;
    }

    public void RegisterExtraBird(Bird b)
    {
        active.Add(b);
        b.OnFinished += OnBirdFinished;
    }

    void OnBirdFinished(Bird b)
    {
        active.Remove(b);
        active.RemoveAll(x => x == null);
        if (rig.follow == b.transform) rig.follow = LeadBird;
        if (active.Count == 0 && State == GState.Playing) StartCoroutine(NextAfter(0.15f, levelToken));
    }

    IEnumerator NextAfter(float t, int token)
    {
        yield return new WaitForSeconds(t);
        if (token == levelToken) LoadNextBird();
    }

    IEnumerator CheckLose(int token)
    {
        loseCheckRunning = true;
        yield return new WaitForSeconds(2.5f);
        loseCheckRunning = false;
        if (token != levelToken) yield break;
        if (queue.Count > 0 || onSling != null || active.Count > 0) { LoadNextBird(); yield break; }   // bu arada ekstra ördek eklendi
        if (State == GState.Playing && EnemiesLeft > 0)
        {
            Stars = 0;
            int lose = Economy.LoseReward(DamagePoints);
            Economy.Eggs += lose;
            EggsEarned = lose + NestEggs;
            SetState(GState.Lost);
            Sfx.Play(Sfx.Lose, 1f, false);
        }
    }

    public void EnemyDefeated()
    {
        EnemiesLeft = Mathf.Max(0, EnemiesLeft - 1);
        if (EnemiesLeft == 0 && State == GState.Playing)
        {
            SetState(GState.Won);
            StartCoroutine(WinSequence(levelToken));
        }
    }

    IEnumerator WinSequence(int token)
    {
        yield return new WaitForSeconds(1.5f);
        if (token != levelToken) yield break;

        var remaining = new List<Bird>();
        if (onSling != null) remaining.Add(onSling);
        remaining.AddRange(queue);
        onSling = null;
        queue.Clear();
        sling.ClearBird();
        foreach (var b in remaining)
        {
            if (b == null) continue;
            AddScore(10000, b.transform.position + Vector3.up * 0.7f);
            b.PopForBonus();
            Sfx.Play(Sfx.Star, 0.8f);
            yield return new WaitForSeconds(0.4f);
        }

        Stars = Score >= ThreeStar ? 3 : Score >= TwoStar ? 2 : 1;
        NewRecord = Score > Levels.Best(LevelIndex);
        FirstClear = Levels.Stars(LevelIndex) == 0;
        int win = Economy.WinReward(DamagePoints, FirstClear);
        Economy.Eggs += win;
        EggsEarned = win + NestEggs;
        Levels.Save(LevelIndex, Stars, Score);
        yield return new WaitForSeconds(0.3f);
        Sfx.Play(Sfx.Win, 1f, false);
        StateTime = 0f;   // Hud yıldız animasyonu buradan başlar
        EndShown = true;
    }

    public bool EndShown { get; private set; }

    // Altın yuva kırıldı: her yuva sadece bir kez yumurta verir
    public int NestEggs { get; private set; }
    public void NestBroken(string key, int eggs, Vector3 pos)
    {
        if (string.IsNullOrEmpty(key) || PlayerPrefs.GetInt(key, 0) == 1) return;
        if (State != GState.Playing && State != GState.Won) return;
        PlayerPrefs.SetInt(key, 1);
        Economy.Eggs += eggs;
        NestEggs += eggs;
        Hud.Popup(pos + Vector3.up * 0.5f, "+" + eggs + " " + Loc.T("egg_short"), new Color(1f, 0.85f, 0.2f), 60);
        Sfx.Play(Sfx.Star, 1f, false);
        Fx.Stars(pos, 8);
    }

    public void AddDamage(float hp)
    {
        if (State == GState.Playing || State == GState.Won) DamageHp += hp;
    }

    public void BlockDestroyed(int value)
    {
        if (State == GState.Playing || State == GState.Won) DestroyedScore += value;
    }

    // ================================================================== ekstra ördek (dükkândan)
    public bool CanAddExtra => State == GState.Playing || State == GState.Lost;

    // Envanterdeki ördeği sıraya ekler; yoksa ve yumurta yetiyorsa satın alıp ekler.
    public bool UseExtraBird(BirdType t)
    {
        if (!CanAddExtra) return false;
        if (!Economy.UseExtra(t))
        {
            if (!Economy.BuyExtra(t) || !Economy.UseExtra(t)) return false;
        }
        bool revive = State == GState.Lost;
        if (revive)
        {
            SetState(GState.Playing);
            EndShown = false;
        }
        var b = Bird.Create(t, QueuePos(queue.Count) + Vector3.left * 3f);
        b.OnFinished += OnBirdFinished;
        queue.Add(b);
        b.HopTo(QueuePos(queue.Count - 1), 0.4f, 1.2f, false);
        Fx.Puff(b.transform.position, 0.6f, 5);
        Sfx.Play(Sfx.Star, 0.8f);
        if (onSling == null && active.Count == 0) StartCoroutine(NextAfter(0.45f, levelToken));
        return true;
    }

    public void AddScore(int amount, Vector3 worldPos)
    {
        if (amount <= 0) return;
        Score += amount;
        Color c = amount >= 5000 ? new Color(1f, 0.85f, 0.2f) : Color.white;
        Hud.Popup(worldPos, amount.ToString(), c, amount >= 5000 ? 58 : 40);
    }

    // ================================================================== otomatik test için
    public bool AutoLaunch(Vector2 velocity)
    {
        if (State != GState.Playing || onSling == null || !onSling.IsReady || !rig.IntroDone) return false;
        return sling.AutoLaunch(velocity);
    }

    public void DebugStart(int index, bool play)
    {
        TransT = -1f;
        onCovered = null; onRevealed = null;
        Paused = false;
        LoadLevel(index, false);
        BeginPlay(false);
    }

    void LateUpdate()
    {
        if (State != GState.Won) EndShown = State == GState.Lost;
    }
}
