import PingBackendClientProxy from "./PingBackendClientProxy";
import type { BackendClient } from 'client/dist/index';

function createClient(){
    return new PingBackendClientProxy() as BackendClient;
}

export default createClient;