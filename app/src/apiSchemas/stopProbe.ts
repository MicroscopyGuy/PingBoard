import { object, z } from 'zod';
import 'zod-openapi';

export const stopProbe = z.object({
    probeIdentifiers: z.string().array()
})