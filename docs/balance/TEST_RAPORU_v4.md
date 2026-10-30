# DuckThis Feather Fury: QA Test Raporu v4 (görünmez sınır duvarı sonrası)

**Test tarihi:** 30.10.2026
**Test edilen düzeltme:**
- Her bölümde x = maxX + 0,25'te görünmez sınır duvarı (`LevelLoader.Build`, `SinirDuvari`). Duvar 1 birim kalınlığında ve zeminden yaklaşık 32 birim yüksekliğe kadar uzanıyor.
- Yedek eleme çizgisi duvarın arkasına alındı: x > maxX + 1,6 (`Destructible.cs:46`).
- Sol tarafta eleme çizgisi x < −10,5.
- Kamera sağda maxX + 1,2'ye kadar gösteriyor. Duvarın yüzü bu yüzden ekranın sağ kenarından yaklaşık 1 birim içeride kalıyor.

**Yöntem (Unity editörü):**
- **L13 yeniden üretimi:** 5 tam oyun (`v4_L13a`–`e`). Ördekler bitene kadar `aimshot` atıldı ve her atıştan sonra görüntü ile `state` (HUD sayacı) alındı. Ek olarak `autotest 13 13 12 1` çalıştırıldı (motor 10 denemede kesti) ve iki "sekme" testi yapıldı (`launch 17 7`, `launch 19 9`).
- **Diğer bölümler:** 18 bölümde 1–3 atış dizisi oynandı: 2, 20, 28, 44, 60, 64, 72, 90, 100, 120, 160, 200, 391, 460, 471 ve diğerleri. Ayrıca 33 doğrulanmış duvarın her biri ördekler bitene kadar oynandı. Toplam yaklaşık 70 atış dizisi ve 365 görüntü (`Logs/v4_*.png`).
- **Oynanış:** Her dünya için `autotest A B 3 1` (25 × 20 bölüm, 1.500 deneme).
  - Ham çıktılar `_out/v4_w01.txt` … `v4_w25.txt`, birleşik dosya `_out/v4_all_main.txt`.
  - Ana koşuda 500/500 bölüm SAĞLAM.
- **Tekrar:** 0/3 kalan 65 bölüm `autotest N N 6 1` ile yeniden oynandı (art arda olanlar tek komutta).
  - Çıktılar `_out/v4_re_*.txt`, birleşik dosya `_out/v4_all_re.txt`.
  - Toplamda 0/9 kalan bölüm "doğrulanmış duvar" sayıldı.
- **Bölüm tablosu:** `levels_v4.csv` (ana koşu, 3 deneme).

> **Ölçüm notları (önemli):**
> 1. **v3 ile v4 aynı bölümler değil.** `LevelGen.cs`, `AutoTester.cs` ve `GameManager.cs` 24.10 02:20'de değişmiş. v3 verisi bundan 5 dakika önce (02:15) yazılmıştı.
>    - 122 üretilmiş bölümün ördek sayısı, 356 bölümün yıldız eşikleri değişti.
>    - Elle yapılmış 1–40. bölümler birebir aynı. Bu bölümlerde kazanma v3'te 73/120, v4'te 71/120. Yani **duvarın kazanma oranına ölçülebilir bir etkisi yok.** Aşağıdaki +10 puanlık genel artış büyük ölçüde üretici ve bot değişikliklerinden geliyor.
> 2. Unity ön planda değilken oyun neredeyse duruyordu. Bekleyen komut 10 sn içinde okunmazsa `cmd.sh` onun üzerine yazdığı için bazı komutlar kayboldu. Unity birkaç kez öne getirildi, komutlar yeniden gönderildi ve bozulan diziler baştan oynandı. Sonuçlar etkilenmedi.
> 3. **Ekran görüntüsü testleri normal oyun olduğu için oyuncu kaydını değiştirdi.** `autotest` kaydı geri yüklüyor, `ui play` ile oynanan oyunlar yüklemiyor. Kazanılan bölümlerin yıldız ve en iyi skorları kayda geçti: L2, L20 (en iyi 84.300), L44 (38.800), L160, L391, L460, L471. Gerekirse bu değerler elle sıfırlanmalı (`reset` gönderilmedi).

---

## 1. Ana soru: Tilki hâlâ haritadan çıkabiliyor ya da HUD'un saydığı tilki görünmez kalabiliyor mu?

### Cevap: **HAYIR.** Bu testte tek bir örnek bile çıkmadı.

