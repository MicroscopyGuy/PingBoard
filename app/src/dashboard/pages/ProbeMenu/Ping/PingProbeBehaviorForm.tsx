import {ProbeFormProps} from '../CommonInput/ProbeFormTypes';
import { PingProbeFormProps } from './PingProbeFormTypes';
import {useContext} from 'react';
import {OnProbeFormChangeContext, 
        ProbeConfigChange, 
        StartProbeFormDataContext
       } from '../ProbeConfigFormManager';
import { StartProbe } from 'src/apiSchemas/startProbeType';

function PingProbeBehaviorForm(props : PingProbeFormProps){

    const onInputChange = useContext<ProbeConfigChange>(OnProbeFormChangeContext);
    const updatedProbeConfig = useContext<StartProbe>(StartProbeFormDataContext);

    return (
        <div className='grid grid-cols-1 '>
            <text className='probeOptions-title'>Target</text>
            <input className = "probeOptions pingProbe behavior"
                type = "text"
                placeholder = "IPAddress or website here"
                value = {updatedProbeConfig.target.target}
                onChange = {(e) => {onInputChange('target', e.target.value)}}
                />

            <text className='probeOptions-title'>Ttl</text>
            <input className = "probeOptions pingProbe behavior"
                type = "text"
                placeholder = "Ttl here"
                value = {updatedProbeConfig.ttl}
                onChange = {(e) => {onInputChange('maxTtl', e.target.value)}}
                />
            
            <text className='probeOptions-title'>TimeoutMs</text>
            <input className = "probeOptions pingProbe behavior"
                type = "text"
                placeholder = "Timeout in milliseconds, here"
                value = {updatedProbeConfig.}
                onChange = {(e) => {onInputChange('timeoutMs', e.target.value)}}
                />

            <text className='probeOptions-title'>PacketPayload</text>
            <input className = "probeOptions pingProbe behavior"
                type = "text"
                placeholder = "Optional: indicate what you'd like each packet to contain"
                value = {updatedProbeConfig.packetPayload}
                onChange = {(e) => {onInputChange('packetPayload', e.target.value)}}
                />
        </div>
    );

}

export default PingProbeBehaviorForm;