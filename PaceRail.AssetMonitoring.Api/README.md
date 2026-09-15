# PaceRail Infrastructure Asset Monitoring API

> Production-grade, enterprise-standard backend system built with **.NET 10** and **PostgreSQL** for PACE Infrastructure Solutions. Manages rail civils and electrical assets across the UK rail network using linear referencing (ELRs and Chainage) and spatial proximity safety evaluation.

---

## 🛠 Tech Stack & Architecture

| Layer | Technology |
| :--- | :--- |
| **Framework** | .NET 10 (ASP.NET Core Web API) |
| **Database** | PostgreSQL with Entity Framework Core |
| **Domain Engine** | Linear Chainage Proximity Evaluator (`RailChainageEvaluator`) |
| **Infrastructure** | Nginx Reverse Proxy on Ubuntu 24.04 (DigitalOcean VPS) |
| **CI/CD Pipeline** | GitHub Actions Automated Workflows |
| **Dashboard** | HTML5 / CSS3 Single-Page Interface (`wwwroot/index.html`) |

---

## ⚡ Key Architecture & Features

* **Linear Referencing System:** Complete asset tracking across Engineers Line References (ELRs) and start chainages (miles/yards).
* **Spatial Safety Rule Engine:** Scans adjacent rail assets along the line reference when a **Critical Defect** is flagged, automatically cascading nearby assets within a 0.5-mile radius to **Inspection Due**.
* **Zero-Dependency Health Checks:** Live PostgreSQL database health monitoring exposed at `/health`.
* **Global Exception Middleware:** Unified, structured JSON error handling across all API endpoints.
* **Developer Workflow:** Full `dotnet watch` hot-reloading support and automated startup database seeding.

---

## 📁 Repository Structure

```text
PaceRail.AssetMonitoring/
│
├── PaceRail.AssetMonitoring.Api/
│   ├── Controllers/
│   │   └── AssetsController.cs          # API endpoints & safety triggers
│   ├── Data/
│   │   ├── AppDbContext.cs              # EF Core context & indexing
│   │   └── DbInitializer.cs             # Automatic startup data seeding
│   ├── Middleware/
│   │   └── ExceptionMiddleware.cs       # Global exception handling
│   ├── Models/
│   │   ├── RailAsset.cs                 # Rail asset domain model
│   │   ├── InspectionLog.cs             # Inspection history entity
│   │   └── AssetRuleViolation.cs        # Spatial safety violation model
│   ├── Services/
│   │   └── RailChainageEvaluator.cs     # Linear proximity domain logic
│   ├── wwwroot/
│   │   └── index.html                   # Dashboard UI & alerts
│   └── Program.cs                       # DI container & startup pipeline
│
├── scripts/
│   ├── setup-server.sh                  # Ubuntu server provisioning
│   └── backup-db.sh                     # PostgreSQL backup automation
│
└── .github/workflows/
    └── deploy.yml                       # GitHub Actions CI/CD pipeline