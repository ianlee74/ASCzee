# Implementation Plan: Markdown ASCII Presentation Engine

**Branch**: `001-markdown-presentation-engine` | **Date**: 2026-02-21 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-markdown-presentation-engine/spec.md`

## Summary

Implement a markdown-driven ASCII presentation CLI that treats `#` and `##` as slide boundaries, supports keyboard-first presentation flow, persists annotations in `{filename}.notes.md`, resumes prior session state, and provides a main menu with Exit/Start New/Create a Song actions.

## Technical Context

**Language/Version**: C# / .NET 8 (`net8.0`)  
**Primary Dependencies**: .NET BCL (`System.Console`, `System.IO`, `System.Text`), xUnit test stack (`xunit`, `Microsoft.NET.Test.Sdk`, `coverlet.collector`)  
**Storage**: Local markdown files (`.md`) and annotated notes files (`.notes.md`) on disk  
**Testing**: `dotnet test` with xUnit; parser/unit tests + viewer interaction tests where feasible  
**Target Platform**: Cross-platform terminal environments supported by .NET console input  
**Project Type**: CLI application with single executable and separate test project  
**Performance Goals**: Interactive key response appears immediate to presenter (target <100ms perceived input-to-render), notes persistence updates during session without noticeable UI pauses  
**Constraints**: Offline-capable, terminal-only rendering, ASCII-compatible output, deterministic keyboard mappings from spec  
**Scale/Scope**: Single-presenter session, decks from 1 to ~200 slides, option boxes and notes per slide, one notes artifact per source file

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Constitution file `.specify/memory/constitution.md` currently contains placeholders only and defines no enforceable project principles or gates.
- Result (Pre-Phase-0): **PASS (no actionable gates to evaluate)**.
- Practical guardrails applied anyway: keep architecture simple, test parser and state transitions, avoid unnecessary dependencies, preserve CLI-first behavior.
- Result (Post-Phase-1): **PASS (design artifacts remain aligned with above guardrails)**.

## Project Structure

### Documentation (this feature)

```text
specs/001-markdown-presentation-engine/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── markdown-presentation-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
src/
├── ASCzee/
│   ├── Program.cs
│   ├── Presentation.cs
│   ├── Slide.cs
│   ├── PresentationParser.cs
│   └── PresentationViewer.cs
└── ASCzee.Tests/
    └── PresentationParserTests.cs

scripts/
├── build.sh
├── run.sh
└── test.sh
```

**Structure Decision**: Keep the existing single CLI project plus companion test project. Add feature behavior by extending parser/viewer/state models in `src/ASCzee` and validating behavior in `src/ASCzee.Tests`.

## Complexity Tracking

No constitution violations identified that require exception tracking.
