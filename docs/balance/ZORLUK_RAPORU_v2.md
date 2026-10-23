# DuckThis Feather Fury: Zorluk ve Denge Raporu v2 (denge düzeltmeleri sonrası)

**Test tarihi:** 23.10.2026
**Yöntem:** v1 ile aynı. Unity editöründeki yerleşik otomatik oyuncu (`autotest`) kullanıldı.
- 500 bölümün tamamı `autotest A B 3 1` ile oynandı: her bölüm 3 deneme, UPG=1. Toplam 1500 deneme yapıldı.
- Mağazasız karşılaştırma `autotest A B 2 0` ile yapıldı: 1, 3, 5, 10, 15, 20 ve 25. dünyalar, 2'şer deneme (280 deneme).
- Şüpheli bölümler 6'şar denemeyle tekrar oynandı:
  - 1–3. dünyalarda 0/3 kalanlar: 6, 23, 27, 34–37, 43, 46, 51–55.
  - 0/3 kalan boss'lar: 40, 100, 120, 160, 300, 400, 460.
  - Ayrıca L13, L18 ve L99.
- Ham veri `levels_v2.csv` dosyasında (UPG=1 ana koşu, bölüm başına bir satır). Bot çıktılarının kopyaları projede şu dosyalarda:
  - `_out/v2_world_NN.txt`
  - `_out/v2_noupg_NN.txt`
  - `_out/v2_re_*.txt`
  - `_out/v2_all.csv` (tüm koşuların birleşik dökümü)

> **Yorumlama ölçeği (v1 ile aynı):**
> - Bot kazanma oranı ≥%70: insan için kolay.
> - %40–70: orta.
> - %20–40: zor.
> - <%15: duvar.
>
> Bölüm başına 3 deneme olduğu için tek bölüm sonuçları gürültülü. Bu yüzden bir bölüme "duvar" demek için tekrar testi (0/9 ya da 0/11) ya da art arda gelen 0/3 serileri esas alındı.

---

## 1. Genel bulgular (v1 → v2)

