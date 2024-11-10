// See the Electron documentation for details on how to use preload scripts:
// https://www.electronjs.org/docs/latest/tutorial/process-model#preload-scripts
import { ListAnomaliesRequest, ListAnomaliesResponse } from "client/dist/gen/service_pb";
import { contextBridge, ipcRenderer } from "electron";

contextBridge.exposeInMainWorld("apiBridge", {
  makeApiRequest: (rpcName: string, request: any) => ipcRenderer.invoke("api:makeRequest", [
    rpcName, request
  ])
})