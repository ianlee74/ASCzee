# Quickstart: Markdown ASCII Presentation Engine

## Prerequisites

- .NET SDK supporting `net8.0`
- Terminal that can send arrow keys, `Insert`, `F1`, `Esc`, `Enter`, and `Space`

## Build and Test

- Build solution:
  - `dotnet build ASCzee.sln`
- Run tests:
  - `dotnet test ASCzee.sln`

## Run a Presentation

- Command:
  - `dotnet run --project src/ASCzee -- <path-to-presentation.md>`

## Minimal Sample Markdown

```markdown
# Quarterly Review
Welcome to the review.

## Priorities
- [ ] Release quality
- [ ] Cost optimization

### Notes for presenter
Mention customer feedback first.
```

## Runtime Controls

- Slides: `Left Arrow`, `Right Arrow`
- Option boxes: `Up/Down` to focus, `Space` to toggle, mouse click to toggle (when supported)
- Notes: `Insert` to record note
- Notes jump: `F1` to notes slide, `Esc` (on notes slide) to return
- Main menu: `Esc` (non-notes slide), `Up/Down` + `Enter`

## Main Menu Actions

- `Exit`: quit and keep notes
- `Start New`: delete current `.notes.md` state and restart from first slide
- `Create a Song`: generate copyable summary prompt for `suno.com`

## Resume Behavior

- If `<source>.notes.md` exists, running presentation again restores prior selections and notes.

## Manual Verification Checklist

1. Open sample markdown and verify `#` and `##` create slides.
2. Verify `###` appears as body content on current slide.
3. Toggle `- [ ]` options and confirm render changes to `[X]`.
4. Click an option box (if terminal supports mouse) and confirm toggle behavior.
5. Add notes with `Insert`; confirm they appear in notes slide output.
6. Press `F1` then `Esc`; verify return to prior slide.
7. Press `Esc` on normal slide; verify main menu appears.
8. Select each menu action and confirm expected behavior.
9. Re-run same deck and verify notes/selections resume from `.notes.md`.
