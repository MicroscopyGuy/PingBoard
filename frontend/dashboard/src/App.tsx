import { useState, useContext } from 'react'
import createClient from 'client'
import './App.css'
import { ConnectError } from '@connectrpc/connect';
import PingBackendProvider from './PingBackendContext';
import { DatabaseContext } from './PingBackendContext';
import {AnomaliesTable } from './AnomaliesTable';

// Need pinging state (active | inactive) to be globally accessible
// Perhaps create PingStateManager?

const client = createClient("http://localhost:5245");

interface PingStartButtonProps{
  pingTarget: string

}

function PingStartButton({pingTarget}: PingStartButtonProps){
  //const [isDisabled, setDisabled] = useState<boolean>(false);
  const databaseContext = useContext(DatabaseContext);

  function startPinging(){
    if (pingTarget === ""){
      alert("Must enter a pinging target");
      return;
    }

    //setDisabled(true);
    const client = databaseContext.client;
    client!.startPinging({ target: pingTarget })
      //.then(() => setDisabled(true))
      .catch((err) => console.log( err instanceof ConnectError
                            ? `Start Pinging: Error code:${err.code}, message: ${err.message}`
                            : `Error message:${err.message}`));
  }

 

  return <button className="ping-button ping-start" onClick={ startPinging } disabled={ databaseContext?.pingStatus?.active }>Start</button>
}

function PingStopButton(){
  //const [isDisabled, setDisabled] = useState<boolean>(false);
  const databaseContext = useContext(DatabaseContext);
  //
  function stopPinging(){
    // if already pinging, should do something to prevent it, front-end validation, etc
    client.stopPinging({}) // check on this, empty type
      //.then(() => setDisabled(true))
      .catch((err) => console.log(`Stop Pinging: message: ${err.message}`));
  }

  return <button className="ping-button ping-stop" onClick={ stopPinging } disabled={ !databaseContext?.pingStatus?.active }>Stop</button>
}


interface PingTargetInputManagerProps{
  pingTarget: string,
  onPingTargetChanged: (pingTarget: string) => void;
}

function PingTargetInputManager(props: PingTargetInputManagerProps){
  const databaseContext = useContext(DatabaseContext);

  return <input className = "ping-target"
          type = "text"
          placeholder = "IPAddress or website here"
          value = {props.pingTarget}
          onChange={(e) => {props.onPingTargetChanged(e.target.value)}}
          disabled={ databaseContext?.pingStatus?.active }
          />
}


function PingStartStopMenu(){
  const [pingTarget, setPingTarget] = useState<string>("")
  function onPingTargetChanged(newPingTarget: string){
    setPingTarget(newPingTarget); 
  } 

 return (
  <div className="ping-start-stop-menu">
    <PingStartButton pingTarget={pingTarget}/>
    <PingStopButton />
    <PingTargetInputManager pingTarget={pingTarget} onPingTargetChanged={ onPingTargetChanged }/>
  </div>
 )
}




function DashboardLayout(){
  return (
      <div className="flex-container dashboard">
        <PingStartStopMenu />
        <AnomaliesTable />
      </div>
  )
}

function App() {

  return (
    <PingBackendProvider>
      <DashboardLayout/>
    </PingBackendProvider>
    
  )
}

export default App


