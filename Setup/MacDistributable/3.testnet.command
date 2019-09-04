#  This will define an environment variable that contains the full absolute path 
#+ to the directory in which the .command file is kept, and then will cd into 
#+ that directory
ABSPATH="$(cd "$(dirname "$0")" && pwd)"
cd "$ABSPATH"
./XelsD -testnet

