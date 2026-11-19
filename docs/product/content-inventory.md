# Content inventory

The single source of truth for **what** the site says. Every number comes from the CV (`CV-Manuel-Bravo`, Oct 2026). If a number isn't in the CV, it doesn't go on the site.

During implementation this becomes `content/{locale}/*.json` (see [ADR-0004](../architecture/adr/0004-i18n-content-as-data.md)).

---

## 1. Identity

| Key | EN | PT-BR | ES |
|---|---|---|---|
| `name` | Manuel Bravo | Manuel Bravo | Manuel Bravo |
| `role` | Lead Software Engineer | Lead Software Engineer | Lead Software Engineer |
| `tagline` | I modernize critical platforms without stopping production. | Modernizo plataformas críticas sem parar a produção. | Modernizo plataformas críticas sin detener la producción. |
| `subtitle` | Full-stack .NET · Test automation & quality · AWS · Team building | Full-stack .NET · Automação de testes e qualidade · AWS · Formação de times | Full-stack .NET · Automatización de pruebas y calidad · AWS · Formación de equipos |
| `location` | Brazil · open to remote | Brasil · aberto a remoto | Brasil · abierto a remoto |
| `languages` | Portuguese (native) · Spanish (native) · English (full professional) | | |
| `contact` | linkedin.com/in/manuelbravoc (only channel) | | |

> ⚠️ Contact is **LinkedIn only**: no e-mail or phone on the site. Publish a version of the CV PDF without phone and e-mail (LinkedIn URL only).

> TODO(manuel): the Recruiter Mode wireframe says "20+ years" of experience, but the career log starts in 2008 (~18 years). The site shows "Since 2008". If there is earlier experience that isn't in the CV, add it to the log and the number can change.
>
> TODO(manuel): no CV PDF yet. The "Download CV" button appears automatically once `cvUrl` is set in `content/{locale}/identity.json`.

---

## 2. Headline metrics (the "dashboard")

These are shown as gauges / counters. Always display them as **before → after**.

| ID | Metric | Before | After | Δ | Where | Trace stage |
|---|---|---|---|---|---|---|
| M1 | Appointment-booking journey | 12–20 s | ~1.2 s | ~10–16× faster | Porto | 1 · Client |
| M2 | Reimbursement fraud | baseline | −40 % | −40 % | Porto | 2 · Edge |
| M3 | API platform cost | CA API Gateway | Sensedia | −30 % | Porto | 3 · Gateway |
| M4 | Security vulnerabilities | legacy .NET/SOAP | Node.js (~80 % migrated) | −70 % | Porto | 4 · Services |
| M5 | Critical processing workflow | 8 h | 4 h | −50 % | Porto | 5 · Cache & Messaging |
| M6 | Critical batch (rental platform) | 8 h | 2 h | −75 % | Nunes Imóveis | 6 · Database |
| M7 | Developer productivity (AI pilot) | baseline | +40 % | +40 % | Porto | 7 · Pit Crew |
| M8 | Unit-test coverage gate | — | ≥ 90 % (Sonar) | — | Porto | 7 · Pit Crew |
| M9 | SLA compliance | 70 % | 98 % | +28 pp | Indra | Service Log |
| M10 | Developer overtime | ~40 h/mo | ~10 h/mo | −75 % | Indra | Service Log |
| M11 | Delivery speed (Agile transition) | Waterfall | Scrum | +30 % | Indra | Service Log |
| M12 | Annual profit margin sustained | — | ~32 % | — | Indra | Service Log |
| M13 | Time-clock device cost | R$ 3,000 | R$ 800 | −73 % | DIMEP | Service Log |
| M14 | Market system performance | baseline | +20 % | +20 % | COREON | 6 · Database |
| M15 | WPF POS responsiveness | baseline | +15 % | +15 % | Linx | 1 · Client |

---

## 3. The Trace — stage content

Format for each stage: **Layer → Problem → Action → Result → Tech**. Copy is a draft; final wording is done per locale.

