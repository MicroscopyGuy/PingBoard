import { useState, useEffect, useContext} from 'react';
import { Button } from '@/components/ui/button';
import { SquarePlus } from 'lucide-react';
import { ProbeFormCard } from './ProbeFormCard';


// will need callback to submit form info later
export function AddProbeButton(){
    const [addingProbe, setAddingProbe] = useState<boolean>(false);

    /*
    const onProbeSubmitted = (): void => {
        setProbeActive(!probeActive);
    }*/

    return (
        <div>
            <Button className="btn add-probe" onClick={() => setAddingProbe(!addingProbe)}>
                <SquarePlus/>
                Add Probe
                {!addingProbe && <ProbeFormCard />}
            </Button>
        </div>
    )
}