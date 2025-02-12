import { ProbeManagementTable } from './ProbesTable/ProbeManagementTable';
import { useState, useEffect } from 'react';
import { ProbeStatus, columns } from './ProbesTable/ProbeManagementTableColumns';
import { ProbeFormDialog } from './CommonInput/ProbeFormDialog';
import {ProbeConfigFormManager} from './ProbeConfigFormManager'


export default function ProbeManagementPage(){
    const [probeStatuses, setProbeStatuses] = useState<ProbeStatus[]>([{
        status: "ok", 
        target: "google.com",
        probetype:"ping",
        id:"qx234876-23409Ajg"
    }]);

    return (
        <div className='app-page'>
            <ProbeConfigFormManager />
            <div className="tbl probe-management">
                <ProbeManagementTable columns={columns} data={probeStatuses}/>
            </div>
        </div> 
    );
}


