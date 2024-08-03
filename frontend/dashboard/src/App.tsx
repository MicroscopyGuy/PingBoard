import { useState } from 'react'
import createClient from 'client'
import './App.css'
import { ConnectError } from '@connectrpc/connect';


// Need pinging state (active | inactive) to be globally accessible
// Perhaps create PingStateManager?

const client = createClient("http://localhost:5245");

interface PingStartButtonProps{
  pingTarget: string
}

function PingStartButton({pingTarget}: PingStartButtonProps){
  const [isDisabled, setDisabled] = useState<boolean>(false);
  //
  function startPinging(){

    if (pingTarget === ""){
      alert("Must enter a pinging target");
      return;
    }

    setDisabled(true);
    client.startPinging({ target: pingTarget })
      .then(() => setDisabled(true))
      .catch((err) => console.log( err instanceof ConnectError
                            ? `Start Pinging: Error code:${err.code}, message: ${err.message}`
                            : `Error message:${err.message}`));
  }

  return <button className="ping-button ping-start" onClick={ startPinging } disabled={ isDisabled }>Start</button>
}

function PingStopButton(){
  const [isDisabled, setDisabled] = useState<boolean>(false);
  //
  function stopPinging(){

    // if already pinging, should do something to prevent it, front-end validation, etc
    client.stopPinging({}) // check on this, empty type
      .then(() => setDisabled(true))
      .catch((err) => console.log(`Stop Pinging: message: ${err.message}`));
  }

  return <button className="ping-button ping-stop" onClick={ stopPinging } disabled= { isDisabled }>Stop</button>
}


interface PingTargetInputManagerProps{
  pingTarget: string,
  onPingTargetChanged: (pingTarget: string) => void;
}

function PingTargetInputManager(props: PingTargetInputManagerProps){
  return <input className = "ping-target"
          type = "text"
          placeholder = "IPAddress or website here"
          value = {props.pingTarget}
          onChange={(e) => {props.onPingTargetChanged(e.target.value)}}
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
    <PingTargetInputManager pingTarget={pingTarget} onPingTargetChanged={onPingTargetChanged}/>
  </div>
 )
}

function ArrayToTableData(array: any[]){
  return <tr>{ array.map((e, index) => <td key={index}>{e}</td>) }</tr>;
}

function AnomaliesTable(){
  const sampleData = [
    ["1","2024-06-07 08:31:14.898", "2024-06-07 08:31:15.776", "8.8.8.8", "3", "4", "5", "1", "0"],
    ["2","2024-06-07 08:31:15.912", "2024-06-07 08:31:16.789", "8.8.8.8", "3", "3.875", "6", "1.286", "0"],
    ["3","2024-06-07 08:31:16.912", "2024-06-07 08:31:17.788", "8.8.8.8", "3", "4", "5", "1.143", "0"]
  ];

  return (
    <>
      <table className="styled-table anomalies">
        <thead>
          <tr>
            <th>Number</th>
            <th>StartTime</th>
            <th>EndTime</th>
            <th>Target</th>
            <th>MinimumPing</th>
            <th>AveragePing</th>
            <th>MaximumPing</th>
            <th>Jitter</th>
            <th>PacketLoss</th>
          </tr>
        </thead>
        <tbody>
          {ArrayToTableData(sampleData[0])}
          {ArrayToTableData(sampleData[1])}
          {ArrayToTableData(sampleData[2])}
        </tbody>
      </table>
    </>
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
    <DashboardLayout/>
  )
}

export default App


