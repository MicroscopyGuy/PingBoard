import {useContext} from 'react';
import {OnProbeFormChangeContext, 
        ProbeConfigChange, 
        StartProbeFormDataContext
       } from '../ProbeConfigFormManager';
import { StartProbe, PingConfig } from 'src/apiSchemas/startProbeTypes';


function PingProbeBehaviorForm(){

    const onInputChange = useContext<ProbeConfigChange>(OnProbeFormChangeContext);
    const updatedProbeConfig = useContext<StartProbe>(StartProbeFormDataContext);

    return (
        <div className='grid grid-cols-1 '>
            <text className='probeOptions-title'>Target</text>
            <input className = "probeOptions pingProbe behavior"
                type = "text"
                placeholder = "IPAddress or website here"
                value = {(updatedProbeConfig as PingConfig).target.target}
                onChange = {(e) => {onInputChange('target', e.target.value)}}
                />

            <text className='probeOptions-title'>Ttl</text>
            <input className = "probeOptions pingProbe behavior"
                type = "text"
                placeholder = "Ttl here"
                value = {(updatedProbeConfig as PingConfig).ttl}
                onChange = {(e) => {onInputChange('maxTtl', e.target.value)}}
                />
            
            <text className='probeOptions-title'>TimeoutMs</text>
            <input className = "probeOptions pingProbe behavior"
                type = "text"
                placeholder = "Timeout in milliseconds, here"
                value = {(updatedProbeConfig as PingConfig).timeout}
                onChange = {(e) => {onInputChange('timeoutMs', e.target.value)}}
                />

            <text className='probeOptions-title'>PacketPayload</text>
            <input className = "probeOptions pingProbe behavior"
                type = "text"
                placeholder = "Optional: indicate what you'd like each packet to contain"
                value = {(updatedProbeConfig as PingConfig).packetPayload}
                onChange = {(e) => {onInputChange('packetPayload', e.target.value)}}
                />
        </div>
    );

}

export default PingProbeBehaviorForm;