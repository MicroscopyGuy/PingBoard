import {useState, useEffect, createContext, useContext } from 'react';
import { PingConfig, StartProbe } from '../../../apiSchemas/startProbeTypes'
import { string } from 'zod';
import { ProbeFormDialog } from './CommonInput/ProbeFormDialog';




export type ProbeConfigChange = (jsonPropName: string, jsonPropValue: any) => void;
export const OnProbeFormChangeContext = createContext<ProbeConfigChange >(null);
export const StartProbeFormDataContext = createContext<StartProbe>({})


export function ProbeConfigFormManager(){
    const [probeData, setProbeData] = useState<StartProbe>({});

    const onFormDataChange = (jsonPropName: string, jsonPropValue: any) => {
        let updatedFormJson = {
            ...probeData,
            [jsonPropName]: jsonPropValue,
        };
        setProbeData(updatedFormJson);
    }

    console.log(probeData);

return (
    <>
        <OnProbeFormChangeContext.Provider value={onFormDataChange}>
            <StartProbeFormDataContext.Provider value={probeData}>
            <div className= "btn add-probe">
                <ProbeFormDialog />
            </div>
            </StartProbeFormDataContext.Provider>
        </OnProbeFormChangeContext.Provider>
    </>
    )
 
}