**Kanıt:**
- Yaklaşık 70 atış dizisinin her atışında HUD'daki "TİLKİ: N" ile ekranda sayılan tilki sayısı **birebir tuttu**.
- Bu dizilerin 33'ü doğrulanmış duvarlarda ördekler bitene kadar oynandı. Bunların hepsinde kalan tilkiler ekranda görünüyor ve sayıları HUD ile aynı.
- **Sağa itilen tilki duvarda duruyor ve görünür kalıyor.** Ekranın sağ kenarı 1100 px; duvara dayanmış tilkiler x ≈ 1000–1045 px'te. Örnekler:
  - `v4_L13c_s4.png`, `v4_L13c_s6.png`, `v4_L13c_s7.png`, `v4_L13d_s6.png`: L13 miğferli tilkisi duvarın dibinde.
  - `v4_W55_last.png`, `v4_W282_last.png`, `v4_W289_last.png`, `v4_W297_last.png`, `v4_W412_last.png`, `v4_W455_last.png`.
  - `v4_L72b_last.png`: iki tilki duvarda, HUD 7, ekranda 7 tilki.
- **v3'te hatanın yeniden üretildiği L460** iki kez oynandı:
  - `v4_L460a_s8.png`: HUD 5, ekranda 5 tilki.
  - `v4_L460b_s7.png` (HUD 3, ekranda 3) ve `v4_L460b_s8.png` (HUD 2, ekranda 2). Bu oyun 9 atışta **kazanıldı**.
- **L28:** v3'te son tilki ekrandan yarı taşıyordu. Şimdi sağdaki çerçeve ve tilki ekranın içinde (`v4_L28a_s0.png`, `v4_L28b_s4.png`, `v4_L28c_s4.png`). Üç oyunda da HUD ile ekran aynı.
- **Eleme çizgisinin anlamı:** Duvarın yüzü (maxX + 0,25) kamera kenarından (maxX + 1,2) yaklaşık 1 birim içeride. Tilkinin merkezi en fazla yaklaşık maxX − 0,25'e kadar gidebiliyor. v3'teki 0,7 birimlik görünmez şerit artık fiziksel olarak erişilemez.
- **Üst ve sol taraf:**
  - Duvar yaklaşık zemin + 32 birim yüksekliğinde. Hiçbir tilki ya da blok duvarın üstünden aşmadı.
  - Sol tarafa giden ve sol çizgide (x < −10,5) elenen tilki ekran görüntülerinde gözlenmedi.
  - Autotest sırasında L400'de sapanın yanında bir "5000" puan yazısı görüldü. Bu, sola fırlayan bir tilkinin elenip sayıldığını gösteriyor olabilir; doğrulanamadı.
  - Solda HUD'un saydığı ama görünmeyen tilki yok.

---

## 2. L13 "KALE DUVARI" yeniden üretimi

| Oyun | Nişan | Atış | Sonuç | Kalan / HUD | Ne oldu |
|---|---|---|---|---|---|
| a | aimshot 0 | 7 | Kaybetti | 1 / 1 | Miğferli tilki taş sütunun arkasında yerinde kaldı, görünür (`v4_L13a_s6.png`, `s7`). |
| b | aimshot 0 | 7 | Kaybetti | 1 / 1 | Aynı; atışlar taş sütuna çarptı (`v4_L13b_s5.png`, `s7`). |
| c | aimshot 1 | 7 | Kaybetti | 1 / 1 | Miğferli tilki sağa yuvarlandı, **duvarda durdu**, üstüne taş sütun devrildi. Ekranda kaldı (`v4_L13c_s4.png`, `s6`, `s7`). |
| d | aimshot 1 | 7 | Kaybetti | 1 / 1 | Aynı: tilki duvarda, 2 atış daha yedi, ölmedi, görünür (`v4_L13d_s6.png`). |
| e | aimshot 0 | 7 | Kaybetti | 2 / 2 | İki tilki görünür (`v4_L13e_s7.png`). |
| autotest | rastgele | 3 + 10 | 2/3 + 1/10 | – | L13 toplamda 3/13 (%23). |

- **5 oyunun hiçbirinde görünmez tilki yok.** HUD her atışta ekranla tutarlı.
- Tilki yuvarlandı mı? **Evet, 2/5 oyunda** (c, d). Her ikisinde de duvarda durdu ve ekranda kaldı.
- **Ancak L13 hâlâ zor (bot için yaklaşık %20).** Bu bir hata değil, denge sorunu:
  - Sağdaki miğferli tilki taş sütunun arkasında. Bot düz atışlarla sütuna çarpıyor.
  - Tilki itildiğinde artık "dışarı çıkıp ölü sayılmıyor", duvarın köşesinde kalıyor ve ancak hasarla ölüyor.
