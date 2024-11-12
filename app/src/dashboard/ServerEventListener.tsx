import * as proto from "client/dist/gen/service_pb";
import { useEffect } from 'react';

interface CustomEventMap {
    "pingonofftoggle": proto.ServerEvent_PingOnOffToggle;
    "pinganomaly": proto.ServerEvent_PingAnomaly;
    "pingagenterror": proto.ServerEvent_PingAgentError;
}

/*
function addEventHandler<T extends keyof CustomEventMap>(eventName: T, handler: (e: CustomEvent<CustomEventMap[T]>) => void) {
    window.addEventListener(eventName, handler as EventListener);
}*/


function dispatchEvent<T extends keyof CustomEventMap>(eventName: T, payload: CustomEventMap[T]) {
    window.dispatchEvent(new CustomEvent(eventName, { detail: payload}))
}


function useServerEventListener<T extends keyof CustomEventMap> (serverEventName: T, handler: (e: CustomEvent<CustomEventMap[T]>) => void) {
    useEffect(() => {
        window.addEventListener(serverEventName, handler as EventListener);
        return () => window.removeEventListener(serverEventName, handler as EventListener);
    }, [serverEventName, handler]);
}


export {dispatchEvent, type CustomEventMap, useServerEventListener};