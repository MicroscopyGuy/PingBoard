import { JSXElementConstructor, ReactElement, ReactNode, ReactPortal, useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'

type PingButtonAction = {
  value: "Start" | "Stop"
};

function PingButton(prop: PingButtonAction){
  const className = prop.value === "Start" ? "ping-button ping-start" : "ping-button ping-stop";
  return (
    <>
      <button className={ className }> { prop.value } </button>
    </>
  )
}

function PingTargetInput(){
  return (
    <>
      <input className="ping-target" type="text" placeholder="IPAddress or website here"></input>
    </>
  )
}

function PingStartStopMenu(){
  return(
    <>
    <div className="ping-start-stop-menu">
      <PingButton value="Start"/> 
      <PingButton value="Stop" />
      <PingTargetInput />
    </div>
    </>
  )
}

function ArrayToTableData(array: any[]){
  return <tr>{ array.map(e => <td>{e}</td>) }</tr>;
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

function App() {

  return (
    <div> 
      <PingStartStopMenu />
      <AnomaliesTable />
    </div>
  )
}

export default App


