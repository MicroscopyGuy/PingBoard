import { ReactNode } from "react";
import { createContext, useRef, useEffect, useState, useCallback } from "react";
import createClient from 'client'
import { PingStatusMessage } from "client/dist/gen/service_pb";

type DatabaseContext = {
    pingStatus?: PingStatusMessage
    client?: BackendClient
}

const DatabaseContext = createContext<DatabaseContext>({});

type PingBackendProviderProps = {
    children : ReactNode
};

type BackendClient = ReturnType<typeof createClient>;

export default function PingBackendProvider(p : PingBackendProviderProps){
    const backendClient = useRef<BackendClient | null>(null);
    const [pingStatusMessage, setPingStatusMessage] = useState<PingStatusMessage>();

    // *************** Create client if not yet made *****************/
    if (backendClient.current === null){
        backendClient.current = createClient("http://localhost:5245");
    }

    const grabPingStatusMessages = useCallback(async(signal : AbortSignal) => {
        const statusStream = backendClient.current!.getPingingStatus([], { signal : signal });
        for await (const pingStatus of statusStream) {
            setPingStatusMessage(pingStatus);
            console.log(`You've got mail:`);
            console.log(pingStatus);
        }
        console.log("WARNING: No longer listening for PingStatusMessages");
    }, [backendClient, setPingStatusMessage]);

    useEffect(()=>{
        const abortToken = new AbortController();
        grabPingStatusMessages(abortToken.signal)

        return ()=>{ abortToken.abort("I'm just here so I dont get fined") };
    }, [grabPingStatusMessages]);

    return (
        <DatabaseContext.Provider value={{pingStatus: pingStatusMessage, client: backendClient.current }}>
            { p.children }
        </DatabaseContext.Provider>
    )
     
}
