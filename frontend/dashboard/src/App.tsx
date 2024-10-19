
import './App.css';
import PingBackendProvider from './PingBackendContext';
import { AnomaliesTable } from './AnomaliesTable';
import PingStartStopMenu from "./PingStartStopMenu.tsx";

/**
 * @description The component that pulls together the various high-level components needed to form the dashboard 
 */
function DashboardLayout(){
  return (
      <div className="flex-container dashboard">
        <PingStartStopMenu/>
        <AnomaliesTable/>
      </div>
  )
}

/**
 * @description A component that represents the full application, to be used in main.tsx 
 */
function App() {
  return (
    <PingBackendProvider>
      <DashboardLayout/>
    </PingBackendProvider>
    
  )
}

export default App