# DuckThis Feather Fury – Proje Notları
(eski adı: Ördek Fırtınası)

Angry Birds tarzı, pixel-art, ördek temalı 2D fizik/fırlatma oyunu (Android, yatay ekran).
Geliştirici: DuckThisGames — paket adı: com.duckthisgames.featherfury — sürüm 1.0 (aksi söylenene kadar 1.0 kalır)
Proje dizini: D:\projects\OrdekOyunu — Unity 6000.6.6f1 (Built-in render, eski Input Manager)
GitHub: https://github.com/Duck-This-Games/DuckThisFeatherFury

## Oyun
- Açılış ekranı: "DUCKTHIS GAMES" + "DuckThisGames tarafından geliştirildi" (dokununca geçer)
- 20 bölüm, 4 dünya: Göl Kıyısı, Sonbahar Ormanı, Karlı Dağlar, Gün Batımı Çiftliği
- Kademeli zorluk: tilki canı x1 → x1.5, blok dayanıklılığı x1 → x1.3 (LevelLoader)
- 6 ördek: Yeşilbaş (kanat çırpma), Mandarin (üçe bölünme, buz), Tarakdiş (dalış, tahta),
  Misk Ördeği (çakılma, taş), Angut (yumurta bombası), Kara Ördek (patlama)
- Düşmanlar: tilkiler (yavru, normal, miğferli, patron). Malzemeler: tahta, taş, buz, TNT, kaya
- 7 dil: TR, EN, DE, ES, FR, PT, IT (Loc.cs)

## Ekonomi: Altın Yumurta (Economy.cs)
- Kazanma: yıldız x10 + yıkım oranı x20 + ilk geçiş bonusu 15. Kaybetme: yıkım oranı x8
- Dükkân: her ördek için Hasar (4 sv, +%15; 40/80/140/220), Otomatik Nişan (3 sv; 60/120/200), Ekstra ördek (20–35)
- Oyunda "+" butonu ile ekstra ördek; kaybedince "+ Ördekle Devam"
- PlayerPrefs: eggs, dmg_i, aim_i, extra_i

## Unity menüsü "Ördek Oyunu"
Oyunu Yeniden Kur • Bölümleri Otomatik Test Et • Tüm Bölümlerin Kilidini Aç • İlerlemeyi Sıfırla • Android APK Al (Builds/DuckThis_FeatherFury.apk)

## Son otomatik test (yükseltmesiz basit yapay zekâ)
20 bölüm sağlam; 80 denemede 35 kazanma. Bölüm 1 ve 6'yı bot geçemiyor, elle geçilebiliyor.
