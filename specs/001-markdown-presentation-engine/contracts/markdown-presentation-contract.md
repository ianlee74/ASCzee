# Contract: Markdown Presentation CLI and Notes Artifact

## 1) CLI Invocation Contract

- Command: `asczee <markdown-filename>`
- Input:
  - Required positional argument: path to markdown source file.
- Output:
  - Starts interactive terminal presentation session.
  - On startup failures, emits user-readable error message and exits non-zero.

## 2) Slide Parsing Contract

- `# <text>` starts a **Title Slide** section.
- `## <text>` starts a **Standard Slide** section.
- `###` and deeper headings are parsed as body content inside current slide.
- Body content for a slide is all markdown content until the next `#` or `##` heading.
- GitHub-style task list items (`- [ ]` / `- [x]`) are recognized as interactive option boxes.

## 3) Keyboard Interaction Contract

### Presentation mode
- `Right Arrow`: next slide (if available).
- `Left Arrow`: previous slide (if available).
- `Up Arrow` / `Down Arrow`: move option-box focus on current slide.
- `Space`: toggle focused option box (`[]` <-> `[X]`).
- Mouse click on option box: toggle clicked option box (`[]` <-> `[X]`) when mouse input is available.
- `Insert`: capture a note for current session.
- `F1`: jump to notes slide.
- `Esc`:
  - If currently on notes slide -> return immediately to previously viewed slide.
  - Otherwise -> open main menu slide.

### Main menu mode
- Menu options: `Exit`, `Start New`, `Create a Song`.
- `Up Arrow` / `Down Arrow`: move focused option.
- `Enter`: execute focused option.

## 4) Main Menu Action Contract

- `Exit`:
  - Ends presentation immediately.
  - Retains existing notes selections and notes artifact.

- `Start New`:
  - Deletes current `{filename}.notes.md` if it exists.
  - Initializes fresh in-memory notes/selection state.
  - Returns to slide 1.

- `Create a Song`:
  - Generates copyable plain-text prompt for use with `suno.com`.
  - Uses available inputs from slides, selected options, and presenter notes.
  - Does not fail when selections/notes are absent.

## 5) Notes Artifact Contract (`{filename}.notes.md`)

- Location: same directory as source markdown.
- Naming: source base name + `.notes.md` suffix.
- Content requirements:
  - Includes complete presentation content.
  - Reflects current option selections.
  - Appends dedicated notes slide at end containing captured notes.
- Recovery contract:
  - On startup, if artifact exists and is readable, restore prior selections and notes where possible.
  - If artifact is partially invalid, recover valid portions and report user-facing warning.

## 6) Error/Feedback Contract

System presents clear user-facing feedback when:
- Source markdown cannot be read/parsed.
- Notes artifact cannot be loaded/saved/deleted.
- Recovery is partial due to invalid notes content.
- Mouse input is unavailable (keyboard controls remain available).
