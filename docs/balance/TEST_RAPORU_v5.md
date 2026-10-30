# DuckThis Feather Fury: QA Test Raporu v5 (yeni TNT kuralları, geçirgen sınır duvarı, tilki yuvarlanma direnci)

**Test tarihi:** 30.10.2026
**Test edilen değişiklikler:**
1. Yeni TNT kuralları (`Destructible.HandleTntCollision`): ördek > 4 birim/sn, diğer cisimler hız × ağırlık çarpanı ≥ 7, her patlama; zemin ve sınır duvarı asla; ilk 2,5 sn asla; zayıf darbede kırmızı yanıp sönme.
2. Sınır duvarı: maxX + 1,6'da, görünmez, ördekler içinden geçiyor (`Bird.IgnoreWall`); kamera maxX + 2,6'ya kadar.
3. Tilkilerde yuvarlanma direnci (`Fox.FixedUpdate`, `angularDamping = 6`).
4. Kazanılamayan bölüm düzeltmeleri: çatı kirişi taş olmasın, normal bölümde en çok 7 tilki, miğferli tilki varsa ek ördek, geç oyunda tilki canı tavanı, L35/L37'ye +1 ördek.

**Yöntem (Unity editörü):**
- **Ekran görüntüsü testleri:** TNT için 12 bölüm (5, 8, 10, 16, 30, 40, 41–64 taraması, 44, 65–87 taraması, 70), duvar için 6 bölüm (13, 28, 72, 100, 460, 471), yuvarlanma için kare kare kayıt (L10, L28, L70).
  - Görüntüler `Logs/v5_*.png`, kare kayıtları `Logs/rec_v5*/`, birleştirilmiş paftalar `_out/v5m/*.png`.
  - Ek olarak `launch` ile hedefli atış (L8) ve `egg` komutuyla yumurta bombası (L8) kullanıldı.
- **Denge koşusu:** Her dünya için `autotest A B 3 1` (25 × 20 bölüm, 1.500 deneme).
  - Ham çıktılar `_out/v5_w01.txt` … `v5_w25.txt`, birleşik dosya `_out/v5_all_main.txt`. **500/500 bölüm SAĞLAM.**
- **Tekrar:** 0/3 kalan 64 bölüm `autotest N N 6 1` ile yeniden oynandı (art arda olanlar tek komutta).
  - Çıktılar `_out/v5_re_*.txt`, birleşik dosya `_out/v5_all_re.txt`.
  - 0/9 kalanlar "doğrulanmış duvar" sayıldı. Bunların 25'i de ördekler bitene kadar oynanıp görüntülendi (`Logs/v5_X<N>_*.png`, pafta `_out/v5m/v5_X<N>.png`). L100 için `v5_W100_*` kullanıldı.
- **Bölüm tablosu:** `levels_v5.csv` (ana koşu, 3 deneme; v4 ile aynı sütunlar).

> **Ölçüm notları:**
> 1. **3 denemelik örneklem gürültülü.** 60 denemelik bir dünyada ±8–10 puanlık oynama normal. Dünya farkları yön gösterir; 10 puanın altındaki farklar anlamlı değildir.
> 2. **Unity ön planda değilken `autotest` komutu okunuyor ama koşu başlamıyor.** Oyun modundan çıkış sonrası `delayCall` bekliyor. Bu üç kez oldu (w01 öncesi, w09 ve birkaç tekrar koşusu). Unity öne getirildi, komut yeniden gönderildi. Sonuçlar etkilenmedi.
> 3. **`ui play` ile oynanan oyunlar kayda geçti** (`autotest` kaydı geri yüklüyor, `ui play` yüklemiyor):
>    - L8: 3★, en iyi skor **58.900** (yumurta testi, ördek atılmadan).
>    - L10: 3★, 71.800.
>    - L13: 1★, 18.800 (yeni rekor).
>    - L471: 3★, 37.300.
>
>    Diğer ekran görüntüsü oyunları kaybedildi. `reset` gönderilmedi.

---

## 1. TNT: Belirtildiği gibi çalışıyor mu?

