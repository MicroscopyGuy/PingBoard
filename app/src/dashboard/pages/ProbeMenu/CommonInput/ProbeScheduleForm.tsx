import {ProbeFormProps} from './ProbeFormTypes';

export type ProbeScheduleJson = {
    spread: string;
}

export function ProbeScheduleForm(probeFormProps: ProbeFormProps, probeScheduleJson: ProbeScheduleJson ){
    return (
        <div className='grid grid-cols-1'>
            <text className='probeOptions-title'>Spread</text>
            <input className = "probeOptions schedule"
                type = "text"
                placeholder = "Input the desired spread"
                value = {probeScheduleJson["spread"]}
                onChange = {(e) => {probeFormProps.onInputChange('spread', e.target.value)}}
                />
        </div>
    );

}
