using UnityEngine;

// Oyunun tüm pixel-art görsellerine tek yerden erişim.
// Assets/Resources/GameArt.asset dosyasından yüklenir (GameBuilder otomatik doldurur).
public class GameArt : ScriptableObject
{
    static GameArt instance;
    public static GameArt I
    {
        get
        {
            if (instance == null) instance = Resources.Load<GameArt>("GameArt");
            return instance;
        }
    }

    [System.Serializable]
    public class BirdArt
    {
        public Sprite idle, blink, up, down, hit;
    }

    [System.Serializable]
    public class FoxArt
    {
        public Sprite idle, blink, scared, hurt;
    }

    [System.Serializable]
    public class ThemeArt
    {
        public Sprite sky, far, near, ground, water, dirt, sun, cloudA, cloudB, bush, decor0, decor1, decor2;
        public Color transitionColor = Color.black;
        public bool stars;
    }

    [Header("Kuşlar (BirdType sırasıyla)")]
    public BirdArt[] birds = new BirdArt[6];

    [Header("Tilkiler")]
    public FoxArt fox = new FoxArt();
    public FoxArt foxHelmet = new FoxArt();

    [Header("Bloklar")]
    public Sprite wood, stone, ice, tnt, boulder, crack1, crack2, egg, nestGold, nestEmpty;

    [Header("Sapan")]
    public Sprite slingBack, slingFront, pouch;

    [Header("Temalar")]
    public ThemeArt[] themes = new ThemeArt[8];
    public Sprite starPx;

    [Header("Efektler")]
    public Sprite fxDot, fxPuff, fxPixel, fxFeather, fxStar, fxBoom;

    [Header("Arayüz")]
    public Font font;
    public Texture2D uiPanel, uiRibbon, uiStar, uiStarEmpty, btnGreen, btnOrange, btnBlue, btnGrey, pill;
    public Texture2D iconRestart, iconPause, iconPlay, iconNext, iconMenu, iconLock, iconHome, iconExit, iconPlus, iconShop, uiEgg;

    [Header("Sesler")]
    public AudioClip[] duckVoice = new AudioClip[6], duckAbility = new AudioClip[6], duckHit = new AudioClip[6];
    public AudioClip musicMenu, musicWorld, musicWorld2;
    public AudioClip sLaunch, sFlap, sPop, sDash, sHit, sThud, sWood, sStone, sIce, sBoom, sFox, sClick, sStar, sStretch, sWin, sLose, sWhoosh;

    public BirdArt Bird(BirdType t) => birds[(int)t];
}
