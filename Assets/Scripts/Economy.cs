using UnityEngine;

// Altın Yumurta ekonomisi: tilkilerden geri alınan altın yumurtalar para birimidir.
// Bölüm sonunda yıldıza ve yıkım oranına göre kazanılır; dükkânda her ördek türü için
// Hasar ve Otomatik Nişan yükseltmeleri ile ekstra ördek alınır. Hepsi PlayerPrefs'te saklanır.
public static class Economy
{
    public const int MaxDamage = 4;
    public const int MaxAim = 3;
    public const int TypeCount = 6;

    static readonly int[] DamageCosts = { 40, 80, 140, 220 };
    static readonly int[] AimCosts = { 60, 120, 200 };
    static readonly int[] ExtraCosts = { 20, 25, 25, 30, 35, 30 };   // BirdType sırasıyla

    public static int Eggs
    {
        get => PlayerPrefs.GetInt("eggs", 0);
        set { PlayerPrefs.SetInt("eggs", Mathf.Max(0, value)); PlayerPrefs.Save(); }
    }

    public static int DamageLevel(BirdType t) => Mathf.Clamp(PlayerPrefs.GetInt("dmg_" + (int)t, 0), 0, MaxDamage);
    public static int AimLevel(BirdType t) => Mathf.Clamp(PlayerPrefs.GetInt("aim_" + (int)t, 0), 0, MaxAim);
    public static int Extra(BirdType t) => Mathf.Max(0, PlayerPrefs.GetInt("extra_" + (int)t, 0));

    // oyun içi etkiler
    public static float DamageMult(BirdType t) => 1f + 0.15f * DamageLevel(t);
    public static float AimRate(BirdType t)                        // derece / saniye
    {
        switch (AimLevel(t)) { case 1: return 25f; case 2: return 45f; case 3: return 70f; default: return 0f; }
    }

    public static int DamageCost(BirdType t) { int l = DamageLevel(t); return l < MaxDamage ? DamageCosts[l] : -1; }
    public static int AimCost(BirdType t) { int l = AimLevel(t); return l < MaxAim ? AimCosts[l] : -1; }
    public static int ExtraCost(BirdType t) => ExtraCosts[(int)t];

    public static int TotalExtras()
    {
        int n = 0;
        for (int i = 0; i < TypeCount; i++) n += Extra((BirdType)i);
        return n;
    }

    static bool Spend(int cost)
    {
        if (cost < 0 || Eggs < cost) return false;
        Eggs -= cost;
        return true;
    }

    public static bool BuyDamage(BirdType t)
    {
        if (!Spend(DamageCost(t))) return false;
        PlayerPrefs.SetInt("dmg_" + (int)t, DamageLevel(t) + 1);
        PlayerPrefs.Save();
        return true;
    }

    public static bool BuyAim(BirdType t)
    {
        if (!Spend(AimCost(t))) return false;
        PlayerPrefs.SetInt("aim_" + (int)t, AimLevel(t) + 1);
        PlayerPrefs.Save();
        return true;
    }

    public static bool BuyExtra(BirdType t)
    {
        if (!Spend(ExtraCost(t))) return false;
        PlayerPrefs.SetInt("extra_" + (int)t, Extra(t) + 1);
        PlayerPrefs.Save();
        return true;
    }

    public static bool UseExtra(BirdType t)
    {
        if (Extra(t) <= 0) return false;
        PlayerPrefs.SetInt("extra_" + (int)t, Extra(t) - 1);
        PlayerPrefs.Save();
        return true;
    }

    // Bölüm sonu ödülü: yıldız x10 + yıkım oranı x20 + ilk geçiş bonusu 15
    public static int WinReward(int stars, float destruction, bool firstClear)
        => stars * 10 + Mathf.RoundToInt(Mathf.Clamp01(destruction) * 20f) + (firstClear ? 15 : 0);

    // Kaybedince küçük teselli: yıkım oranı x8
    public static int LoseReward(float destruction) => Mathf.RoundToInt(Mathf.Clamp01(destruction) * 8f);

    // Bir ördek türü hangi bölümde tanıtılıyor (dükkânda kilit için)
    public static int IntroLevel(BirdType t)
    {
        if (t == BirdType.Mallard) return 0;
        for (int i = 0; i < Levels.Count; i++)
        {
            var d = Levels.Get(i);
            if (d.newBird.HasValue && d.newBird.Value == t) return i;
        }
        return 0;
    }

    public static bool TypeUnlocked(BirdType t) => Levels.Unlocked(IntroLevel(t));

    // ---------------------------------------------------------------- yıldız hediyeleri
    // Her bölümde (20 level, en fazla 60 yıldız) iki hediye ördek: 20 ve 45 yıldızda.
    // Hediye = o ana kadar tanışılan türlerden rastgele 1 ekstra ördek. Bölüm başına en fazla 2 ördek.
    public static readonly int[] GiftStars = { 20, 45 };

    public static int WorldStars(int world)
    {
        int s = 0;
        for (int k = 0; k < Levels.PerWorld; k++) s += Levels.Stars(world * Levels.PerWorld + k);
        return s;
    }

    public static bool GiftClaimed(int world, int k) => PlayerPrefs.GetInt("gift_" + world + "_" + k, -1) >= 0;
    public static BirdType GiftDuck(int world, int k) => (BirdType)Mathf.Max(0, PlayerPrefs.GetInt("gift_" + world + "_" + k, 0));
    public static bool GiftReady(int world, int k) => !GiftClaimed(world, k) && WorldStars(world) >= GiftStars[k];

    public static bool AnyGiftReady(int world)
    {
        for (int k = 0; k < GiftStars.Length; k++) if (GiftReady(world, k)) return true;
        return false;
    }

    public static BirdType ClaimGift(int world, int k)
    {
        var pool = new System.Collections.Generic.List<BirdType>();
        for (int t = 0; t < TypeCount; t++) if (TypeUnlocked((BirdType)t)) pool.Add((BirdType)t);
        if (pool.Count == 0) pool.Add(BirdType.Mallard);
        var duck = pool[Random.Range(0, pool.Count)];
        PlayerPrefs.SetInt("gift_" + world + "_" + k, (int)duck);
        PlayerPrefs.SetInt("extra_" + (int)duck, Extra(duck) + 1);
        PlayerPrefs.Save();
        return duck;
    }

    public static void ResetAll()
    {
        PlayerPrefs.DeleteKey("eggs");
        for (int i = 0; i < TypeCount; i++)
        {
            PlayerPrefs.DeleteKey("dmg_" + i);
            PlayerPrefs.DeleteKey("aim_" + i);
            PlayerPrefs.DeleteKey("extra_" + i);
        }
        for (int w = 0; w < Levels.WorldCount; w++)
            for (int k = 0; k < GiftStars.Length; k++) PlayerPrefs.DeleteKey("gift_" + w + "_" + k);
        PlayerPrefs.Save();
    }
}
