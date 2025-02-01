import { ProbeManagementTable } from './ProbesTable/ProbeManagementTable';
import {useState, useEffect } from 'react';
import { ProbeStatus, columns } from './ProbesTable/ProbeManagementTableColumns';
import { AddProbeButton } from './CommonInput/ProbeStartButton';

export default function ProbeManagementPage(){
    const [probeStatuses, setProbeStatuses] = useState<ProbeStatus[]>([{
        status: "ok", 
        target: "google.com",
        probetype:"ping",
        id:"qx234876-23409Ajg"
    }]);

    return (
        <div className='w-[800]px bg-red'>
            <AddProbeButton />
            <ProbeManagementTable columns={columns} data={probeStatuses}/>
        </div> 
    );
}