### Özet: **Büyük ölçüde evet.** Başlangıçta patlama yok, ördek darbesi ve zincir patlama çalışıyor, hafif darbeler patlatmıyor. **Ancak "düşen taş veya kaya patlatır" kuralı pratikte neredeyse hiç tetiklenmiyor.** Eşik, oyundaki gerçek düşüş yükseklikleri için fazla yüksek.

| Kontrol | Sonuç | Kanıt |
|---|---|---|
| İlk 2,5 sn patlama yok | **GEÇTİ.** 8, 10, 16, 30, 40, 44, 70 bölümlerinde `ui begin`'den 0,6 sn ve 3 sn sonra alınan karelerde tüm TNT'ler yerinde. 41–87 taramasında TNT'li 15 bölümün hiçbirinde başlangıç patlaması yok. Stabilite koşusu da 500/500 SAĞLAM. | `v5_T8_t0/t3`, `v5_T16_t0/t3`, `v5_T10_t0/t3`, `v5_T40_t0/t3`, `v5_T44_t0/t3`, `v5_T70_t0/t3`, `_out/v5m/scan1.png`, `scan2.png` |
| Ördek doğrudan vurursa patlar | **GEÇTİ.** L8'de üst TNT'ye yüksek yay atışı (`launch 5.61 12.83`): TNT patladı, iki alt TNT zincirle patladı, 3 tilki öldü (ekranda üç kez "600" TNT puanı). L10 ikinci atışta ördek TNT'ye çarptı: iki TNT patladı, patron + 3 tilki öldü. | `_out/v5m/v5_H8b.png` (`v5_H8b_2`), `_out/v5m/v5_T10.png` (`v5_T10_s2a`) |
| Zincirleme TNT | **GEÇTİ.** L8 (iki kez) ve L10'da yan TNT'ler zincirle patladı. | Aynı |
| Yumurta bombası (Angut) patlatır | **GEÇTİ.** `egg 6 1` ile L8 üst TNT'ye yumurta bırakıldı. 0,3 sn içinde 3 TNT patladı, kale tamamen yıkıldı, ördek atmadan kazanıldı. | `_out/v5m/v5_E8.png` |
| Kara Ördek (Scoter) patlaması | Ekran görüntüsüyle ayrıca denenmedi. Kod incelemesi: `CrowExplode` hasarı 28 × (0,25 + k) ≥ 7 > TNT canı 4, yani 3 birim yarıçap içindeki her TNT patlar. ✔ | `Bird.cs:345` |
| Hafif darbe (yuvarlanan tilki, sarsılan tahta) patlatmaz | **GEÇTİ.** L10'da üst kat çöktü: tahta kirişler ve iki miğferli tilki TNT'lerin üstüne düştü, TNT patlamadı. L16'da TNT taş kirişten yere devrildi, patlamadı. L70'te miğferli tilki TNT'ye dayalı 3 atış boyunca durdu. L44'te kule TNT'nin yanına çöktü. | `_out/v5m/r10crop.png`, `v5_T16.png` (`s4`), `rec_v5r70.png`, `r70crop.png`, `v5_T44.png` |
| **Düşen taş veya kaya patlatır** | **DOĞRULANAMADI; büyük olasılıkla çoğu durumda patlatmıyor.** ~15 atış dizisinde ağır cisim düşmesiyle tek bir TNT patlaması görülmedi (L44 kule çöküşü, L70 kaya rafı, L10 taş sütun devrilmesi). Kontrollü bir test için `egg 1.3 -2.5` ile L5'te taş sütunu TNT'ye itmek denendi; yumurta görünmedi, hiçbir şey olmadı (aşağıda yeni sorun #6). | `v5_T44.png`, `r70crop.png`, `rE5.png` |
| Kırmızı yanıp sönme | Görsel olarak yakalanamadı (0,35 sn sürüyor; 20 fps kayıtta seçilemedi). Kodda mevcut (`LateUpdate`, eşik × 0,4 üstü darbe). | `Destructible.cs` |

**Eşik hesabı (kod: `power = çarpma hızı × clamp(√(kütle/2), 0,4, 1,8) ≥ 7`).** TNT üstünde 1 birim yükseklikten serbest düşüş için gereken yükseklik:

| Cisim | Kütle | Çarpan | Gereken hız | Gereken düşüş |
|---|---|---|---|---|
| Kaya (r 0,45, yoğunluk 3) | 1,9 | 0,98 | 7,2 m/s | **≈ 2,6 birim** |
| Taş küp 1×1 | 3,0 | 1,22 | 5,7 m/s | ≈ 1,7 birim |
| Taş sütun 0,5×2,4 | 3,6 | 1,34 | 5,2 m/s | ≈ 1,4 birim |
| Uzun taş kiriş 4,6×0,5 | 6,9 | 1,8 | 3,9 m/s | ≈ 0,8 birim |
| Normal tilki | 1,2 | 0,77 | 9,0 m/s | ≈ 4,2 birim (fiilen yalnız fırlatılınca) |
| Patron tilki | 4,0 | 1,41 | 5,0 m/s | ≈ 1,3 birim |

- **"Kaya rafı" şablonu (LevelGen case 4) kendi amacını tutturamıyor.**
  - Kaya kirişin üstünde (y ≈ −1,1), TNT yerde (üstü −3,0). Düşüş yaklaşık 1,9 birim, hız 6,1 m/s, güç ≈ 6,0 < 7.
  - Yani kaya dümdüz TNT'ye düşse bile **patlamaz**, en fazla kırmızı yanıp söner. Bunun için kayanın ayrıca yanlamasına hız kazanması gerekir.
- **Tutarsızlık:** TNT 10 birimden yere düşse patlamıyor (zemin = asla), ama 1,2 birimden bir tahta kirişin üstüne düşerse patlayabiliyor. Bu kural gereği, ama oyuncuya tuhaf görünebilir.
- **Kıyas:** Elle yapılmış TNT'li 16 bölümde kazanma 30/48'den 27/48'e düştü (v4 → v5). TNT'siz 24 elle yapılmış bölümde ise 41/72'den 30/72'ye düştü. TNT kuralının etkisi küçük; asıl düşüş başka kaynaklı (§4, §5).
- **Tek belirgin TNT kaynaklı gerileme L38 LAV KÖPRÜSÜ: v4'te 3/3, v5'te 0/9.**
  - TNT, yerdeki tilkinin hemen yanında. Kaya ve çerçeveler TNT'yi artık tetiklemiyor; ördekler üst çerçevelere düşüyor (`v5_X38.png`).

---

## 2. Sınır duvarı: Ördekler geçiyor mu, tilkiler içeride kalıyor mu, HUD tutuyor mu?

### Özet: **Evet, üçü de.**

| Bölüm | Kaleyi aşan atış | Ördek duvardan geçti mi? | Sağa giden tilki/blok | HUD = ekran? | Kanıt |
|---|---|---|---|---|---|
| 13 | `launch 17 9` | **Evet.** İz ekranın sağ kenarından çıkıyor, geri sekme yok. | Miğferli tilki taş sütunun arkasında, sağ kenara yakın; 6 atış boyunca görünür kaldı. | Her atışta 3 → 1 → 0. Kazanıldı. | `_out/v5m/v5_W13.png` |
| 28 | `launch 17 9` | Atış feneri vurdu (yay çok alçak). Sonraki ördekler kaleye düştü. | – | 3 tilki görünür, HUD 3. | `v5_W28.png` |
| 72 | `launch 18 7` | Atış taş çatıdan sola sekti (duvardan değil). | Tilkiler kulelerde kaldı. | HUD 5 → 4 → 2, ekranla aynı. | `v5_W72.png` |
| 100 (boss) | `launch 15 13` | **Evet**, iz sağ kenardan çıkıyor. | – | HUD 8 → 3, ekranla aynı. | `v5_W100.png` |
| 460 (boss) | `launch 15 13` | **Evet.** | Bir tilki (patlamayla fırlayan) sağ kenarda duvara dayandı, 6 atış boyunca **görünür kaldı** (ekran x ≈ %94). | HUD 5 → 3 → 2 → 1, ekranla aynı. | `v5_W460.png` (`s2`–`s7a`) |
| 471 | `launch 15 13` | **Evet.** | – | 3 → 2 → 0. Kazanıldı. | `v5_W471.png` |

- **Ek olarak** 25 doğrulanmış duvarın ördekler bitene kadar oynandığı ~150 atışta HUD sayacı ile ekrandaki tilki sayısı her seferinde aynıydı. Ekran dışında ya da görünmez tilki yok.
- **Geometri (kod):** Duvar merkezi maxX + 2,1, genişlik 1, yüzü maxX + 1,6. Tilki merkezi en fazla maxX + 1,05 (patronda + 0,78) olabilir. Kamera maxX + 2,6'ya kadar gösteriyor, yani duvara dayanan tilki **her zaman ekranda**. Yedek eleme çizgisi (maxX + 3) duvarın arkasında kaldığı için fiilen erişilemez.
- **Kozmetik:** `LevelLoader.cs:57–58`'deki yorum hâlâ eski değerleri ("maxX + 0.25", "maxX + 1.2") yazıyor.

---

## 3. Yuvarlanma: Tilkiler çabuk duruyor mu?

### Özet: **Evet.** Kaleden düşen tilkiler düştükleri yerin 0,5–1,5 birim içinde duruyor. Kenara kayan tilki görülmedi.

- **L10** (`r10crop.png`): Üst kat çöktü, iki miğferli tilki ~2 birimden yere düştü, TNT'lerin hemen yanında durdu.
- **L28** (`rec_v5rR28_2.png`): Fenerin üst katı yıkıldı, tilkiler fenerin tabanına düşüp orada kaldı.
- **L13** (`v5_W13.png`, `s2`): Kulübe devrildi, tilki yerinde kaldı.
- **L63, L158, L412, L416:** Yapı çöktü, tilkiler enkazın içinde kaldı (`v5_X63.png`, `v5_X158.png`, `v5_X412.png`, `v5_X416.png`).
- Uçuş etkilenmiyor gibi: L460'ta patlamayla fırlayan tilki sağ duvara kadar uçtu.

**Yan etkiler:**
- **Kod notu:** Sönümleme |v.y| < 0,6 olan her durumda uygulanıyor. Buna yatay uçuşun tepe noktası ve hareket eden bir kalasın üstünde durmak da dahil. Uçuşu ölçülebilir biçimde etkilemiyor, ama "yerde mi" kontrolü (temas ya da raycast) daha doğru olur.
- **Denge etkisi (önemli):**
  - Tilkiler artık çöküşten sonra yuvarlanıp başka cisimlere çarparak hasar almıyor ve enkazın içinde kalıyor.
  - Doğrulanmış duvarların 8'i bu türden (B): yapı çöküyor, tilkiler sağ kalıyor.
  - Elle yapılmış bölümlerdeki düşüşün (§4) en olası nedeni bu.

---

## 4. Yeni sorunlar

| # | Önem | Sorun | Kanıt / ayrıntı | Öneri |
|---|---|---|---|---|
| 1 | **Yüksek** | **"Çatı kirişi asla taş olmaz" düzeltmesi bir şablonda uygulanmamış.** `LevelGen.cs:227` (case 6, "ikiz kule + köprü"): `l.Beam(c.d > 10f ? S : W, …)`. D > 10 olan her bölümde iki kulenin/kulübenin üstündeki köprü kirişi **taş**. | **25 doğrulanmış duvarın 10'u** bu şablon: 43, 55, 90, 143, 297, 298, 333, 356, 376, 490. Ördekler taş köprüye ve taş sütunlara çarpıyor. L43, L90, L356'da ördekler bittiğinde puan **0**. | Satırı `l.Beam(W, …)` ya da `c.Mat(true)` yap. Tek satırlık düzeltme; tahmini 8–10 duvarı kaldırır. |
| 2 | **Yüksek** | **Elle yapılmış 1–40 belirgin zorlaştı:** kazanma 71/120 (v4) → **57/120** (v5). Bu bölümlerin içeriği değişmedi (yalnız L35/L37'ye +1 ördek). | W1 %75 → %61,7, W2 %43,3 → %33,3. Yeni 0/3: 2, 13, 21, 23, 25, 38. Yeni duvarlar: **25, 28, 38**. Muhtemel neden yuvarlanma direnci (tilkiler artık yuvarlanıp kendini/başkasını yaralamıyor) ve zayıf TNT tetiklemesi. 3 denemeyle ~1,8σ; gürültü olabilir, ama yön tutarlı. | `Fox.FixedUpdate` sönümlemesini yumuşat (3,5 → ~2) ya da yalnız temas halindeyken uygula. Elle bölümleri 6–9 denemeyle yeniden ölç. |
| 3 | Orta | **Ağır cisim TNT eşiği (7) gerçek düşüşler için yüksek.** | §1 tablosu: kaya ≥ 2,6, taş küp ≥ 1,7 birim düşmeli. Kaya rafı şablonu kendi TNT'sini patlatamıyor. Testlerde hiç ağır cisim kaynaklı patlama yok. L38 3/3 → 0/9. | `TntObjectImpulse`'ı ~5'e indir ya da kayalara (boulder) ayrı çarpan ver (ör. √kütle yerine kütle). |
| 4 | Düşük | **Eider şok dalgası TNT'yi yalnız ~2,85 birim içinde patlatıyor** (yarıçap 3,8; hasar 8 × (0,25 + k) < 4 dış halkada). Dış halkadaki TNT yalnız itiliyor. | Spec listesinde değil; tutarlılık notu. | Bilerek mi, karar ver; ya TNT'yi tüm yarıçapta patlat ya da şok dalgasını TNT'ye hiç uygulama. |
| 5 | Düşük | **Ördek sayısı formülü bazı bölümlerde ördeği 2–3 azalttı:** 65 bölüm azaldı, 105 arttı. Örnek: L86 7 → 5, L88 7 → 4, L140 9 → 6. | Bunlar çoğunlukla 7 tilki tavanıyla küçülen kaleler. Azalan grubun kazanması %55 → %74 (iyi). Ama **L86 duvar oldu**: 5 tilki, 5 ördek, taş alt katlı kule. | `need` formülüne alt sınır olarak "tilki sayısı" ekle (`Mathf.Max(…, c.foxes)`). |
| 6 | Düşük (dev aracı) | `egg 1.3 -2.5` komutu L5'te log'a düştü (`Editor.log`), ama hiçbir kare kaydında yumurta ya da patlama görünmedi. `egg 6 1` (L8) çalıştı. | `_out/v5m/rE5.png`, `rE5c.png` | Yumurta zemin altında ya da başka bir collider içinde doğmuş olabilir. `EggBomb.Drop`'a başlangıç çakışma kontrolü eklenebilir. Oyuncuyu etkilemez. |
| 7 | Düşük | L73 ile L77 başlangıç görüntüsü aynı görünüyor (aynı yapı, aynı tilki dizilişi). | `_out/v5m/scan2.png` | `dup_bad.txt` kontrolüne tekrar bakılmalı. |
| 8 | Not | Eski sorun sürüyor: L187'de çatı tilkisi ilk karede düşüyor (v4 §3). L187 artık 0/3 + 4/6, duvar değil. | – | – |

---

## 5. Dünya bazında tablo (UPG = 1, 3 deneme; v4 ile karşılaştırma)

- "Duvar" = 0/3 + tekrar 0/6 (toplam 0/9).
- Boss sütununda tekrar sonucu `+x/6` olarak verildi.

| Dünya | Bölümler | Kazanma v5 | Kazanma v4 | Fark | 0/3 | Duvar (0/9) | Boss | Tek satır karar |
|---|---|---|---|---|---|---|---|---|
| 1 | 1–20 | %61,7 | %75,0 | −13 | 2 (2, 13) | – | 3/3 | Zorlaştı; L2 ve L13 tekrarda 2/6 ve 3/6, duvar değil. |
| 2 | 21–40 | **%33,3** | %43,3 | −10 | 9 | 25, 28, 35, 37, 38 | 1/3 | **Oyunun en zor dünyası; daha da kötüleşti.** L35/L37'ye +1 ördek yetmedi (taş kabuk). |
| 3 | 41–60 | %48,3 | %65,0 | −17 | 7 | 42, 43, 55 | 0/3, +3/6 | Belirgin düştü; 43 ve 55 taş köprü. |
| 4 | 61–80 | %50,0 | %63,3 | −13 | 5 | 63 | 3/3 | L72 duvar olmaktan çıktı (5 tilki). L63 çöküşte sağ kalan tilkiler. |
| 5 | 81–100 | %43,3 | %55,0 | −12 | 6 | 86, 89, 90, 100 | **0/3, +0/6** | **Boss L100 hâlâ duvar** (8 tilki, 9 ördek). |
| 6 | 101–120 | %68,3 | %61,7 | +7 | 2 | – | 0/3, +1/6 | Duvarlar (102, 105, 117) kalktı. |
| 7 | 121–140 | %81,7 | %85,0 | −3 | 2 | – | 3/3 | Kolay. |
| 8 | 141–160 | %71,7 | %66,7 | +5 | 3 | 143, 158 | 3/3 | 143 taş köprü, 158 çöküş. |
| 9 | 161–180 | %76,7 | %85,0 | −8 | 2 | – | 1/3 | İyi. |
| 10 | 181–200 | %76,7 | %78,3 | −2 | 2 | – | 3/3 | 187 düzeldi (4/6). |
| 11 | 201–220 | %88,3 | %85,0 | +3 | 0 | – | 3/3 | Fazla kolay. |
| 12 | 221–240 | %81,7 | %80,0 | +2 | 1 | – | 3/3 | Kolay. |
| 13 | 241–260 | %83,3 | %83,3 | 0 | 2 | – | 3/3 | Kolay. |
| 14 | 261–280 | %81,7 | %80,0 | +2 | 0 | – | 3/3 | Kolay, duvarsız. |
| 15 | 281–300 | %61,7 | %68,3 | −7 | 6 | 297, 298 | 3/3 | Geç oyunun en zor dünyası; ikisi de taş köprü. |
| 16 | 301–320 | %83,3 | %70,0 | +13 | 0 | – | 3/3 | İyileşti. |
| 17 | 321–340 | %71,7 | %73,3 | −2 | 2 | 333 | 3/3 | 333 taş köprülü kulübeler. |
| 18 | 341–360 | %80,0 | %83,3 | −3 | 2 | 356, 357 | 3/3 | İki yeni duvar (köprü + taş kule). |
| 19 | 361–380 | %75,0 | %68,3 | +7 | 2 | 376 | 2/3 | 364 düzeldi; 376 taş köprü. |
| 20 | 381–400 | %83,3 | %76,7 | +7 | 0 | – | 3/3 | İyi; boss düzeldi. |
| 21 | 401–420 | %78,3 | %80,0 | −2 | 3 | 412, 416 | 3/3 | Çöküşte sağ kalan miğferliler. |
| 22 | 421–440 | %70,0 | %63,3 | +7 | 3 | 436 | 3/3 | Orta. |
| 23 | 441–460 | %75,0 | %63,3 | +12 | 2 | – | 3/3 | İyileşti; boss artık 3/3 (v4: 0/3). |
| 24 | 461–480 | %81,7 | %83,3 | −2 | 0 | – | 3/3 | Kolay, duvarsız. |
| 25 | 481–500 | %81,7 | %73,3 | +8 | 1 | 490 | 3/3 | Final kolay; 490 taş köprü. |
| **Toplam** | 1–500 | **%71,5** | %72,4 | −1 | **64** (v4: 65) | **25** (v4: 33) | 1 duvar boss (L100) | |

**Ek gözlemler:**
- **100'lük bloklarda kazanma:**

| Blok | v5 | v4 |
|---|---|---|
| 1–100 | **%47** | %60 |
| 101–200 | %75 | %75 |
| 201–300 | %79 | %79 |
| 301–400 | %79 | %74 |
| 401–500 | %77 | %73 |

  Zorluk eğrisi ters: oyunun en zor kısmı ilk 100 bölüm. Yeni oyuncu için risk.
- **Ördek sayısına göre (L41+):**

| Ördek | Bölüm | Kazanma | 0/3 |
|---|---|---|---|
| 4 | 31 | %76 | 2 |
| 5 | 232 | %77 | 22 |
| 6 | 137 | %70 | 21 |
| 7 | 51 | %68 | 5 |
| 8 | 7 | %71 | 2 |
| 9 | 2 | %33 | 1 |

- **Ördek sayısı değişen 168 üretilmiş bölüm:** %65,7 → %71,0. Değişmeyen 292 bölüm: %78,1 → %75,1. Ördek ayarı yardımcı oldu; fizik değişiklikleri genel olarak hafif zorlaştırdı.
- **3 yıldız alınan bölüm:** 238 (v4: 263).
- **v4 duvarlarından 23'ü kalktı:** 62, 72, 83, 102, 105, 117, 144, 187, 204, 227, 252, 277, 282, 289, 309, 364, 386, 399, 423, 443, 455, 469, 489.
- **Süren 10 duvar:** 35, 37, 43, 55, 63, 90, 100, 297, 298, 412.
- **Yeni 15 duvar:** 25, 28, 38, 42, 86, 89, 143, 158, 333, 356, 357, 376, 416, 436, 490.

---

## 6. Doğrulanmış duvarlar (0/9): neden kazanılmıyor ve öneriler

- Hepsi ördekler bitene kadar oynandı. **Her oyunda kalan tilkiler görünür ve HUD ile aynı sayıda.** Duvar dışına ya da ekran dışına çıkan tilki yok.
- **Neden türleri:**
  - **A. Taş kabuk:** Taş köprü veya çatı, taş sütunlar. Ördek hasarı çok düşük.
  - **A6.** A türünün özel hâli: case 6 taş köprü kirişi (§4 #1).
  - **B. Çöküşten sağ çıkan tilkiler:** Yapı yıkılıyor, miğferli tilkiler enkazda sağ kalıyor; yuvarlanma direnciyle hareketsizler.
  - **C. Tilki/ördek oranı:** Tilki sayısı ördek sayısına eşit ya da fazla.
  - **T.** TNT'nin tetiklenmemesi.

| Bölüm | Tilki / Ördek | Kalan (HUD = ekran) | Tür | Neden | Önerilen düzeltme |
|---|---|---|---|---|---|
| 25 FİRAVUN TİLKİ | 6 / 5 | 6 (puan 0) | A + T | Patron ve TNT'ler taş çerçevenin içinde. Ördekler dış taş sütunlara çarpıp sekiyor, TNT'ye ulaşmıyor. | Alt çerçeve sütunlarını tahta yap ya da TNT'yi sütunun dışına koy; +1 Kara Ördek. |
| 28 DENİZ FENERİ | 5 / 4 | 1 | C | Fener tek atışta yıkılıyor, ama sağdaki kulübe tilkisi 3 atışa dayanıyor. | +1 ördek (kıl payı). |
| 35 ŞOGUN TİLKİ | 7 / 7 | 7 (puan 1.100) | A | Çift taş kat (`Frame(S,S)` + `Beam(S)`). +1 ördek hiçbir şey değiştirmedi. | `b = Beam(S…)` → `W`; alt `Frame(S,S)` → `Frame(S,W)`. |
| 37 OBSİDYEN KULE | 6 / 6 | 5 | A | 3 katlı tamamen taş kule (`Frame(S,S)` ×2). Hasar ≈ 0. | Orta katı `Frame(W,W)` yap. |
| 38 LAV KÖPRÜSÜ | 4 / 4 | 4 | T + C | v4'te 3/3. Yerdeki TNT tilkinin yanında, kaya ve çerçeveler artık patlatmıyor. | `TntObjectImpulse` ~5 ya da +1 ördek. |
| 42 | 6 / 5 | 3 | C + B | Tilki ördekten fazla, kule çöküşünden sağ kalanlar. | `need ≥ tilki sayısı`. |
| 43 | 3 / 4 | 3 (puan 0) | **A6** | Taş köprü + taş sütunlu iki kulübe (v2'den beri duvar). | §4 #1. |
| 55 | 5 / 5 | 4 | **A6** | İkiz kule + taş köprü. | §4 #1. |
| 63 | 6 / 5 | 5 | B (+ A) | Yapı tamamen çöktü, 5 miğferli tilki enkazda sağ. | +1 ördek; miğferli oranını düşür ya da çöküş hasarını artır. |
| 86 | 5 / 5 | 4 | A + C | v4'te 7 ördek, şimdi 5. Alt katları taş kule + yan kulübe. | `need ≥ tilki sayısı + 1`. |
| 89 | 5 / 6 | 3 | B + T | Yerdeki TNT hiç patlamadı; tilkiler sağdaki kuleden düşüp sağ kaldı. | TNT eşiği; +1 ördek. |
| 90 | 5 / 5 | 5 (puan 0) | **A6** | İkiz taş kule + taş köprü. | §4 #1. |
| 100 (boss) | 8 / 9 | 3 | B + C | Patron + miğferliler; ördekler bitiyor. | Boss'ta en çok 7 tilki ya da +1 Kara Ördek. |
| 143 | 6 / 7 | 4 | **A6** | Taş köprülü ikiz kule. | §4 #1. |
| 158 | 4 / 5 | 4 (puan 500) | B | Kule çöktü, 4 tilki enkazda sağ. | Yuvarlanma sönümünü yumuşat; +1 ördek. |
| 297 | 5 / 6 | 5 | **A6** | Taş köprülü ikiz kule (v4'ten beri). | §4 #1. |
| 298 | 6 / 6 | 3 | **A6** | Gökdelen + taş köprülü ikiz kulübe. | §4 #1. |
| 333 | 3 / 5 | 3 | **A6** | Taş köprülü iki kulübe, miğferli. | §4 #1. |
| 356 | 6 / 6 | 6 (puan 0) | **A6** | Taş köprülü ikiz kule. | §4 #1. |
| 357 | 6 / 6 | 4 | A | Gökdelenin alt yarısı taş sütun, kule ayakta kalıyor. | Gökdelen alt katlarını tahta + taş karışık yap. |
| 376 | 5 / 6 | 4 | **A6** | Taş köprülü ikiz kule. | §4 #1. |
| 412 | 6 / 7 | 3 | B | Köprü çöktü, miğferliler enkazda sağ (v4'ten beri). | Çöküş hasarı / sönüm. |
| 416 | 5 / 6 | 3 | B | Kule çöktü, 3 tilki sağ. | Aynı. |
| 436 | 4 / 5 | 2 | B + C | Köprü yıkıldı, 2 tilki sağ (kıl payı). | +1 ördek. |
| 490 | 6 / 6 | 4 | **A6** | Taş köprülü ikiz kulübe + kule. | §4 #1. |

**Özet:**
- 25 duvarın 10'u tek satırlık `LevelGen.cs:227` düzeltmesiyle büyük olasılıkla kalkar.
- 4'ü (25, 35, 37, 357) elle ya da şablonla yapılmış tam taş yapılar.
- 7'si çöküşten sağ kalan tilkiler (B); yuvarlanma sönümü ve miğfer ayarıyla ilgili.
- Kalanlar oran (C) ve TNT eşiği (T) kaynaklı.

---

## 7. Düzeltme önerileri (öncelik sırasıyla)

1. **`LevelGen.cs:227`:** Case 6 köprü kirişini tahta yap (`W` ya da `c.Mat(true)`).
2. **Tilki yuvarlanma sönümü:**
   - Yalnız zemin/blok temasındayken uygula.
   - Katsayıyı 3,5'ten ~2'ye indir.
   - Elle yapılmış 1–40 ve W2–W5'i 6+ denemeyle yeniden ölç.
3. **TNT:** `TntObjectImpulse` 7 → ~5, ya da kayalar için kütle çarpanını artır. Böylece "düşen taş/kaya patlatır" kuralı ve "kaya rafı" şablonu gerçekten çalışır.
4. **Ördek formülü:** `need = Mathf.Max(need, c.foxes)` alt sınırı (L42, L86 gibi oran duvarları için).
5. **Elle bölümler:**
   - L35: taş kiriş → tahta.
   - L37: orta kat tahta.
   - L25: TNT'yi dışarı al ya da sütunlar tahta.
   - L28 ve L38: +1 ördek.
6. **Boss L100:** 7 tilki ya da +1 Kara Ördek.
7. **Küçük işler:**
   - `LevelLoader.cs` duvar yorumunu güncelle.
   - Eider şok dalgası ile TNT davranışını netleştir.
   - `egg` dev komutundaki görünmeyen yumurta.
   - L73/L77 tekrarını kontrol et.
