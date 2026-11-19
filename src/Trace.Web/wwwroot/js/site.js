// Progressive enhancement only (interaction-spec: nothing in L0 depends on this file).
// Listeners are delegated to the document, so they keep working after Blazor's enhanced navigation
// swaps the page content.
document.documentElement.classList.add('js');

document.addEventListener('click', (event) => {
  const target = event.target instanceof Element ? event.target : null;

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
