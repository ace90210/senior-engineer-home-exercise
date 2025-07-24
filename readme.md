# People.API – Take‑Home Assessment

## 📌 Overview

This is a small CRUD API built using **.NET 8 Minimal API**, intended to demonstrate containerization and CI pipeline setup with **TeamCity** via **Kotlin DSL**, as outlined in the take-home brief.

The solution provides:

* An API to manage people records with basic validation.
* Dockerized deployment.
* A partially implemented TeamCity CI stack with room for expansion.

---

## 🚀 Quick Start

### Local API Usage

You can start the API locally using Docker:

```bash
docker compose up
```

Then navigate to:

* **Swagger UI**: [http://localhost:8080/swagger](http://localhost:8080/swagger)
* **Health Check**: [http://localhost:8080/health](http://localhost:8080/health)

> Health check returns HTTP `200 OK` for basic liveness verification.

### API Capabilities

This is a minimal CRUD API that supports:

* `POST /people`: Add a new person
* `GET /people`: List all people
* `PUT /people/{id}`: Update a person by ID
* `DELETE /people/{id}`: Delete a person by ID

Request payloads follow:

```json
{
  "name": "John Doe",
  "dateOfBirth": "1990-01-01"
}
```

Validation is in place for name length and required fields.

---

## ✅ Implementation Summary

### ✔ API Implementation

* Built using **.NET 8 Minimal API** with clean separation of logic.
* Model: `{ Id: int, Name: string, DateOfBirth: DateOnly }`.
* **Validation** is implemented, with mindful avoidance of over-engineering (e.g. no repository pattern or caching).
* **Swagger (OpenAPI)** integrated.
* **Http testing file** added examples for all scenarios
* **Health endpoint** provided at `/health`.

> ⚠️ **Challenge**: Validation using `record` types in minimal APIs doesn't work cleanly with data annotations due to model binding quirks. Debugging this cost about an hour. I pivoted to using standard objects for reliable behavior.

### ✔ Unit Testing

* Used `WebApplicationFactory` to support integration-style testing of minimal APIs.
* This approach enables full-stack HTTP request testing without exposing internal logic unnecessarily.
* Only basic tests included due to time prioritization for Docker/CI efforts.

---

## 🐳 Dockerization

### Dockerfile

* Multi-stage `Dockerfile` implemented.
* Self-contained `linux-x64` build.
* Runs as **non-root** using `mcr.microsoft.com/dotnet/aspnet:8.0-alpine`.
* Internal + external port: `8080`.

### docker-compose.yml

```yaml
ports:
  - "8080:8080"
```

Enables `docker compose up` to start the API locally.

---

## ⚙️ TeamCity CI Stack

### What’s Completed

* Created `compose.ci.yml` to spin up:

  * `teamcity-server` (on port 8111)
  * `teamcity-agent` from `Dockerfile.agent`
  * Local Docker `registry` (on port 5000)
* Agent has Docker CLI installed and mounts `/var/run/docker.sock`.

### In Progress / Blockers

* **Kotlin DSL** (`/.teamcity/settings.kts`) setup was **not completed** due to lack of prior experience.
* Initial Kotlin files were started but **did not compile** correctly.
* Spent significant time troubleshooting, but with limited Kotlin DSL experience and timebox constraints (4h), I was unable to complete this portion.

> I relied on AI tools for Kotlin DSL scaffolding, but they did not generate a working setup. This was the main area I fell short on.

---

## 🛠️ build.ps1

A PowerShell helper script is provided to bring up the CI stack and poll readiness:

```ps1
ci-up
```

Or manually:

```bash
docker compose -f compose.ci.yml up -d && open http://localhost:8111
```

---

## 🧾 Deliverable Structure

```
├── .teamcity/
│   └── settings.kts         # (incomplete) Kotlin DSL config
├── src/
│   ├── People.Api/
│   ├── People.Data/
│   └── People.Tests/
├── build.ps1
├── compose.ci.yml
├── docker-compose.yml
├── Dockerfile
├── Dockerfile.agent
└── README.md
```

---

## 📉 Tradeoffs

| Area            | Summary                                                                    |
| --------------- | -------------------------------------------------------------------------- |
| Time Management | Prioritized working API and Docker setup over complete CI/Kotlin coverage. |
| Testing         | Added only basic test cases due to time constraints.                       |
| CI/CD           | TeamCity agent/server/registry boot OK. Kotlin DSL remains incomplete.     |
| Learning Gap    | Kotlin DSL was unfamiliar; needed more ramp-up time to deliver end-to-end. |

---

## 🏁 Final Notes

I hit some challenges and this is far from my best work sadly a number of compounding challenges delayed me more than expected. they included
* using records with validation was new to me and had unexpected quirks
* moving the docker file to the root combined with my mistake using the auto generated docker and moving it, i normally keep it in the project, i know how to move it but didnt realise the pre built docker project *really* does not like being moved and caused me all sorts or trouble before i just removed and did it manually
* had no time to learn kotlin so was unable to get it working


If given additional time, I would focus next on:

* Finishing the Kotlin DSL build steps and registry push.
* Expanding unit test coverage.
* Adding input model separation and improved validation handling for minimal API idioms.

If this was a early production prototype i would
* Implement caching with redis
* Create a deployment pipeline
* implement a real EF provider
* lots more
