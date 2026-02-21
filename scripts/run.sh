#!/usr/bin/env bash
# Run the ASCzee console app.
# Usage: ./scripts/run.sh <presentation-file.ascz>
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$REPO_ROOT"

if [[ $# -eq 0 ]]; then
    echo "Usage: $0 <presentation-file.ascz>"
    exit 1
fi

dotnet run --project src/ASCzee/ASCzee.csproj -- "$@"
