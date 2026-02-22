# Tasks: Markdown ASCII Presentation Engine

**Input**: Design documents from `/specs/001-markdown-presentation-engine/`
**Prerequisites**: plan.md (required), spec.md (required), research.md, data-model.md, contracts/, quickstart.md

**Tests**: No dedicated test-first tasks are listed because TDD was not explicitly mandated in the specification. Validation is included in polish and quickstart verification tasks.

**Organization**: Tasks are grouped by user story so each story can be implemented and verified independently.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story]**: User story label (`[US1]`, `[US2]`, etc.)
- Every task includes an exact file path

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Align project entry points and docs for markdown-based presentations.

- [x] T001 Update CLI usage/help and markdown input validation messaging in src/ASCzee/Program.cs
- [x] T002 Update run instructions and markdown examples in README.md
- [x] T003 [P] Add markdown sample deck for manual checks in specs/001-markdown-presentation-engine/quickstart.md

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core domain/session infrastructure required before all user stories.

**⚠️ CRITICAL**: No user story implementation should begin until this phase is complete.

- [x] T004 Extend presentation domain types for slide kinds and interactive items in src/ASCzee/Slide.cs
- [x] T005 [P] Extend presentation-level session state fields in src/ASCzee/Presentation.cs
- [x] T006 [P] Add session state container for navigation, selections, and notes in src/ASCzee/PresentationSession.cs
- [x] T007 [P] Add notes artifact load/save/delete service skeleton in src/ASCzee/NotesArtifactService.cs
- [x] T008 Add shared keyboard action mapping model in src/ASCzee/InputAction.cs
- [x] T009 Refactor viewer loop to route through session + input actions in src/ASCzee/PresentationViewer.cs

**Checkpoint**: Foundation complete — user stories can proceed.

---

## Phase 3: User Story 1 - Run and Navigate a Markdown Presentation (Priority: P1) 🎯 MVP

**Goal**: Open markdown presentations, build slides from `#`/`##` sections, render large headings, and navigate via left/right arrows.

**Independent Test**: Run `dotnet run --project src/ASCzee -- <deck.md>` and verify slide segmentation, title rendering, and left/right navigation boundaries.

### Implementation for User Story 1

- [x] T010 [P] [US1] Implement heading-boundary parsing for `#` and `##` with `###+` as body content in src/ASCzee/PresentationParser.cs
- [x] T011 [P] [US1] Add ASCII banner renderer for title/heading output in src/ASCzee/AsciiBannerRenderer.cs
- [x] T012 [US1] Render title slide and standard slide layouts with markdown-like body flow in src/ASCzee/PresentationViewer.cs
- [x] T013 [US1] Enforce left/right slide navigation with first/last-slide boundary handling in src/ASCzee/PresentationViewer.cs
- [x] T014 [US1] Wire markdown startup path and parser invocation for `.md` deck files in src/ASCzee/Program.cs

**Checkpoint**: US1 delivers a usable MVP presentation flow.

---

## Phase 4: User Story 2 - Interact with Option Boxes During Delivery (Priority: P2)

**Goal**: Support GitHub-style task list items (`- [ ]` / `- [x]`) as interactive option boxes with Up/Down focus, Space toggle, and mouse-click toggle.

**Independent Test**: Open a slide with multiple `- [ ]` items; verify focus movement using Up/Down, toggling with Space, and mouse-click toggling updates visible state.

### Implementation for User Story 2

- [x] T015 [P] [US2] Parse GitHub-style task list option boxes (`- [ ]` / `- [x]`) and map to slide option entities in src/ASCzee/PresentationParser.cs
- [x] T016 [US2] Track per-slide option focus index and movement rules in src/ASCzee/PresentationSession.cs
- [x] T017 [US2] Handle Up/Down focus movement and Space toggle input in src/ASCzee/PresentationViewer.cs
- [x] T018 [US2] Render option boxes with `[]` / `[X]` state in slide body output in src/ASCzee/PresentationViewer.cs
- [x] T019 [US2] Propagate toggled option state into in-memory presentation/session model in src/ASCzee/Presentation.cs
- [x] T020 [US2] Add mouse-click option toggle handling with keyboard fallback in src/ASCzee/PresentationViewer.cs

**Checkpoint**: US2 option-box interaction is independently functional.

---

## Phase 5: User Story 3 - Capture, Review, and Resume Notes (Priority: P3)

**Goal**: Capture notes with Insert, maintain `{filename}.notes.md`, jump to notes slide (F1), return via Esc, and resume state on reopen.

**Independent Test**: Capture notes and selections, restart app, confirm restored state from `.notes.md`, and verify F1/Esc notes navigation behavior.

### Implementation for User Story 3

