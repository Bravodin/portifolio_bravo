// Progressive enhancement only (interaction-spec: nothing in L0 depends on this file).
// Listeners are delegated to the document, so they keep working after Blazor's enhanced navigation
// swaps the page content.
document.documentElement.classList.add('js');

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
