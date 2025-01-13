import { app, BrowserWindow } from "electron";
import path from "path";
import { spawn } from "child_process";
import { join } from "path";
import * as http from "http";
import { ipcMain } from "electron";
import {
  ListAnomaliesResponse,
  PingTarget,
  ServerEvent,
} from "client/dist/gen/service_pb";
import { PingBoardService } from "client";
import { Empty } from "@bufbuild/protobuf";
import type {
  Maybe,
  PromiseOrNever,
  BackendClient,
  PromiseReturningBackendClient,
} from "./types";
import createClient from "./createClient";
import { resolve } from "path";
import getPort from 'get-port';

declare const MAIN_WINDOW_VITE_DEV_SERVER_URL: string;
declare const MAIN_WINDOW_VITE_NAME: string;

/******USED WHEN STARTING BACKEND SEPARATELY FOR DEBUG SESSION*******/
const DEBUG_MODE = false;
const DEBUG_MODE_PORT = 5245; // located in ServiceExtensions.cs
/********************************************************************/


// Handle creating/removing shortcuts on Windows when installing/uninstalling.
if (require("electron-squirrel-startup")) {
  app.quit();
}

const createWindow = () => {
  // Create the browser window.
  const mainWindow = new BrowserWindow({
    width: 1920,
    height: 1080,
    webPreferences: {
      preload: path.join(__dirname, "preload.js"),
    },
  });

  // and load the index.html of the app.
  if (MAIN_WINDOW_VITE_DEV_SERVER_URL) {
    mainWindow.loadURL(MAIN_WINDOW_VITE_DEV_SERVER_URL);
  } else {
    mainWindow.loadFile(
      path.join(__dirname, `../renderer/${MAIN_WINDOW_VITE_NAME}/index.html`)
    );
  }

  // Open the DevTools.
  //mainWindow.webContents.openDevTools();
  return mainWindow;
};

const serverPath = resolve("../backend/dist");

const startBackend = async (): Promise<number> => {
  let portNumber;
  if (!DEBUG_MODE){
    portNumber = await getPort();
    const p = spawn(join(serverPath, "PingBoard.exe"), [], {
      cwd: serverPath,
      env: {
        LOG_TO_FILE: "false",
        SERVER_PORT: `${portNumber}`
      },
      //stdio: 'inherit'
    });
    console.log(`StartBackend process ID: ${process.pid}`);
    
    await new Promise<void>((resolve, reject) => {
      p.stdout.on("data", (info) => {
        if (info.toString().includes("Now listening on:")) {
          resolve();
        }
        console.log(info.toString());
      });
  
      p.on("exit", (exitCode) => reject(`Process exited with code ${exitCode}`));
      p.on("error", (error) => reject(`${error}`));
      p.stderr.on("data", (info) => console.log(info));
    });
    console.log(`StartBackend returns port#: ${portNumber}`)
  }
  else{
    portNumber = DEBUG_MODE_PORT;
  }

  return portNumber;
};

let backendClient: BackendClient;

async function tryMakeApiRequest<T extends keyof PromiseReturningBackendClient>(
  rpcMethod: T,
  request: Parameters<PromiseReturningBackendClient[T]>[0]
): Promise<Maybe<Awaited<ReturnType<PromiseReturningBackendClient[T]>>>> {
  try {
    const result = await makeApiRequest(rpcMethod, request);
    return { successful: true, result: result };
  } catch (error) {
    console.log(error);
    return { successful: false, error: error };
  }
}

async function makeApiRequest<T extends keyof PromiseReturningBackendClient>(
  rpcMethod: T,
  request: Parameters<PromiseReturningBackendClient[T]>[0]
): Promise<Awaited<ReturnType<PromiseReturningBackendClient[T]>>> {
  if ((rpcMethod as any) === "getLatestServerEvent") {
    return [] as any;
  }

  const kind = PingBoardService.methods[rpcMethod].kind;
  const inputType = PingBoardService.methods[rpcMethod].I;
  console.log(`Method name: ${rpcMethod.toString()}`);
  // console.log(`Method type: ${PingBoardService.methods[rpcMethod].kind}`);
  let jsonRequest;

  if (inputType.typeName == "google.protobuf.Empty") {
    jsonRequest = {};
  } else {
    jsonRequest = inputType.fromJson(request as any);
  }

  const pbc = backendClient as PromiseReturningBackendClient;
  const result = await pbc[rpcMethod](jsonRequest);
  //console.log(`Result for Method name: ${rpcMethod.toString()}`);

  const outputType = PingBoardService.methods[rpcMethod].O;
  //console.log(outputType);
  if (outputType.typeName === "google.protobuf.Empty") {
    return {} as Promise<Awaited<ReturnType<PromiseReturningBackendClient[T]>>>;
  }

  return (result as any).toJson();
}

async function listenServerEvents(w: BrowserWindow) {
  const iterable = await backendClient.getLatestServerEvent({});

  for await (const event of iterable) {
    w.webContents.send("serverEventReceived", event.toJson());
    console.log(event.toJson());
    //callback(event.toJson());
  }
}

app.on("ready", async () => {
  const portNumber = await startBackend();
  console.log(`app.on("ready") port#: ${portNumber}`)
  ipcMain.handle("api:makeRequest", (e, args) =>
    tryMakeApiRequest(args[0], args[1])
  );

  ipcMain.handle("preloadLog", (logStr) => console.log(logStr));

  const mainWindow = createWindow();
  backendClient = createClient(`http://localhost:${portNumber}`);
  listenServerEvents(mainWindow);
});

// Quit when all windows are closed, except on macOS. There, it's common
// for applications and their menu bar to stay active until the user quits
// explicitly with Cmd + Q.
app.on("window-all-closed", () => {
  if (process.platform !== "darwin") {
    app.quit();
  }
});

app.on("activate", () => {
  // On OS X it's common to re-create a window in the app when the
  // dock icon is clicked and there are no other windows open.
  if (BrowserWindow.getAllWindows().length === 0) {
    createWindow();
  }
});
