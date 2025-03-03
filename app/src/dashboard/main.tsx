import React from 'react'
import ReactDOM from 'react-dom/client'
//import App from "./App"
import App from "./AppNew"
import "@cloudscape-design/global-styles/index.css"



import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hub")
    .build();

connection.on("GetMessage", async () => {
    console.log('Received GetMessage Request');
    let promise = new Promise((resolve, reject) => {
        setTimeout(() => {
            resolve("message");
        }, 100);
    });
    return promise;
});


// remember to put App back

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <> </> {`</><App />`} 
  </React.StrictMode>,
)
