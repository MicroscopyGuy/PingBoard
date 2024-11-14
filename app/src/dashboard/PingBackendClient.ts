import PingBackendClientProxy from "./PingBackendClientProxy";
import type { BackendClient } from "../types";

function createClient() {
  const p = new PingBackendClientProxy() as BackendClient;
  (window as any).clientProxy = p;
  return p;
}

export default createClient;
