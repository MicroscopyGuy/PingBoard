
type ProbeOptionInput = number | string | boolean;

export type ProbeFormProps = {
    onInputChange: (jsonPropName: string, newPropVal: ProbeOptionInput) => void;
    showAdvancedOptions: boolean;
    probingActive: boolean;
}

export type ProbeConfigFormManagerProps = {
    onInputChange: (formData: any) => void;
    probingActive: boolean;
}

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