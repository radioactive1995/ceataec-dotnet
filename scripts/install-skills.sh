#!/usr/bin/env bash
# Install personal skills only. No harness CLI, Python, network or admin access needed.
set -euo pipefail

usage() {
    echo 'Usage: bash scripts/install-skills.sh cursor|claude|both [--dry-run] [--user-home DIR]'
}
fail() { echo "$*" >&2; exit 1; }
[[ $# -gt 0 ]] || { usage; exit 2; }
harness=$1; shift
case "$harness" in
    cursor) directory=.cursor; other=.claude ;;
    claude|both) directory=.claude; other=.cursor ;;
    -h|--help) usage; exit 0 ;;
    *) usage; exit 2 ;;
esac
user_home=${HOME:?HOME must identify your user directory}
dry_run=false
while [[ $# -gt 0 ]]; do
    case "$1" in
        --dry-run) dry_run=true; shift ;;
        --user-home) [[ $# -ge 2 ]] || fail 'Missing --user-home value'; user_home=$2; shift 2 ;;
        *) usage; exit 2 ;;
    esac
done
[[ -d "$user_home" && ! -L "$user_home" ]] || fail 'User directory must exist and not be a symlink.'
user_home=$(cd "$user_home" && pwd -P)
plugin=$(cd "$(dirname "${BASH_SOURCE[0]}")/../plugins/ceataec-dotnet" && pwd -P)
destination="$user_home/$directory/skills"
stage=$(mktemp -d)
trap 'rm -rf "$stage"' EXIT
skills=(review-score refactor scaffold)

# Stage independent skill folders; each contains its own copy of the shared principles.
for skill in "${skills[@]}"; do
    mkdir -p "$stage/ceataec-$skill/references"
    sed -e "s/^name: $skill$/name: ceataec-$skill/" \
        -e 's|../../spec/standard.md|references/standard.md|g' \
        "$plugin/skills/$skill/SKILL.md" > "$stage/ceataec-$skill/SKILL.md"
    cp "$plugin/spec/standard.md" "$stage/ceataec-$skill/references/standard.md"
done

# Check all conflicts before creating any installation files. Never overwrite local edits.
for path in "$user_home/$directory" "$destination"; do
    [[ ! -L "$path" ]] || fail "Refusing symlink: $path"
    [[ ! -e "$path" || -d "$path" ]] || fail "Not a directory: $path"
done
for skill in "${skills[@]}"; do
    target="$destination/ceataec-$skill"
    duplicate="$user_home/$other/skills/ceataec-$skill"
    [[ ! -e "$duplicate" && ! -L "$duplicate" ]] || fail "Duplicate installation at $duplicate. Inspect/remove it first; 'both' uses .claude for both clients."
    [[ ! -L "$target" ]] || fail "Refusing symlink: $target"
    if [[ -e "$target" ]]; then
        [[ -d "$target" ]] || fail "Not a directory: $target"
        [[ -z $(find "$target" -type l -print -quit) ]] || fail "Refusing symlinks inside $target"
        diff -qr "$stage/ceataec-$skill" "$target" >/dev/null || fail "Existing content differs: $target. Back up local edits and remove this skill folder before reinstalling."
    fi
done
if "$dry_run"; then
    echo "Would install three personal skills in $destination (no installation files written)."
    exit 0
fi
mkdir -p "$destination"
for skill in "${skills[@]}"; do
    target="$destination/ceataec-$skill"
    if [[ ! -d "$target" ]]; then
        # mkdir fails if a competing install claimed the destination after preflight.
        mkdir "$target"
        cp -R "$stage/ceataec-$skill/." "$target/"
    fi
done
echo "Installed three personal skills in $destination. Restart your client and select /ceataec-review-score, /ceataec-refactor or /ceataec-scaffold."
if [[ "$harness" == both ]]; then
    echo 'Cursor also discovers these Claude personal skills; no second copy is needed.'
fi