| # | Stage (game "level") | Garage metaphor | Problem | Action | Result | Tech chips |
|---|---|---|---|---|---|---|
| 1 | **Client** | Handlebars & dashboard | A 2016 Ionic app where a critical booking journey took 12–20 s | Led the migration to native iOS (Swift) and Android (Kotlin); moved regression testing to Compose Testing + XCUITest | Booking ~1.2 s | Swift · Kotlin · Jetpack Compose Testing · XCUITest · WPF |
| 2 | **Edge & Security** | Ignition key & immobilizer | Fraud in reimbursement workflows | Added facial biometrics to reimbursement | Fraud −40 % | Biometrics · AWS · Firewall troubleshooting |
| 3 | **API Gateway** | Fuel injection | High platform cost on CA API Gateway | Migrated the gateway to Sensedia | Cost −30 % | Sensedia · CA API Gateway · REST · SOAP |
| 4 | **Services** | Engine block (rebuild) | Legacy .NET/SOAP services with a large vulnerability surface | Modernized ~80 % of them to Node.js | Vulnerabilities −70 % | Node.js · TypeScript · C# · .NET · Microservices |
| 5 | **Cache & Messaging** | Transmission | Intermittent failures and duplicate processing | Message idempotency + Redis caching; reworked the architecture of a critical workflow | Workflow 8 h → 4 h; better reliability | Redis · Idempotency · AWS |
| 6 | **Database** | Oil & cooling | Deadlocks and slow batches on high-volume data | Tuned queries, transactions, locking strategy and stored procedures | Batch 8 h → 2 h; deadlocks eliminated; +20 % throughput | SQL Server · Oracle · MySQL · LINQ |
| 7 | **Pit Crew** (Observability & Quality) | Diagnostic scanner & pit crew | Production issues that span the app, chatbot and web; releases without a safety net | End-to-end RCA with OpenSearch; Selenium / Postman / JMeter; BDD/TDD; 90 % Sonar gate; Copilot + Claude pilot | +40 % productivity; load tests before go-live | OpenSearch · Selenium · JMeter · Postman · Sonar · GitHub Copilot · Claude |

### 3.1 Stage 1 story: the benchmark behind the dyno

Source: Manuel, 2026-11-19 (not in the CV). It adds **no new numbers**: the dyno still uses M1 (12–20 s → ~1.2 s).

- **The benchmark:** his manager asked for a benchmark of the Ionic app against native. Manuel ran it and presented the numbers. The dyno replays that benchmark.
- **Beyond speed:** the numbers only measure performance. Going native also put the app closer to the hardware, which makes wearables easier to support. This is a **possibility**, not a shipped feature. Don't present it as delivered.

| Key | EN | PT-BR | ES |
|---|---|---|---|
| `stage1.story.benchmark` | My manager asked for a benchmark: the Ionic app against native. I ran it and presented the numbers. This dyno replays it. | Meu gestor pediu um benchmark: o app Ionic contra o nativo. Eu rodei e apresentei os números. Este dinamômetro reproduz esse teste. | Mi gerente pidió un benchmark: la app Ionic contra la nativa. Lo ejecuté y presenté los números. Este dinamómetro lo reproduce. |
| `stage1.story.beyondSpeed` | Speed is only what a dyno measures. Going native also put the app closer to the hardware, opening the door to wearables. | Velocidade é só o que o dinamômetro mede. Ir para o nativo também deixou o app mais perto do hardware e abriu caminho para wearables. | La velocidad es solo lo que mide un dinamómetro. Pasar a nativo también acercó la app al hardware y abrió la puerta a los wearables. |

TODO(manuel): who the numbers were presented to (manager only, or leadership too?), and whether the benchmark drove the migration decision. Until then the copy just says "presented the numbers".

---

## 4. Service Log (career timeline)

Shown as a motorcycle **service history**: each entry is a "revision", with odometer = year.

| Odometer | Shop (company) | Role | Highlight |
|---|---|---|---|
| 2022 → now | Porto | Lead Engineer | Porto Saúde digital modernization (Trace stages 1–7) |
| 2022 | Digisystem | Operation Manager | Pre-sales and solution design for AMS / Service Desk / Cloud across BR + LATAM |
| 2019 → 2022 | Indra | Project Manager | Waterfall → Scrum, +30 % delivery speed; ~32 % margin sustained |
| 2018 → 2019 | Indra | Lead Software Engineer | Overtime 40 → 10 h/mo; RFPs worth R$500K–1M |
| 2017 → 2018 | Indra | Senior Software Engineer | SLA 70 % → 98 % (ITIL / HPSM) |
| 2016 → 2017 | Linx | Enterprise Software Engineer | WPF POS +15 % responsiveness |
| 2013 → 2016 | COREON | Software Engineer | +20 % performance for financial/insurance clients |
| 2009 → 2013 | DIMEP | Software Engineer | VB6 → C#/WPF; TCP/IP hardware SDK; device cost −73 % |
| 2008 → 2009 | Nunes Imóveis | Software Engineer | VB6 / Classic ASP → .NET; batch 8 h → 2 h |

