import { app, BrowserWindow } from 'electron';
import path from 'path';
import { spawn } from 'child_process';
import { join } from 'path';
import * as http from 'http';
import { ipcMain } from 'electron'
import createClient from 'client';
import { PingTarget } from 'client/dist/gen/service_pb';
import { PingBoardService } from 'client';
import { Empty } from "@bufbuild/protobuf";


declare const MAIN_WINDOW_VITE_DEV_SERVER_URL: string;
declare const MAIN_WINDOW_VITE_NAME : string;

// Handle creating/removing shortcuts on Windows when installing/uninstalling.
if (require('electron-squirrel-startup')) {
  app.quit();
}

const createWindow = () => {
  // Create the browser window.
  const mainWindow = new BrowserWindow({
    width: 1920,
    height: 1080,
    webPreferences: {
      preload: path.join(__dirname, 'preload.js')
    },
  });

  // and load the index.html of the app.
  if (MAIN_WINDOW_VITE_DEV_SERVER_URL) {
    mainWindow.loadURL(MAIN_WINDOW_VITE_DEV_SERVER_URL);
  } else {
    mainWindow.loadFile(path.join(__dirname, `../renderer/${MAIN_WINDOW_VITE_NAME}/index.html`));
  }

  // Open the DevTools.
  mainWindow.webContents.openDevTools();
};

const serverPath = "C:/Users/LeorGoldberg/Documents/Projects/PingBoard/backend/PingBoard/bin/Release/net8.0/win-x64/publish/";
const socketPath = "client.sock";

const startBackend = () => {
  const p = spawn(join(serverPath, "PingBoard.exe"),
    [],
    {
      cwd: serverPath,
      env: {
        //LOG_TO_FILE:"true",
        CLIENT_SOCKET_PATH:socketPath
      }
    }
  )

  p.stdout.on('data', (info) => {console.log(info.toString())});
}

type BackendClient = ReturnType<typeof createClient>;
let backendClient : BackendClient;

// type rpcName = keyof BackendClient; type union of rpc names
// type la = BackendClient['listAnomalies'] //type definition for list anomalies
// type laRet = ReturnType<la>; //return type of list anomalies
// type waitedLaRet = Awaited<laRet>;
// type returnTypes = ReturnType<BackendClient[keyof BackendClient]> type union of return types for all rpcs in BackendClient
// type firstParameters = Parameters<BackendClient[keyof BackendClient]>[0] type union of first parameters for all rpcs in BackendClient

// rpc returns promises, but Typescript doesn't know that that ReturnType<BackendClient[T] is a promise, so we await it and then 
// explicitly make it a promise. Awaited unwraps the type ONLY if it's a promise

type StreamingOrNever<T extends keyof BackendClient> = ReturnType<BackendClient[T]> extends AsyncIterable<any> ? T : never; 
type PromiseOrNever<T extends keyof BackendClient> = ReturnType<BackendClient[T]> extends Promise<any> ? T : never;
type PromiseReturningBackendClient = {
  [K in keyof BackendClient as PromiseOrNever<K>]: BackendClient[K]
}


async function makeApiRequest<T extends keyof PromiseReturningBackendClient>(
  rpcMethod: T,
  request: Parameters<PromiseReturningBackendClient[T]>[0]
): Promise<Awaited<ReturnType<PromiseReturningBackendClient[T]>>> {

  const inputType = PingBoardService.methods[rpcMethod].I;
  console.log(`Method type: ${PingBoardService.methods[rpcMethod].kind}`);
  let jsonRequest;

  if (inputType.typeName == "google.protobuf.Empty"){
    jsonRequest = {};
  } else{
    console.log(inputType);
    jsonRequest = inputType.fromJson(request as any);
  }

  const pbc = backendClient as PromiseReturningBackendClient;
  const result = await pbc[rpcMethod](jsonRequest);
  console.log(result);

  const outputType = PingBoardService.methods[rpcMethod].O;
  if (outputType.typeName === "google.protobuf.Empty"){
    return {} as Promise<Awaited<ReturnType<PromiseReturningBackendClient[T]>>>
  }

  return (outputType as any).toJson(result);
}



app.on('ready', () => {
  startBackend();
  ipcMain.handle("api:makeRequest", (e, args) => makeApiRequest(args[0], args[1]));
  createWindow();
  backendClient = createClient("http://localhost:5245");
});

// Quit when all windows are closed, except on macOS. There, it's common
// for applications and their menu bar to stay active until the user quits
// explicitly with Cmd + Q.
app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});

app.on('activate', () => {
  // On OS X it's common to re-create a window in the app when the
  // dock icon is clicked and there are no other windows open.
  if (BrowserWindow.getAllWindows().length === 0) {
    createWindow();
  }
});

// In this file you can include the rest of your app's specific main process
// code. You can also put them in separate files and import them here.
