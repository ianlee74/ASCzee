#!/usr/bin/env bash
# Build the ASCzee solution.
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$REPO_ROOT"

echo "Building ASCzee..."
dotnet build ASCzee.slnx --configuration Release
echo "Build succeeded."
