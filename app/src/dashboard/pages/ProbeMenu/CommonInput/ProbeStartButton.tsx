import {useState, useEffect, useContext} from 'react';
import { Button } from '@/components/ui/button';
import {SquarePlus} from 'lucide-react';


export function AddProbeButton(){
    const [probeActive, setProbeActive] = useState<boolean>(false);

    const onProbeSubmitted = (): void => {
        setProbeActive(!probeActive);
    }

    return (
        <div>
            <Button>
                Add Probe <SquarePlus    />
            </Button>
        </div>
    )
}