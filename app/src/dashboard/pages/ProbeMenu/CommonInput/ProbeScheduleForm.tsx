import {ProbeFormProps} from './ProbeFormTypes';

export type ProbeScheduleJson = {
    spread: string;
}

export function ProbeScheduleForm(probeFormProps: ProbeFormProps, probeScheduleJson: ProbeScheduleJson ){
    return (
        <div>
            <text> Spread </text>
            <input className = "probeOptions-Schedule"
                type = "text"
                placeholder = "Input the desired spread"
                value = {probeScheduleJson["spread"]}
                onChange = {(e) => {probeFormProps.onInputChange('spread', e.target.value)}}
                disabled = {probeFormProps.probingActive}/>
        </div>
    );

}
