import { z } from 'zod';
import 'zod-openapi';

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

const ipRegex = "^((?:(?:25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)\\.){3}(?:25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d))|(((([0-9a-f]{1,4}:){7}([0-9a-f]{1,4}|:))|(([0-9a-f]{1,4}:){6}(:[0-9a-f]{1,4}|((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3})|:))|(([0-9a-f]{1,4}:){5}(((:[0-9a-f]{1,4}){1,2})|:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3})|:))|(([0-9a-f]{1,4}:){4}(((:[0-9a-f]{1,4}){1,3})|((:[0-9a-f]{1,4})?:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3}))|:))|(([0-9a-f]{1,4}:){3}(((:[0-9a-f]{1,4}){1,4})|((:[0-9a-f]{1,4}){0,2}:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3}))|:))|(([0-9a-f]{1,4}:){2}(((:[0-9a-f]{1,4}){1,5})|((:[0-9a-f]{1,4}){0,3}:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3}))|:))|(([0-9a-f]{1,4}:){1}(((:[0-9a-f]{1,4}){1,6})|((:[0-9a-f]{1,4}){0,4}:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3}))|:))|(:(((:[0-9a-f]{1,4}){1,7})|((:[0-9a-f]{1,4}){0,5}:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3}))|:))))$/i";
export const ipAddressTargetSchema = z
    .string()
    .regex(new RegExp(ipRegex))
    /*
    .openapi({
        ref: "ipAddressTarget"
    });*/

const hostnameRegex = "^((?=.{1,253}\\.?$)[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?(?:\\.[a-z0-9](?:[-0-9a-z]{0,61}[0-9a-z])?)*\\.?)|(((([0-9a-f]{1,4}:){7}([0-9a-f]{1,4}|:))|(([0-9a-f]{1,4}:){6}(:[0-9a-f]{1,4}|((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3})|:))|(([0-9a-f]{1,4}:){5}(((:[0-9a-f]{1,4}){1,2})|:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3})|:))|(([0-9a-f]{1,4}:){4}(((:[0-9a-f]{1,4}){1,3})|((:[0-9a-f]{1,4})?:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3}))|:))|(([0-9a-f]{1,4}:){3}(((:[0-9a-f]{1,4}){1,4})|((:[0-9a-f]{1,4}){0,2}:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3}))|:))|(([0-9a-f]{1,4}:){2}(((:[0-9a-f]{1,4}){1,5})|((:[0-9a-f]{1,4}){0,3}:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3}))|:))|(([0-9a-f]{1,4}:){1}(((:[0-9a-f]{1,4}){1,6})|((:[0-9a-f]{1,4}){0,4}:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3}))|:))|(:(((:[0-9a-f]{1,4}){1,7})|((:[0-9a-f]{1,4}){0,5}:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3}))|:))))$/i";
export const hostnameTargetSchema = z
    .string()
    .regex(new RegExp(hostnameRegex))
    /*.
    .openapi({
        ref: "hostnameTarget"
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


const tracerouteProbeConfigSchema = z.object({
    probeType: z.literal('traceroute'),
    target: hostnameTargetSchema,
    ttl: ttlSchema,
    doReverseDns: tracerouteProbeReverseDnsSchema,
    packetPayload: packetPayloadSchema,
    probeInterval: probeIntervalSchema,
})

const pingProbeConfigSchema = z.object({
    probeType: z.literal('ping'), 
    target: z.union([ipAddressTargetSchema, hostnameTargetSchema]),
    ttl: ttlSchema,
    timeout: timeoutSchema,
    packetPayload: packetPayloadSchema,
    probeInterval: probeIntervalSchema,
    
})
  /*
  .openapi({
        description: "Parameters used to configure a PingProbe",
        ref: "pingProbeSchema"
  });*/

// will have more supported probes later
const probeSchema = z.discriminatedUnion('probeType', [pingProbeConfigSchema, tracerouteProbeConfigSchema], );

export { tracerouteProbeConfigSchema, pingProbeConfigSchema, probeSchema }