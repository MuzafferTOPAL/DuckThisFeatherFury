from PIL import Image, ImageDraw, ImageFont, ImageFilter, ImageChops
import math, random
P='/mnt/user-data/outputs/OrdekOyunu2/Assets/Art/Pixel/'
FONT='/mnt/user-data/outputs/OrdekOyunu2/Assets/Art/Fonts/Jersey10-Regular.ttf'
OUT='/home/claude/logo2/'
INK=(26,20,31,255)

def sp(n,k=1):
    im=Image.open(P+n).convert('RGBA')
    return im.resize((im.width*k,im.height*k),Image.NEAREST) if k!=1 else im

def text_mask(txt,scale):
    # Jersey 10 is drawn on a 75-unit grid (1400 upm): render at its native pixel size, then scale up
    f=ImageFont.truetype(FONT,1400/75)
    bb=f.getbbox(txt)
    m=Image.new('L',(bb[2]-bb[0]+4,bb[3]-bb[1]+4),0)
    d=ImageDraw.Draw(m); d.fontmode='1'
    d.text((2-bb[0],2-bb[1]),txt,font=f,fill=255)
    m=m.point(lambda v:255 if v>127 else 0)
    return m.resize((m.width*scale,m.height*scale),Image.NEAREST)

def dilate(m,r):
    for _ in range(r): m=m.filter(ImageFilter.MaxFilter(3))
    return m

def styled_text(txt,size,top,bot,depth,depth_col,outline=2,hl=None):
    m=text_mask(txt,size)
    pad=outline+depth+2
    W,H=m.width+pad*2,m.height+pad*2
    base=Image.new('L',(W,H),0); base.paste(m,(pad,pad))
    out=Image.new('RGBA',(W,H),(0,0,0,0))
    # extrude (3D)
    for i in range(depth,0,-1):
        sh=ImageChops.offset(base,0,i)
        out.paste(Image.new('RGBA',(W,H),depth_col),(0,0),dilate(sh,outline))
    out.paste(Image.new('RGBA',(W,H),INK),(0,0),dilate(base,outline))
    # vertical gradient fill (banded, pixel style)
    bb=base.getbbox(); h=bb[3]-bb[1]
    grad=Image.new('RGBA',(W,H))
    gd=ImageDraw.Draw(grad)
    bands=5
    for y in range(H):
        t=min(1,max(0,(y-bb[1])/max(1,h-1)))
        t=round(t*(bands-1))/(bands-1)
        c=tuple(int(top[i]+(bot[i]-top[i])*t) for i in range(3))+(255,)
        gd.line([(0,y),(W,y)],fill=c)
    out.paste(grad,(0,0),base)
    if hl:   # highlight: top row pixels of each glyph
        up=ImageChops.subtract(base,ImageChops.offset(base,0,1))
        out.paste(Image.new('RGBA',(W,H),hl),(0,0),up)
    return out

def burst(W,H,cx,cy,r,cols,n=16):
    im=Image.new('RGBA',(W,H),(0,0,0,0)); d=ImageDraw.Draw(im)
    for i in range(n):
        a0=2*math.pi*i/n; a1=2*math.pi*(i+1)/n+0.01
        d.polygon([(cx,cy),(cx+r*math.cos(a0),cy+r*math.sin(a0)),(cx+r*math.cos(a1),cy+r*math.sin(a1))],fill=cols[i%2])
    mask=Image.new('L',(W,H),0); ImageDraw.Draw(mask).ellipse([cx-r,cy-r,cx+r,cy+r],fill=255)
    out=Image.new('RGBA',(W,H),(0,0,0,0)); out.paste(im,(0,0),mask)
    ring=Image.new('L',(W,H),0); ImageDraw.Draw(ring).ellipse([cx-r-2,cy-r-2,cx+r+2,cy+r+2],fill=255)
    o2=Image.new('RGBA',(W,H),(0,0,0,0)); o2.paste(Image.new('RGBA',(W,H),INK),(0,0),ring); o2.alpha_composite(out)
    return o2

