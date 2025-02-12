import {useState, useEffect, useContext} from 'react'
//import {emptyPingProbeForm, PingProbeFormJson} from './PingProbeFormTypes';
import PingProbeBehaviorForm from './PingProbeBehaviorForm';
import PingProbeThresholdsForm from './PingProbeThresholdsForm';
import {ProbeScheduleForm} from '../CommonInput/ProbeScheduleForm';
import { ProbeConfigChange, OnProbeFormChangeContext } from '../ProbeConfigFormManager';



export function PingProbeConfigForm(){
    const [viewAdvOptions, setViewAdvOptions] = useState<boolean>(false);
    const onFormDataChange = useContext<ProbeConfigChange>(OnProbeFormChangeContext);


    return (
        <div className="probeConfigForm">
            <PingProbeBehaviorForm 
                /*{...probeDataJson}*/
                showAdvancedOptions={viewAdvOptions}
                onInputChange={onFormDataChange}
            />

            <PingProbeThresholdsForm
                /*{...probeDataJson}*/
                showAdvancedOptions={viewAdvOptions}
                onInputChange={onFormDataChange}
            />

            <ProbeScheduleForm
                /*{...probeDataJson}*/
                showAdvancedOptions={viewAdvOptions}
                onInputChange={onFormDataChange}
            />
        </div>
    )

}