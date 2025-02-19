import {useContext, useState, useEffect} from 'react';
import {OnProbeFormChangeContext, 
        ProbeConfigChange, 
        StartProbeFormDataContext
       } from '../ProbeConfigFormManager';
import { StartProbe, PingConfig } from 'src/apiSchemas/startProbeTypes';
import { useDisplayFormValue } from '../ProbeConfigFormManager'
import { pingTargetSchema } from '../../../../apiSchemas/startProbe';


function PingProbeBehaviorForm(){
    const [targetInfo, setTargetInfo] = useState<string>("");
    const onInputChange = useContext<ProbeConfigChange>(OnProbeFormChangeContext);
    const updatedProbeConfig = useContext<Partial<StartProbe>>(StartProbeFormDataContext);

    //console.log(updatedProbeConfig)
    return (
        <div className='grid grid-cols-1 '>
            <text className='probeOptions-title'>Target</text>
            <input className = "probeOptions pingProbe behavior"
                type = "text"
                placeholder = "IPAddress or website here"
                value = {targetInfo}
                onChange = {(e) => {
                    setTargetInfo(e.target.value);
                    
                }}
                onBlur={() =>{
                    for (let schema of pingTargetSchema._def.options){
                        if (schema._def.shape().target.safeParse(targetInfo).success){
                            const targetObj = { 
                                'target': targetInfo, 
                                'targetType': schema._def.shape().targetType._def.value
                            };
                            onInputChange('target', targetObj)
                        }
                    }}
                }  
                />

            <text className='probeOptions-title'>Ttl</text>
            <input className = "probeOptions pingProbe behavior"
                type = "text"
                placeholder = "Ttl here"
                value = {useDisplayFormValue(updatedProbeConfig, "ttl")}
                onChange = {(e) => {onInputChange('ttl', e.target.value)}}
                />
            
            <text className='probeOptions-title'>TimeoutMs</text>
            <input className = "probeOptions pingProbe behavior"
                type = "text"
                placeholder = "Timeout in milliseconds, here"
                value = {useDisplayFormValue(updatedProbeConfig, "timeout")}
                onChange = {(e) => {onInputChange('timeout', e.target.value)}}
                />

            <text className='probeOptions-title'>PacketPayload</text>
            <input className = "probeOptions pingProbe behavior"
                type = "text"
                placeholder = "Optional: indicate what you'd like each packet to contain"
                value = {useDisplayFormValue(updatedProbeConfig, "packetPayload")}
                onChange = {(e) => {onInputChange('packetPayload', e.target.value)}}
                />
        </div>
    );

}

export default PingProbeBehaviorForm;