def logo(badge):
    W,H=240,150
    c=Image.new('RGBA',(W,H),(0,0,0,0))
    if badge:
        c.alpha_composite(burst(W,H,W//2,H//2+2,74,[(255,196,64,255),(255,160,40,255)],20))
    random.seed(7)
    # feathers flying from the duck
    fe=sp('fx_feather.png',2)
    for i in range(10):
        x=random.randint(30,110); y=random.randint(10,60)
        c.alpha_composite(fe.rotate(random.choice([0,90,180,270]),expand=True),(x,y))
    # speed lines behind the duck
    d=ImageDraw.Draw(c)
    for y,l in [(22,26),(30,36),(38,22),(46,30)]:
        d.line([(8,y),(8+l,y)],fill=(255,255,255,230)); d.line([(8,y+1),(8+l,y+1)],fill=INK)
    # title
    t1=styled_text('DUCKTHIS',3,(255,236,120),(255,150,30),4,(150,60,20,255),2,(255,252,220,255))
    t2=styled_text('FEATHER FURY',2,(255,120,70),(205,40,40),3,(110,20,30,255),2,(255,210,190,255))
    x1=(W-t1.width)//2; y1=40
    c.alpha_composite(t1,(x1,y1))
    x2=(W-t2.width)//2; y2=y1+t1.height-8
    c.alpha_composite(t2,(x2,y2))
    # angry flying duck crashing in from the top left
    duck=sp('bird_duck_up.png',2)
    c.alpha_composite(duck,(30,6))
    # scared fox peeking over the right end
    fox=sp('foxh_scared.png',2).transpose(Image.FLIP_LEFT_RIGHT)
    c.alpha_composite(fox,(W-62,y1-26))
    # golden egg + stars
    egg=sp('ui_egg.png',1)
    c.alpha_composite(egg,(x2+t2.width-2,y2+t2.height-20))
    st=sp('fx_star.png',1)
    for p in [(x2+t2.width+12,y2+t2.height-24),(x2+t2.width+10,y2+t2.height-6),(x2-8,y2+8)]: c.alpha_composite(st,p)
    bb=c.getbbox(); c=c.crop((max(0,bb[0]-2),max(0,bb[1]-2),min(W,bb[2]+2),min(H,bb[3]+2)))
    return c

def up(im,k): return im.resize((im.width*k,im.height*k),Image.NEAREST)

for badge,name in [(False,'logo_duckthis_feather_fury'),(True,'logo_duckthis_feather_fury_badge')]:
    L=logo(badge); big=up(L,6); big.save(OUT+name+'.png')
    for bg,tag in [((54,120,190),'sky'),((255,255,255),'white')]:
        pv=Image.new('RGBA',(big.width+120,big.height+120),bg+(255,)); pv.alpha_composite(big,(60,60)); pv.convert('RGB').save(OUT+name+'_on_'+tag+'.png')

# square app icon: angry duck + title word on a sunburst
W=80; ic=Image.new('RGBA',(W,W),(0,0,0,0))
ic.alpha_composite(burst(W,W,40,34,58,[(255,196,64,255),(255,160,40,255)],16))
fe=sp('fx_feather.png',1)
for p in [(64,12),(68,22),(8,40),(5,30),(62,40)]: ic.alpha_composite(fe,p)
d=ImageDraw.Draw(ic)
for y,l in [(14,9),(20,12),(26,8)]: d.line([(3,y),(3+l,y)],fill=(255,255,255,255))
duck=sp('bird_duck_up.png',2); ic.alpha_composite(duck,(16,4))
t=styled_text('FURY',2,(255,236,120),(255,150,30),3,(150,60,20,255),2,(255,252,220,255))
ic.alpha_composite(t,((W-t.width)//2,W-t.height-2))
sq=Image.new('RGBA',(W,W),(0,0,0,0)); d=ImageDraw.Draw(sq)
d.rounded_rectangle([0,0,W-1,W-1],radius=15,fill=INK)
m=Image.new('L',(W,W),0); ImageDraw.Draw(m).rounded_rectangle([2,2,W-3,W-3],radius=13,fill=255)
inner=Image.new('RGBA',(W,W),(70,150,220,255)); inner.alpha_composite(ic)
sq.paste(inner,(0,0),m)
up(sq,13).save(OUT+'icon_duckthis_feather_fury.png')
print('done')

# ---- Android adaptive icon layers (432x432 = 108dp x4): full-bleed background + foreground in the safe zone
W=108
bgL=Image.new('RGBA',(W,W),(70,150,220,255)); bgL.alpha_composite(burst(W,W,54,50,80,[(255,196,64,255),(255,160,40,255)],18))
fe=sp('fx_feather.png',1)
for p in [(76,30),(80,40),(24,58),(22,48)]: bgL.alpha_composite(fe,p)
up(bgL,4).save(OUT+'icon_adaptive_bg.png')
fgL=Image.new('RGBA',(W,W),(0,0,0,0))
duck=sp('bird_duck_up.png',2); fgL.alpha_composite(duck,((W-duck.width)//2,(W-duck.height)//2))
t=None
up(fgL,4).save(OUT+'icon_adaptive_fg.png')
print('adaptive')
