const observers = new WeakMap();

function randomFromSeed(text) {
    let h = 2166136261;
    for (const c of text) h = Math.imul(h ^ c.charCodeAt(0), 16777619);
    return () => { h += 0x6D2B79F5; let t = h; t = Math.imul(t ^ t >>> 15, t | 1); t ^= t + Math.imul(t ^ t >>> 7, t | 61); return ((t ^ t >>> 14) >>> 0) / 4294967296; };
}

function sparkle(ctx, x, y, radius, rays, color, alpha) {
    ctx.save(); ctx.translate(x, y); ctx.strokeStyle = color; ctx.globalAlpha = alpha; ctx.lineWidth = Math.max(1, radius / 5); ctx.shadowBlur = radius * 2; ctx.shadowColor = color;
    ctx.beginPath();
    for (let i = 0; i < rays; i++) { const a = i * Math.PI / rays; ctx.moveTo(0, 0); ctx.lineTo(Math.cos(a) * radius, Math.sin(a) * radius); ctx.moveTo(0, 0); ctx.lineTo(-Math.cos(a) * radius, -Math.sin(a) * radius); }
    ctx.stroke(); ctx.fillStyle = '#fff9d4'; ctx.beginPath(); ctx.arc(0, 0, Math.max(1, radius / 4), 0, Math.PI * 2); ctx.fill(); ctx.restore();
}

function drawText(ctx, text, x, y, maxWidth, preferredSize) {
    let size = preferredSize;
    do { ctx.font = `bold ${size}px Georgia, "Yu Mincho", serif`; size--; } while (ctx.measureText(text).width > maxWidth && size > 12);
    ctx.save(); ctx.textAlign = 'center'; ctx.textBaseline = 'middle'; ctx.font = `bold ${size + 1}px Georgia, "Yu Mincho", serif`;
    ctx.fillStyle = '#fff4c8'; ctx.shadowColor = '#080316'; ctx.shadowBlur = 12; ctx.shadowOffsetY = 3; ctx.fillText(text, x, y); ctx.restore();
}

export function draw(canvas, number, title, salt) {
    canvas._arcanaState = { number, title, salt };
    const rect = canvas.getBoundingClientRect(), dpr = window.devicePixelRatio || 1;
    canvas.width = Math.round(rect.width * dpr); canvas.height = Math.round(rect.height * dpr);
    const ctx = canvas.getContext('2d'); ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
    const w = rect.width, h = rect.height, rnd = randomFromSeed(`${number}|${title}|${salt}`);
    const hue = Math.floor(rnd() * 360), hue2 = (hue + 70 + rnd() * 80) % 360;
    const gradient = ctx.createRadialGradient(w * .48, h * .38, 0, w * .5, h * .5, h * .9);
    gradient.addColorStop(0, `hsl(${hue2} 75% 28%)`); gradient.addColorStop(.45, `hsl(${hue} 58% 15%)`); gradient.addColorStop(1, '#05040d'); ctx.fillStyle = gradient; ctx.fillRect(0, 0, w, h);
    for (let i = 0; i < 12; i++) { const x = rnd() * w, y = rnd() * h, r = 30 + rnd() * 150, g = ctx.createRadialGradient(x, y, 0, x, y, r); g.addColorStop(0, `hsla(${hue2} 100% 75% / ${.03 + rnd() * .08})`); g.addColorStop(1, 'transparent'); ctx.fillStyle = g; ctx.fillRect(x - r, y - r, r * 2, r * 2); }
    for (let i = 0; i < 420; i++) { ctx.fillStyle = `hsla(${40 + rnd() * 35} 100% 85% / ${.16 + rnd() * .65})`; ctx.beginPath(); ctx.arc(rnd() * w, rnd() * h, .25 + rnd() * 1.4, 0, Math.PI * 2); ctx.fill(); }
    for (let i = 0; i < 34; i++) sparkle(ctx, rnd() * w, rnd() * h, 2 + rnd() * 11, rnd() > .6 ? 4 : 2, `hsl(${42 + rnd() * 25} 100% 85%)`, .3 + rnd() * .7);
    ctx.strokeStyle = 'rgba(255,231,163,.52)'; ctx.lineWidth = 2; ctx.strokeRect(18, 18, w - 36, h - 36); ctx.strokeStyle = 'rgba(255,231,163,.25)'; ctx.strokeRect(26, 26, w - 52, h - 52);
    drawText(ctx, number || '0', w / 2, h * .095, w * .76, w * .105); drawText(ctx, title || 'ARCANA', w / 2, h * .91, w * .80, w * .070);
}

export function observeResize(card, canvas, number, title, salt) {
    observers.get(canvas)?.disconnect();
    const observer = new ResizeObserver(() => {
        const state = canvas._arcanaState || { number, title, salt };
        draw(canvas, state.number, state.title, state.salt);
    });
    observer.observe(card); observers.set(canvas, observer);
}

export function savePng(canvas, number, title) {
    const link = document.createElement('a');
    link.download = `arcana_${number}_${title.replace(/[^a-z0-9_-]+/gi, '_')}.png`;
    link.href = canvas.toDataURL('image/png'); link.click();
}
