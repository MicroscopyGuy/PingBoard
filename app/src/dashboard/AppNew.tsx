import './index.css';
import PingBackendProvider from './PingBackendContext';
import { ThemeProvider } from '@/components/theme-provider';
import { AppSidebar } from '@/components/app-sidebar'; 
import DashboardLayout from './DashboardLayoutNew';

/**
 * @description A component that represents the full application, to be used in main.tsx 
 */
function App() {
  return (
    <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
      <PingBackendProvider>
        <div className="h-screen v-screen">
            <DashboardLayout>
                <AppSidebar/>
            </DashboardLayout>
        </div>
      </PingBackendProvider>
    </ThemeProvider>
    
    
  )
}

export default App