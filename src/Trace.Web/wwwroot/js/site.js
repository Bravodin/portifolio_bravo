// Progressive enhancement only (interaction-spec: nothing in L0 depends on this file).
// Listeners are delegated to the document, so they keep working after Blazor's enhanced navigation
// swaps the page content.
document.documentElement.classList.add('js');

// ---------- Lightbox ----------
// Photo links open their large version in a native <dialog> on top of the page. Without JS they simply open
// the image (L0). The dialog gives focus handling and Esc for free; arrows, swipe and the counter are added here.
const lightbox = {
  links: [],
  index: 0,

  dialog() {
    let dialog = document.querySelector('dialog.lightbox');
    if (dialog) return dialog;

    dialog = document.createElement('dialog');
    dialog.className = 'lightbox';
    dialog.innerHTML =
      '<figure class="lightbox-figure"><img class="lightbox-img" alt=""><figcaption class="lightbox-caption"></figcaption></figure>' +
      '<p class="lightbox-count" aria-live="polite"></p>' +
      '<button type="button" class="lightbox-close" autofocus>✕</button>' +
      '<button type="button" class="lightbox-prev">‹</button>' +
      '<button type="button" class="lightbox-next">›</button>';
    document.body.append(dialog);

    dialog.querySelector('.lightbox-close').addEventListener('click', () => dialog.close());
    dialog.querySelector('.lightbox-prev').addEventListener('click', () => lightbox.step(-1));
    dialog.querySelector('.lightbox-next').addEventListener('click', () => lightbox.step(1));
    dialog.addEventListener('click', (e) => { if (e.target === dialog) dialog.close(); });

    let startX = null;
    dialog.addEventListener('touchstart', (e) => { startX = e.touches[0].clientX; }, { passive: true });
    dialog.addEventListener('touchend', (e) => {
      if (startX === null) return;
      const dx = e.changedTouches[0].clientX - startX;
      if (Math.abs(dx) > 50) lightbox.step(dx < 0 ? 1 : -1);
      startX = null;
    });
    return dialog;
  },

  open(link) {
    this.links = [...document.querySelectorAll('a.photo-link')];
    this.index = Math.max(0, this.links.indexOf(link));
    const dialog = this.dialog();
    const labels = link.closest('[data-lightbox-label]')?.dataset ?? {};
    dialog.setAttribute('aria-label', labels.lightboxLabel ?? '');
    dialog.querySelector('.lightbox-close').setAttribute('aria-label', labels.lightboxClose ?? 'Close');
    dialog.querySelector('.lightbox-prev').setAttribute('aria-label', labels.lightboxPrev ?? 'Previous');
    dialog.querySelector('.lightbox-next').setAttribute('aria-label', labels.lightboxNext ?? 'Next');
    this.show();
    dialog.showModal();
    // Focus a control that never gets replaced (the caption's credit link does, on every photo change).
    dialog.querySelector('.lightbox-close').focus();
  },

  step(delta) {
    if (this.links.length < 2) return;
    this.index = (this.index + delta + this.links.length) % this.links.length;
    this.show();
  },

  show() {
    const dialog = this.dialog();
    const link = this.links[this.index];
    const img = dialog.querySelector('.lightbox-img');
    img.src = link.href;
    img.alt = link.querySelector('img')?.alt ?? '';
    const caption = link.closest('figure')?.querySelector('figcaption');
    dialog.querySelector('.lightbox-caption').replaceChildren(...(caption ? caption.cloneNode(true).childNodes : []));
    dialog.querySelector('.lightbox-count').textContent = (this.index + 1) + ' / ' + this.links.length;
    const single = this.links.length < 2;
    dialog.querySelector('.lightbox-prev').hidden = single;
    dialog.querySelector('.lightbox-next').hidden = single;
    // Warm up the neighbours so arrows feel instant.
    for (const d of [-1, 1]) {
      const next = this.links[(this.index + d + this.links.length) % this.links.length];
      if (next) new Image().src = next.href;
    }
  },
};

// Arrows on the whole document while the lightbox is open, so they keep working wherever focus is.
document.addEventListener('keydown', (event) => {
  if (!document.querySelector('dialog.lightbox[open]')) return;
  if (event.key === 'ArrowLeft') { event.preventDefault(); lightbox.step(-1); }
  if (event.key === 'ArrowRight') { event.preventDefault(); lightbox.step(1); }
});

// Capture phase, so the lightbox sees the click before Blazor's enhanced navigation does.
// Plain left clicks only: Ctrl/Cmd/Shift-click still opens the image in a new tab.
document.addEventListener('click', (event) => {
  const photo = event.target instanceof Element ? event.target.closest('a.photo-link') : null;
  if (photo && typeof HTMLDialogElement === 'function' && event.button === 0 && !event.ctrlKey && !event.metaKey && !event.shiftKey && !event.altKey) {
    event.preventDefault();
    event.stopPropagation();
    lightbox.open(photo);
  }
}, true);

document.addEventListener('click', (event) => {
  const target = event.target instanceof Element ? event.target : null;

  // Video facade (ADR-0010): load YouTube only now that the visitor asked for it.
  const video = target?.closest('a[data-youtube]');
  if (video) {
    event.preventDefault();
    const iframe = document.createElement('iframe');
    iframe.src = 'https://www.youtube-nocookie.com/embed/' + encodeURIComponent(video.dataset.youtube) + '?autoplay=1&playsinline=1&rel=0';
    iframe.title = video.dataset.title ?? '';
    iframe.allow = 'autoplay; encrypted-media; picture-in-picture; fullscreen';
    iframe.allowFullscreen = true;
    iframe.className = 'video-frame';
    video.replaceWith(iframe);
    iframe.focus();
    return;
  }

  if (target?.closest('[data-print]')) {
    window.print();
    return;
  }

  // Remember the visitor's language choice for their next visit (ADR-0004: no automatic redirects).
  const lang = target?.closest('a[data-lang]')?.getAttribute('data-lang');
  if (lang) {
    try { localStorage.setItem('trace.lang', lang); } catch { /* storage blocked: nothing to remember */ }
  }
});
