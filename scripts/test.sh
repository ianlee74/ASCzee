#!/usr/bin/env bash
# Run all tests for the ASCzee solution.
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$REPO_ROOT"

echo "Running tests..."
dotnet test ASCzee.slnx --configuration Release
echo "All tests passed."
