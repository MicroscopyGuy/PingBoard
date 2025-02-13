import * as probeConfigTypes  from './startProbe';
import {infer, z} from 'zod';

export type StartProbe = z.infer<typeof probeConfigTypes.startProbe>;
export type PingConfig = z.infer<typeof probeConfigTypes.pingProbeConfigSchema>;
export type TracerouteConfig = z.infer<typeof probeConfigTypes.tracerouteProbeConfigSchema>