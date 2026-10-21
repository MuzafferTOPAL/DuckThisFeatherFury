using System;
using System.Collections.Generic;
using UnityEngine;

// Oyuncu profili — ileride hesap (Google / e-posta) ve bulut kaydı için hazırlık.
// Şu an herkes "misafir" olarak oynar; hesap açmak zorunlu değildir.
// Hesap eklendiğinde:  misafir profil -> hesaba bağlanır (Link), ilerleme Export()/Import() ile buluta taşınır.
public static class PlayerProfile
{
    public enum AccountType { Guest, Google, Email }

    [Serializable]
    public class SaveData
    {
        public int version = 1;
        public string playerId;
        public long savedAtUnix;
        public int eggs;
        public List<int> stars = new List<int>();
        public List<int> best = new List<int>();
        public List<int> damage = new List<int>();
        public List<int> aim = new List<int>();
        public List<int> extra = new List<int>();
        public List<string> nests = new List<string>();
    }

    public static string PlayerId
    {
        get
        {
            var id = PlayerPrefs.GetString("player_id", "");
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString("N");
                PlayerPrefs.SetString("player_id", id);
                PlayerPrefs.Save();
            }
            return id;
        }
    }

    public static AccountType Type => (AccountType)PlayerPrefs.GetInt("account_type", 0);
    public static bool IsGuest => Type == AccountType.Guest;
    public static string DisplayName => PlayerPrefs.GetString("display_name", "");

    // Hesap sistemi eklendiğinde giriş başarılı olunca çağrılacak
    public static void Link(AccountType type, string accountId, string displayName)
    {
        PlayerPrefs.SetInt("account_type", (int)type);
        PlayerPrefs.SetString("account_id", accountId ?? "");
        PlayerPrefs.SetString("display_name", displayName ?? "");
        PlayerPrefs.Save();
    }

    public static void Ensure() { var _ = PlayerId; }

    // Tüm ilerlemeyi tek bir JSON'a çevirir (bulut kaydı / cihaz değişimi için)
    public static string Export()
    {
        var d = new SaveData { playerId = PlayerId, savedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), eggs = Economy.Eggs };
        for (int i = 0; i < Levels.Count; i++) { d.stars.Add(Levels.Stars(i)); d.best.Add(Levels.Best(i)); }
        for (int t = 0; t < Economy.TypeCount; t++)
        {
            d.damage.Add(Economy.DamageLevel((BirdType)t));
            d.aim.Add(Economy.AimLevel((BirdType)t));
            d.extra.Add(Economy.Extra((BirdType)t));
        }
        foreach (var k in Levels.NestKeys()) if (PlayerPrefs.GetInt(k, 0) == 1) d.nests.Add(k);
        return JsonUtility.ToJson(d);
    }

    // Buluttan gelen kaydı birleştirir: her alanda büyük olan kazanır (ilerleme kaybolmaz)
    public static void Import(string json)
    {
        var d = JsonUtility.FromJson<SaveData>(json);
        if (d == null) return;
        Economy.Eggs = Mathf.Max(Economy.Eggs, d.eggs);
        for (int i = 0; i < Mathf.Min(Levels.Count, d.stars.Count); i++) Levels.Save(i, d.stars[i], i < d.best.Count ? d.best[i] : 0);
        for (int t = 0; t < Economy.TypeCount; t++)
        {
            if (t < d.damage.Count) PlayerPrefs.SetInt("dmg_" + t, Mathf.Max(Economy.DamageLevel((BirdType)t), d.damage[t]));
            if (t < d.aim.Count) PlayerPrefs.SetInt("aim_" + t, Mathf.Max(Economy.AimLevel((BirdType)t), d.aim[t]));
            if (t < d.extra.Count) PlayerPrefs.SetInt("extra_" + t, Mathf.Max(Economy.Extra((BirdType)t), d.extra[t]));
        }
        foreach (var k in d.nests) PlayerPrefs.SetInt(k, 1);
        PlayerPrefs.Save();
    }
}
