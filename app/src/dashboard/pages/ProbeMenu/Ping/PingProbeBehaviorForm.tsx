import {ProbeFormProps} from '../CommonInput/ProbeFormTypes';
import { PingProbeFormProps } from './PingProbeFormTypes';


function PingProbeBehaviorForm(props : PingProbeFormProps){
    return (
        <div>
            <text> Target </text>
            <input className = "probeOptions-pingProbe behavior"
                type = "text"
                placeholder = "IPAddress or website here"
                value = {props["target"]}
                onChange = {(e) => {props.onInputChange('target', e.target.value)}}
                disabled = {props.probingActive}/>

            <text> Ttl </text>
            <input className = "probeOptions-pingProbe behavior"
                type = "text"
                placeholder = "Ttl here"
                value = {props["maxTtl"]}
                onChange = {(e) => {props.onInputChange('maxTtl', e.target.value)}}
                disabled = {props.probingActive}/>
            
            <text> TimeoutMs </text>
            <input className = "probeOptions-pingProbe behavior"
                type = "text"
                placeholder = "Timeout in milliseconds, here"
                value = {props["timeoutMs"]}
                onChange = {(e) => {props.onInputChange('timeoutMs', e.target.value)}}
                disabled = {props.probingActive}/>

            <text> PacketPayload </text>
            <input className = "probeOptions-pingProbe behavior"
                type = "text"
                placeholder = "Optional: indicate what you'd like each packet to contain"
                value = {props["packetPayload"]}
                onChange = {(e) => {props.onInputChange('packetPayload', e.target.value)}}
                disabled = {props.probingActive}/>
        </div>
    );

}

export default PingProbeBehaviorForm;