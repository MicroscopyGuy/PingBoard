import {useState, useEffect, useContext} from 'react'
import {emptyPingProbeForm, PingProbeFormJson} from './PingProbeFormTypes';
import PingProbeBehaviorForm from './PingProbeBehaviorForm';
import PingProbeThresholdsForm from './PingProbeThresholdsForm';
import {ProbeScheduleForm} from '../CommonInput/ProbeScheduleForm';


export function PingProbeConfigForm(){
    const [probeDataJson, setProbeDataJson] = useState<PingProbeFormJson>(emptyPingProbeForm);
    const [viewAdvOptions, setViewAdvOptions] = useState<boolean>(false);

    const onFormDataChange = (jsonPropName: string, jsonPropValue: any) => {
        let updatedFormJson = {
            ...probeDataJson,
            [jsonPropName]: jsonPropValue,
        };
        setProbeDataJson(updatedFormJson);
    }

    return (
        <div className="grid grid-cols-1">
            <PingProbeBehaviorForm 
                {...probeDataJson}
                showAdvancedOptions={viewAdvOptions}
                onInputChange={onFormDataChange}
            />

            <PingProbeThresholdsForm
                {...probeDataJson}
                showAdvancedOptions={viewAdvOptions}
                onInputChange={onFormDataChange}
            />

            <ProbeScheduleForm
                {...probeDataJson}
                showAdvancedOptions={viewAdvOptions}
                onInputChange={onFormDataChange}
            />
        </div>
    )

}