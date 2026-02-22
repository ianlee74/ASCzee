# Feature Specification: Markdown ASCII Presentation Engine

**Feature Branch**: `001-markdown-presentation-engine`  
**Created**: 2026-02-21  
**Status**: Draft  
**Input**: User description: "I want this app to be an ASCII based presentation engine that uses markdown files as a definition for the presentation..."

## Clarifications

### Session 2026-02-21

- Q: How should `Esc` behave during presentation flow? → A: If on notes slide, pressing `Esc` immediately returns to the previously viewed slide; from any other slide, pressing `Esc` opens a main menu with `Exit`, `Start New`, and `Create a Song` options.
- Q: What inputs should be used for `Create a Song` prompt generation? → A: Use presentation slides plus selected options plus presenter notes.
- Q: How should `Create a Song` behave if selections or notes are missing? → A: Generate from available data and skip missing selections/notes.
- Q: How should markdown heading levels `###` and deeper be handled? → A: Only `#` and `##` create slides; `###`+ are rendered inside the current slide body.
- Q: How should main menu actions be selected and confirmed? → A: Use Up/Down to select and Enter to confirm.
- Q: What checkbox syntax should option boxes use? → A: Use GitHub-style markdown task list items (`- [ ]` / `- [x]`) for option boxes.
- Q: Should option boxes support mouse interaction? → A: Yes, mouse clicking an option box toggles it.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Run and Navigate a Markdown Presentation (Priority: P1)

As a presenter, I can start the app with a markdown file and move slide-by-slide using keyboard navigation so I can deliver a talk in the terminal.

**Why this priority**: Starting and navigating slides is the core value of the product; without this, no presentation can be delivered.

**Independent Test**: Can be fully tested by opening a markdown file, verifying title slide and regular slide rendering, and navigating forward/backward through all slides.

**Acceptance Scenarios**:

1. **Given** a valid markdown file path, **When** the presenter runs `asczee {markdown filename}`, **Then** the presentation opens at the first slide.
2. **Given** a `#` heading section, **When** that section is shown, **Then** the heading text is displayed as large full-width ASCII art and section body content is shown beneath it in ASCII-friendly markdown formatting.
3. **Given** a `##` heading section, **When** that section is shown, **Then** the heading text is displayed prominently across the top and the section body content is shown until the next slide-defining heading.
4. **Given** the presenter is viewing any non-final slide, **When** Right Arrow is pressed, **Then** the next slide is shown.
5. **Given** the presenter is viewing any non-first slide, **When** Left Arrow is pressed, **Then** the previous slide is shown.

---

### User Story 2 - Interact with Option Boxes During Delivery (Priority: P2)

As a presenter, I can move through checkbox-style options on a slide and toggle selections so I can annotate decisions live while presenting.

**Why this priority**: Interactive selection is a key differentiator and is required for producing meaningful annotated outputs.

**Independent Test**: Can be fully tested on a single slide containing multiple `[]` items by moving focus with Up/Down and toggling with Space.

**Acceptance Scenarios**:

1. **Given** a slide body containing GitHub-style task list items (`- [ ]` or `- [x]`), **When** the slide is displayed, **Then** those items are treated as selectable option boxes.
2. **Given** a slide with selectable option boxes, **When** the presenter presses Up Arrow or Down Arrow, **Then** focus moves between option boxes in visible order.
3. **Given** a focused option box, **When** the presenter presses Space, **Then** the option toggles selected state and selected state is shown with `X` between brackets.
4. **Given** a slide with selectable option boxes, **When** the presenter clicks an option box with the mouse, **Then** that option toggles selected state immediately.

---

### User Story 3 - Capture, Review, and Resume Notes (Priority: P3)

As a presenter, I can capture notes during delivery, review all notes quickly, and resume previous session state from an annotated file.

**Why this priority**: Persisted annotations and notes are essential for iterative rehearsals and post-presentation records.

**Independent Test**: Can be fully tested by taking notes with Insert, making option selections, closing and reopening the same presentation, and confirming restored state and notes-slide navigation.

**Acceptance Scenarios**:

1. **Given** the presentation is open, **When** Insert is pressed, **Then** the presenter can record a note linked to the current session.
2. **Given** the presentation has notes and/or option selections, **When** the session is active, **Then** an annotated markdown file named `{filename}.notes.md` is maintained with full presentation content plus current selections and captured notes.
3. **Given** notes exist, **When** the notes output is generated, **Then** all notes appear in a dedicated notes slide at the end of the presentation.
4. **Given** the presenter is on any slide, **When** F1 is pressed, **Then** the view jumps to the notes slide.
5. **Given** the presenter jumped to notes using F1, **When** Escape is pressed, **Then** the view returns to the previously viewed slide.
6. **Given** a `{filename}.notes.md` file already exists, **When** the presentation is opened again, **Then** prior option selections and notes are restored and the presenter can continue from persisted state.

