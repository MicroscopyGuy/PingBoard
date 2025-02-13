
type ProbeOptionInput = number | string | boolean;


export type Probe = {
    value: string;
    label: string;
}

export const probes = [
    {
      value: "ping",
      label: "Ping",
    },
]