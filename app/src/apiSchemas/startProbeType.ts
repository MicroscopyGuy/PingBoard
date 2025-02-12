import {startProbe } from './startProbe';
import {infer, z} from 'zod';

export type StartProbe = z.infer<typeof startProbe>;