import { ReactNode } from "react";
import { createContext, useEffect, useCallback } from "react";
import type * as bc from 'client/dist/gen/service_pb';
import { dispatchEvent, CustomEventMap } from "./ServerEventListener";
import { BackendClient } from 'client/dist/index';
import createClient from './PingBackendClient';

type DatabaseContext = {
    client?: BackendClient
}

export const DatabaseContext = createContext<DatabaseContext>({});

type PingBackendProviderProps = {
    children : ReactNode
};

const backendClient = createClient();

export default function PingBackendProvider(p : PingBackendProviderProps){
    const grabPingServerEvents = useCallback(async(signal : AbortSignal) => {
        const statusStream = backendClient.getLatestServerEvent([], { signal : signal });
        for await (const serverEvent of statusStream) {
            console.log(serverEvent);
            dispatchEvent(serverEvent.ServerEvent.case!.toLowerCase() as keyof CustomEventMap, serverEvent.ServerEvent.value!);
            console.log(`You've got mail`);
            
        }
        console.log("WARNING: No longer listening for ServerEvents");
    }, []);

    useEffect(()=>{
        const abortToken = new AbortController();
        grabPingServerEvents(abortToken.signal)

        return ()=>{
            try{
                abortToken.abort("I'm just here so I dont get fined");
            }
            catch(error){
                console.log(`ServerEvent stream aborted.: ${error}`);
            }
        };
    }, [grabPingServerEvents]);

    return (
        <DatabaseContext.Provider value={{client: backendClient }}>
            { p.children }
        </DatabaseContext.Provider>
    )
     
}
