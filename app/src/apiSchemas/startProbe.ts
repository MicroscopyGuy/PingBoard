import {
    OpenAPIRegistry,
    OpenApiGeneratorV3,
    extendZodWithOpenApi,
  } from '@asteasolutions/zod-to-openapi';
  import { z } from 'zod';
  extendZodWithOpenApi(z);
import { ipAddressTargetSchema, hostnameTargetSchema } from './common/probeTargetSchemas';

const ttlSchema = z
    .number()
    .min(1)
    .max(255)
    .default(128)
    .optional()
    /*
    .openapi({
        description: "Time To Live: the number of hops a packet is allowed before its dropped",
        examples: [64, 128],
        ref: "ttl",
    });*/

const timeoutSchema = z
    .number()
    .min(1000)
    .max(2000)
    .default(1500)
    .optional()
    /*
    .openapi({
        description: "The amount of time (in milliseconds) before a packet is considered lost.",
        examples: [1000, 2000],
        ref:"timeout"
    });*/

const packetPayloadSchema = z
    .string()
    .min(1)
    .max(64)
    .default("MIT License, Sent by: https://github.com/MicroscopyGuy/PingBoard")
    .optional()
    /*
    .openapi({
        description: "The arbitrary string to be included in each sent packet. Functionally inconsequential.",
        ref:"packetPayload"
    });*/

const probeIntervalSchema = z
    .number()
    .min(500)
    .max(36000)
    .default(1000)
    .optional()
    /*
    .openapi({
        description: "Execute the probe every X milliseconds",
        examples: [500, 2000],
        ref: "probeInterval"
    });*/

const tracerouteProbeTraceDelay = z.number()
    .min(5)
    .max(15)
    .default(10)
    .optional();

const tracerouteProbeReverseDnsSchema = z
    .boolean()
    .default(true)
    .optional()

export const tracerouteProbeConfigSchema = z.object({
    probeType: z.literal('traceroute'),
    target: hostnameTargetSchema,
    ttl: ttlSchema,
    doReverseDns: tracerouteProbeReverseDnsSchema,
    packetPayload: packetPayloadSchema,
    probeInterval: probeIntervalSchema,
}).openapi(
    "tracerouteProbeConfig"
)

export const pingProbeConfigSchema = z.object({
    probeType: z.literal('ping'), 
    target: z.discriminatedUnion('targetType', [ipAddressTargetSchema, hostnameTargetSchema]),
    ttl: ttlSchema,
    timeout: timeoutSchema,
    packetPayload: packetPayloadSchema,
    probeInterval: probeIntervalSchema,
}).openapi(
    "pingProbeConfig"
)
  /*
  .openapi({
        description: "Parameters used to configure a PingProbe",
        ref: "pingProbeSchema"
  });*/

// will have more supported probes later
export const startProbe = z.discriminatedUnion('probeType', [pingProbeConfigSchema, tracerouteProbeConfigSchema],);


type probeReq = z.infer<typeof startProbe>;
