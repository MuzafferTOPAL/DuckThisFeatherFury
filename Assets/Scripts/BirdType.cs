using UnityEngine;

// Fırlatılan ördek türleri. İsim ve yetenek metinleri Loc (çoklu dil) içinde.
// Sıra kayıtlarla uyumlu kalsın diye yeni türler sona eklenir.
public enum BirdType { Mallard, Mandarin, Merganser, Muscovy, Scoter, Shelduck, Pintail, Eider, Goldeneye }

// Her ördek türünün fizik değerleri, malzemelere karşı hasar çarpanı, sprite hizalaması ve açılış bölümü.
public static class BirdInfo
{
    public const int Count = 9;

    public struct Data
    {
        public float scale;            // çizim ölçeği (Yeşilbaş = 1)
        public float mass;
        public float wood, stone, ice, fox; // malzemeye göre hasar çarpanı
        public int unlockWorld;        // bu bölümden (0'dan) itibaren kullanılabilir
        public Color featherColor;

        public float Radius => 0.5f * scale;
        public Vector2 SpriteOffset => new Vector2(1f, 2.5f) * scale / 16f;   // gövde merkezini köke oturt
        public float StandHeight => 7f * scale / 16f;                          // ayakların zemine oturması
    }

    // Başlangıçta 3 tür; her 3 bölümde bir yeni tür (4, 7, 10, 13, 16, 19. bölümler)
    static readonly Data[] data =
    {
        // Yeşilbaş: kanat çırpma
        new Data { scale = 1f, mass = 2f, wood = 1f, stone = 1f, ice = 1f, fox = 1f, unlockWorld = 0, featherColor = new Color(0.66f, 0.7f, 0.74f) },
        // Mandarin ördeği: üçe bölünme, buza karşı güçlü
        new Data { scale = 0.8f, mass = 0.9f, wood = 0.6f, stone = 0.4f, ice = 2.5f, fox = 1f, unlockWorld = 0, featherColor = new Color(0.88f, 0.48f, 0.16f) },
        // Büyük tarakdiş: hızlı dalış, tahtaya karşı güçlü
        new Data { scale = 1f, mass = 1.6f, wood = 2.2f, stone = 0.6f, ice = 1f, fox = 1f, unlockWorld = 0, featherColor = new Color(0.93f, 0.95f, 0.96f) },
        // Misk ördeği: ağır çakılma, taşa karşı güçlü
        new Data { scale = 1.25f, mass = 4f, wood = 1.2f, stone = 2.2f, ice = 1f, fox = 1f, unlockWorld = 3, featherColor = new Color(0.16f, 0.18f, 0.23f) },
        // Kara ördek: patlama
        new Data { scale = 0.95f, mass = 2.2f, wood = 1f, stone = 1f, ice = 1f, fox = 1f, unlockWorld = 9, featherColor = new Color(0.12f, 0.13f, 0.19f) },
        // Angut: yumurta bombası bırakıp yukarı fırlama
        new Data { scale = 1.05f, mass = 2f, wood = 1f, stone = 1f, ice = 1f, fox = 1f, unlockWorld = 6, featherColor = new Color(0.85f, 0.46f, 0.18f) },
        // Kılkuyruk: bumerang gibi geri döner, arkadan vurur
        new Data { scale = 1f, mass = 1.6f, wood = 1.4f, stone = 0.8f, ice = 1.2f, fox = 1.2f, unlockWorld = 12, featherColor = new Color(0.72f, 0.74f, 0.78f) },
        // Pufla: şok dalgası, her şeyi iter
        new Data { scale = 1.2f, mass = 3.5f, wood = 1.2f, stone = 1.5f, ice = 1f, fox = 1f, unlockWorld = 15, featherColor = new Color(0.95f, 0.94f, 0.9f) },
        // Altıngöz: en yakın tilkiye kilitlenip dalar, tilkilere çok güçlü
        new Data { scale = 0.95f, mass = 1.8f, wood = 1f, stone = 1f, ice = 1f, fox = 1.8f, unlockWorld = 18, featherColor = new Color(0.96f, 0.97f, 0.98f) },
    };

    public static Data Get(BirdType t) => data[(int)t];
    public static int UnlockLevel(BirdType t) => data[(int)t].unlockWorld * Levels.PerWorld;
    public static bool Available(BirdType t, int levelIndex) => levelIndex >= UnlockLevel(t);

    public static float DamageVs(BirdType t, BlockMaterial m)
    {
        var d = data[(int)t];
        switch (m)
        {
            case BlockMaterial.Wood: return d.wood;
            case BlockMaterial.Stone: return d.stone;
            case BlockMaterial.Ice: return d.ice;
            case BlockMaterial.Fox: return d.fox;
            default: return 1f;
        }
    }
}
