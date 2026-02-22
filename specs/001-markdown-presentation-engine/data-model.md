# Data Model: Markdown ASCII Presentation Engine

## Entity: PresentationDocument

- Description: In-memory representation of a markdown presentation and runtime context.
- Fields:
  - `SourcePath` (string, required)
  - `Slides` (ordered list of `SlideSection`, required, min 1)
  - `CurrentSlideIndex` (int, >=0, < slide count)
  - `PreviousSlideIndexBeforeNotesJump` (int?, nullable)
  - `HasExistingNotesFile` (bool)
- Relationships:
  - 1-to-many with `SlideSection`
  - 1-to-1 (optional) with `AnnotatedDocument`

## Entity: SlideSection

- Description: Display unit derived from heading boundaries.
- Fields:
  - `SlideId` (string or int, unique)
  - `SlideType` (enum: `TitleSlide`, `StandardSlide`, `NotesSlide`, `MainMenuSlide`)
  - `HeadingText` (string, required except menu)
  - `BodyBlocks` (ordered text blocks)
  - `OptionItems` (ordered list of `OptionBoxItem`)
- Validation Rules:
  - `TitleSlide` originates from markdown `#` sections.
  - `StandardSlide` originates from markdown `##` sections.
  - `###`+ headings remain in `BodyBlocks` of current section.

## Entity: OptionBoxItem

- Description: Selectable checkbox-like item extracted from GitHub-style markdown task list entries.
- Fields:
  - `OptionId` (string/int, unique within slide)
  - `RawText` (string, required)
  - `IsSelected` (bool)
  - `OrderIndex` (int, >=0)
- Validation Rules:
  - Only GitHub-style task list occurrences (`- [ ]` / `- [x]`) are interactive.
  - Render format is `[]` when false, `[X]` when true.

## Entity: InputCapability

- Description: Runtime input capability flags for keyboard and optional mouse support.
- Fields:
  - `KeyboardEnabled` (bool, required)
  - `MouseEnabled` (bool, required)
- Validation Rules:
  - If `MouseEnabled` is false, all interactive actions remain reachable via keyboard.

## Entity: PresenterNote

- Description: Presenter-authored note captured at runtime.
- Fields:
  - `NoteId` (string/int, unique)
  - `SlideIdAtCapture` (reference to `SlideSection`)
  - `Content` (string, non-empty)
  - `CreatedAtSessionOrder` (int, monotonic)
- Relationships:
  - many-to-one with `SlideSection`
  - many-to-one with `AnnotatedDocument`

## Entity: AnnotatedDocument

- Description: Persisted `{filename}.notes.md` state artifact.
- Fields:
  - `Path` (string, required)
  - `PresentationContent` (string markdown)
  - `SelectedOptionsSnapshot` (list of selected option references)
  - `NotesSectionContent` (string markdown appended as terminal notes slide)
  - `LastUpdatedUtc` (datetime)
- Validation Rules:
  - Must include full presentation content and a dedicated notes slide section.
  - Should be loadable on restart to restore options and notes where possible.

## Entity: MainMenuState

- Description: Runtime state when presenter opens the Escape menu.
- Fields:
  - `IsOpen` (bool)
  - `FocusedAction` (enum: `Exit`, `StartNew`, `CreateSong`)
- Validation Rules:
  - Up/Down changes `FocusedAction` in visible order.
  - Enter executes current focused action.

## Entity: SongPromptArtifact

- Description: Copyable prompt text produced for `suno.com`.
- Fields:
  - `PromptText` (string, non-empty)
  - `SourceSummary` (metadata: slide count, selected count, note count)
- Validation Rules:
  - Must always generate from available inputs.
  - Must not fail solely due to missing selections or notes.

## State Transitions

1. `ViewingSlide` -> (`F1`) -> `ViewingNotesSlide`
2. `ViewingNotesSlide` -> (`Esc`) -> `ViewingPreviousSlide`
3. `ViewingAnyNonNotesSlide` -> (`Esc`) -> `ViewingMainMenu`
4. `ViewingMainMenu` -> (`Enter` on `Exit`) -> `SessionEnded`
5. `ViewingMainMenu` -> (`Enter` on `StartNew`) -> `SessionResetToSlide1`
6. `ViewingMainMenu` -> (`Enter` on `CreateSong`) -> `SongPromptGenerated` -> `ViewingMainMenu`
7. `ViewingSlide` -> (`Insert`) -> `NoteCaptured` -> `ViewingSlide`
8. `SessionStart` + existing notes file -> `RestoredSessionState`
9. `ViewingSlide` + (`MouseClick` on option box) -> `OptionToggled`
