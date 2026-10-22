#!/usr/bin/env python3
"""Merges the language tables into Resources/Loc/{code}.json and renders the text atlases
for languages whose script is not in the Jersey 10 pixel font.

The tokenizer here must stay identical to LocText.Tokenize in the C# code."""
import json, os, sys, importlib.util, re
from fontTools.ttLib import TTFont
from PIL import Image, ImageDraw, ImageFont, features

HERE = os.path.dirname(os.path.abspath(__file__))
ASSETS = sys.argv[1] if len(sys.argv) > 1 else '/mnt/user-data/outputs/OrdekOyunu2/Assets'
OUT_LOC = os.path.join(ASSETS, 'Resources', 'Loc')
OUT_ATL = os.path.join(ASSETS, 'Resources', 'LocAtlas')
os.makedirs(OUT_LOC, exist_ok=True); os.makedirs(OUT_ATL, exist_ok=True)
assert features.check('raqm')

ORDER = ['en', 'tr', 'de', 'es', 'fr', 'pt', 'it', 'pl', 'id', 'fil', 'sw', 'ha', 'vi', 'ru', 'uk',
         'zh-Hans', 'zh-Hant', 'ja', 'ko', 'hi', 'bn', 'mr', 'te', 'ta', 'gu', 'pa', 'th', 'ar', 'ur', 'fa']
NATIVE = {'en': 'ENGLISH', 'tr': 'TÜRKÇE', 'de': 'DEUTSCH', 'es': 'ESPAÑOL', 'fr': 'FRANÇAIS', 'pt': 'PORTUGUÊS',
          'it': 'ITALIANO', 'pl': 'POLSKI', 'id': 'BAHASA INDONESIA', 'fil': 'FILIPINO', 'sw': 'KISWAHILI', 'ha': 'HAUSA',
          'vi': 'TIẾNG VIỆT', 'ru': 'РУССКИЙ', 'uk': 'УКРАЇНСЬКА', 'zh-Hans': '简体中文', 'zh-Hant': '繁體中文', 'ja': '日本語',
          'ko': '한국어', 'hi': 'हिन्दी', 'bn': 'বাংলা', 'mr': 'मराठी', 'te': 'తెలుగు', 'ta': 'தமிழ்', 'gu': 'ગુજરાતી',
          'pa': 'ਪੰਜਾਬੀ', 'th': 'ไทย', 'ar': 'العربية', 'ur': 'اردو', 'fa': 'فارسی'}
FULL_ATLAS = {'ru', 'uk', 'vi', 'ha'}
SCRIPT_ATLAS = {'zh-Hans', 'zh-Hant', 'ja', 'ko', 'hi', 'bn', 'mr', 'te', 'ta', 'gu', 'pa', 'th', 'ar', 'ur', 'fa'}
RTL = {'ar', 'ur', 'fa'}

INTER = '/usr/share/fonts/opentype/inter/Inter-ExtraBold.otf'
CJK = '/usr/share/fonts/opentype/noto/NotoSansCJK-Black.ttc'
T = '/home/claude/fnt/ttf/'
FONTS = {'ru': (INTER, 0), 'uk': (INTER, 0), 'vi': (INTER, 0), 'ha': (INTER, 0),
         'ja': (CJK, 0), 'ko': (CJK, 1), 'zh-Hans': (CJK, 2), 'zh-Hant': (CJK, 3),
         'hi': (T + 'devanagari.ttf', 0), 'mr': (T + 'devanagari.ttf', 0), 'bn': (T + 'bengali.ttf', 0),
         'te': (T + 'telugu.ttf', 0), 'ta': (T + 'tamil.ttf', 0), 'gu': (T + 'gujarati.ttf', 0),
         'pa': (T + 'gurmukhi.ttf', 0), 'th': (T + 'thai.ttf', 0),
         'ar': (T + 'arabic.ttf', 0), 'ur': (T + 'arabic.ttf', 0), 'fa': (T + 'arabic.ttf', 0)}

AF, LH, BASE, PAD = 40, 72, 50, 3
STAR = '★'

# ---------------------------------------------------------------- load tables
LANGS = {}
for f in ['l_en_tr', 'l_eu', 'l_latin2', 'l_cyr', 'l_cjk', 'l_indic', 'l_rtl_th']:
    spec = importlib.util.spec_from_file_location(f, os.path.join(HERE, f + '.py'))
    m = importlib.util.module_from_spec(spec); spec.loader.exec_module(m)
    LANGS.update(m.LANGS)
