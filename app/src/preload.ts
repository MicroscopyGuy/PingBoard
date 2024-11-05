// See the Electron documentation for details on how to use preload scripts:
// https://www.electronjs.org/docs/latest/tutorial/process-model#preload-scripts
import { contextBridge, ipcRenderer } from "electron";

contextBridge.exposeInMainWorld("apiBridge", {
  makeApiRequest: (method: 'GET', path: string) => ipcRenderer.invoke("api:makeRequest", [
    method, path
  ])
});