import { useState, useContext, useCallback } from 'react';
import './App.css';
import { DatabaseContext } from './PingBackendContext';
import { useServerEventListener } from "./ServerEventListener";
import { PingTarget, ServerEvent_PingOnOffToggle, StartPingingRequest } from "client/dist/gen/service_pb";
import { Empty } from "@bufbuild/protobuf";

interface PingStartButtonProps{
    pingTarget: string,
    pingingActive: boolean
}
  
/**
* @param param0 {pingTarget} : PingStartButtonProps
* @description A button to start pinging after a target is entered in the target box
*/
function PingStartButton({pingTarget, pingingActive}: PingStartButtonProps){
    const databaseContext = useContext(DatabaseContext);
  
    function startPinging(){
      if (pingTarget === ""){
        alert("Must enter a pinging target");
        return;
      }
  
      const client = databaseContext.client;
      var request = new StartPingingRequest({
        target: new PingTarget({
            target: '8.8.8.8'
        })
      });

      client!.startPinging(request)
        .catch((err: Error) => console.log( err instanceof Error && 'code' in err
                              ? `Start Pinging: Error code:${err.code}, message: ${err.message}`
                              : `Error message:${err.message}`));
    }
  
    return <button className="ping-button ping-start" onClick={ startPinging } disabled={ pingingActive }>Start</button>
}
  
  
interface PingStopButtonProps{
    pingingActive: boolean
}

/**
* @description A button to stop the pinging of the active target
*/
function PingStopButton({pingingActive} : PingStopButtonProps ){
    const databaseContext = useContext(DatabaseContext);
  
    const stopPinging = useCallback( () =>{
      databaseContext.client!.stopPinging(new Empty()) 
        .catch((err: Error) => console.log(`Stop Pinging: message: ${err.message}`));
    }, [databaseContext]);
  
    return <button className="ping-button ping-stop" onClick = {stopPinging} disabled = { !pingingActive }>Stop</button>
}
  
interface PingTargetInputManagerProps{
    pingTarget: string,
    onPingTargetChanged: (pingTarget: string) => void,
    pingingActive: boolean;
}
  
/**
* @param props {PingTargetInputManagerProps}
* @description Controls and outputs the textbox in which the pinging target is specified by the user
*/
function PingTargetInputManager(props: PingTargetInputManagerProps){
    return <input className = "ping-target"
            type = "text"
            placeholder = "IPAddress or website here"
            value = {props.pingTarget}
            onChange = {(e) => {props.onPingTargetChanged(e.target.value)}}
            disabled = {props.pingingActive}/>
}
  
/**
* @description A component which represents the entirety of the pinging controls, and is responsible
*              for tracking whether pinging is on or off via the ServerEvent stream.
*/
export default function PingStartStopMenu(){
    const [pingTarget, setPingTarget] = useState<string>("");
    const [pingingActive, setPingingActive ] = useState<boolean>(false);
  
    function onPingTargetChanged(newPingTarget: string){
      setPingTarget(newPingTarget); 
    }
    
    const eventHandler = useCallback((e: CustomEvent<ServerEvent_PingOnOffToggle>) => {
      setPingingActive(e.detail.active);
      console.log("PingStartStopMenu: PingOnOffToggle event");
      console.log(e);
    }, [setPingingActive]);
  
    useServerEventListener("pingonofftoggle", eventHandler);
  
   return (
    <div className="ping-start-stop-menu">
      <PingStartButton pingTarget = {pingTarget} pingingActive = {pingingActive}/>
      <PingStopButton pingingActive = {pingingActive}/>
      <PingTargetInputManager pingTarget = {pingTarget} onPingTargetChanged = {onPingTargetChanged} pingingActive = {pingingActive}/>
    </div>
   )
}