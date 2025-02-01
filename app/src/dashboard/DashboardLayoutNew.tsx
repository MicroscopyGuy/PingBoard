import { SidebarProvider, SidebarTrigger } from "@/components/ui/sidebar"
import { AppSidebar } from "@/components/app-sidebar"
import { Outlet } from "react-router";
 
export default function Layout() {
  return (
    <div className="app-layout">
        <SidebarProvider>
        <div className="sidebar-area">
        <AppSidebar />
        <main>
            <SidebarTrigger />
        </main>
        </div>

        <div className="content-area">
            <Outlet/>
        </div>
        </SidebarProvider>
    </div>
  )
}