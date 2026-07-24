# WedPlan 💍

Moderne, mobile-first Web-App zur gemeinsamen Hochzeitsplanung – Blazor Web App (.NET 10) mit MudBlazor und Supabase.

---

## 📑 Inhaltsverzeichnis
- [Wichtige Links & Dienste](#-wichtige-links--dienste)
- [Konten & Zugänge](#-konten--zugänge)
- [Umgebungsvariablen (Environment)](#-umgebungsvariablen-environment)
- [Deployment (Live schalten)](#-deployment-live-schalten)
- [Kaltstart verhindern (UptimeRobot)](#-kaltstart-verhindern-uptimerobot)
- [Supabase-Konfiguration](#-supabase-konfiguration)
- [Lokale Entwicklung](#-lokale-entwicklung)
- [Projektstruktur](#-projektstruktur)
- [Technologie-Stack](#-technologie-stack)

---

## 🔗 Wichtige Links & Dienste

| Dienst | Zweck | Link |
|--------|-------|------|
| **Live-Website** | Die öffentliche App | https://wedplan-1snh.onrender.com |
| **GitHub Repository** | Quellcode & Deployment-Trigger | https://github.com/MatayC/WedPlan |
| **Render Dashboard** | Hosting der App (Free Tier) | https://dashboard.render.com |
| **Supabase Dashboard** | Datenbank, Auth, Storage | https://supabase.com/dashboard |
| **UptimeRobot Monitor** | Hält die App wach (Ping alle 5 Min) | https://dashboard.uptimerobot.com/monitors/803585174 |

---

## 👤 Konten & Zugänge

| Dienst | Konto / E-Mail | Hinweis |
|--------|----------------|---------|
| GitHub | `MatayC` | Repo-Besitzer |
| E-Mail (Kontakt) | `Matay.coglan03@gmail.com` | Für Render, Supabase, UptimeRobot |
| Render | via GitHub / E-Mail | Auto-Deploy verbunden mit `MatayC/WedPlan` |
| Supabase | Projekt **A&MWeddingPlanner** | Projekt-ID: `mgcmmchqgyaouynwxoes` |
| UptimeRobot | via E-Mail | Monitor-ID: `803585174` |

> ⚠️ **Passwörter und geheime Keys niemals hier oder im Repo speichern!** Nur Konto-Namen/E-Mails zur Orientierung.

---

## 🔐 Umgebungsvariablen (Environment)

Diese werden **im Render-Dashboard** unter `wedplan → Environment` gesetzt (NICHT im Code):

| Variable | Wert / Format | Beschreibung |
|----------|---------------|--------------|
| `SUPABASE_URL` | `https://mgcmmchqgyaouynwxoes.supabase.co` | Basis-URL des Supabase-Projekts (**ohne** `/rest/v1/`) |
| `SUPABASE_KEY` | `sb_publishable_...` | Publishable Key (aus Supabase → Settings → API Keys) |
| `ASPNETCORE_ENVIRONMENT` | `Production` | Automatisch via Dockerfile/render.yaml |
| `DOTNET_hostBuilder__reloadConfigOnChange` | `false` | Verhindert `inotify`-Fehler im Container |

---

## 🚀 Deployment (Live schalten)

Render ist mit GitHub verbunden → **Auto-Deploy** bei jedem Push.

```powershell
git add .
git commit -m "Beschreibung der Änderung"
git push
```

→ Render baut & deployt automatisch (~3–5 Min). Status im Render-Dashboard unter **Events** (auf „Deploy live" warten).

**Manuelles Deploy** (falls nötig): Render → `wedplan` → oben rechts **Manual Deploy** → **Deploy latest commit**.

---

## ⏰ Kaltstart verhindern (UptimeRobot)

Der Render Free Tier legt die App nach ~15 Min Inaktivität schlafen (Kaltstart 30–60 s).
**UptimeRobot** ruft die App alle 5 Minuten auf → sie bleibt wach.

- **Monitor:** https://dashboard.uptimerobot.com/monitors/803585174
- **Typ:** HTTP(s), **Intervall:** 5 Minuten
- **URL:** https://wedplan-1snh.onrender.com

---

## 🗄️ Supabase-Konfiguration

- **Projekt:** A&MWeddingPlanner (`mgcmmchqgyaouynwxoes`)
- **Dashboard:** https://supabase.com/dashboard/project/mgcmmchqgyaouynwxoes
- **Datenbank-Schema:** siehe [`WedPlan/SupabaseSchema.sql`](WedPlan/SupabaseSchema.sql)

### Wichtig: Redirect-URLs für Login
Nach dem Deploy die Live-Domain hinterlegen:
**Supabase → Authentication → URL Configuration → Redirect URLs**
```
https://wedplan-1snh.onrender.com/**
```

---

## 💻 Lokale Entwicklung

Voraussetzung: .NET 10 SDK, Visual Studio 2026.

Supabase-Keys lokal über **User Secrets** setzen (nicht in Git):
```powershell
cd WedPlan
dotnet user-secrets set "Supabase:Url" "https://mgcmmchqgyaouynwxoes.supabase.co"
dotnet user-secrets set "Supabase:Key" "sb_publishable_..."
```

Starten:
```powershell
dotnet run --project WedPlan
```

---

## 📁 Projektstruktur

```
WedPlan/
├── Components/
│   ├── Pages/        → Seiten (Dashboard, Budget, Gäste, Aufgaben, …)
│   ├── Layout/       → MainLayout, NavMenu, LoginLayout
│   └── Dialogs/      → Bearbeitungs-Dialoge
├── Models/           → Domänen- & Supabase-Row-Modelle
├── Services/         → Auth, Budget, Wedding, Permissions, Supabase
├── SupabaseSchema.sql→ Datenbank-Schema & RLS-Policies
Dockerfile            → Container-Build für Render
render.yaml           → Render Blueprint (Free Tier)
```

### Seiten der App
| Route | Seite | Beschreibung |
|-------|-------|--------------|
| `/` | Dashboard | Startseite mit Übersicht |
| `/login` | Login/Registrierung | Anmeldung |
| `/onboarding` | Onboarding | Projekt erstellen/beitreten |
| `/gaeste` | Gästeliste | Gäste verwalten |
| `/budget` | Budget-Planer | Kosten & Sparplan |
| `/wohnung` | Wohnungsplanung | Wohnungskosten |
| `/aufgaben` | To-Do / Checkliste | Aufgaben |
| `/zeitplan` | Zeitplan | Ablaufplan |
| `/sitzordnung` | Sitzordnung | Tischplanung |
| `/einstellungen` | Einstellungen | Projekt, Rechte, Titelbilder |

---

## 🛠️ Technologie-Stack

- **.NET 10** – Blazor Web App (Interactive Server)
- **MudBlazor** – UI-Framework
- **Supabase** – PostgreSQL, Auth, Storage, Realtime, RLS
- **Blazored.LocalStorage** – Session-Persistenz
- **Docker** – Container-Build
- **Render.com** – Hosting (Free Tier)
- **UptimeRobot** – Uptime-Monitoring / Keep-Alive