- **Sekme testi** (`v4_rico13_b1`–`b4`): Kaleyi aşan bir ördek miğferli tilkiye çarptı, tilkiyi devirdi (hasar 0) ve **duvarda durup söndü** (duman efekti). Geri sekmedi.

---

## 3. Duvarın yol açtığı yeni sorunlar

| Kontrol | Sonuç | Kanıt |
|---|---|---|
| Ördek duvardan tuhaf sekiyor mu? | **Hayır.** Duvar zemin malzemesini kullanıyor; ördek momentumunu kaybedip hemen bitiyor. Geri sekme gözlenmedi. | `v4_rico13_b2.png`–`b4.png`, `v4_rico13_a1.png`, `a3.png` |
| Bloklar duvarda yığılıyor mu? | **Hafif, zararsız.** Kirişler duvara yaslanıyor, taş sütunlar dik üst üste duruyor, bir çerçeve içindeki tilkiyle duvara kaydı. Titreşen ya da sıkışıp patlayan parça yok. Oyunu engellemiyor. | `v4_L471a_s4.png`, `v4_L460b_s8.png`, `v4_W204_last.png`, `v4_W227_last.png`, `v4_W386_last.png`, `v4_W469_last.png` |
| Duvara sıkışmış, vurulamayan tilki var mı? | **Tam vurulamayan yok, ama vurması zor olan var.** Duvar köşesindeki tilki üstten yay atışıyla vurulabiliyor. Önüne taş sütun ya da kiriş düşerse koruma kazanıyor. L13 c/d'de bu tilki 2–3 atışa dayandı. | `v4_L13c_s6.png`, `v4_L13d_s6.png`, `v4_W227_last.png` |
| Dolaylı etki: "itip öldürme" yolu kapandı | v3'te kenara itilen tilki ölü sayılıyordu; artık sayılmıyor. Bu doğru davranış, ama bazı bölümler biraz zorlaşabilir. Elle yapılmış bölümlerde kazanma neredeyse aynı kaldı (73 → 71 / 120). | §5 |
| Stabilite | 500/500 SAĞLAM (ana koşu). | `_out/v4_all_main.txt` |
| Eski sorun sürüyor: atış öncesi çökme | L187'de çatıdaki tilki ilk karede düşüyor (v3 §3'teki sorun). Yapı daha sonra 500 puanlık hasarla tamamen çöktü, 5 tilki sağ kaldı. | `v4_W187_s0.png`, `v4_W187_last.png` |

**Sonuç:** Duvar kritik bir yan etki üretmiyor.

---

## 4. Dünya bazında tablo (UPG = 1, 3 deneme; v3 ile karşılaştırma)

- "Duvar" = 0/3 + tekrar 0/6 (toplam 0/9).
- Boss sütununda tekrar sonucu `+x/6` olarak verildi.
- v3 karşılaştırması ölçüm notu 1 nedeniyle yalnızca yön gösterir.

