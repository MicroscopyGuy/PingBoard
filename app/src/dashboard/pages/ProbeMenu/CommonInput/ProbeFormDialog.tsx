import {useState} from 'react';
import {Card} from '@/components/ui/card';
import {Button} from '@/components/ui/button'
import { SquarePlay, SquarePlus } from 'lucide-react';
import { ProbeSelectionDropDown } from './ProbeSelectionDropDown';
import { probes, Probe } from './ProbeFormTypes';
import { ProbeFormTypeDecider} from './ProbeFormTypeDecider';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
  } from '@/components/ui/dialog';
import '../../../App.css'



export function ProbeFormDialog(){
    const [probeType, setProbeType] = useState<Probe>(null);
    const [viewAdvanced, setViewAdvanced] = useState<boolean>(false); // not implemented yet

    return (
        <Dialog>
        <DialogTrigger asChild>
            <Button variant="outline">Add Probe<SquarePlus/></Button>
        </DialogTrigger>
        <DialogContent className="sm:max-w-[525px]">
        <DialogHeader>
            <DialogTitle>Choose and configure Probe</DialogTitle>
            <DialogDescription>
            Select an operation and then configure the probe. Click "Send Probe" when done.
            </DialogDescription>
        </DialogHeader>
        <ProbeSelectionDropDown
                    probes={probes}
                    onSelection={setProbeType}
                    probeSelection={probeType}/>
                    
            <ProbeFormTypeDecider probe={probeType} /> 
        <DialogFooter>
            <Button type="submit" className="btn send-probe">Send Probe<SquarePlay/></Button>
        </DialogFooter>
        </DialogContent>
        </Dialog>
        
    )
}
