import { StartProbe } from 'src/apiSchemas/startProbeTypes';
import { OnProbeFormChangeContext, ProbeConfigChange, StartProbeFormDataContext } from '../ProbeConfigFormManager';
import { useContext } from 'react';

export type ProbeScheduleJson = {
    spread: string;
}

export function ProbeScheduleForm(){
    const onInputChange = useContext<ProbeConfigChange>(OnProbeFormChangeContext);
    const updatedProbeConfig = useContext<Partial<StartProbe>>(StartProbeFormDataContext);

    return (
        <div className='grid grid-cols-1'>
            <text className='probeOptions-title'>Interval</text>
            <input className = "probeOptions schedule"
                type = "text"
                placeholder = "Input the desired interval"
                value = {updatedProbeConfig.probeInterval}
                onChange = {(e) => {onInputChange('interval', e.target.value)}}
                />
        </div>
    );

}
