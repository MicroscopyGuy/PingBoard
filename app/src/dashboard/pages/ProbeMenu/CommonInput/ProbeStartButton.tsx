import {useState, useEffect, useContext} from 'react';
import { Button } from '@/components/ui/button';


export function StartProbeButton(){
    const [probeActive, setProbeActive] = useState<boolean>(false);

    const onProbeSubmitted = (): void => {
        setProbeActive(!probeActive);
    }

    return (
        <div>
            <Button>
                Did it work
            </Button>
        </div>
    )
}