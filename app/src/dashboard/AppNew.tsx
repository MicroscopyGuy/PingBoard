import './index.css';
import PingBackendProvider from './PingBackendContext';
import { ThemeProvider } from '@/components/theme-provider';
import { AppSidebar } from '@/components/app-sidebar'; 
import DashboardLayout from './DashboardLayoutNew';
import { Routes, Route, HashRouter } from "react-router";
import SummariesGraph from './SummariesGraph';
import TestElement from './TestElement';
import { AnomaliesTable } from './AnomaliesTable';
import ProbeManagementPage from './pages/ProbeMenu/ProbeManagementPage';


/**
 * @description A component that represents the full application, to be used in main.tsx 
 */
function App() {
  return (
    <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
      <PingBackendProvider>
        <div className="h-screen v-screen">
            <HashRouter>
                <Routes>
                    <Route path='/' element={<DashboardLayout />} >
                        <Route path='/home' element={<ProbeManagementPage />} />
                        <Route path='/probe-graph' element={<AnomaliesTable />} />
                        <Route path='/settings'/>
                        <Route path='/github'/>
                    </Route>
                </Routes>
            </HashRouter>
        </div>
      </PingBackendProvider>
    </ThemeProvider>
  )
}

export default App