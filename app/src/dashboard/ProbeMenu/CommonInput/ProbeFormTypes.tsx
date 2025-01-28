
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