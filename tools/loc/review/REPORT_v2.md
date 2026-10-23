# DuckThis Feather Fury: Son Yerelleştirme Kontrolü (v2)

30 dil incelendi. Bu turdaki 63 düzeltme `fixes_v2.json` dosyasında; hepsi `tools/loc/fixes_lqa.json` içine işlendi.
Türkçedeki BÖLÜM/LEVEL ayrımına dokunulmadı.

## Türkçe ördek adları
| # | Eski | Yeni | Gerekçe |
|---|---|---|---|
| 0 | YEŞİLBAŞ ÖRDEK | **YEŞİL BAŞLI ÖRDEK** | Geliştiricinin kararı; doğal ad. |
| 1 | MANDARİN ÖRDEĞİ | aynı | Herkes tanıyor. |
| 2 | BÜYÜK TARAKDİŞ | **TARAKDİŞ ÖRDEK** | "Büyük" yalnızca kuş gözlemcileri için bir ayrım; ad bu haliyle anlaşılmıyor. |
| 3 | MİSK ÖRDEĞİ | aynı | Doğru ve yaygın. |
| 4 | KARA ÖRDEK | aynı | Hem resmî ad hem anlaşılır. |
| 5 | ANGUT | **ANGIT ÖRDEĞİ** | "Angut" mecazen "ahmak" demek, oyuncu hakaret sanabilir. Resmî ad "angıt". |
| 6 | KILKUYRUK | **KILKUYRUK ÖRDEK** | "Ördek" eklenince ne olduğu anlaşılıyor. |
| 7 | PUFLA | **PUFLA ÖRDEĞİ** | Tek başına bir anlam ifade etmiyor. |
| 8 | ALTINGÖZ | **ALTINGÖZ ÖRDEK** | Kılkuyruk ile aynı mantık. |

## Diğer dillerde ördek adları (öne çıkanlar)
- **en:**
  - PINTAIL DUCK, EIDER DUCK
  - GOLDENEYE DUCK ("Duck" eklenmezse ad bir James Bond filmini çağrıştırıyor)
- **sw:** BATA BUKINI aslında "kaz" demek; **BATA MUSCOVY** yapıldı.
- **id:**
  - BERUWOK MERAH, kaynaklarda geçmeyen uydurma bir ad; **BEBEK MERAH BATA** yapıldı.
  - BEBEK MERGANSER
- **fil:** ördek adları "PATONG … / PULANG PATO / ITIM NA PATONG DAGAT" kalıbına getirildi.
- **ru/uk:**
  - ГАГА
  - ВЕЛИКИЙ КРОХАЛЬ (kelime sırası doğallaştı)
- **zh-Hans/zh-Hant:** bilimsel önekler ve eski resmî adlar gündelik adlarla değişti (秋沙鸭, 绒鸭, 番鴨, 赤麻鴨).
- **Hint dilleri (hi, bn, mr, te, ta, gu, pa) ve ur:** "YEŞİLBAŞ" sorununun karşılığıydı; tek kelimelik kısa adların sonuna "ördek" kelimesi eklendi (बत्तख / হাঁস / बदक / బాతు / வாத்து / બતક / ਬੱਤਖ / بطخ).
- **ar:**
  - بط البلبول (tek başına "bülbül" ile karışıyordu)
  - البط الذهبي العين (dilbilgisi hatası düzeltildi)

## Diğer değişiklikler
- `add_btn` tüm dillerde "+ ÖRDEK" kalıbına getirildi. Butonun altında görünen kısa etiket bu.
- **th:** `pause` kısaltıldı (หยุด).
- **de:** DIE FÜCHSE HABEN GEWONNEN!

## Puanlar (bu turdaki düzeltmelerden sonra)
| Dil | Dilbilgisi | Yerel kullanım | Ördek adları | Genel | Önceki |
|---|---|---|---|---|---|
| en | 10 | 9.5 | 9.5 | 9.5 | 9.5 |
| tr | 9 | 8.5 | 9.5 | **9** | 8 |
| de | 10 | 9 | 10 | 9.5 | 9 |
| es | 9 | 8.5 | 8 | 8.5 | 8.5 |
| fr | 9.5 | 9 | 10 | 9.5 | 9.5 |
| pt | 9 | 8.5 | 7.5 | 8.5 | 8.5 |
| it | 9.5 | 9 | 9.5 | 9.5 | 9.5 |
| pl | 9 | 8.5 | 9.5 | 9 | 8.5 |
| id | 8.5 | 8.5 | 7.5 | 8 | 7.5 |
| fil | 8 | 7.5 | 7.5 | 7.5 | 7.5 |
| sw | 7 | 7 | 7 | 7 | 6 |
| ha | 7 | 6.5 | 6.5 | 6.5 | 6.5 |
| vi | 9 | 9 | 9 | 9 | 8.5 |
| ru | 9.5 | 9 | 9.5 | 9.5 | 9 |
| uk | 9.5 | 9 | 9.5 | 9.5 | 9 |
| zh-Hans | 9.5 | 9 | 9.5 | 9.5 | 9.5 |
| zh-Hant | 9.5 | 9 | 9.5 | 9.5 | 9.5 |
| ja | 10 | 9.5 | 10 | 9.5 | 9.5 |
| ko | 9 | 9 | 9 | 9 | 9 |
| hi | 9 | 8.5 | 8.5 | 8.5 | 8.5 |
| bn | 9 | 8.5 | 8.5 | 8.5 | 8.5 |
| mr | 8.5 | 8 | 8 | 8 | 8 |
| te | 8.5 | 8 | 8 | 8 | 8 |
| ta | 8.5 | 8 | 8 | 8 | 8 |
| gu | 8.5 | 8 | 8.5 | 8.5 | 8 |
| pa | 8.5 | 8 | 8.5 | 8.5 | 8 |
| th | 9 | 9 | 8.5 | 9 | 9 |
| ar | 9 | 8.5 | 8.5 | 8.5 | 8.5 |
| ur | 8.5 | 8 | 8.5 | 8.5 | 8.5 |
| fa | 9 | 8.5 | 8.5 | 8.5 | 8.5 |

## Ana dil konuşanına sorulması gerekenler
- **es / pt:** Bazı ördek adları resmî ve yabancı duruyor (SERRETA GRANDE, NEGROLA, ARRÁBIO). pt'de bu adlar Portekiz'de kullanılıyor, metnin geri kalanı ise Brezilya Portekizcesi.
- **ar:** البط الخضاري / الشهرمان الأحمر adları oyuncuya tanıdık gelmeyebilir.
- **mr / te / ta:** "Chakravak" ile yapılan adlar edebî kalıyor.
- **sw / ha:** Ana dil kontrolü şart. Şüpheli olanlar:
  - sw: ULIO NAO, RASI, RISASI
  - ha: WAHAR GIZO, KOGON MACIJI
- **bn / fa:** Yerel rakamlar ile Batı rakamları karışık kullanılıyor.

Kaynaklar: FAO AGROVOC (sw "bata bukini" = kaz), Qatar Birds (البلبول = Northern Pintail).
