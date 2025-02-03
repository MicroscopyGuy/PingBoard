import {ProbeFormProps} from '../CommonInput/ProbeFormTypes';
import {PingProbeFormProps} from './PingProbeFormTypes';

function PingProbeThresholdsForm(props: PingProbeFormProps){

    return (
        <div className='grid grid-cols-1'>
            <text className='probeOptions-title'>Maximum Rtt</text>
            <input className = "probeOptions pingProbe thresholds"
                type = "text"
                placeholder = "Time before packet is considered lost"
                value = {props["maxAllowedRtt"]}
                onChange = {(e) => {props.onInputChange('maxAllowedRtt', e.target.value)}}
                />
        </div>
    );
}

export default PingProbeThresholdsForm;