| Dünya | Bölümler | Kazanma v4 | Kazanma v3 | Fark | 0/3 | Duvar (0/9) | Boss | Tek satır karar |
|---|---|---|---|---|---|---|---|---|
| 1 | 1–20 | %75,0 | %70,0 | +5 | 0 | – | 2/3 | Dostane; L2 artık geçiliyor. L13 hâlâ yaklaşık %20. |
| 2 | 21–40 | %43,3 | %51,7 | −8 | 7 | 35, 37 | 1/3 | **Oyunun en zor dünyası.** Elle yapılmış, 7 adet 0/3. |
| 3 | 41–60 | %65,0 | %31,7 | +33 | 3 | 43, 55 | 3/3 | Büyük iyileşme (üretici değişikliği); boss düzeldi. |
| 4 | 61–80 | %63,3 | %65,0 | −2 | 3 | 62, 63, 72 | 3/3 | Orta; 3 duvar. |
| 5 | 81–100 | %55,0 | %43,3 | +12 | 3 | 83, 90, 100 | **0/3, +0/6** | Boss hâlâ duvar (8 tilki, 8 ördek). |
| 6 | 101–120 | %61,7 | %46,7 | +15 | 6 | 102, 105, 117 | 1/3 | Taş çatılı kulübeler. |
| 7 | 121–140 | %85,0 | %56,7 | +28 | 0 | – | 3/3 | Kolay. |
| 8 | 141–160 | %66,7 | %66,7 | 0 | 3 | 144 | 3/3 | Dengeli. |
| 9 | 161–180 | %85,0 | %63,3 | +22 | 0 | – | 2/3 | Kolay. |
| 10 | 181–200 | %78,3 | %75,0 | +3 | 3 | 187 | 3/3 | İyi. |
| 11 | 201–220 | %85,0 | %83,3 | +2 | 1 | 204 | 3/3 | Fazla kolay. |
| 12 | 221–240 | %80,0 | %76,7 | +3 | 1 | 227 | 3/3 | Kolay. |
| 13 | 241–260 | %83,3 | %65,0 | +18 | 1 | 252 | 3/3 | Kolay. |
| 14 | 261–280 | %80,0 | %61,7 | +18 | 2 | 277 | 3/3 | Kolaylaştı. |
| 15 | 281–300 | %68,3 | %68,3 | 0 | 5 | 282, 289, 297, 298 | 3/3 | **4 duvarla geç oyunun en çok duvarlı dünyası.** |
| 16 | 301–320 | %70,0 | %71,7 | −2 | 3 | 309 | 3/3 | İyi. |
| 17 | 321–340 | %73,3 | %50,0 | +23 | 1 | – | 3/3 | Duvarsız. |
| 18 | 341–360 | %83,3 | %76,7 | +7 | 2 | – | 3/3 | Kolay. |
| 19 | 361–380 | %68,3 | %65,0 | +3 | 4 | 364 | 3/3 | Orta. |
| 20 | 381–400 | %76,7 | %56,7 | +20 | 3 | 386, 399 | 1/3 | İyileşti; boss zor. |
| 21 | 401–420 | %80,0 | %75,0 | +5 | 1 | 412 | 3/3 | Kolay. |
| 22 | 421–440 | %63,3 | %65,0 | −2 | 4 | 423 | 3/3 | Orta-zor. |
| 23 | 441–460 | %63,3 | %51,7 | +12 | 4 | 443, 455 | **0/3, +1/6** | Boss artık kazanılabiliyor (elle 1/2, bot 1/9). |
| 24 | 461–480 | %83,3 | %65,0 | +18 | 2 | 469 | 3/3 | Kolay. |
| 25 | 481–500 | %73,3 | %63,3 | +10 | 3 | 489 | 3/3 | Final W23'ten kolay. |
| **Toplam** | 1–500 | **%72,4** | %62,6 | +10 | **65** (v3: 91) | **33** (v3: 62) | 1 duvar boss (L100) | |

**Ek gözlemler:**
- **100'lük bloklarda kazanma:** 1–100 %60 · 101–200 %75 · 201–300 %79 · 301–400 %74 · 401–500 %73. Zorluk eğrisi L200'den sonra hâlâ yükselmiyor.
- **Ördek sayısına göre (L41+):**

| Ördek | Bölüm | Kazanma | 0/3 |
|---|---|---|---|
| 5 | 247 | %82 | 14 |
| 6 | 161 | %61 | 39 |
| 7 | 10 | %57 | 2 |

  6 ördekli bölümler hâlâ en zor grup.
- **Gökdelen slotları (4/11/18, L41+):** %69,6 (v3: %76,8). 11 adet 0/3; doğrulanmış duvar olarak kalanlar 144, 204, 298, 364.
- **Elle oynanan boss sonuçları (aimshot):** L20 1/2, L60 0/2, L100 0/2, L120 0/2, L160 1/2, L200 0/1, L460 1/2.

---

## 5. Doğrulanmış duvarlar (0/9): neden kazanılmıyor

- 33 bölümün **tamamı** ördekler bitene kadar oynanıp ekran görüntüsü alındı (`Logs/v4_W<N>_last.png`; 72, 90 ve 100 için `v4_L72a_s6m`, `v4_L90a_s5m`, `v4_L100a_s8m` ve `…b_last`).
- **Hepsinde kalan tilkiler görünür ve HUD ile aynı sayıda.** Hiçbiri harita dışında ya da gizli değil.
- Hiçbir tilki tamamen taşla mühürlenmiş değil. Hepsine en az üstten ya da yandan bir yay atışıyla ulaşılabiliyor.

