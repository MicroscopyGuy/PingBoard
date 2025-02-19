import {useState} from 'react';
import {Card} from '@/components/ui/card';
import {Button} from '@/components/ui/button'
import { SquarePlay, SquarePlus } from 'lucide-react';
import { ProbeSelectionDropDown } from './ProbeSelectionDropDown';
import { Probe } from './ProbeFormTypes';
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


interface ProbeFormDialogProps{
    onSelection: (probe: Probe) => void;
    selectedProbe: Probe;
    probes: Probe[]
}


export function ProbeFormDialog(props: ProbeFormDialogProps){

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
            probes={props.probes}
            onSelection={props.onSelection}
            probeSelection={props.selectedProbe}/>
                    
            <ProbeFormTypeDecider probe={props.selectedProbe} /> 
        <DialogFooter>
            <Button type="submit" /*onClick={}*/ className="btn send-probe">Send Probe<SquarePlay/></Button>
        </DialogFooter>
        </DialogContent>
        </Dialog>
        
    )
}
