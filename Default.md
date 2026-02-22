# ASCzee Validation Deck
Welcome to ASCzee.

Use this deck to validate parser, rendering, navigation, options, notes capture, menu actions, and persistence.

## Validation Checklist
- [ ] Confirm startup opens this file at slide 1
- [ ] Confirm `#` renders as title slide style
- [ ] Confirm Right Arrow goes to next slide
- [ ] Confirm Left Arrow goes to previous slide
- [ ] Confirm Esc opens main menu from non-notes slides
- [ ] Confirm F1 jumps to notes slide
- [ ] Confirm Esc on notes returns to prior slide

### Parser rule check
This is a level-3 heading and should remain body content on this slide.

#### Level-4 heading also stays in the body
No new slide should be created by `###` or deeper headings.

## Option Boxes (State + Focus)
Use Up/Down to move focus and Space to toggle the focused row.

- [ ] Unchecked option A
- [x] Pre-checked option B
- [ ] Unchecked option C
- [x] Pre-checked option D

### Mouse toggle check
If terminal supports mouse input, clicking a checkbox row should toggle it.

## Notes Capture Flow
Press Insert and add notes while on this slide.

Suggested notes to add:
- First rehearsal note
- Audience reminder
- Timing adjustment

After adding notes:
- Press F1 to jump to notes slide
- Press Esc to return here

## Hyperlink Validation
Markdown links in slide content should render link text in blue.

- Project home: [ASCzee repository](https://github.com/)
- Suno create page: [Open Suno Create](https://suno.com/create)
- .NET docs: [Learn .NET](https://dotnet.microsoft.com/en-us/learn)

Use Up/Down to focus the link list at the bottom, then press Enter to open the selected link.

## Main Menu Actions
From this (non-notes) slide, press Esc to open the main menu.

Validate each action:
- Exit: closes app and keeps `.notes.md`
- Start New: removes current `.notes.md` and restarts at slide 1
- Create a Song: generates a copyable prompt from slides + options + notes

## Hyperlink Testing
Markdown hyperlinks should be selectable and selecting them should open the hyperlink in a browser window.

- [Amazon.com](https://amazon.com)
- [SavvyFi.co](https://savvyfi.co)

## Persistence / Resume
1. Toggle some options and add at least one note.
2. Exit the app.
3. Re-run with this same file.
4. Confirm options and notes are restored from `Default.notes.md`.

## Sparse Input Song Prompt Check
For degraded-input behavior, run Start New and do NOT add notes/select options.
Then run Create a Song and confirm prompt still generates.

## End of Deck
If you can navigate to here and back, boundary handling is working.
