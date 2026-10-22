using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

// Projeyi kurar: pixel sprite ayarları, GameArt, Game sahnesi, oyuncu/Android ayarları.
// Ayrıca ProjectSettings/ordek_cmd.txt dosyasındaki komutları çalıştırır (uzaktan test için):
//   refresh | rebuild | play | stop | autotest | level N | shot | android | unlockall | reset
[InitializeOnLoad]
public static class GameBuilder
{
    const int Version = 19;
    const string VersionFile = "ProjectSettings/OrdekOyunuBuilder.txt";
    const string CmdFile = "ProjectSettings/ordek_cmd.txt";
    const string ScenePath = "Assets/Scenes/Game.unity";
    const string PixelDir = "Assets/Art/Pixel";
    const string SoundDir = "Assets/Art/Sounds";
    const string FontPath = "Assets/Art/Fonts/Jersey10-Regular.ttf";
    const string IconPath = "Assets/Art/icon.png";
    const string ArtAssetPath = "Assets/Resources/GameArt.asset";
    const string ApkPath = "Builds/DuckThis_FeatherFury.apk";

    static double nextPoll;

    static GameBuilder()
    {
        EditorApplication.update += Poll;
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (File.Exists(ScenePath) && ReadVersion() >= Version) return;
            BuildAll();
        };
    }

    static int ReadVersion()
    {
        try { return File.Exists(VersionFile) ? int.Parse(File.ReadAllText(VersionFile).Trim()) : 0; }
        catch { return 0; }
    }

    // ================================================================== menü
    [MenuItem("Ördek Oyunu/Oyunu Yeniden Kur")]
    public static void BuildAll()
    {
        if (!File.Exists(PixelDir + "/bird_duck_idle.png"))
        {
            Debug.LogError("Ördek Oyunu: pixel çizimler bulunamadı (" + PixelDir + ")");
            return;
        }
        EnsureFolder("Assets/Scenes");
        EnsureFolder("Assets/Resources");
        ConfigureImports();
        var art = CreateGameArt();
        CreateScene();
        ConfigurePlayer();
        AssetDatabase.SaveAssets();
        File.WriteAllText(VersionFile, Version.ToString());
        Debug.Log("Ördek Oyunu: kurulum tamam (sürüm " + Version + "). Game sahnesinde Play'e bas!");
    }

    [MenuItem("Ördek Oyunu/Bölümleri Otomatik Test Et")]
    public static void AutoTest()
    {
        PlayerPrefs.SetInt("autotest", 1);
        PlayerPrefs.Save();
        if (!EditorApplication.isPlaying)
        {
            EditorSceneManager.OpenScene(ScenePath);
            EditorApplication.EnterPlaymode();
        }
        else Debug.Log("Ördek Oyunu: Play modundan çıkıp tekrar gir, test başlayacak.");
    }

    [MenuItem("Ördek Oyunu/Tüm Bölümlerin Kilidini Aç")]
    public static void UnlockAll()
    {
        PlayerPrefs.SetInt("unlock_all", PlayerPrefs.GetInt("unlock_all", 0) == 1 ? 0 : 1);
        PlayerPrefs.Save();
        Debug.Log("Ördek Oyunu: tüm bölümler " + (PlayerPrefs.GetInt("unlock_all") == 1 ? "açık" : "normal kilitli"));
    }

    [MenuItem("Ördek Oyunu/İlerlemeyi Sıfırla")]
    public static void ResetProgress()
    {
        for (int i = 0; i < Levels.Count; i++) { PlayerPrefs.DeleteKey("stars_" + i); PlayerPrefs.DeleteKey("best_" + i); }
        foreach (var k in Levels.NestKeys()) PlayerPrefs.DeleteKey(k);
        PlayerPrefs.DeleteKey("last_level");
        PlayerPrefs.DeleteKey("unlock_all");
        Economy.ResetAll();
        PlayerPrefs.Save();
        Debug.Log("Ördek Oyunu: ilerleme sıfırlandı");
    }

    [MenuItem("Ördek Oyunu/Android APK Al")]
    public static void BuildAndroid()
    {
        string result;
        try
        {
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android))
            {
                result = "HATA: Android Build Support modülü kurulu değil.";
            }
            else
            {
                ConfigurePlayer();
                Directory.CreateDirectory("Builds");
                EditorUserBuildSettings.buildAppBundle = false;
                var opts = new BuildPlayerOptions
                {
                    scenes = new[] { ScenePath },
                    locationPathName = ApkPath,
                    target = BuildTarget.Android,
                    targetGroup = BuildTargetGroup.Android,
                    options = BuildOptions.None
                };
                var report = BuildPipeline.BuildPlayer(opts);
                var sum = report.summary;
                bool ok = sum.result == BuildResult.Succeeded && File.Exists(ApkPath) && File.GetLastWriteTime(ApkPath) > DateTime.Now.AddMinutes(-30);
                result = ok
                    ? $"TAMAM: {ApkPath} ({new FileInfo(ApkPath).Length / (1024 * 1024f):F1} MB, {sum.totalTime.TotalSeconds:F0} sn)"
                    : $"HATA: APK oluşmadı ({sum.result}). Android modülü yeni kurulduysa Unity'yi kapatıp yeniden aç. Ayrıntı Editor.log içinde.";
            }
        }
        catch (Exception e)
        {
            result = "HATA: " + e.Message;
        }
        File.WriteAllText("Logs/build_result.txt", DateTime.Now + "\n" + result);
        Debug.Log("Ördek Oyunu Android: " + result);
    }

    // ================================================================== uzaktan komutlar
    static float F(string v) => float.Parse(v, System.Globalization.CultureInfo.InvariantCulture);

    static void Poll()
    {
        if (EditorApplication.timeSinceStartup < nextPoll) return;
        nextPoll = EditorApplication.timeSinceStartup + 0.5;
        if (!File.Exists(CmdFile)) return;
        string cmd;
        try { cmd = File.ReadAllText(CmdFile).Trim(); File.Delete(CmdFile); }
        catch { return; }
        if (string.IsNullOrEmpty(cmd)) return;
        Debug.Log("Ördek Oyunu komut: " + cmd);
        var parts = cmd.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        try
        {
            switch (parts[0])
            {
                case "refresh": AssetDatabase.Refresh(); break;
                case "rebuild": if (!EditorApplication.isPlaying) BuildAll(); break;
                case "play":
                    if (!EditorApplication.isPlaying) { EditorSceneManager.OpenScene(ScenePath); EditorApplication.EnterPlaymode(); }
                    break;
                case "stop": if (EditorApplication.isPlaying) EditorApplication.ExitPlaymode(); break;
                case "autotest":
                    PlayerPrefs.SetInt("autotest_from", parts.Length > 1 ? int.Parse(parts[1]) : 1);
                    PlayerPrefs.SetInt("autotest_to", parts.Length > 2 ? int.Parse(parts[2]) : Levels.Count);
                    PlayerPrefs.SetInt("autotest_trials", parts.Length > 3 ? int.Parse(parts[3]) : 4);
                    PlayerPrefs.SetInt("autotest_upg", parts.Length > 4 ? int.Parse(parts[4]) : 0);
                    if (EditorApplication.isPlaying) { PlayerPrefs.SetInt("autotest", 1); EditorApplication.ExitPlaymode(); EditorApplication.delayCall += AutoTest; }
                    else AutoTest();
                    break;
                case "eggs": Economy.Eggs = int.Parse(parts[1]); break;
                case "stars":   // stars bölüm sayı  -> o bölümün ilk N levelini 3 yıldızlı yap (test)
                    {
                        int w = int.Parse(parts[1]) - 1, n = int.Parse(parts[2]);
                        for (int k = 0; k < Levels.PerWorld; k++) PlayerPrefs.SetInt("stars_" + (w * Levels.PerWorld + k), k < n ? 3 : 0);
                        PlayerPrefs.Save();
                    }
                    break;
                case "lang": { int li; if (int.TryParse(parts[1], out li)) Loc.Set(Loc.Codes[Mathf.Clamp(li, 0, Loc.Codes.Length - 1)]); else Loc.Set(parts[1]); } break;
                case "aimshot":
                    if (GameManager.Instance != null)
                        GameManager.Instance.AutoLaunch(AutoTester.AimAtFox(GameManager.Instance, parts.Length > 1 ? int.Parse(parts[1]) : 0, out _));
                    break;
                case "rec":      // rec ad saniye
                    if (EditorApplication.isPlaying)
                        DevRecorder.Record(parts[1], float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture));
                    break;
                case "recshot":  // recshot ad saniye gecikme tnt|fox yetenekZamanı
                    if (EditorApplication.isPlaying)
                        DevRecorder.RecordShot(parts[1], F(parts[2]), F(parts[3]), parts[4], parts.Length > 5 ? F(parts[5]) : 0f, parts.Length > 6 ? F(parts[6]) : 0f);
                    break;
                case "setp": PlayerPrefs.SetInt(parts[1], int.Parse(parts[2])); PlayerPrefs.Save(); break;
                case "maxgame":  // Game penceresini tam ekran yap/geri al
                    {
                        var gvType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
                        var gv = EditorWindow.GetWindow(gvType);
                        gv.maximized = parts.Length < 2 || parts[1] != "off";
                    }
                    break;
                case "unlockall": PlayerPrefs.SetInt("unlock_all", 1); PlayerPrefs.Save(); break;
                case "reset": ResetProgress(); break;
                case "android": BuildAndroid(); break;
                case "level":
                    if (EditorApplication.isPlaying && GameManager.Instance != null)
                        GameManager.Instance.DebugStart(int.Parse(parts[1]) - 1, true);
                    break;
                case "ability":
                    if (GameManager.Instance != null && GameManager.Instance.LeadBird != null)
                        GameManager.Instance.LeadBird.GetComponent<Bird>().TestAbility();
                    break;
                case "egg":
                    EggBomb.Drop(new Vector2(float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture),
                                             float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture)), new Vector2(0f, -9f));
                    break;
                case "state":
                    if (GameManager.Instance != null)
                    {
                        var gm = GameManager.Instance;
                        File.WriteAllText("Logs/state.txt", $"{gm.State} level={gm.LevelIndex + 1} score={gm.Score} foxes={gm.EnemiesLeft} stars={gm.Stars}");
                    }
                    break;
                case "shot":
                    string name = parts.Length > 1 ? parts[1] : "shot";
                    ScreenCapture.CaptureScreenshot("Logs/" + name + ".png");
                    break;
                case "launch":   // launch vx vy  (otomatik atış)
                    if (GameManager.Instance != null)
                        GameManager.Instance.AutoLaunch(new Vector2(float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture),
                                                                    float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture)));
                    break;
                case "ui":       // ui menu|select|play
                    if (GameManager.Instance != null)
                    {
                        var gm = GameManager.Instance;
                        if (parts[1] == "menu") gm.OpenMenu();
                        else if (parts[1] == "select") gm.OpenLevelSelect();
                        else if (parts[1] == "play") gm.PlayLevel(int.Parse(parts[2]) - 1);
                        else if (parts[1] == "begin") gm.BeginPlay(true);
                        else if (parts[1] == "pause") gm.SetPaused(true);
                        else if (parts[1] == "resume") gm.SetPaused(false);
                        else if (parts[1] == "shop") gm.OpenShop();
                        else if (parts[1] == "skip") gm.FinishSplash();
                        else if (parts[1] == "extra") gm.UseExtraBird((BirdType)int.Parse(parts[2]));
                        else if (parts[1] == "picker" && Hud.Instance != null) Hud.Instance.OpenPicker();
                        else if (parts[1] == "langs" && Hud.Instance != null) { gm.OpenMenu(); Hud.Instance.OpenLangPicker(); }
                    }
                    break;
            }
        }
        catch (Exception e) { Debug.LogError("Komut hatası: " + e); }
    }

    // ================================================================== içe aktarma
    static void ConfigureImports()
    {
        AssetDatabase.Refresh();
        AssetDatabase.StartAssetEditing();
        try
        {
            foreach (var file in Directory.GetFiles(PixelDir, "*.png"))
            {
                string path = file.Replace('\\', '/');
                string name = Path.GetFileNameWithoutExtension(path);
                var imp = AssetImporter.GetAtPath(path) as TextureImporter;
                if (imp == null) continue;

                Vector2 pivot = new Vector2(0.5f, 0.5f);
                if (name.StartsWith("far_") || name.StartsWith("near_") || name.StartsWith("decor") || name.StartsWith("bush_") || name.StartsWith("sling_"))
                    pivot = new Vector2(0.5f, 0f);
                float border = name.StartsWith("block_") && name != "block_tnt" ? 3f : 0f;

                imp.textureType = TextureImporterType.Sprite;
                imp.spriteImportMode = SpriteImportMode.Single;
                imp.spritePixelsPerUnit = 16f;
                imp.mipmapEnabled = false;
                imp.alphaIsTransparency = true;
                imp.filterMode = FilterMode.Point;
                imp.wrapMode = TextureWrapMode.Clamp;
                imp.textureCompression = TextureImporterCompression.Uncompressed;
                imp.npotScale = TextureImporterNPOTScale.None;
                imp.spriteBorder = new Vector4(border, border, border, border);

                var st = new TextureImporterSettings();
                imp.ReadTextureSettings(st);
                st.spriteMeshType = SpriteMeshType.FullRect;
                st.spriteAlignment = (int)SpriteAlignment.Custom;
                st.spritePivot = pivot;
                st.spriteGenerateFallbackPhysicsShape = false;
                imp.SetTextureSettings(st);
                imp.SaveAndReimport();
            }

            foreach (var file in Directory.GetFiles(SoundDir, "*.wav"))
            {
                var ai = AssetImporter.GetAtPath(file.Replace('\\', '/')) as AudioImporter;
                if (ai == null) continue;
                ai.forceToMono = true;
                var ss = ai.defaultSampleSettings;
                bool isMusic = Path.GetFileName(file).StartsWith("music_");
                ss.loadType = isMusic ? AudioClipLoadType.CompressedInMemory : AudioClipLoadType.DecompressOnLoad;
                ss.compressionFormat = isMusic ? AudioCompressionFormat.Vorbis : AudioCompressionFormat.ADPCM;
                ss.quality = isMusic ? 0.45f : 1f;
                ai.defaultSampleSettings = ss;
                ai.SaveAndReimport();
            }

            var fi = AssetImporter.GetAtPath(FontPath) as TrueTypeFontImporter;
            if (fi != null)
            {
                fi.fontRenderingMode = FontRenderingMode.HintedRaster;
                fi.SaveAndReimport();
            }

            var ii = AssetImporter.GetAtPath(IconPath) as TextureImporter;
            if (ii != null)
            {
                ii.textureType = TextureImporterType.Default;
                ii.filterMode = FilterMode.Point;
                ii.mipmapEnabled = false;
                ii.textureCompression = TextureImporterCompression.Uncompressed;
                ii.SaveAndReimport();
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }
        AssetDatabase.Refresh();
    }

    static Sprite Sp(string name)
    {
        var s = AssetDatabase.LoadAssetAtPath<Sprite>($"{PixelDir}/{name}.png");
        if (s == null) Debug.LogWarning("Sprite yok: " + name);
        return s;
    }

    static Texture2D Tx(string name) => AssetDatabase.LoadAssetAtPath<Texture2D>($"{PixelDir}/{name}.png");
    static AudioClip Au(string name) => AssetDatabase.LoadAssetAtPath<AudioClip>($"{SoundDir}/{name}.wav");

    static GameArt CreateGameArt()
    {
        var art = AssetDatabase.LoadAssetAtPath<GameArt>(ArtAssetPath);
        if (art == null)
        {
            art = ScriptableObject.CreateInstance<GameArt>();
            AssetDatabase.CreateAsset(art, ArtAssetPath);
        }

        string[] birdNames = { "duck", "mandarin", "merganser", "muscovy", "scoter", "shelduck", "pintail", "eider", "goldeneye" };
        art.birds = new GameArt.BirdArt[birdNames.Length];
        for (int i = 0; i < birdNames.Length; i++)
        {
            string b = "bird_" + birdNames[i] + "_";
            art.birds[i] = new GameArt.BirdArt { idle = Sp(b + "idle"), blink = Sp(b + "blink"), up = Sp(b + "up"), down = Sp(b + "down"), hit = Sp(b + "hit") };
        }
        art.fox = new GameArt.FoxArt { idle = Sp("fox_idle"), blink = Sp("fox_blink"), scared = Sp("fox_scared"), hurt = Sp("fox_hurt") };
        art.foxHelmet = new GameArt.FoxArt { idle = Sp("foxh_idle"), blink = Sp("foxh_blink"), scared = Sp("foxh_scared"), hurt = Sp("foxh_hurt") };

        art.wood = Sp("block_wood"); art.stone = Sp("block_stone"); art.ice = Sp("block_ice"); art.tnt = Sp("block_tnt");
        art.boulder = Sp("boulder"); art.egg = Sp("egg"); art.nestGold = Sp("nest_gold"); art.nestEmpty = Sp("nest_empty"); art.crack1 = Sp("crack_1"); art.crack2 = Sp("crack_2");
        art.slingBack = Sp("sling_back"); art.slingFront = Sp("sling_front"); art.pouch = Sp("pouch");

        Color[] trans =
        {
            new Color(0.07f, 0.15f, 0.27f), new Color(0.24f, 0.1f, 0.05f),
            new Color(0.14f, 0.24f, 0.36f), new Color(0.09f, 0.06f, 0.17f),
            new Color(0.36f, 0.17f, 0.08f), new Color(0.03f, 0.27f, 0.38f),
            new Color(0.32f, 0.2f, 0.36f), new Color(0.13f, 0.03f, 0.04f),
            new Color(0.08f, 0.16f, 0.13f), new Color(0.07f, 0.07f, 0.2f),
            // 11-25. bölümler (temel temaların gece / kızıl / altın / fırtına / şafak çeşitlemeleri)
            new Color(0.06f, 0.08f, 0.16f), new Color(0.27f, 0.11f, 0.10f), new Color(0.06f, 0.08f, 0.16f),
            new Color(0.43f, 0.35f, 0.20f), new Color(0.06f, 0.08f, 0.16f), new Color(0.29f, 0.17f, 0.19f),
            new Color(0.06f, 0.08f, 0.16f), new Color(0.20f, 0.04f, 0.04f), new Color(0.17f, 0.19f, 0.21f),
            new Color(0.34f, 0.29f, 0.35f), new Color(0.17f, 0.19f, 0.21f), new Color(0.34f, 0.29f, 0.35f),
            new Color(0.14f, 0.35f, 0.40f), new Color(0.29f, 0.17f, 0.19f), new Color(0.14f, 0.06f, 0.13f)
        };
        var starry = new HashSet<int> { 3, 7, 8, 9, 10, 12, 14, 16, 17, 18 };
        art.themes = new GameArt.ThemeArt[trans.Length];
        for (int i = 0; i < trans.Length; i++)
        {
            art.themes[i] = new GameArt.ThemeArt
            {
                sky = Sp("sky_" + i), far = Sp("far_" + i), near = Sp("near_" + i), ground = Sp("ground_" + i),
                water = Sp("water_" + i), dirt = Sp("dirt_" + i), sun = Sp("sun_" + i),
                cloudA = Sp("cloudA_" + i), cloudB = Sp("cloudB_" + i), bush = Sp("bush_" + i),
                decor0 = Sp("decor0_" + i), decor1 = Sp("decor1_" + i), decor2 = Sp("decor2_" + i),
                transitionColor = trans[i], stars = starry.Contains(i)
            };
        }
        art.starPx = Sp("star_px");
        art.fxDot = Sp("fx_dot"); art.fxPuff = Sp("fx_puff"); art.fxPixel = Sp("fx_pixel");
        art.fxFeather = Sp("fx_feather"); art.fxStar = Sp("fx_star"); art.fxBoom = Sp("fx_boom");

        art.font = AssetDatabase.LoadAssetAtPath<Font>(FontPath);
        art.uiPanel = Tx("ui_panel"); art.uiRibbon = Tx("ui_ribbon"); art.uiStar = Tx("ui_star"); art.uiStarEmpty = Tx("ui_star_empty");
        art.btnGreen = Tx("ui_btn_green"); art.btnOrange = Tx("ui_btn_orange"); art.btnBlue = Tx("ui_btn_blue"); art.btnGrey = Tx("ui_btn_grey");
        art.pill = Tx("ui_pill");
        art.iconRestart = Tx("icon_restart"); art.iconPause = Tx("icon_pause"); art.iconPlay = Tx("icon_play");
        art.iconNext = Tx("icon_next"); art.iconMenu = Tx("icon_menu"); art.iconLock = Tx("icon_lock");
        art.iconHome = Tx("icon_home"); art.iconExit = Tx("icon_exit");
        art.iconPlus = Tx("icon_plus"); art.iconGift = Tx("icon_gift"); art.iconShop = Tx("icon_shop"); art.uiEgg = Tx("ui_egg");

        art.sLaunch = Au("sfx_launch"); art.sFlap = Au("sfx_flap"); art.sPop = Au("sfx_pop"); art.sDash = Au("sfx_dash");
        art.sHit = Au("sfx_hit"); art.sThud = Au("sfx_thud"); art.sWood = Au("sfx_wood"); art.sStone = Au("sfx_stone");
        art.sIce = Au("sfx_ice"); art.sBoom = Au("sfx_boom"); art.sFox = Au("sfx_fox"); art.sClick = Au("sfx_click");
        art.sStar = Au("sfx_star"); art.sStretch = Au("sfx_stretch"); art.sWin = Au("sfx_win"); art.sLose = Au("sfx_lose");
        art.sWhoosh = Au("sfx_whoosh");
        string[] duckNames = birdNames;   // BirdType sırası
        art.duckVoice = new AudioClip[duckNames.Length]; art.duckAbility = new AudioClip[duckNames.Length]; art.duckHit = new AudioClip[duckNames.Length];
        for (int i = 0; i < duckNames.Length; i++)
        {
            art.duckVoice[i] = Au("duck_voice_" + duckNames[i]);
            art.duckAbility[i] = Au("duck_ability_" + duckNames[i]);
            art.duckHit[i] = Au("duck_hit_" + duckNames[i]);
        }
        art.musicMenu = Au("music_menu"); art.musicWorld = Au("music_world"); art.musicWorld2 = Au("music_world2");

        EditorUtility.SetDirty(art);
        AssetDatabase.SaveAssets();
        return art;
    }

    static void CreateScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        var cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 6f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.35f, 0.65f, 0.93f);
        camGo.transform.position = new Vector3(0, 0, -10);
        camGo.AddComponent<AudioListener>();
        camGo.AddComponent<CameraRig>();

        var gm = new GameObject("GameManager");
        gm.AddComponent<GameManager>();
        gm.AddComponent<Hud>();

        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
    }

    static void ConfigurePlayer()
    {
        PlayerSettings.productName = "DuckThis Feather Fury";
        PlayerSettings.companyName = "DuckThisGames";
        PlayerSettings.bundleVersion = "1.0";
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android, "com.duckthisgames.featherfury");
        PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        // Uyumluluk: 32 bit (ARMv7, ör. Android Go telefonlar) ve 64 bit ARM;
        // Android 8.0 (API 26, Unity 6.6nın desteklediği en eski sürüm) ve üstü.
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
        PlayerSettings.Android.bundleVersionCode = 4;
        // Grafik: önce OpenGL ES 3 (her cihazda kararlı; bazı ucuz cihazlarda Vulkan sürücüleri çöküyor), yoksa Vulkan
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3, UnityEngine.Rendering.GraphicsDeviceType.Vulkan });
        PlayerSettings.Android.optimizedFramePacing = true;
        PlayerSettings.Android.forceInternetPermission = false;
        PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;
        PlayerSettings.SetManagedStrippingLevel(UnityEditor.Build.NamedBuildTarget.Android, ManagedStrippingLevel.Low);
        PlayerSettings.MTRendering = true;

        var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath);
        if (icon != null)
            PlayerSettings.SetIcons(UnityEditor.Build.NamedBuildTarget.Unknown, new[] { icon }, IconKind.Any);
    }

    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = Path.GetDirectoryName(path).Replace('\\', '/');
        if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
    }
}
