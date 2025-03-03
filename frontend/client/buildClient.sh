#!/bin/sh
set -euo pipefail

mkdir -p src/gen || true

kiota generate -l typescript -d ../../backend/PingBoard/PingBoard.json -c PingBoardClient -o ./src

#cat ../../backend/PingBoard/ServerEventSchemas/ServerEventSchema.json | pnpm dlx json-schema-to-zod -m esm -n "ServerEventLookup" -t -wj | pnpm dlx prettier > src/gen/types.ts

pnpm dlx tsx bin/eventTypesGen.ts

tsup src/index.ts --format cjs,esm --dts --clean --sourcemap