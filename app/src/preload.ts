// See the Electron documentation for details on how to use preload scripts:
// https://www.electronjs.org/docs/latest/tutorial/process-model#preload-scripts
import {
  ListAnomaliesRequest,
  ListAnomaliesResponse,
  ServerEvent,
} from "client/dist/gen/service_pb";
import { contextBridge, ipcRenderer } from "electron";

contextBridge.exposeInMainWorld("apiBridge", {
  makeApiRequest: (rpcName: string, request: any) =>
    ipcRenderer.invoke("api:makeRequest", [rpcName, request]),
});

contextBridge.exposeInMainWorld("eventSubscriber", {
  getServerEvents: (callback: (se: ServerEvent) => void) => {
    ipcRenderer.once("serverEventReceived", (e, se) => callback(se));
  },
});
