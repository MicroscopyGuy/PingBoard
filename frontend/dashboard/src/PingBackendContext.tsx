import { ReactNode } from "react";
import { createContext, useRef, useEffect, useState, useCallback } from "react";
import createClient from 'client'
import { dispatchEvent, useServerEventListener, CustomEventMap } from "./ServerEventListener.tsx";

type DatabaseContext = {
    client?: BackendClient
}

export const DatabaseContext = createContext<DatabaseContext>({});

type PingBackendProviderProps = {
    children : ReactNode
};

type BackendClient = ReturnType<typeof createClient>;
const backendClient = createClient("http://localhost:5245");

export default function PingBackendProvider(p : PingBackendProviderProps){
    const grabPingServerEvents = useCallback(async(signal : AbortSignal) => {
        const statusStream = backendClient.getLatestServerEvent([], { signal : signal });
        for await (const serverEvent of statusStream) {
            dispatchEvent(serverEvent.ServerEvent.case!.toLowerCase() as keyof CustomEventMap, serverEvent.ServerEvent.value!);
            console.log(`You've got mail`);
            console.log(serverEvent);
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
