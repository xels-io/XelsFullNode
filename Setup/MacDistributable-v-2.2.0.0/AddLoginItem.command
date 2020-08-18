ABSPATH="$(cd "$(dirname "$0")" && pwd)"
FULLPATH="${ABSPATH}/1.mainnet.command"
osascript -e "tell application \"System Events\" to make login item at end with properties {path:\"${FULLPATH}\", hidden:true}"
