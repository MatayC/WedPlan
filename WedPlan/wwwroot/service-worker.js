// WedPlan Service Worker
// Strategie: Cache-first für statische Assets, Network-first für Navigation,
// Offline-Fallback wenn kein Netz und kein Cache vorhanden.

const CACHE_VERSION = 'wedplan-v1';
const OFFLINE_URL   = '/offline.html';

// Statische Assets die sofort gecacht werden (App Shell)
const APP_SHELL = [
  '/',
  '/offline.html',
  '/manifest.json',
  '/app.css',
  '/icons/icon-192.png',
  '/icons/icon-512.png',
  '/icons/apple-touch-icon.png',
  'https://fonts.googleapis.com/css2?family=Cormorant+Garamond:wght@500;600;700&family=Poppins:wght@300;400;500;600&display=swap'
];

// ── Install: App Shell cachen ────────────────────────────────────────────────
self.addEventListener('install', event => {
  event.waitUntil(
    caches.open(CACHE_VERSION).then(cache => {
      // Offline-Seite muss zwingend gecacht sein
      return cache.addAll(APP_SHELL).catch(err => {
        console.warn('[SW] Einige App-Shell-Assets konnten nicht gecacht werden:', err);
        // Trotzdem offline.html cachen
        return cache.add(OFFLINE_URL);
      });
    }).then(() => self.skipWaiting())
  );
});

// ── Activate: Alte Caches aufräumen ─────────────────────────────────────────
self.addEventListener('activate', event => {
  event.waitUntil(
    caches.keys().then(keys =>
      Promise.all(
        keys
          .filter(key => key !== CACHE_VERSION)
          .map(key => caches.delete(key))
      )
    ).then(() => self.clients.claim())
  );
});

// ── Fetch: Caching-Strategie ─────────────────────────────────────────────────
self.addEventListener('fetch', event => {
  const { request } = event;
  const url = new URL(request.url);

  // Nur GET-Requests behandeln
  if (request.method !== 'GET') return;

  // Blazor-SignalR und _framework Requests: immer ans Netz
  if (url.pathname.startsWith('/_blazor') ||
      url.pathname.startsWith('/_framework') ||
      url.pathname.includes('blazor.web.js') ||
      url.hostname !== self.location.hostname && !url.hostname.includes('fonts.')) {
    return;
  }

  // Navigations-Requests (HTML-Seiten): Network-first, Offline-Fallback
  if (request.mode === 'navigate') {
    event.respondWith(
      fetch(request).catch(() =>
        caches.match(OFFLINE_URL)
      )
    );
    return;
  }

  // Statische Assets (CSS, JS, Fonts, Icons): Cache-first
  event.respondWith(
    caches.match(request).then(cached => {
      if (cached) return cached;

      return fetch(request).then(response => {
        // Nur erfolgreiche Antworten cachen
        if (!response || response.status !== 200 || response.type === 'error') {
          return response;
        }
        const toCache = response.clone();
        caches.open(CACHE_VERSION).then(cache => cache.put(request, toCache));
        return response;
      }).catch(() => caches.match(OFFLINE_URL));
    })
  );
});