| Ölçüt | v1 | v2 |
|---|---|---|
| Toplam bot kazanma (UPG=1) | 629/1500 = %41,9 | **739/1500 = %49,3** |
| 0/3 kalan bölüm | 188 (CSV'ye göre; v1 metni 184 diyor) | **145** |
| Stabilite | 500/500 SAĞLAM | 500/500 SAĞLAM |
| Ördek dağılımı | 7 ördekli: 207 bölüm | 4: 40, 5: 211, 6: 180, 7: 57, 8: 12 (verilen dağılımla birebir aynı) |
| 7 ördekli bölümler (L41+) | 207 bölüm, %28,7 kazanma, 112 tanesi 0/3 | **54 bölüm, %54,9 kazanma, 13 tanesi 0/3** |
| 6 ördekli bölümler (L41+) | 140 bölüm, %43,1, 45 tanesi 0/3 | **175 bölüm, %33,1, 80 tanesi 0/3** ← yeni sorun |
| 5 ördekli bölümler (L41+) | 81 bölüm, %65 | 195 bölüm, %59, 37 tanesi 0/3 |
| Yıldız dağılımı (L41+, kazanılanlar) | 1★ %58, 2★ %20, 3★ %22 | **1★ %32, 2★ %23, 3★ %45** |
| Kümülatif altın (bot ortalaması) | L100: 8.800, L200: 22.700, L500: 71.600 | L100: 8.960, L200: 21.300, L300: 35.000, L400: 45.750, **L500: 55.800** |
| 0/3 kalan boss | 9 | 7 (bunların 5'i 6 denemelik tekrarda da 0/6) |
| En uzun 0/3 serisi | 8 bölüm (49–56 ve 357–364) | ana koşuda 4 bölüm (129–132, 385–388, 410–413, 450–453); tekrar testiyle **51–55 = 5 bölüm (1/30)** |

**Kısa özet:**
- Düzeltmeler büyük ölçüde işe yaradı. Genel kazanma +7 puan arttı, 0/3 sayısı %23 azaldı, 8 bölümlük duvar serileri ortadan kalktı ve yıldız dağılımı sağlıklı hale geldi.
- Ancak üç sorun kaldı ya da yeni çıktı:
  1. **Zorluk 7 ördekli dev kalelerden 6 ördekli bölümlere kaydı.** Bu bölümlerde kazanma %33 ve 145 duvarın 80'i bu grupta. Kaleyi küçültme kuralı dev kaleleri "orta-dev" kalelere çevirmiş, ama bu kaleler hâlâ çok zor.
  2. **3. ve 5. dünyalar hâlâ en zor iki dünya.** 5. dünya (%23) artık oyunun en zor dünyası. Kurallar her yerde uygulanmıyor: dünya açılış bölümlerinden 4'ü 7 ördekli dev kale (61, 81, 141, 241).
  3. **Mağazanın etkisi neredeyse kayboldu.** Mağazasız kazanma 10, 15, 20 ve 25. dünyalarda UPG=1 ile aynı. 3. ve 5. dünyalarda ise mağazasız oran daha yüksek (bkz. §3).

---

## 2. Dünya bazında puanlar

Sütunlar:
- **Kazanma:** UPG=1 ile ortalama bot kazanma oranı.
- **Mağazasız:** UPG=0 ile kazanma oranı (yalnızca test edilen dünyalar).
- **0/3:** dünyada hiç kazanılamayan bölüm sayısı.
- **Boss:** ana koşu sonucu; parantez içinde v1.
- **Altın/bölüm:** deneme başına ortalama altın (kayıplar dahil).
- **Zorluk /10:** 10 en zor.
- **Denge /10:** akış ve adalet; 10 en iyi.

Her hücrede değer **v2 (v1)** biçiminde.

| Dünya | Bölümler | Kazanma | Mağazasız | 0/3 | Boss | En zor | En kolay | Altın/bölüm | **Zorluk /10** | **Denge /10** | Tek satır karar |
|---|---|---|---|---|---|---|---|---|---|---|---|
| 1 | 1–20 | %65,0 (%61,7) | %85 (%60) | 2 (2) | 2/3 (2/3) | L13 (0/9, 7 ördek) | L7, L8, L17 (3/3, 1 atış) | 62 (58) | **3 (3)** | **6 (6)** | Dostane; tek gerçek duvar L13. |
| 2 | 21–40 | %43,3 (%43,3) | – | 6 (5) | 0/3, tekrarda 1/6 (0/3) | L34, L35, L37 (her biri 0/9) | L22, L29 (1 atış) | 76 (73) | **5 (5)** | **4 (5)** | İkinci yarısı (34–40) hâlâ sert. |
| 3 | 41–60 | %31,7 (%21,7) | %40 (–) | 6 (11) | 1/3 (2/3) | L43 (0/11), 51–55 (tekrarda 1/30) | L41 (3/3, 1 atış) | 91 (75) | **7 (8)** | **3 (2)** | Düzeldi ama 51–55 hâlâ 5 bölümlük duvar. |
| 4 | 61–80 | %58,3 (%43,3) | – | 5 (9) | 2/3 (2/3) | L64 (7 ördek), L63/66/72/75 | L65, L70, L77 (1 atış) | 130 (111) | **5 (6)** | **5 (4)** | 3. dünyadan belirgin kolay; açılış L61 dev kale. |
| 5 | 81–100 | **%23,3** (%26,7) | %48 (%22) | **12** (12) | 0/3, tekrarda 0/6 (1/3) | L81 açılış (7 ördek, 0/3), 88–90, 95–96, L100 | L85, L94 (3/3, 2 atış) | 89 (124) | **8 (7)** | **2 (3)** | **Oyunun en kötü dünyası**; düzeltmeler etki etmemiş. |
| 6 | 101–120 | %40,0 (%26,7) | – | 7 (10) | 0/3, tekrarda 0/6 (1/3) | L120 Boss (0/9), 102–108 arası bir bölüm atlayarak 0/3 | L113 (2 atış) | 109 (137) | **6 (7)** | **5 (4)** | Belirgin iyileşme; boss duvar. |
| 7 | 121–140 | %38,3 (%26,7) | – | 9 (12) | 3/3 (2/3) | 129–132 (4 seri) | L121, L125 | 109 (122) | **7 (8)** | **4 (3)** | 6'lı seri 4'e indi, hâlâ en sert orta-oyun serisi. |
| 8 | 141–160 | %36,7 (%28,3) | – | 6 (11) | 0/3, tekrarda 0/6 (0/3) | L160 Boss (0/9, 6 ördek), 144–150 | L156 (1 atış) | 111 (132) | **7 (7)** | **5 (4)** | Daha az duvar; boss adil değil. |
| 9 | 161–180 | %51,7 (%40,0) | – | 2 (8) | 1/3 (0/3) | L171, L173 | L165, L177 | 141 (130) | **5 (6)** | **7 (5)** | En iyi akan dünyalardan biri. |
| 10 | 181–200 | %53,3 (%46,7) | %55 (%28) | 7 (7) | 1/3 (1/3) | L181 açılış (0/3), 198–199 (boss öncesi 0/3) | L188 | 147 (172) | **5 (6)** | **5 (5)** | Boss öncesi yumuşatma burada çalışmıyor. |
| 11 | 201–220 | **%75,0** (%55,0) | – | 3 (7) | 3/3 (0/3) | 202–203 | L220 Boss (1 atış!), L201 | 155 (171) | **3 (5)** | **3 (4)** | **Fazla kolay**; W1'den bile kolay, boss 1 atış. |
| 12 | 221–240 | %68,3 (%50,0) | – | 2 (4) | 3/3 (2/3) | L221 açılış (0/3), L227 | L228 (1 atış) | 160 (209) | **4 (5)** | **5 (6)** | Kolay; W10'dan kolay. |
| 13 | 241–260 | %53,3 (%46,7) | – | 5 (6) | 3/3 (2/3) | 249–250, L245 | L244 (1 atış) | 121 (128) | **5 (5)** | **6 (6)** | Dengeli. |
| 14 | 261–280 | %48,3 (%38,3) | – | 6 (8) | 3/3 (1/3) | 271–272, 276–277 (L276 4 ördekle 0/3) | L261, L274 | 135 (170) | **6 (6)** | **6 (6)** | Dengeli; doğru yönde. |
| 15 | 281–300 | %60,0 (%48,3) | %58 (%28) | 6 (8) | 0/3, tekrarda 1/6 (0/3) | L300 Boss (1/9, yalnızca 5 ördek), 282–283 | L290 (1 atış) | 116 (139) | **5 (6)** | **5 (5)** | Boss'a ekstra ördek verilmemiş. |
| 16 | 301–320 | %60,0 (%58,3) | – | 3 (4) | 1/3 (1/3) | L306, L309, L316 | L305 | 116 (165) | **5 (4)** | **6 (4)** | Artık W15'ten kolay değil; düşüş giderildi. |
| 17 | 321–340 | %50,0 (%36,7) | – | 5 (8) | 2/3 (1/3) | 335–336, L339 | L329 | 105 (162) | **6 (6)** | **6 (5)** | Dengeli. |
| 18 | 341–360 | %53,3 (%41,7) | – | 6 (9) | 1/3 (0/3) | 355–356, L358 | L341, L349, L350 | 107 (155) | **5 (6)** | **6 (4)** | 357–360 duvarı çözüldü. |
| 19 | 361–380 | %45,0 (%35,0) | – | 7 (9) | 2/3 (1/3) | L361 açılış (0/3), L364, 369–370 | L374 (1 atış) | 104 (163) | **6 (7)** | **5 (4)** | 4'lü açılış duvarı dağıldı ama açılış hâlâ 0/3. |
| 20 | 381–400 | %50,0 (%41,7) | %52 (%35) | 6 (7) | 0/3, tekrarda 0/6 (0/3) | **385–388 (4 seri, 6'şar ördek)**, L400 Boss (0/9) | L391, L393 (1 atış) | 103 (164) | **6 (6)** | **4 (5)** | Yeni 4'lü seri ve duvar boss. |
| 21 | 401–420 | %46,7 (**%61,7**) | – | 6 (3) | 3/3 (2/3) | **410–413 (4 seri)**, L419 (7 ördek) | L420 Boss (3/3) | 108 (191) | **6 (4)** | **5 (4)** | Geç-oyun kolaylaşması giderildi; yerine 4'lü seri geldi. |
| 22 | 421–440 | %43,3 (%41,7) | – | 8 (8) | 1/3 (1/3) | L421 açılış, 423–424, 433–434 | L422 (2 atış) | 103 (165) | **6 (6)** | **5 (5)** | Orta-zor ve dalgalı. |
| 23 | 441–460 | **%40,0** (%45,0) | – | **10** (6) | 0/3, tekrarda 0/6 (0/3) | **450–453 (4 seri)**, L460 Boss (0/9) | L444 (1 atış) | 84 (142) | **7 (6)** | **4 (5)** | Geç oyunun en zoru; duvarlar fazla. |
| 24 | 461–480 | %43,3 (%41,7) | – | 5 (6) | 1/3 (1/3) | 468–470 | L465, L475 | 109 (171) | **6 (6)** | **6 (5)** | Dengeli, orta-zor. |
| 25 | 481–500 | %53,3 (%41,7) | %50 (%20) | 5 (8) | 2/3 (0/3) | 488–489, L492, L494 | L484, L496 | 96 (152) | **5 (7)** | **5 (4)** | Duvar serisi gitti, ama final W21–24'ten kolay. |

**Mağazasız toplam:** 155/280 = %55. 1. dünyada UPG etkisi yok (seviye 0). 1. dünyadaki %85 mağazasız / %65 UPG=1 farkı 2 ve 3 denemelik örneklerin gürültüsünden geliyor.

---

## 3. Önceki 12 önerinin durumu

| # | Öneri (v1) | Durum | Kanıt |
|---|---|---|---|
| 1 | L41–60 geçişini yumuşat | **Kısmen çözüldü** | W3 kazanma %21,7 → %31,7, 0/3 sayısı 11 → 6, 49–56 sekizlisi kırıldı. Ancak **51, 52, 54 ve 55 tekrar testinde 0/6, 53 ise 1/6**. 51–55 toplamı 1/30 ile fiilen 5 bölümlük bir duvar. Mağazasız koşuda da 50–55 arası 2 denemede yalnızca 1 galibiyet. 57–60 arası 6–7 ördekli ve her biri 1/3. W3 hâlâ W2 ve W4'ten belirgin zor. |
| 2 | L43 ve L99'u düzelt | **Kısmen çözüldü** | **L99 çözüldü:** 4 ördekle UPG 1/3 + tekrar 4/6 + mağazasız 1/2 = 6/11, 3★ da alınabiliyor. **L43 çözülmedi:** 4 ördek olmasına rağmen 0/3 + 0/6 + 0/2 = **0/11**. Ortalama hasar yalnızca ~400–580 ve tilkiye ulaşılamıyor gibi. Ördek eklemek yetmemiş; kale ya da tilki yerleşimi elle incelenmeli. |
| 3 | L13 ve L18 duvarlarını kaldır | **Kısmen çözüldü** (L18) / **çözülmedi** (L13) | **L13 "Kale Duvarı":** 7 ördekle UPG 0/3 + tekrar 0/6 = **0/9**, mağazasız 1/2. Ekstra ördek etkisiz; ortalama hasar ~550 ve duvar delinemiyor. **L18 "Piramit":** 1/3 + 1/6 + 1/2 = 3/11 (%27). Hâlâ zor, ama artık duvar değil. **L40 Boss:** 0/3 + 1/6 = 1/9; v1'de de 1/9'du, değişmedi. |
| 4 | 7 ördekli dev kaleleri sınırla | **Çözüldü** (yan etkisiyle) | Boss dışında her dünyada en fazla 4 dev kale var ve art arda en fazla 2 tane geliyor (kural tutuyor). 7 ördekli bölümler 207 → 54, kazanma %28,7 → %54,9, 0/3 sayısı 112 → 13. 357–364 ve 492–495 serileri tamamen kayboldu. 129–134 serisi 129–132'ye (4 bölüm) indi. **Yan etki:** küçültülen kaleler 6 ördekli oldu ve bu grup (175 bölüm) %33 kazanma ile 80 duvar barındırıyor. |
| 5 | "Seri koruyucu" (2 zor → 1 kolay) | **Kısmen çözüldü** | 8 ve 6 bölümlük seriler yok. Ancak ana koşuda 4 bölümlük 0/3 serileri sürüyor: 129–132, 385–388, 410–413, 450–453. Ördek sayısına bakınca kural birçok yerde ihlal ediliyor: art arda 3 ya da daha fazla ≥6 ördekli bölüm var (örneğin 385–389 beş bölüm, 474–478 beş bölüm, 57–60, 106–108, 164–167). Kural "zor" bölümü ördek sayısından farklı tanımlıyorsa, bu tanım gerçek zorluğu yakalamıyor. |
| 6 | L100 sonrası eğriyi dikleştir | **Kısmen çözüldü** | 100 bölümlük bloklarda kazanma: 1–100 %44,3 · 101–200 %44,0 · 201–300 **%61,0** · 301–400 %51,7 · 401–500 %45,3. W21 kolaylaşması giderildi (%61,7 → %46,7) ve W16 düşüşü yok. 300'den sonra eğri doğru yönde. Ancak **W11 (%75) ve W12 (%68) artık oyunun en kolay dünyaları**: W10'dan 22 puan kolay, W1'den bile kolay. Ekranda görünen D değeri hâlâ L500'de 59,0; büyüme gizli tilki ve blok dayanıklılığından geliyor. |
| 7 | Yükseltme tavanını geç oyuna yay | **Çözülmedi** (bu turda dokunulmadı); sorun büyüdü | UPG=1 ile mağazasız oran farkı: W10 %53 / %55, W15 %60 / %58, W20 %50 / %52, W25 %53 / %50. **Yükseltmenin ölçülebilir bir etkisi kalmadı.** v1'de fark +15 ile +22 puandı. W3 (%32 / %40) ve W5 (%23 / **%48**) dünyalarında mağazasız bot daha iyi oynuyor. Örneğin L81, 88, 89 ve 92 UPG=1 ile 0/3, mağazasız 2/2. Bu durum ya gürültüden ya da UPG=1 profilinin (yeni ördek türü veya güç ayarı) botun atışını bozmasından kaynaklanıyor olabilir. Kontrol edilmeli. |
| 8 | Geç oyun altınını sınırla | **Çözüldü** | W21–25 için altın/bölüm 152–191 → 84–109. Kümülatif altın L500'de 71.600 → 55.800, L300–500 aralığında 32.600 → 20.700. Altın W9–12'de 141–160 ile zirve yapıp sonra düşüyor. Yan not: W5–8'de de altın 121–137'den 89–111'e düştü; muhtemelen daha küçük kaleler daha az hasar ve altın veriyor. |
| 9 | 2★ / 3★ eşiklerini düşür | **Çözüldü** | Kazanılan L41+ bölümlerde 1★ oranı %58 → %32, 3★ oranı %22 → %45. Boss'larda kazanılsa da çoğunlukla hâlâ 1★ alınıyor (örneğin L180 100.000 puanla 2★, L200 1★, L260 1★). |
| 10 | Boss'ları zor ama adil yap | **Kısmen çözüldü** | 0/3 boss sayısı 9 → 7. Ancak 6 denemelik tekrarda **L100, L120, L160, L400 ve L460 0/6** (her biri toplam 0/9), L40 ve L300 1/6. **L300 (5 ördek), L160 ve L400 (6 ördek)** dünyadaki normal bölümlerden fazla ördeğe sahip değil; "boss'a +1 ördek" her yerde uygulanmamış. Buna karşılık L220 ve L420 5 ördekle 3/3, L220 tek atışta kazanılıyor. Boss zorluğu tutarsız. Boss öncesi yumuşatma: 357–359 düzeldi, ama 198–199, 399 ve 419 hâlâ 0/3. |
| 11 | W5 ve W19 açılış duvarlarını kır | **Kısmen çözüldü** (W19) / **çözülmedi** (W5) | **W5:** L81 hâlâ **7 ördekli dev kale ve 0/3**. 81–84 dörtlüsü 0, 1, 0, 0 galibiyetle v1'e çok yakın. **W19:** 361–364 serisi kırıldı (0, 1, 1, 0) ama L361 hâlâ 0/3. "Dünyanın ilk bölümü hafif" kuralı ihlal ediliyor: L61, L81, L141 ve L241 7 ördekli; L181, L221, L361 ve L421 6 ördekli ve 0/3. |
| 12 | Final için hafif bir artış | **Çözülmedi** | W21 %46,7 → W22 %43,3 → W23 **%40,0** → W24 %43,3 → W25 **%53,3**. Final dünyası son beş dünyanın en kolayı ve mağazasız oranı %20'den %50'ye çıktı. Zirve W23'te kalıyor; final bir "tırmanış" hissi vermiyor. |

---

## 4. Genel değerlendirme

### Zorluk artık düzgün artıyor mu?
**Kısmen.**
- Bot kazanma eğrisi (UPG=1, dünya 1 → 25): %65 → 43 → **32** → 58 → **23** → 40 → 38 → 37 → 52 → 53 → **75** → 68 → 53 → 48 → 60 → 60 → 50 → 53 → 45 → 50 → 47 → 43 → 40 → 43 → 53.
- **Olumlu:** 11. dünyadan 23. dünyaya kadar oranın %75'ten %40'a inmesi, zorluğun geç oyunda istenen gibi kademeli arttığını gösteriyor. v1'deki W16 ve W21 kolaylaşmaları ve 8 bölümlük duvarlar yok. Dalgalanmanın genliği belirgin biçimde azaldı.
- **Olumsuz:**
  1. İlk 100 bölüm hâlâ testere dişi gibi: W3 ve W5 dipler, W4 bir tepe.
  2. W9–12'de belirgin bir kolaylaşma var (%52 → %75). Oyuncu 200. bölüm civarında oyunu W1'den kolay buluyor.
  3. Final (W25), W23'ten kolay.

### Kalan duvarlar nerede?
- **Kesin duvarlar (tekrar testiyle ≥0/9 ya da 0/11):**
  - L13
  - L43
  - L34, L35, L37
  - L51, L52, L54, L55
  - L100, L120, L160, L400, L460 (boss'lar)
- **4 bölümlük 0/3 serileri:** 129–132, 385–388, 410–413, 450–453.
- **Dünya geneli duvar:** W5 (%23, 12 tane 0/3; 81–100 arasında 2/3'ün üzerinde kazanılan bölüm yalnızca 85, 94 ve 97).
- **Yapısal neden:** 6 ördekli bölümler (175 tanesi %33 kazanıyor). Örneğin W20'deki 385–388 bölümlerinin hepsi 6 ördekli, 2★ eşiği 51.500–60.000 ve hepsi 0/3.

### Erken oyun dostane mi?
**1. dünya için evet, 2–3. dünyalar için hâlâ hayır.**
- W1 %65 kazanıyor (mağazasız %85).
- Tek gerçek duvar L13 (0/9). L6 ilk koşuda 0/3 çıktı, ama tekrarda 2/6 ve mağazasız 2/2 kazandı; bu gürültü.
- L18 artık "zor" seviyesinde (%27).
- W2'nin ikinci yarısı (34, 35, 37 ve boss L40) 0/9 ile v1'den daha sert. Oyuncu ilk 40 bölümde 4 duvarla karşılaşıyor.
- W3 iyileşti, ama 51–55 serisi oyuncu kaybı riski taşıyor.

### Geç oyun yeterince zor mu?
**Evet, ama zirvesi yanlış yerde.**
- W21–25 ortalaması %45,3 ile "orta-zor" seviyesinde ve W11–20 ortalamasından (%56) zor. Bu, v1'e göre doğru yönde bir değişim.
- Altın kısıtlaması ve tilki ve blok dayanıklılığının büyümesi işe yarıyor.
- Ancak final dünyası (W25 %53) bir öncekilerden kolay.
- Mağazanın geç oyunda bir fark yaratmaması (UPG ≈ mağazasız) yükseltme satın almanın anlamını azaltıyor.

### Ek ördek kaç bölümde alınabilir (300–1000 altın)?

| Dünya | Ort. altın/bölüm (v2) | 300 altın | 1000 altın | v1'e göre |
|---|---|---|---|---|
| 1 | ~62 | ~5 bölüm | ~16 bölüm | aynı |
| 2–3 | ~76–91 | ~4 | ~11–13 | biraz daha hızlı |
| 4 | ~130 | ~2–3 | ~8 | daha hızlı |
| 5–8 | ~89–111 | ~3 | ~9–11 | daha yavaş (v1'de ~8–9) |
| 9–12 | ~141–160 | ~2 | ~6–7 | benzer |
| 13–20 | ~103–135 | ~3 | ~8–10 | daha yavaş (v1'de ~6–7) |
| 21–25 | ~84–109 | ~3–4 | ~9–12 | belirgin daha yavaş (v1'de ~5–6) |

- Hesap, kayıplar dahil deneme başına ortalamaya dayanıyor.
- Ekonomi artık geç oyunda daha sıkı ve hedefe uygun.
- Pahalı ördek 9–12. dünyalarda en ucuz; bu, o bölgedeki kolaylaşmayı destekliyor.

---

## 5. Kalan somut öneriler

1. **L43'ü elle düzeltin.** 4 ördekle 0/11 ve çok düşük hasar (~400–580), tilkinin erişilemez ya da tamamen korumalı olduğunu gösteriyor. Tohumu (seed) değiştirin ya da tilkiyi açığa alın.
2. **L13 "Kale Duvarı"nı yeniden tasarlayın.** 7. ördek işe yaramadı (0/9). Duvarın bir katını kaldırın ya da duvarın önüne TNT koyun.
3. **L34, L35, L37 ve L40 için W2'nin ikinci yarısını yumuşatın.** Bu üç el yapımı bölümün her biri 0/9. Birer ördek ekleyin ya da birer tilki çıkarın.
4. **51–55 serisini kırın.** Örneğin L52 ve L54'ü 4 ördekli hafif bölüm yapın, ya da L41–60 için yapı ölçeğini ×0,72 yerine ×0,6'dan başlatın.
5. **W5'i (81–100) baştan dengeleyin.** L81'i hafif yapın (şu an 7 ördekli). L88 ve L92 4 ördekle UPG=1'de 0/3 ama mağazasız 2/2; UPG profilinin bu bölümleri neden bozduğunu inceleyin. L100 boss'u (0/9) 8 ördek ya da daha küçük kaleyle düzeltin.
6. **"İlk bölüm hafif" ve "2 zor → 1 hafif" kurallarının uygulandığını doğrulayın.**
   - İlk bölüm kuralının ihlal edildiği açılışlar: L61, L81, L141, L241 (7 ördek); L181, L221, L361, L421 (6 ördek, 0/3).
   - 2 zor → 1 hafif kuralının ihlal edildiği seriler: 385–389, 474–478, 57–60, 164–167.
   - "Zor" tanımına 6 ördekli ve 2★ eşiği 50.000'in üzerinde olan bölümler de girmeli.
7. **6 ördekli bölümleri hafifletin.** 175 bölüm %33 kazanıyor; bu grubun hedefi %45–50 olmalı. Kale küçültme kuralını 6 ördekli bölümlere de uygulayın, ya da 6 ördekli bölümde tilki sayısını 1 azaltın.
8. **Boss tutarlılığı sağlayın.**
   - L160 ve L400 (6 ördek) ile L300 (5 ördek) için "boss +1 ördek" kuralı uygulanmamış görünüyor.
   - L100, L120, L160, L400 ve L460 0/9; bu boss'lara ekstra ördek ya da daha küçük kale verin.
   - Öte yandan L220 ve L420 3/3 ve L220 tek atışlık. Boss'un dünyanın en kolay bölümü olmaması için alt sınır koyun.
9. **W11–12 kolaylaşmasını giderin.** Bu iki dünya %75 ve %68 ile W10'dan kolay. Burada tilki ve blok dayanıklılığına ek bir artış, ya da 200 civarında bir yeni ördek türü varsa ona karşılık D'ye +2 bonus ekleyin.
10. **Finali yükseltin.** W25 (%53) W23'ten (%40) kolay. 481–500 için dayanıklılık artışını ikiye katlayın ya da 491–499 arasına 1–2 dev kale ekleyin (kural sınırı içinde). Hedef W25 ≈ %38–40.
11. **Mağaza etkisini geri getirin.** Geç oyunda UPG=1 ile mağazasız oran arasında fark yok. Yükseltme değerlerini artırın ya da 15. dünyadan sonra yeni bir yükseltme kademesi ekleyin. Ayrıca W3 ve W5'te UPG=1'in daha kötü sonuç vermesinin bot profilinden mi oyundan mı kaynaklandığını kontrol edin (örneğin yeni ördek türü atış sırasına girip botun isabetini düşürüyor olabilir).

---

## 6. Test notları

- Stabilite sorunu yok: 500/500 SAĞLAM.
- `Bitti` satırından sonra gönderilen yeni bir `autotest` komutu iki kez başlamadı (L81 ve L221 koşularında) ve yeniden gönderildi. Unity odağı bu oturumda hiç kaybolmadı.
- Tekrar testlerinde 6 deneme kullanıldı. Tek bölümler için ±1–2 galibiyet sapma beklenmeli. Mağazasız testteki 2 deneme özellikle gürültülü.
- Bölüm düzeyindeki tüm v2 verisi `levels_v2.csv` dosyasında. Tekrar ve mağazasız sonuçları CSV'de yok; bunlar projedeki `_out/v2_*` dosyalarında.

---

## 7. v1 ve v2 dünya puanları

| Dünya | Kazanma v1 → v2 | Zorluk v1 → v2 | Denge v1 → v2 |
|---|---|---|---|
| 1 | %61,7 → %65,0 | 3 → 3 | 6 → 6 |
| 2 | %43,3 → %43,3 | 5 → 5 | 5 → 4 |
| 3 | %21,7 → %31,7 | 8 → 7 | 2 → 3 |
| 4 | %43,3 → %58,3 | 6 → 5 | 4 → 5 |
| 5 | %26,7 → %23,3 | 7 → 8 | 3 → 2 |
| 6 | %26,7 → %40,0 | 7 → 6 | 4 → 5 |
| 7 | %26,7 → %38,3 | 8 → 7 | 3 → 4 |
| 8 | %28,3 → %36,7 | 7 → 7 | 4 → 5 |
| 9 | %40,0 → %51,7 | 6 → 5 | 5 → 7 |
| 10 | %46,7 → %53,3 | 6 → 5 | 5 → 5 |
| 11 | %55,0 → %75,0 | 5 → 3 | 4 → 3 |
| 12 | %50,0 → %68,3 | 5 → 4 | 6 → 5 |
| 13 | %46,7 → %53,3 | 5 → 5 | 6 → 6 |
| 14 | %38,3 → %48,3 | 6 → 6 | 6 → 6 |
| 15 | %48,3 → %60,0 | 6 → 5 | 5 → 5 |
| 16 | %58,3 → %60,0 | 4 → 5 | 4 → 6 |
| 17 | %36,7 → %50,0 | 6 → 6 | 5 → 6 |
| 18 | %41,7 → %53,3 | 6 → 5 | 4 → 6 |
| 19 | %35,0 → %45,0 | 7 → 6 | 4 → 5 |
| 20 | %41,7 → %50,0 | 6 → 6 | 5 → 4 |
| 21 | %61,7 → %46,7 | 4 → 6 | 4 → 5 |
| 22 | %41,7 → %43,3 | 6 → 6 | 5 → 5 |
| 23 | %45,0 → %40,0 | 6 → 7 | 5 → 4 |
| 24 | %41,7 → %43,3 | 6 → 6 | 5 → 6 |
| 25 | %41,7 → %53,3 | 7 → 5 | 4 → 5 |
| **Ortalama** | **%41,9 → %49,3** | **5,9 → 5,6** | **4,5 → 4,9** |
