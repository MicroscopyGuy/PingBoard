import { Probe } from './ProbeFormTypes'
import {PingProbeConfigForm} from '../Ping/PingProbeConfigForm'

type ProbeFormDeciderProps = {
    probe: Probe;
}
// will need a callback to give the probe form it decides necessary, the means of
// submitting the form info to the parent component
export function ProbeFormTypeDecider(props: ProbeFormDeciderProps){
    if (!props.probe){
        return;
    }
    
    switch (props.probe.label){
        case 'Ping':
            return <PingProbeConfigForm/>;
        
        default:
            throw new Error(`No probe form has been registered for probe type: ${props.probe.label}`);
    }
}