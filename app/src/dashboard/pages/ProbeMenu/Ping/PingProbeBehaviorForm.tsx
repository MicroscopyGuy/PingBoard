import {ProbeFormProps} from '../CommonInput/ProbeFormTypes';
import { PingProbeFormProps } from './PingProbeFormTypes';


function PingProbeBehaviorForm(props : PingProbeFormProps){
    return (
        <div className='grid grid-cols-1 '>
            <text className='probeOptions-title'>Target</text>
            <input className = "probeOptions pingProbe behavior"
                type = "text"
                placeholder = "IPAddress or website here"
                value = {props["target"]}
                onChange = {(e) => {props.onInputChange('target', e.target.value)}}
                />

            <text className='probeOptions-title'>Ttl</text>
            <input className = "probeOptions pingProbe behavior"
                type = "text"
                placeholder = "Ttl here"
                value = {props["maxTtl"]}
                onChange = {(e) => {props.onInputChange('maxTtl', e.target.value)}}
                />
            
            <text className='probeOptions-title'>TimeoutMs</text>
            <input className = "probeOptions pingProbe behavior"
                type = "text"
                placeholder = "Timeout in milliseconds, here"
                value = {props["timeoutMs"]}
                onChange = {(e) => {props.onInputChange('timeoutMs', e.target.value)}}
                />

            <text className='probeOptions-title'>PacketPayload</text>
            <input className = "probeOptions pingProbe behavior"
                type = "text"
                placeholder = "Optional: indicate what you'd like each packet to contain"
                value = {props["packetPayload"]}
                onChange = {(e) => {props.onInputChange('packetPayload', e.target.value)}}
                />
        </div>
    );

}

export default PingProbeBehaviorForm;