---

### User Story 4 - Open Main Menu and Run Session Actions (Priority: P3)

As a presenter, I can open a main menu from slides and choose session actions so I can exit safely, reset session state, or generate a song prompt.

**Why this priority**: This controls termination/reset behavior and adds post-meeting value through content summarization.

**Independent Test**: Can be fully tested by pressing Escape from a non-notes slide, selecting each menu option, and verifying resulting presentation and notes-file state.

**Acceptance Scenarios**:

1. **Given** the presenter is on any non-notes slide, **When** Escape is pressed, **Then** a main menu page is displayed.
2. **Given** the main menu is open, **When** `Exit` is selected, **Then** the presentation closes and existing notes are retained.
3. **Given** the main menu is open and a `{filename}.notes.md` exists, **When** `Start New` is selected, **Then** the current notes file is deleted, a fresh notes state is started, and the presentation restarts from the first slide.
4. **Given** the main menu is open, **When** `Create a Song` is selected, **Then** the system generates a copyable AI prompt that briefly summarizes presentation highlights for use on `suno.com`.
5. **Given** the main menu is open and session interactions exist, **When** `Create a Song` is selected, **Then** prompt generation uses slide content, current option selections, and captured presenter notes.
6. **Given** the main menu is open and selections and/or notes are absent, **When** `Create a Song` is selected, **Then** a prompt is still generated from available inputs without blocking the presenter.
7. **Given** the main menu is open, **When** the presenter presses Up Arrow or Down Arrow, **Then** menu focus moves between menu options in visible order.
8. **Given** a main menu option is focused, **When** Enter is pressed, **Then** the focused option is executed.

### Edge Cases

- Markdown file path is missing, unreadable, or not markdown-formatted.
- The file includes only body text and no `#` or `##` headings.
- Multiple `#` sections exist; each must be treated as a title slide section in file order.
- A slide contains no option boxes, or only one option box.
- Markdown text contains bracket-like patterns that are not GitHub-style task list items and should not become selectable.
- The presenter presses Left Arrow on the first slide or Right Arrow on the last slide.
- F1 is pressed before any note is captured.
- Escape is pressed without a prior F1 jump context.
- Insert is pressed repeatedly on the same slide.
- Existing `.notes.md` file is partially invalid or out-of-sync with current source markdown.
- Escape is pressed while already on the main menu page.
- `Start New` is chosen when no `.notes.md` file currently exists.
- `Create a Song` is chosen before any options are selected or notes are captured.
- A terminal does not support mouse input events.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST accept startup in the form `asczee {markdown filename}` and load the referenced markdown presentation source.
- **FR-002**: System MUST interpret each markdown `#` section as a Presentation Title Slide section.
- **FR-003**: System MUST render `#` section heading text as large ASCII art spanning full display width as closely as terminal constraints permit.
- **FR-004**: System MUST render content under each `#` section heading beneath the heading using ASCII-friendly markdown formatting.
- **FR-005**: System MUST interpret each markdown `##` section as a regular slide boundary.
- **FR-006**: System MUST render each `##` heading prominently across the top of its slide and render following section content until the next `#` or `##` heading.
- **FR-006A**: System MUST treat markdown headings `###` and deeper as in-slide body content and MUST NOT create additional slide boundaries from them.
- **FR-007**: System MUST recognize GitHub-style markdown task list items (`- [ ]` / `- [x]`) as option boxes.
- **FR-008**: System MUST allow option box focus movement using Up Arrow and Down Arrow keys.
- **FR-009**: System MUST toggle focused option box selection with Space and display selected state as `[X]`.
- **FR-009A**: System MUST toggle the clicked option box selection when mouse interaction is available and an option box is clicked.
- **FR-010**: System MUST navigate slides using Left Arrow and Right Arrow keys.
- **FR-011**: System MUST allow note entry during presentation when Insert is pressed.
- **FR-012**: System MUST maintain an annotated markdown output named `{filename}.notes.md` during presentation runtime.
- **FR-013**: System MUST include full presentation content, current option selections, and captured notes in `{filename}.notes.md`.
- **FR-014**: System MUST place captured notes in a dedicated notes slide appended to the end of the annotated presentation.
- **FR-015**: System MUST restore prior selections and notes from `{filename}.notes.md` when available at startup.
- **FR-016**: System MUST jump to the notes slide when F1 is pressed from any slide.
- **FR-017**: System MUST return to the previously viewed slide immediately when Escape is pressed on the notes slide.
- **FR-018**: System MUST open a main menu page when Escape is pressed from any slide other than the notes slide.
- **FR-019**: Main menu page MUST provide `Exit`, `Start New`, and `Create a Song` options.
- **FR-019A**: Main menu page MUST support Up Arrow and Down Arrow for focus movement across menu options.
- **FR-019B**: Main menu page MUST execute the currently focused menu option when Enter is pressed.
- **FR-020**: Selecting `Exit` MUST close the presentation without deleting existing notes.
- **FR-021**: Selecting `Start New` MUST delete the current `{filename}.notes.md` file if present, initialize a fresh notes state, and restart from the first slide.
- **FR-022**: Selecting `Create a Song` MUST generate a brief, copyable AI prompt summarizing presentation highlights for use on `suno.com`.
- **FR-023**: `Create a Song` prompt generation MUST use three inputs: presentation slide content, current option-box selections, and captured presenter notes.
- **FR-024**: If option selections or presenter notes are unavailable, `Create a Song` prompt generation MUST proceed using available inputs and MUST NOT fail solely due to missing optional inputs.
- **FR-025**: System MUST preserve normal slide navigation behavior after returning from notes review.
- **FR-026**: System MUST provide clear user feedback when source file or notes file cannot be loaded, persisted, or deleted.
- **FR-027**: System MUST gracefully fall back to keyboard-only option-box interaction when mouse input is unavailable.

