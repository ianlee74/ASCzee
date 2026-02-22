# ASCzee

ASCII-native, markdown-driven terminal presentation engine.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

ASCzee turns markdown files into interactive terminal presentations with keyboard-first controls, live option toggles, persistent notes, and a song-prompt workflow for `suno.com`.

## Table of Contents

- [Features](#features)
- [Quick Start](#quick-start)
- [Slide Syntax](#slide-syntax)
- [Controls](#controls)
- [Song Prompt Workflow](#song-prompt-workflow)
- [Output Artifacts](#output-artifacts)
- [Project Structure](#project-structure)
- [Development](#development)
- [Contributing](#contributing)
- [License](#license)

## Features

- Markdown slide parsing (`#` and `##` define slide boundaries)
- Block-style ASCII art headers for presentation titles and slide headers
- Interactive task-list options using GitHub checkbox syntax (`- [ ]`, `- [x]`)
- Markdown hyperlink rendering with blue link text and selectable link list
- Notes capture and resume via `.notes.md` artifact
- Main menu actions: `Exit`, `Start New`, `Create a Song`
- Song prompt workflow with:
  - genre selection from popular genres list
  - character count + 1000-character limit display
  - copy prompt to clipboard
  - open `https://suno.com/create`
  - save prompt to `<presentation>.songprompt.txt` and open in Notepad

## Quick Start

### Prerequisites

- .NET SDK 8.0+
- Windows, macOS, or Linux terminal

### Run

```bash
dotnet run --project src/ASCzee -- Default.md
```

Use [Default.md](Default.md) as a built-in validation deck.

## Slide Syntax

- `# Heading`
  - starts a title slide
- `## Heading`
  - starts a standard slide
- `###` and deeper headings
  - stay inside the current slide body
- `- [ ] Task` / `- [x] Task`
  - become interactive option rows
- `[Label](https://example.com)`
  - renders `Label` in blue and appears in the selectable links list at slide bottom

## Controls

### Presentation

- `Left` / `Right`: previous / next slide
- `Up` / `Down`: move option focus
- `Space`: toggle focused option (`[]` ↔ `[X]`)
- `Enter`: open focused hyperlink (when link focus is active)
- `Insert`: capture presenter note
- `F1`: jump to notes slide
- `Esc`:
  - on notes slide: return to previous slide
  - otherwise: open main menu

### Main Menu

- `Up` / `Down`: move menu focus
- `Enter`: execute focused action
- `Esc`: close menu

## Song Prompt Workflow

When you choose `Create a Song` from main menu:

1. Select a genre from popular genres list
2. ASCzee generates a prompt using:
   - slide content
   - selected options
   - captured notes
3. Prompt page shows:
   - `Suno` limit (`1000`) and current character count
   - prompt file location (`<presentation>.songprompt.txt`)
4. Available actions:
   - `Open Prompt in Notepad`
   - `Copy Prompt to Clipboard`
   - `Open suno.com/create`

## Output Artifacts

### Notes Artifact

ASCzee persists presentation state in:

- `<presentation>.notes.md`
- Example: `Default.notes.md`

This file stores:

- selected option states
- captured notes
- notes slide content for resume

### Song Prompt Artifact

ASCzee writes generated song prompts to:

- `<presentation>.songprompt.txt`
- Example: `Default.songprompt.txt`

## Project Structure

```text
src/
  ASCzee/         # application
  ASCzee.Tests/   # test project

scripts/
  build.sh
  run.sh
  test.sh

specs/
  001-markdown-presentation-engine/
```

## Development

### Build

```bash
dotnet build ASCzee.sln
```

### Test

```bash
dotnet test ASCzee.sln
```

### Run local validation deck

```bash
dotnet run --project src/ASCzee -- Default.md
```

## Contributing

Contributions are welcome.

- Open an issue for bugs or feature requests
- Submit focused pull requests with clear descriptions
- Include tests or validation notes for behavior changes

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE).
