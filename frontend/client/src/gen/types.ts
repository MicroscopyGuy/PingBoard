import { z } from "zod"

export default z.object({
  ProbeAnomaly: z
    .union([
      z.object({
        ProbeId: z.string().uuid(),
        ProbeType: z.string(),
        Target: z.string(),
        EventId: z.string().uuid().optional(),
        EventTime: z.string().datetime({ offset: true }).optional(),
      }),
      z.null(),
    ])
    .optional(),
  ProbeError: z
    .union([
      z.object({
        ProbeId: z.string().uuid(),
        ProbeType: z.string(),
        ErrorDescription: z.string(),
        Target: z.string(),
        EventId: z.string().uuid().optional(),
        EventTime: z.string().datetime({ offset: true }).optional(),
      }),
      z.null(),
    ])
    .optional(),
  ProbeStatus: z
    .union([
      z.object({
        ProbeId: z.string().uuid(),
        Status: z.string(),
        Target: z.string(),
        ProbeType: z.string(),
        EventId: z.string().uuid().optional(),
        EventTime: z.string().datetime({ offset: true }).optional(),
      }),
      z.null(),
    ])
    .optional(),
  ProbeInfo: z
    .union([
      z.object({
        ProbeId: z.string().uuid(),
        Target: z.string(),
        ProbeType: z.string(),
        EventId: z.string().uuid().optional(),
        EventTime: z.string().datetime({ offset: true }).optional(),
      }),
      z.null(),
    ])
    .optional(),
  AdminError: z
    .union([
      z.object({
        ErrorDescription: z.string(),
        EventId: z.string().uuid().optional(),
        EventTime: z.string().datetime({ offset: true }).optional(),
      }),
      z.null(),
    ])
    .optional(),
})
