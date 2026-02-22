# Research: Markdown ASCII Presentation Engine

## Decision 1: Markdown sectioning model

- Decision: Treat only `#` and `##` headings as slide boundaries; render `###` and deeper headings as in-slide content.
- Rationale: Matches clarified product behavior and keeps navigation deterministic for presenters.
- Alternatives considered:
  - All heading levels create slides (rejected: over-fragments decks).
  - Only `##` creates slides (rejected: conflicts with title-slide requirement).

## Decision 2: Checkbox parsing and interaction

- Decision: Recognize GitHub-style task list items (`- [ ]` / `- [x]`) as interactive option boxes, with keyboard (Up/Down + Space) and mouse click toggle support.
- Rationale: GitHub-style task lists are familiar markdown syntax and mouse support improves live presentation ergonomics.
- Alternatives considered:
  - Raw `[]` markers without list-item semantics (rejected: less markdown-consistent and higher false-positive parsing risk).
  - Keyboard-only interaction (rejected: limits accessibility and presenter speed during live sessions).

## Decision 3: Notes persistence strategy

- Decision: Maintain a sibling `{filename}.notes.md` artifact containing full presentation plus selected state and appended notes slide.
- Rationale: Supports recoverability/resume and keeps source markdown immutable.
- Alternatives considered:
  - Overwrite source markdown in place (rejected: destructive and hard to diff).
  - Store state in JSON sidecar only (rejected: contradicts explicit `.notes.md` artifact requirement).

## Decision 4: Session resume and reset behavior

- Decision: On startup, load state from existing `.notes.md` when available; `Start New` deletes notes file (if present) and restarts from slide 1.
- Rationale: Aligns with presenter workflow for rehearsals and fresh reruns.
- Alternatives considered:
  - Always prompt whether to resume (rejected: adds extra flow not required by spec).
  - Keep old notes and start from slide 1 without delete (rejected: violates explicit reset expectation).

## Decision 5: Escape and main-menu control model

- Decision: `Esc` on notes slide returns to previous slide; `Esc` elsewhere opens main menu. Main menu uses Up/Down + Enter.
- Rationale: Resolves key conflicts while preserving quick note-review jump-back behavior.
- Alternatives considered:
  - `Esc` exits globally (rejected: conflicts with notes return requirement).
  - Number-key menu only (rejected: less consistent with arrow-key-centric UX).

## Decision 6: "Create a Song" prompt generation

- Decision: Generate a copyable prompt from available presentation content, selected options, and notes; do not fail if selections/notes are absent.
- Rationale: Meets FR-022 to FR-024 and supports graceful behavior in sparse sessions.
- Alternatives considered:
  - Require all three inputs before generation (rejected: blocks common usage).
  - Integrate directly with external music API/site (rejected: out of scope; spec requires prompt text only).

## Decision 7: ASCII large-title rendering approach

- Decision: Use a fixed internal ASCII banner style for `#` title slides and large `##` headers, constrained by terminal width.
- Rationale: No extra runtime dependencies and predictable cross-platform behavior.
- Alternatives considered:
  - External FIGlet package/tool (rejected: avoid dependency and installation complexity).
  - Plain non-banner headings (rejected: misses core visual requirement).

## Decision 8: Testing strategy

- Decision: Expand parser tests and add state-behavior tests (navigation, menu routing, selection toggling, notes-state restore) at unit level.
- Rationale: Highest confidence for core rules in current architecture without brittle terminal-render snapshot coupling.
- Alternatives considered:
  - Full end-to-end terminal automation only (rejected: heavier and less deterministic for immediate implementation).
  - Manual-only verification (rejected: insufficient regression protection).
