import {useState} from 'react';
import {Card} from '@/components/ui/card';
import {Button} from '@/components/ui/button'
import { SquarePlay } from 'lucide-react';
import { ProbeSelectionDropDown } from './ProbeSelectionDropDown';
import { probes } from './ProbeFormTypes';

export function ProbeCardForm(){
    const [probeType, setProbeType] = useState<string>('');
    const [viewAdvanced, setViewAdvanced] = useState<boolean>(false);

    return (
        <>
            <div>
                <ProbeSelectionDropDown probes={probes} currentSelection/>
            </div>
            <div>
                <Button>
                    Send Probe 
                    <SquarePlay/>
                </Button>
            </div>
            <div>
                <Card/>
            </div>
        </>

    )
}