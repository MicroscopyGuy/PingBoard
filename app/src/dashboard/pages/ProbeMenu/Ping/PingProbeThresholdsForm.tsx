import {ProbeFormProps} from '../CommonInput/ProbeFormTypes';
import {PingProbeFormProps} from './PingProbeFormTypes';

function PingProbeThresholdsForm(props: PingProbeFormProps){

    return (
        <div>
            <text> Target </text>
            <input className = "probeOptions-pingProbe behavior"
                type = "text"
                placeholder = "IPAddress or website here"
                value = {props["maxAllowedRtt"]}
                onChange = {(e) => {props.onInputChange('maxAllowedRtt', e.target.value)}}
                disabled = {props.probingActive}/>
        </div>
    );
}

export default PingProbeThresholdsForm;