**Neden türleri:**
- **A. Taş çatılı kulübe / kalkan:** Tilki taş tavan ve taş sütunlar arasında, önünde taş blok var. Bot düz atışla sütuna çarpıyor, hasar çok düşük.
- **B. Dayanıklı miğferli tilki:** Yapı tamamen çöküyor ama miğferli tilkiler sağ kalıyor. Geç oyunda tilki canı çok yüksek.
- **C. Tilki/ördek oranı:** Tilki sayısı ördek sayısına eşit ya da fazla; miğfer ve boss da var.

| Bölüm | Tilki / Ördek | Ördekler bittiğinde kalan (HUD = ekran) | Tür | Neden |
|---|---|---|---|---|
| 35 | 7 / 6 | 5 | C + A | Şogun boss tilki + 2 miğferli tilki çift taş tavan altında. |
| 37 | 6 / 5 | 4 | C + A | 3 katlı taş kule, 2 miğferli; az ördek. |
| 43 | 3 / 4 | 2 | A | Taş tavanlı iki kulübe, ortalama hasar yaklaşık 380 (v2'den beri duvar). |
| 55 | 7 / 6 | 3 | C | Çok tilki; bir tilki duvara itildi ama görünür. |
| 62 | 8 / 6 | 2 | C | 8 tilki 6 ördek; 6 tilki öldü, sınırda. |
| 63 | 6 / 5 | 5 | A + C | Taş tavanlı 2 katlı kale. |
| 72 | 7 / 7 | 4 (a), 7 (b) | C + A | 3 taş çerçeve + miğfer; 2 tilki duvarda, görünür. |
| 83 | 8 / 7 | 4 | C + B | 8 tilki, miğferli. |
| 90 | 5 / 5 | 3–4 | B + C | 4 miğferli tilki taş çerçevelerde. |
| 100 (boss) | 8 / 8 | 3–5 | B + C | Boss + miğferliler; 8 ördek yetmiyor. |
| 102 | 6 / 6 | 3 | A | L43 şablonu: taş tavanlı kulübeler. |
| 105 | 5 / 6 | 1 | A | Son tilki taş blok arkasındaki kulübede (kıl payı). |
| 117 | 5 / 6 | 1 | A | Aynı desen (kıl payı). |
| 144 | 6 / 6 | 3 | A | Taş tavanlı çift kulübe + miğfer (gökdelen slotu). |
| 187 | 5 / 6 | 5 | B | Yapı çöktü (hasar 500), 5 tilki sağ. Çatı tilkisi atıştan önce düşüyor. |
| 204 | 6 / 6 | 2 | B | Gökdelen çöktü, 2 miğferli tilki yerde sağ. |
| 227 | 6 / 6 | 4 | B | Çöküş sonrası miğferliler sağ; bir çerçeve duvara kaydı. |
| 252 | 5 / 6 | 4 | B | 4 miğferli tilki çöküşten sağ çıktı. |
| 277 | 3 / 5 | 2 | A | Miğferli tilkiler taş levha altında; hasar yaklaşık 800. |
| 282 | 5 / 6 | 4 | B | Miğferli tilkiler; biri duvarda, görünür. |
| 289 | 5 / 6 | 5 | A + B | Taş/tahta iki kule, 4 miğferli; hasar 2.900. |
| 297 | 5 / 5 | 5 | A | Taş kapaklı ikiz kule; hasar 1.300. |
| 298 | 6 / 6 | 3 | A | Taş tavanlı çift kulübe (gökdelen slotu). |
| 309 | 6 / 6 | 2 | A | Çift taş levha sığınak. |
| 364 | 6 / 6 | 1 | A | Taş blok arkasındaki kulübe (kıl payı). |
| 386 | 6 / 6 | 3 | B | Çöküş sonrası dayanıklı tilkiler. |
| 399 | 5 / 6 | 4 | A | Taş çerçeveler. |
| 412 | 6 / 6 | 5 | B | Çöküş sonrası 5 tilki sağ, biri duvarda. |
| 423 | 5 / 6 | 3 | A + B | Taş kapaklı kule, miğferli. |
| 443 | 5 / 6 | 2 | A | Taş kapaklı kulübe. |
| 455 | 3 / 5 | 2 | A | Kulübe + miğferli; biri duvarda. |
| 469 | 6 / 6 | 4 | B | Miğferliler çöküşten sağ. |
| 489 | 3 / 5 | 2 | A | Taş kulübe + miğferli. |

- **v3'e göre:** 38 duvar kalktı (2, 28, 30, 47, 50, 54, 60, 66, 82, 96, 107, 108, 120, 129, 137, 154, 193, 196, 208, 250, 265, 272, 275, 325, 339, 356, 369, 375, 391, 433, 436, 450, 452, 460, 466, 474, 488, 494).
- **Yeni duvarlar (9):** 35, 62, 144, 187, 204, 252, 298, 364, 455. Bunların çoğu v3'te 1/3–3/3 idi; bölüm içeriği değiştiği ya da 3 denemelik örneklem gürültülü olduğu için yeni görünüyor olabilirler.

---

## 6. Düzeltme önerileri

### A. Sınır duvarı (çalışıyor; küçük iyileştirmeler)
1. **Duvarı koru.** Görünmez tilki sorunu kapandı. `FieldMaxX + 1.6` yedek çizgisi zararsız; kalabilir.
2. **Köşeye sıkışan tilki için güvenlik ağı:**
   - Duvara 0,6 birimden yakın, 3 sn boyunca hareketsiz ve üstü kapalı bir tilki için duvar yüzeyine hafif bir sola itme (`PhysicsMaterial2D` sürtünmesini 0 yapmak) yeterli olabilir.
   - Ya da ördekler bittiğinde, duvara temas eden tilkilerin kalan canının %50'sini bir "son sarsıntı" ile düşür.
3. **İsteğe bağlı görsel:** Duvar yerine sağ kenara çit, kaya ya da uçurum gibi görünür bir sınır sprite'ı koy. Oyuncu tilkinin neden orada durduğunu anlar.
4. **AutoTester'a kontrol ekle:** Kayıp anında canlı bir tilki kamera görüş alanı dışındaysa satıra `GÖRÜNMEZ TİLKİ` yazdır. Bu hata gerilerse otomatik yakalanır.

### B. Taş çatılı kulübe şablonu (A türü, 17 duvar: 43, 102, 105, 117, 144, 277, 289, 297, 298, 309, 364, 399, 423, 443, 455, 489, 63)
1. Taş tavan + taş yan sütun + önde taş blok birlikte olmasın. En az bir parça tahta ya da buz olsun.
2. Ya da tavanı tahtaya çevir.
3. Kulübenin içine ya da yanına TNT koy.
4. L105, L117 ve L364 "tek tilki kaldı" ile bitiyor; bu kulübede +1 ördek yeterli.

### C. Dayanıklı miğferli tilki (B türü: 187, 204, 227, 252, 282, 386, 412, 469, 83, 90)
1. `FoxHealthMult` geç oyunda çöküş hasarına göre fazla yüksek. L180'den sonra çarpanı yaklaşık %20 düşür.
2. Ya da düşen taş ve tahtanın tilkiye verdiği hasarı (`massFactor`) artır.

### D. Tilki/ördek oranı (C türü: 35, 37, 55, 62, 72, 83, 100)
- `PickDucks` kuralı: **ördek ≥ tilki sayısı + miğferli tilki / 2**.
- 8 tilkili normal bölümleri 7 ile sınırla.
- **L100 boss** (8/8, 0/9): +1 ördek ver ya da miğferli tilki sayısını en fazla 3 yap.

### E. Elle yapılmış bölümler
- **W2 (%43, 7 adet 0/3):** L35 ve L37'ye +1 ördek ver ya da birer miğferi kaldır.
- **L13 (yaklaşık %20):** Sağdaki miğferli tilkinin önündeki taş sütunu tahtaya çevir ya da +1 ördek ver. Dünya 1 için fazla zor.

### F. Diğer
- **Atış öncesi çökme sürüyor (L187).** v3 önerisi geçerli: giriş animasyonu bitene kadar fizik gövdelerini uyut.
- **Test altyapısı:**
  - `cmd.sh` 10 sn sonra okunmamış komutun üzerine yazıyor; kuyruk ya da benzersiz dosya adı kullanılmalı.
  - Unity arka plandayken oyun duruyor; `Application.runInBackground = true` ayarlanmalı.

---

## 7. Dosyalar

- Rapor: `/home/claude/balance/TEST_RAPORU_v4.md`
- Tablo: `/home/claude/balance/levels_v4.csv`
- Projede (`D:\projects\OrdekOyunu`):
  - `_out/v4_w01.txt` … `_out/v4_w25.txt`, `_out/v4_all_main.txt`
  - `_out/v4_re_*.txt`, `_out/v4_all_re.txt`
  - `_out/v4_L13_x10.txt`
  - Ekran görüntüleri: `Logs/v4_*.png`
