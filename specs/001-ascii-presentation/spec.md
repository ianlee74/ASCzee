# Feature Spec: ASCzee – ASCII Markup Driven Presentation App

**Feature Branch**: `001-ascii-presentation`
**Created**: 2026-02-21
**Status**: In Progress

## Overview

ASCzee is a terminal-based presentation tool that reads plain-text `.ascz` files and
renders them as interactive slide shows in the console.

## User Scenarios

### Story 1 – View a Presentation (P1)

A user runs `ASCzee slides.ascz` and navigates through slides using arrow keys.

**Acceptance Scenarios**:
1. **Given** a valid `.ascz` file, **When** the user runs `ASCzee <file>`, **Then** the first slide is displayed.
2. **Given** a presentation is open, **When** the user presses Right Arrow / Space / Enter, **Then** the next slide is shown.
3. **Given** a presentation is open, **When** the user presses Left Arrow / Backspace, **Then** the previous slide is shown.
4. **Given** a presentation is open, **When** the user presses Q or Escape, **Then** the app exits cleanly.
5. **Given** an invalid or missing file, **When** the user runs the app, **Then** a clear error message is printed.

## Markup Format

- Slides are separated by lines containing only `---`.
- The first line in a slide starting with `# ` is the slide title.
- Remaining lines form the slide body.

**Example**:
```
# Welcome to ASCzee

The ASCII markup driven presentation tool.

---
# Getting Started

Run:  ASCzee my-talk.ascz
```

## Requirements

- **FR-001**: Parse `.ascz` files into an ordered list of slides.
- **FR-002**: Display each slide with title and body in the terminal.
- **FR-003**: Support keyboard navigation (next, previous, quit).
- **FR-004**: Display slide progress indicator (e.g. "Slide 2/5").
- **FR-005**: Print a helpful error when the file is missing or unreadable.

## Success Criteria

- **SC-001**: All unit tests for `PresentationParser` pass.
- **SC-002**: The app compiles and runs without error on .NET 8.
- **SC-003**: `build.sh`, `run.sh`, and `test.sh` scripts all succeed.
