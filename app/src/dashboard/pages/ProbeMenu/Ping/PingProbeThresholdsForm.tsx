import {useContext}from 'react';
import {OnProbeFormChangeContext, 
        ProbeConfigChange, 
        StartProbeFormDataContext
       } from '../ProbeConfigFormManager';
import { StartProbe, PingConfig } from 'src/apiSchemas/startProbeTypes';
import { useDisplayFormValue } from '../ProbeConfigFormManager'


function PingProbeThresholdsForm(){
    const onInputChange = useContext<ProbeConfigChange>(OnProbeFormChangeContext);
    const updatedProbeConfig = useContext<Partial<StartProbe>>(StartProbeFormDataContext);


    return (
        <div className='grid grid-cols-1'>
            <text className='probeOptions-title'>Maximum Rtt</text>
            <input className = "probeOptions pingProbe thresholds"
                type = "text"
                placeholder = "Time before packet is considered lost"
                value = {useDisplayFormValue(updatedProbeConfig, "maxRtt")}
                onChange = {(e) => {onInputChange('maxRtt', e.target.value)}}
                />
        </div>
    );
}

export default PingProbeThresholdsForm;