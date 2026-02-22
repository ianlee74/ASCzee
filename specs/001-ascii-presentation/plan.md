# Implementation Plan: ASCzee

## Phase 1 – Project Scaffold ✅
- [x] Create `ASCzee.slnx` solution
- [x] Create `src/ASCzee` console app project (net8.0)
- [x] Create `src/ASCzee.Tests` xUnit test project

## Phase 2 – Core Domain ✅
- [x] `Slide` model
- [x] `Presentation` model
- [x] `PresentationParser` – parse `.ascz` markup into slides
- [x] `PresentationViewer` – interactive console viewer

## Phase 3 – Tests ✅
- [x] Unit tests for `PresentationParser`

## Phase 4 – Build & Run Scripts ✅
- [x] `scripts/build.sh`
- [x] `scripts/run.sh`
- [x] `scripts/test.sh`

## Phase 5 – SpecKit Infrastructure ✅
- [x] `.specify/scripts/bash/common.sh`
- [x] `.specify/scripts/bash/check-prerequisites.sh`
- [x] `.specify/scripts/bash/setup-plan.sh`
- [x] `specs/001-ascii-presentation/spec.md`
- [x] `specs/001-ascii-presentation/plan.md`
