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

* **Swagger UI**: [http://localhost:8081/swagger](http://localhost:8080/swagger)
* **Health Check**: [http://localhost:8081/health](http://localhost:8080/health)

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

## ⚙️ Running the TeamCity CI

To run the CI you need to:
* run build.ps1  
* when complete proceed with the install of team city using default settings
* click create project
* use this url for the repository: https://github.com/ace90210/senior-engineer-home-exercise
* click proceed
* select to import settings.kts and proceed with default settings
* **important**
* add the agent as a authorised agent
   * go to the agents tab and find the unauthorised agent from the list
   * add this agent as authorised
   * go back to the project 
* Run the CI

📽️ See a demo video here: [Watch on YouTube](https://youtu.be/b-o_vSLEl3Q)



### In Progress / Blockers

* agent is not automatically registered in team city, **You must register it as an authorised agent first!**
* I did not manage to fully remote root/sudo requirement from the agent container. this was because it was required for running docker push and i did not get time to resolve this properly

---

---

## ⚙️ TeamCity CI Stack

### What’s Completed

* Created `compose.ci.yml` to spin up:

  * `teamcity-server` (on port 8111)
  * `teamcity-agent` from `Dockerfile.agent`
  * Local Docker `registry` (on port 5000)
* Agent has Docker CLI installed and mounts `/var/run/docker.sock`.
* kotlin settings.kts setup to run deployment stages

### In Progress / Blockers

* agent is not automatically registered in team city, **You must register it as an authorised agent first!**
* I did not manage to fully remote root/sudo requirement from the agent container. this was because it was required for running docker push and i did not get time to resolve this properly

---

## 🛠️ build.ps1

A PowerShell helper script is provided to bring up the CI stack and poll readiness:

```ps1
ci-up
```

---

## 🧾 Deliverable Structure

```
├── .teamcity/
│   └── settings.kts         
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
* had no time to learn kotlin so i made the tragic mistake of trying to cut corners by using AI, lost quite abit of time on this before i just looked up the official template from jetbrains for dotnet deploys and reverts engineered it to my needs.


If given additional time, I would focus next on:

* fix the workaround issue in the teamcity agent where i use sudo
* Expanding unit test coverage.
* Adding input model separation and improved validation handling for minimal API idioms.

If this was a early production prototype i would
* Implement caching with redis
* Create a deployment pipeline
* implement a real EF provider
* lots more
