import {useState, useEffect, createContext, useContext } from 'react';
import { PingConfig, StartProbe } from '../../../apiSchemas/startProbeTypes'
import { string } from 'zod';
import { ProbeFormDialog } from './CommonInput/ProbeFormDialog';
import { probes, Probe } from './CommonInput/ProbeFormTypes';
import { StartProbingRequest } from "client/dist/gen/service_pb";
import { DatabaseContext } from '@/PingBackendContext';


export type ProbeConfigChange = (jsonPropName: string, jsonPropValue: any) => void;
export const OnProbeFormChangeContext = createContext<ProbeConfigChange >(null);
export const StartProbeFormDataContext = createContext({}) // put type back as soon as possible

/*
type ProbeApiForms = {
    [UnionMember in StartProbe as UnionMember['probeType']] : {
        [PropertyKey in keyof UnionMember] : UnionMember[PropertyKey]
    }
};*/


const emptyPingProbeConfig = {
} satisfies StartProbe;


type Probes = StartProbe['probeType']

export function useDisplayFormValue(formJson: Record<string, any>, jsonPropName: string): any{
    return (jsonPropName in formJson ? formJson.jsonPropName : "");
}



export function ProbeConfigFormManager(){
    // this is hardcoded for now, but will be properly typed and generalizable to any probe
    // as soon as the frontend/backend are connected and working with pinging
    // tldr; temporarily here while backend/frontend being wired up
    //const [viewAdvanced, setViewAdvanced] = useState<boolean>(false); // not implemented yet
    const [probeData, setProbeData] = useState<Partial<StartProbe>>(emptyPingProbeConfig); 
    const [probeType, setProbeType] = useState<Probe>(null)
    const dbContext = useContext(DatabaseContext);
    //const [viewAdvanced, setViewAdvanced] = useState<boolean>(false); // not implemented yet
    
    const startProbing = (request: Partial<StartProbe> ): void => {

        const reqCopy = probeData;
        probeData['probeType'] = probeType.value as ('ping' | 'traceroute');
        const startProbeReq = new StartProbingRequest({
            requestJson: reqCopy as string
        });

        const client = dbContext.client;
        (client!.startProbing as any)(startProbeReq)
          .catch((err: Error) => console.log( err instanceof Error && 'code' in err
                                ? `Start Pinging: Error code:${err.code}, message: ${err.message}`
                                : `Error message:${err.message}`));
    }
    


    const onFormDataChange = (jsonPropName: string, jsonPropValue: any) => {
        let updatedFormJson = {
            ...probeData,
            [jsonPropName]: jsonPropValue,
        };
        setProbeData(updatedFormJson);
    }

    console.log(probeData);
    console.log(probeType)

    const databaseContext = useContext(DatabaseContext);
  
    
    // make function that gets sent

return (
    <>
        <OnProbeFormChangeContext.Provider value={onFormDataChange}>
            <StartProbeFormDataContext.Provider value={probeData}>
            <div className="btn add-probe">
                <ProbeFormDialog 
                    onSelection={setProbeType}
                    selectedProbe={probeType ? probeType : null}
                    probes={probes}
                    />
            </div>
            </StartProbeFormDataContext.Provider>
        </OnProbeFormChangeContext.Provider>
    </>
    )

}