- [x] T021 [US3] Implement Insert-driven note capture flow bound to current slide context in src/ASCzee/PresentationViewer.cs
- [x] T022 [P] [US3] Implement annotated markdown serialization/deserialization in src/ASCzee/NotesArtifactService.cs
- [x] T023 [US3] Generate and append dedicated notes slide content in notes artifact output in src/ASCzee/NotesArtifactService.cs
- [x] T024 [US3] Load existing `{filename}.notes.md` at startup and restore selections/notes in src/ASCzee/Program.cs
- [x] T025 [US3] Implement F1 jump-to-notes and Esc return-to-previous-slide behavior in src/ASCzee/PresentationViewer.cs
- [x] T026 [US3] Persist notes artifact updates during runtime state changes in src/ASCzee/PresentationViewer.cs

**Checkpoint**: US3 note lifecycle and resume workflow are independently functional.

---

## Phase 6: User Story 4 - Open Main Menu and Run Session Actions (Priority: P3)

**Goal**: Open main menu with Esc (non-notes slides), navigate menu with Up/Down, confirm with Enter, and execute Exit/Start New/Create a Song.

**Independent Test**: Open menu from normal slide, execute each menu action, confirm retention/reset behavior, and verify generated song prompt content.

### Implementation for User Story 4

- [x] T027 [P] [US4] Add main-menu state/action model (`Exit`, `StartNew`, `CreateSong`) in src/ASCzee/MainMenuState.cs
- [x] T028 [US4] Implement Esc-to-menu routing for non-notes slides and menu rendering in src/ASCzee/PresentationViewer.cs
- [x] T029 [US4] Implement Up/Down focus and Enter execution for main menu actions in src/ASCzee/PresentationViewer.cs
- [x] T030 [US4] Implement `Exit` action preserving notes artifact and ending session in src/ASCzee/PresentationViewer.cs
- [x] T031 [US4] Implement `Start New` action deleting `{filename}.notes.md` and restarting from slide 1 in src/ASCzee/NotesArtifactService.cs
- [x] T032 [P] [US4] Implement song prompt generation from slides + selections + notes (with degraded-input fallback) in src/ASCzee/SongPromptGenerator.cs
- [x] T033 [US4] Present copyable Create-a-Song prompt output flow in src/ASCzee/PresentationViewer.cs

**Checkpoint**: US4 main-menu and song-prompt features are independently functional.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Final consistency, documentation, and readiness checks across all stories.

- [x] T034 [P] Update runtime controls and menu behavior documentation in README.md
- [x] T035 [P] Align quickstart verification steps with implemented behavior in specs/001-markdown-presentation-engine/quickstart.md
- [x] T036 Normalize user-facing error and warning messages across runtime flows in src/ASCzee/Program.cs
- [x] T037 Validate automation scripts still run expected commands for build/test cycle in scripts/test.sh

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)**: no dependencies.
- **Phase 2 (Foundational)**: depends on Phase 1; blocks all user stories.
- **Phase 3+ (User Stories)**: depend on Phase 2 completion.
- **Phase 7 (Polish)**: depends on completion of desired user stories.

### User Story Dependencies

- **US1 (P1)**: starts immediately after Foundational; no dependency on other stories.
- **US2 (P2)**: starts after Foundational; builds on parser/viewer foundations from Phase 2 and can integrate with US1 output.
- **US3 (P3)**: starts after Foundational; relies on session + artifact service from Phase 2 and can run independently of US4.
- **US4 (P3)**: starts after Foundational; integrates best once US3 note lifecycle exists, but can be implemented incrementally.

### Suggested Story Order

1. US1 (MVP)
2. US2
3. US3
4. US4

---

## Parallel Opportunities

### User Story 1

- Run T010 and T011 in parallel (parser boundary logic vs banner rendering).

### User Story 2

- Run T015 and T016 in parallel (parser option extraction vs session focus tracking).

### User Story 3

- Run T021 and T024 in parallel (artifact serialization vs viewer notes-navigation behavior).

### User Story 4

- Run T026 and T031 in parallel (menu model vs prompt generation service).

---

## Implementation Strategy

### MVP First (US1 only)

1. Complete Phase 1 and Phase 2.
2. Complete Phase 3 (US1).
3. Validate US1 independently using the quickstart run flow.

### Incremental Delivery

1. Deliver US1 for baseline presentation capability.
2. Add US2 interactive option boxes.
3. Add US3 notes capture + resume.
4. Add US4 main menu and song prompt actions.
5. Finish with Phase 7 polish and docs alignment.

### Parallel Team Strategy

1. Team completes Setup + Foundational together.
2. Then split by story tracks:
   - Dev A: US1/US2 viewer-parser path
   - Dev B: US3 notes artifact path
   - Dev C: US4 menu and song prompt path
3. Merge on shared viewer/session files with short-lived branches and frequent integration.
