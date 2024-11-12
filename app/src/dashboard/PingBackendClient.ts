import PingBackendClientProxy from "./PingBackendClientProxy";
import type { BackendClient } from "client/dist/index";

function createClient() {
  const p = new PingBackendClientProxy() as BackendClient;
  (window as any).clientProxy = p;
  return p;
  return new PingBackendClientProxy() as BackendClient;
}

export default createClient;
