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

> ⚠️ Contact is **LinkedIn or e-mail** (`manuelbravoc@gmail.com`, ADR-0013): no phone on the site. The CV PDF stays without phone and e-mail (LinkedIn URL only).

> Experience: Manuel worked with Delphi and Pascal from 2006 to 2008, before Nunes Imóveis (source: Manuel, 2026-11-19; not in the CV). The site computes the years from `careerStartYear: 2006` at build time ("20 years · since 2006" in 2026), so the number never goes stale.
>
> CV: `content/cv/manuel-bravo-resume.pdf` is Manuel's real CV (2026-11-19, 3 pages, English). Checked before publishing: LinkedIn is the only contact, no e-mail or phone in the text or the PDF metadata. If it is ever replaced by an empty or non-PDF file, the prerender warns locally and **fails the Azure Pipelines build**.

### 1.1 About me

Source: Manuel, 2026-11-19 (polished from his notes; not in the CV). Lives in `content/{locale}/about.json`. No new numbers: the years and stacks come from the career log (§4) and education.

- **Self-taught:** started with Delphi in 2006; learned each new stack hands-on (VB6, .NET, Node.js, cloud, now an MBA in AI Engineering).
- **Hobbies tied to the work:** track days and video games: measure before you touch anything, get the details right, improve one lap at a time.
- **Positioning** (replaced the "Goal" line on 2026-11-21, from a reviewer's suggestion Manuel adopted): hands-on engineering combined with technical leadership: architecture decisions, guiding teams, improving engineering practices, mentoring developers; great engineering = solving the right problems, keeping things simple, delivering measurable business impact.
- **How I work:** hands-on (code, PR reviews, architecture) · quality over quantity · simple first (no over-engineering) · active listening (stakeholders' needs first) · focus on what I can change (circles of control, influence and concern: energy goes to control and influence; concerns are tracked without stalling the team) · mentoring.

#### What I bring to your team

Source: Manuel, 2026-11-21. Home, between About and the Trace (`BringSection`); copy in `content/{locale}/ui.json` (`bring.*`). Three points, text only:

1. Modernizing critical platforms.
2. Technical leadership and growing teams.
3. Lower costs, risk and operational time.

### 1.2 Colophon: built with AI, directed by a human

Lives in `content/{locale}/colophon.json`. Agreed with Manuel on 2026-11-19: he set intent, content and decisions; Claude proposed architecture and solutions and wrote the code; nothing shipped without his review. The review examples are real moments from the build (hosting decision, "20+ years", game copyright, photo curation).

- **Usage figures** (Manuel's `ccusage daily` reports, summed): 2026-11-19, the day the site was built: 248.3 million tokens processed, 608 thousand output tokens, US$ 69.38; 2026-11-21, the refinements: 37,166,480 tokens (36,743,163 cache reads, 99 %), 134,286 output tokens, US$ 12.34. Shown as totals (285.5 million, 742 thousand, US$ 81.72) with the per-day split in the note. Snapshots: update by hand when a later report is used.
- **Repository figures** (commits, ADRs, test methods) are measured at build time by `RepoStats`; they're not stored here.

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
| M15 | WPF POS responsiveness | baseline | +15 % | +15 % | Linx | Service Log (not on stage 1: WPF read as off-topic next to the native-app migration, 2026-11-21) |

---

## 3. The Trace — stage content

Format for each stage: **Layer → Problem → Action → Result → Tech**. Copy is a draft; final wording is done per locale.

> On the site (2026-11-19, Manuel's request): no mechanics vocabulary in the professional sections. The garage-metaphor column below is design history only; stage labels show just "Stage N", stage 7 is "Observability & Quality", the Service Log is titled "Journey" and toolbox drawers have plain names. Motorcycles and mechanics stay in the Garage, where they are actual hobbies.

| # | Stage (game "level") | Garage metaphor | Problem | Action | Result | Tech chips |
|---|---|---|---|---|---|---|
| 1 | **Client** | Handlebars & dashboard | A 2016 Ionic app where a critical booking journey took 12–20 s | Led the migration to native iOS (Swift) and Android (Kotlin); moved regression testing to Compose Testing + XCUITest | Booking ~1.2 s | Swift · Kotlin · Jetpack Compose Testing · XCUITest |
| 2 | **Edge & Security** | Ignition key & immobilizer | Fraud in reimbursement workflows | Added facial biometrics to reimbursement | Fraud −40 % | Biometrics · AWS · Firewall troubleshooting |
| 3 | **API Gateway** | Fuel injection | High platform cost on CA API Gateway | Migrated the gateway to Sensedia | Cost −30 % | Sensedia · CA API Gateway · REST · SOAP |
| 4 | **Services** | Engine block (rebuild) | Legacy .NET/SOAP services with a large vulnerability surface | Modernized ~80 % of them to Node.js | Vulnerabilities −70 % | Node.js · TypeScript · C# · .NET · Microservices |
| 5 | **Cache & Messaging** | Transmission | Intermittent failures and duplicate processing | Message idempotency + Redis caching; reworked the architecture of a critical workflow | Workflow 8 h → 4 h; better reliability | Redis · Idempotency · AWS |
| 6 | **Database** | Oil & cooling | Deadlocks and slow batches on high-volume data | Tuned queries, transactions, locking strategy and stored procedures | Batch 8 h → 2 h; deadlocks eliminated; +20 % throughput | SQL Server · Oracle · MySQL · LINQ |
| 7 | **Pit Crew** (Quality, Observability & AI) | Diagnostic scanner & pit crew | Production issues that span the app, chatbot and web; releases without a safety net | End-to-end RCA with OpenSearch; Selenium / Postman / JMeter; BDD/TDD; 90 % Sonar gate; Copilot + Claude pilot | +40 % productivity; load tests before go-live | OpenSearch · Selenium · JMeter · Postman · Sonar · GitHub Copilot · Claude |

Stage 7 is shown as three blocks (Manuel, 2026-11-21), from the same facts, nothing added: **Quality** (releases without a safety net → Selenium / Postman / JMeter, BDD/TDD, 90 % Sonar gate → load tests before go-live; metric M8) · **Observability** (issues spanning app, chatbot and web → end-to-end RCA with OpenSearch; no result figure yet) · **AI productivity** (Copilot + Claude pilot → +40 %; metric M7).

### 3.1 Stage 1 story: the benchmark behind the dyno

Source: Manuel, 2026-11-19 (not in the CV). It adds **no new numbers**: the dyno still uses M1 (12–20 s → ~1.2 s).

- **The benchmark:** his manager asked for a benchmark of the Ionic app against native. Manuel ran it and presented the numbers. The dyno replays that benchmark.
- **Beyond speed:** the numbers only measure performance. Going native also put the app closer to the hardware, which makes wearables easier to support. This is a **possibility**, not a shipped feature. Don't present it as delivered.

| Key | EN | PT-BR | ES |
|---|---|---|---|
| `stage1.story.benchmark` | My manager asked for a benchmark: the Ionic app against native. I ran it and presented the numbers. This simulation replays it. | Meu gestor pediu um benchmark: o app Ionic contra o nativo. Eu rodei e apresentei os números. Esta simulação reproduz esse teste. | Mi gerente pidió un benchmark: la app Ionic contra la nativa. Lo ejecuté y presenté los números. Esta simulación lo reproduce. |
| `stage1.story.beyondSpeed` | Speed is only what a benchmark measures. Going native also put the app closer to the hardware, opening the door to wearables. | Velocidade é só o que o benchmark mede. Ir para o nativo também deixou o app mais perto do hardware e abriu caminho para wearables. | La velocidad es solo lo que mide un benchmark. Pasar a nativo también acercó la app al hardware y abrió la puerta a los wearables. |

TODO(manuel): who the numbers were presented to (manager only, or leadership too?), and whether the benchmark drove the migration decision. Until then the copy just says "presented the numbers".

### 3.2 Deep cases (stages 1, 4, 6)

Source: Manuel, 2026-11-21 (not in the CV). Shown under "Open case" (`case` in `stages.json`): context, decisions, architecture before → after, and the hard part when he gave one. Facts only; nothing below may be extended without asking him.

- **Stage 1 (Porto, native apps):** the app was on Ionic 3 (2016); going to Ionic 8 was practically a rebuild. Flutter / React Native were turned down to stay close to the hardware without a translation layer. UX redesigned the whole app and the team built the native apps from the new design. Of his 12 engineers, 4 are Kotlin and 4 Swift. Hard part: features kept coming, so the work tripled (Ionic + iOS + Android) until the current app's freeze in November 2026. **Native launch: 30 Nov 2026** (update the copy after launch).
- **Stage 4 (Porto, services):** ~120 .NET/SOAP endpoints on CA API Gateway; project started in 2023, when the company had no .NET approved for the cloud, hence Node.js. New services ran in parallel; Manuel switched routes one by one and asked consumers to use the Sensedia endpoint instead of the old CA API one. The −70 % comes from the security tenant Information Security runs. Target: Node.js containers running on ECS / EKS (AWS). The remaining ~20 % is planned for 2027: the native app (stage 1) took priority with the same team.
- **Stage 6 (Nunes Imóveis, database):** SQL Server; the batch processed payment slips (boletos) and bank reconciliation. Deadlocks came from badly written concurrency and missing transactions. Indexes, keys and removing N+1 queries solved it. The +20 % throughput was measured with Data Analyzer. TODO(manuel): the hard part.

---

## 4. Service Log (career timeline)

Shown as a motorcycle **service history**: each entry is a "revision", with odometer = year.

| Odometer | Shop (company) | Role | Highlight |
|---|---|---|---|
| Jul 2022 → now | Porto | Lead Engineer | Porto Saúde digital modernization (Trace stages 1–7) |

Current-role scope (Manuel, 2026-11-21; "scope" on the Porto entry in log.json; shown on the home page as "Leadership & scope" under What I bring, and as one line in Recruiter Mode): leads a team of 12 people · coordination-level decisions · interviews candidates, the manager makes the hire · chooses the stacks among the company-approved ones · decides the architecture, validated by Information Security ("SI").
| 2022 → Jun 2022 | Digisystem | Operation Manager | Pre-sales and solution design for AMS / Service Desk / Cloud across BR + LATAM |
| 2019 → 2022 | Indra | Project Manager | Waterfall → Scrum, +30 % delivery speed; ~32 % margin sustained |
| 2018 → 2019 | Indra | Lead Software Engineer | Overtime 40 → 10 h/mo; RFPs worth R$500K–1M |
| 2017 → 2018 | Indra | Senior Software Engineer | SLA 70 % → 98 % (ITIL / HPSM) |
| 2016 → 2017 | Linx | Enterprise Software Engineer | WPF POS +15 % responsiveness |
| 2013 → 2016 | COREON | Software Engineer | +20 % performance for financial/insurance clients |
| 2009 → 2013 | DIMEP | Software Engineer | VB6 → C#/WPF; TCP/IP hardware SDK; device cost −73 % |
| 2008 → 2009 | Nunes Imóveis | Software Engineer | VB6 / Classic ASP → .NET; batch 8 h → 2 h |
| 2006 → 2008 | *Early career* (company name not remembered) | Software Developer | Delphi · Pascal. Source: Manuel, 2026-11-19 (not in the CV). Replace "Early career" with the company name if it comes back |

**Education:** MBA in AI Engineering (UNIPDS, 2026–2027, in progress) · MBA in Project Management (Impacta, 2020–2021) · Associate degree in Systems Analysis and Development (FIEO, 2010–2013).

---

## 5. Toolbox (skills)

Grouped as **toolbox drawers**. No percentage bars, which are meaningless. Each tool shows *years used* and *where*.

| Drawer | Tools |
|---|---|
| Core expertise | C#, .NET / .NET Framework, ASP.NET, Node.js, TypeScript, Angular, SQL Server |
| Architecture & Cloud | AWS, Microservices, REST, SOAP, CA API Gateway, Sensedia, Redis, message idempotency, OpenSearch, end-to-end root-cause analysis, Oracle, MySQL |
| Engineering practices | BDD/TDD (Given-When-Then), Sonar 90 % gates, Selenium, JMeter, Postman, Jetpack Compose Testing, XCUITest, Scrum, ITIL / HPSM, GitHub Copilot, Claude, team building, pre-sales & RFPs, budgeting |
| Previous technologies (closed by default) | Java / Spring Boot, WPF, Delphi, Pascal |

Regrouped on 2026-11-21 (Manuel): the old seven drawers showed breadth but hid the current specialty.
---

## 6. Garage (hobbies)

| Item | Copy direction | Link to work | Photo |
|---|---|---|---|
| 🏍️ Motorcycles / track days | Riding, and the feeling of a well-tuned machine | Performance: you feel 1.2 s vs 15 s the same way you feel a tuned engine | ✅ `track-03-lean` (featured), `track-01`, `track-02` |
| 🔧 Mechanics | Taking things apart to understand them, and putting them back better | Legacy modernization: respect the original, upgrade what matters | ✅ `workbench-01` (AI-generated illustration, labelled on the site) |
| 🎮 Games | Progression, feedback loops, a well-designed difficulty curve | Why this site has levels and achievements | ✅ `games-01` (controller photo; the game on screen is blurred). Favourites (Manuel, 2026-11-19), shown as **text only**, never game art: Retro: Demon's Crest, Mega Man X, Mega Man 7, The Legend of Zelda, Metroid · Now: Overwatch, Baldur's Gate, Grand Theft Auto |

Until a category has a photo, its card uses a line-art illustration (blueprint style) instead of an empty slot.

## 6.1 Media

All photos are listed in [`content/media/gallery.json`](../../content/media/gallery.json): file, category, focal point, credit, and `alt` + caption in the 3 languages. To add more photos, see [`content/media/README.md`](../../content/media/README.md).

| ID | Use | Status |
|---|---|---|
| `profile` | Recruiter Mode identity card, Pit Stop, Open Graph image | ✅ (a ≥ 800 px version would be better) |
| `track-01` | Gallery | ✅ credit: @mustelasportmedia |
| `track-02` | Gallery | ✅ credit: B.S Racing · MotoGP Cerezer |
| `track-03` | Garage card "Motorcycles" + gallery | ✅ credit: @mustelasportmedia |
| `track-09` | Gallery (Evo Racing track day) | ✅ credit: @mustelasportmedia · caption: consistency before speed. Manuel kept four track photos (01, 02, 03, 09) on 2026-11-19; 04–08 were removed (still in git history) |
| `short-01` … `short-04` | Garage videos (self-hosted MP4, ADR-0012) | ✅ credit: @mustelasportmedia · descriptions drafted, to confirm |

---

## 7. Microcopy (shared UI strings)

| Key | EN | PT-BR | ES |
|---|---|---|---|
| `cta.recruiter` | Recruiter mode (30 s) | Modo recrutador (30 s) | Modo reclutador (30 s) |
| `cta.ride` | See the Trace | Ver o Trace | Ver el Trace |
| `cta.cv` | Download CV (PDF) | Baixar CV (PDF) | Descargar CV (PDF) |
| `cta.linkedin` | Talk on LinkedIn | Conversar no LinkedIn | Hablar en LinkedIn |
| `hud.progress` | Trace progress | Progresso do trace | Progreso del trace |
| `dyno.run` | Run the benchmark | Rodar o benchmark | Correr el benchmark |
| `a11y.skip` | Skip to content | Pular para o conteúdo | Saltar al contenido |
| `motion.toggle` | Reduce motion | Reduzir movimento | Reducir movimiento |