### Key Entities *(include if feature involves data)*

- **Presentation Source**: The original markdown file, including heading-based sections and body content.
- **Slide Section**: A displayable unit derived from `#` or `##` headings, with heading text, body content, and ordering.
- **Option Box Item**: A selectable list entry derived from `[]` syntax with focus position and selected/unselected state.
- **Option Box Item**: A selectable list entry derived from GitHub-style task list syntax (`- [ ]` / `- [x]`) with focus position and selected/unselected state.
- **Presenter Note**: A timestamped or ordered note captured during a session and associated with the session timeline.
- **Annotated Presentation**: The `{filename}.notes.md` artifact containing original presentation content plus stateful selections and appended notes slide.
- **Navigation Context**: The current slide index and the prior slide index used when jumping to and returning from the notes slide.
- **Main Menu State**: The page state shown when Escape is pressed from non-notes slides, including selected action.
- **Song Prompt**: A generated plain-text summary prompt suitable for copy/paste into `suno.com`, composed from slide content, selected options, and presenter notes.

### Assumptions

- Markdown parsing follows common heading and list semantics, with output adapted to ASCII terminal constraints.
- The original markdown file remains the source of truth; annotations are written to a separate `.notes.md` file.
- If partial restoration is possible from an existing `.notes.md`, recoverable selections and notes are restored and unrecoverable portions are ignored with user-visible feedback.
- Keyboard mappings described in this specification are available in the target terminal environment.
- `Create a Song` produces prompt text only; it does not directly submit content to external services.
- Mouse-click input availability depends on terminal capabilities; when unavailable, keyboard controls remain fully functional.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of test presentations using `#` and `##` headings open successfully via `asczee {markdown filename}` and render the expected slide boundaries.
- **SC-002**: In usability validation, presenters can navigate from first to last slide and back using only Left/Right arrows without assistance in under 60 seconds for a 20-slide deck.
- **SC-003**: In scenario tests with selectable boxes, 100% of option toggles reflect correct visible state (`[]` or `[X]`) immediately after key input.
- **SC-004**: In session persistence tests, reopening a presentation with an existing `.notes.md` restores previously saved notes and option selections with at least 99% state fidelity.
- **SC-005**: In review-flow tests, presenters can jump to notes with F1 and return to their exact prior slide with Escape in 100% of runs.
- **SC-006**: In interaction tests, pressing Escape from any non-notes slide opens the main menu in 100% of runs.
- **SC-007**: In reset tests, selecting `Start New` removes previous session annotations and restarts from slide 1 in 100% of runs.
- **SC-008**: In content-generation tests, selecting `Create a Song` always produces a non-empty, copyable prompt that references at least three presentation highlights.
- **SC-009**: In prompt-source validation tests, 100% of generated song prompts include traceable content from slides, selected options, and presenter notes.
- **SC-010**: In degraded-input tests (no notes and/or no selected options), selecting `Create a Song` still generates a non-empty prompt in 100% of runs.
- **SC-011**: In menu interaction tests, presenters can select any main menu option using Up/Down and execute it with Enter in 100% of runs.
- **SC-012**: In terminals with mouse support, clicking an option box toggles its selection state correctly in 100% of interaction tests.
