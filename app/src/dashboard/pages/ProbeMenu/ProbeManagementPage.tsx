import { ProbeManagementTable } from './ProbesTable/ProbeManagementTable';
import {useState, useEffect } from 'react';
import { ProbeStatus, columns } from './ProbesTable/ProbeManagementTableColumns';
import { AddProbeButton } from './CommonInput/AddProbeButton';
import { ProbeFormCard } from './CommonInput/ProbeFormCard';

export default function ProbeManagementPage(){
    const [probeStatuses, setProbeStatuses] = useState<ProbeStatus[]>([{
        status: "ok", 
        target: "google.com",
        probetype:"ping",
        id:"qx234876-23409Ajg"
    }]);

    return (
        <div className='probe-management-page flex justify-center items-center'>
            <div className= "btn add-probe">
                <ProbeFormCard />
            </div>
            <div className="tbl probe-management">
                <ProbeManagementTable columns={columns} data={probeStatuses}/>
            </div>
            
        </div> 
    );
}
