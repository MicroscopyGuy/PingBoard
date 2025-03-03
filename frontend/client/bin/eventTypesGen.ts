import { jsonSchemaToZod } from "json-schema-to-zod";
import { readFileSync, writeFileSync } from "node:fs";
import prettier from '@prettier/sync';


const mySchemaStr = readFileSync("../../backend/PingBoard/ServerEventSchemas/ServerEventSchema.json").toString('utf8');
const mySchema = JSON.parse(mySchemaStr);

const moduleCode = jsonSchemaToZod(mySchema, { module: "esm" });
const formatted = prettier.format(moduleCode, { semi: false, parser: "typescript" });
writeFileSync("./src/gen/types.ts", formatted);