for c in ORDER:
    if 'LVL' not in LANGS[c]:
        LANGS[c]['LVL'] = json.load(open(os.path.join(HERE, 'lvl_%s.json' % c), encoding='utf-8'))

# Dil incelemesi (LQA) düzeltmeleri: fixes_lqa.json kaynak tabloların üzerine uygulanır
_fx = os.path.join(HERE, 'fixes_lqa.json')
if os.path.exists(_fx):
    for c, parts in json.load(open(_fx, encoding='utf-8')).items():
        for part, items in parts.items():
            for k, v in items.items():
                if part == 'U': LANGS[c]['U'][k] = v
                else: LANGS[c][part][int(k)] = v

ukeys = list(LANGS['en']['U'].keys())
tables = {}
bad = False
for c in ORDER:
    L = LANGS[c]
    miss = [k for k in ukeys if k not in L['U']]
    extra = [k for k in L['U'] if k not in ukeys]
    for name, n in (('DUCKS', 9), ('ABIL', 9), ('W', 25), ('LVL', 40)):
        if len(L[name]) != n: print(c, name, len(L[name]), '!=', n); bad = True
    if miss or extra: print(c, 'missing', miss, 'extra', extra); bad = True
    t = {}
    for k, v in L['U'].items(): t['continue' if k == 'continue_' else k] = v
    for i, v in enumerate(L['DUCKS']): t['bird%d' % i] = v
    for i, v in enumerate(L['ABIL']): t['abil%d' % i] = v
    for i, v in enumerate(L['W']): t['w%d' % i] = v
    for i, v in enumerate(L['LVL']): t['lvl%d' % i] = v
    t['native'] = NATIVE[c]
    for k in ('unlock_at', 'locked_world', 'tip_nest'):
        if '{0}' not in t[k]: print(c, k, 'has no {0}'); bad = True
    tables[c] = t
if bad: sys.exit('table errors')

for c, t in tables.items():
    with open(os.path.join(OUT_LOC, c + '.json'), 'w', encoding='utf-8') as fp:
        json.dump(t, fp, ensure_ascii=False, indent=0)

# ---------------------------------------------------------------- Jersey coverage
JER = set(chr(u) for u in TTFont(os.path.join(ASSETS, 'Art/Fonts/Jersey10-Regular.ttf')).getBestCmap())
with open(os.path.join(OUT_LOC, '_jersey.txt'), 'w', encoding='utf-8') as fp:
    fp.write(''.join(sorted(c for c in JER if c.isprintable() and c not in ' \n\t')))

# ---------------------------------------------------------------- tokenizer (mirror of C#)
import unicodedata
def is_letter(ch): return unicodedata.category(ch).startswith('L')

def is_a(ch, code):
    """True if this char must come from the atlas (not in Jersey, or a letter of a full-atlas language)."""
    if ch == STAR: return False
    return ch not in JER or (code in FULL_ATLAS and is_letter(ch))

def tokenize(text, code):
    """-> list of (token, space_before, is_atlas). Splits at spaces, around the star and where
    atlas chars meet font chars."""
    toks, cur, space, prev = [], '', False, None
    for ch in text.replace(STAR, ' ' + STAR + ' '):
        if ch == ' ':
            if cur: toks.append((cur, space, prev)); cur = ''
            space = True; prev = None
            continue
        a = is_a(ch, code)
        if cur and a != prev:
            toks.append((cur, space, prev)); cur = ''; space = False
        cur += ch; prev = a
    if cur: toks.append((cur, space, prev))
    if toks: toks[0] = (toks[0][0], False, toks[0][2])
    return toks

def runs_of(text, code):
    """All atlas runs (maximal sequences of consecutive atlas tokens) plus every single atlas token."""
    toks = tokenize(text, code)
    res, cur = [], []
    def flush():
        if cur: res.append(cur[0][0] + ''.join((' ' if sp else '') + tk for tk, sp in cur[1:]))
        cur.clear()
    for tk, sp, at in toks:
        if at: cur.append((tk, sp))
        else: flush()
    flush()
    return res, [tk for tk, sp, at in toks if at]

# ---------------------------------------------------------------- rendering
_fcache = {}
def font(path, idx):
    k = (path, idx)
    if k not in _fcache:
        _fcache[k] = ImageFont.truetype(path, AF, index=idx, layout_engine=ImageFont.Layout.RAQM)
    return _fcache[k]