**Education:** MBA in AI Engineering (UNIPDS, 2026–2027, in progress) · MBA in Project Management (Impacta, 2020–2021) · Associate degree in Systems Analysis and Development (FIEO, 2010–2013).

---

## 5. Toolbox (skills)

Grouped as **toolbox drawers**. No percentage bars, which are meaningless. Each tool shows *years used* and *where*.

| Drawer | Tools |
|---|---|
| Engine (languages & frameworks) | C#, .NET / .NET Framework, ASP.NET, Node.js, TypeScript, Angular, Java / Spring Boot, WPF |
| Diagnostics (testing & quality) | Selenium, JMeter, Postman, BDD/TDD (Given-When-Then), Sonar 90 % gates, Jetpack Compose Testing, XCUITest |
| Electronics (APIs, cloud, architecture) | REST, SOAP, Microservices, AWS, CA API Gateway, Sensedia, Redis, message idempotency |
| Scanner (observability) | OpenSearch, end-to-end root-cause analysis |
| Fluids (databases) | SQL Server, Oracle, MySQL |
| Power tools (AI-assisted engineering) | GitHub Copilot, Claude |
| Workshop management | Scrum, ITIL / HPSM, pre-sales & RFPs, budgeting, team building |

---

## 6. Garage (hobbies)

| Item | Copy direction | Link to work | Photo |
|---|---|---|---|
| 🏍️ Motorcycles / track days | Riding, and the feeling of a well-tuned machine | Performance: you feel 1.2 s vs 15 s the same way you feel a tuned engine | ✅ `track-03-lean` (featured), `track-01`, `track-02` |
| 🔧 Mechanics | Taking things apart to understand them, and putting them back better | Legacy modernization: respect the original, upgrade what matters | ⏳ `workbench/` (to provide) |
| 🎮 Games | Progression, feedback loops, a well-designed difficulty curve | Why this site has levels and achievements | ⏳ `games/` (to provide) |

Until a category has a photo, its card uses a line-art illustration (blueprint style) instead of an empty slot.

## 6.1 Media

All photos are listed in [`content/media/gallery.json`](../../content/media/gallery.json): file, category, focal point, credit, and `alt` + caption in the 3 languages. To add more photos, see [`content/media/README.md`](../../content/media/README.md).

| ID | Use | Status |
|---|---|---|
| `profile` | Recruiter Mode identity card, Pit Stop, Open Graph image | ✅ (a ≥ 800 px version would be better) |
| `track-01` | Gallery | ✅ credit: @mustelasportmedia |
| `track-02` | Gallery | ✅ credit: B.S Racing · MotoGP Cerezer |
| `track-03` | Garage card "Motorcycles" + gallery | ✅ credit: @mustelasportmedia |
| `track-04` … `track-09` | Gallery (Evo Racing track day) | ✅ credit: @mustelasportmedia · captions pending |
| `short-01` … `short-04` | Garage videos (YouTube Shorts, ADR-0010) | ✅ credit: @mustelasportmedia · descriptions drafted, to confirm |

---

## 7. Microcopy (shared UI strings)

| Key | EN | PT-BR | ES |
|---|---|---|---|
| `cta.recruiter` | Recruiter mode (30 s) | Modo recrutador (30 s) | Modo reclutador (30 s) |
| `cta.ride` | Start the ride | Dar a partida | Arrancar |
| `cta.cv` | Download CV (PDF) | Baixar CV (PDF) | Descargar CV (PDF) |
| `cta.linkedin` | Talk on LinkedIn | Conversar no LinkedIn | Hablar en LinkedIn |
| `hud.progress` | Trace progress | Progresso do trace | Progreso del trace |
| `dyno.run` | Run the dyno | Rodar o dinamômetro | Correr el dinamómetro |
| `a11y.skip` | Skip to content | Pular para o conteúdo | Saltar al contenido |
| `motion.toggle` | Reduce motion | Reduzir movimento | Reducir movimiento |
