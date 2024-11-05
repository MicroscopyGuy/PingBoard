import { app, BrowserWindow } from 'electron';
import path from 'path';
import { spawn } from 'child_process';
import { join } from 'path';
import * as http from 'http';
import { ipcMain } from 'electron'

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
      preload: path.join(__dirname, 'preload.js'),
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
const startBackend = () => {
  spawn(join(serverPath, "PingBoard.exe"),
    [],
    {
      cwd: serverPath,
      env: {
        LOG_TO_FILE:"true",
        CLIENT_SOCKET_PATH:"/temp/client.sock"
      }
    }
  )
}

async function makeApiRequest(method: 'GET', path: string) {

  const options: http.RequestOptions = {
    socketPath: "/tmp/wizard.sock",   // Replace with a path that is more reasonable to you, a relative path is fine, maybe in AppData
    path: path,
    method: method,
    headers: {
      Accept: "application/json",
    },
  };

  const reqPromise = new Promise<string>((resolve, reject) => {
    const callback = (res: http.IncomingMessage) => {
      console.log(`STATUS: ${res.statusCode}`);
      res.setEncoding("utf8");
      res.on("data", (d) => resolve(d as string));
      res.on("error", (error) => reject(`${error.name}: ${error.message}`));
    };

    const clientRequest = http.request(options, callback);
    clientRequest.end();
  });
  return await reqPromise;
}

app.on('ready', () => {
  ipcMain.handle("api:makeRequest", (e, args) => makeApiRequest(args[0], args[1]));
});



// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.on('ready', createWindow);
app.on('ready', startBackend);

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