_cmaps = {}
def cmap(path, idx):
    k = (path, idx)
    if k not in _cmaps:
        _cmaps[k] = set(TTFont(path, fontNumber=idx).getBestCmap()) if path.endswith('.ttc') else set(TTFont(path).getBestCmap())
    return _cmaps[k]

def split_by_font(text, code):
    """Splits into (substring, (path, idx)) pieces: primary script font, Inter as fallback."""
    prim = FONTS[code]; pc = cmap(*prim); ic = cmap(INTER, 0)
    pieces = []
    for ch in text:
        import unicodedata
        cat = unicodedata.category(ch)
        if ord(ch) in pc: f = prim
        elif cat.startswith('M') or ch in '‌‍' or ch == ' ':
            f = pieces[-1][1] if pieces else prim   # combining marks / joiners / spaces stay with the previous run
        elif ord(ch) in ic: f = (INTER, 0)
        else:
            print('  no glyph', code, repr(ch), hex(ord(ch))); f = prim
        if pieces and pieces[-1][1] == f: pieces[-1][0] += ch
        else: pieces.append([ch, f])
    return pieces

def render(text, code):
    direction = 'rtl' if code in RTL else 'ltr'
    pieces = split_by_font(text, code)
    if direction == 'rtl': pieces = pieces[::-1]
    imgs = []
    for s, f in pieces:
        ft = font(*f)
        w = int(round(ft.getlength(s, direction=direction))) + 4
        im = Image.new('L', (max(w, 1), LH), 0)
        ImageDraw.Draw(im).text((2, BASE), s, fill=255, font=ft, anchor='ls', direction=direction)
        imgs.append(im)
    W = sum(i.width for i in imgs) - 4 * (len(imgs) - 1)
    out = Image.new('L', (max(W, 1), LH), 0)
    x = 0
    for im in imgs:
        out.paste(Image.composite(im, out.crop((x, 0, x + im.width, LH)), im), (x, 0)); x += im.width - 4
    bb = out.getbbox()
    if bb and (bb[1] == 0 or bb[3] == LH): print('  clipped', code, text)
    # trim horizontal empty space, keep a small margin
    if bb: out = out.crop((max(0, bb[0] - 2), 0, min(out.width, bb[2] + 2), LH))
    return out

def pack(images, width=2048):
    x = y = 0; rowh = LH + PAD; pos = []
    for im in images:
        if x + im.width > width: x = 0; y += rowh
        pos.append((x, y)); x += im.width + PAD
    H = y + rowh
    H = (H + 3) // 4 * 4
    return pos, H

def build_atlas(name, entries):
    """entries: list of (text, code). Writes {name}.bytes (PNG) + {name}.txt index."""
    seen, imgs, keys = set(), [], []
    for text, code in entries:
        if text in seen: continue
        seen.add(text); imgs.append(render(text, code)); keys.append(text)
    order = sorted(range(len(imgs)), key=lambda i: -imgs[i].width)
    pos, H = pack([imgs[i] for i in order])
    atlas = Image.new('LA', (2048, H), (255, 0))
    lines = []
    for (x, y), i in zip(pos, order):
        im = imgs[i]
        atlas.paste(Image.merge('LA', (Image.new('L', im.size, 255), im)), (x, y))
        lines.append('%d\t%d\t%d\t%d\t%s' % (x, y, im.width, im.height, keys[i]))
    atlas.convert('RGBA').save(os.path.join(OUT_ATL, name + '.bytes'), 'PNG', optimize=True)
    with open(os.path.join(OUT_ATL, name + "_idx.txt"), 'w', encoding='utf-8') as fp:
        fp.write('\n'.join(lines))
    print('atlas', name, len(keys), 'entries', '2048x%d' % H)

for c in ORDER:
    if c not in FULL_ATLAS and c not in SCRIPT_ATLAS:
        # Jersey languages: make sure every char is covered, else the word goes to an atlas
        need = []
        for v in tables[c].values():
            r, s = runs_of(v, c); need += r + s
        if need:
            print(c, 'needs atlas for', sorted(set(need))[:10])
            build_atlas(c, [(t, c) for t in need])
        continue
    ents = []
    for k, v in tables[c].items():
        if k == 'native': continue
        r, s = runs_of(v, c); ents += [(t, c) for t in r + s]
    build_atlas(c, ents)

# Native language names, for the language picker (each drawn with its own script font)
ents = []
for c in ORDER:
    if c in FULL_ATLAS or c in SCRIPT_ATLAS:
        r, s = runs_of(NATIVE[c], c); ents += [(t, c) for t in r + s]
build_atlas('_names', ents)
