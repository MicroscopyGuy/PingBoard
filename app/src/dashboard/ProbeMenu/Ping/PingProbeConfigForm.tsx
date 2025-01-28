import {useState, useEffect, useContext} from 'react'
import PingProbeBehavior from './PingProbeBehaviorForm'
import PingProbeThresholds from './PingProbeThresholdsForm'
import {emptyPingProbeForm, PingProbeFormJson} from './PingProbeFormTypes';
import PingProbeBehaviorForm from './PingProbeBehaviorForm';
import PingProbeThresholdsForm from './PingProbeThresholdsForm';
import {ProbeScheduleForm} from '../CommonInput/ProbeScheduleForm';


function PingProbeConfigForm(probingActive: boolean){
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
        <div>
            <PingProbeBehaviorForm 
                {...probeDataJson}
                showAdvancedOptions={viewAdvOptions}
                onInputChange={onFormDataChange}
                probingActive={probingActive}
            />

            <PingProbeThresholdsForm
                {...probeDataJson}
                showAdvancedOptions={viewAdvOptions}
                onInputChange={onFormDataChange}
                probingActive={probingActive}
            />

            <ProbeScheduleForm
                {...probeDataJson}
                showAdvancedOptions={viewAdvOptions}
                onInputChange={onFormDataChange}
                probingActive={probingActive}
            />
        </div>
    )

}