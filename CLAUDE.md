# CI/CD Lab — rules for Claude

Plan: "CI/CD Lab — Curriculum & Tracker"
https://claude.ai/code/artifact/02325dfb-5276-425a-9907-7bfd3e3aa63c

## Working agreement
- Ahmed writes every Dockerfile, pipeline, Compose file and script.
- Claude explains, reviews and troubleshoots, and explains every shell command.
  Full solutions only on request or when stuck.
- Exception: Claude scaffolded the Orders API (`src/`, `tests/`) in Session 1, Part 1.
- Failure drills are part of every session.
- No `latest`. Versioned, immutable images only.
- `localhost` inside a runner or job container is that container, not the API.

## Current session
Session 1: Manual release and the Dockerfile
- Part 1: Repo and Orders API scaffold
- Part 2: Manual release by hand -> docs/manual-release.md
- Part 3: The Dockerfile and .dockerignore, cache measurements
- Part 4: Break drills and the build-once pressure test

## Resume point
Done: scaffold extracted.
Broken now: nothing.
Next: tool restore, InitialCreate migration, dotnet test green, first commit.
