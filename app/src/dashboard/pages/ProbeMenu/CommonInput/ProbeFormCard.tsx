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


export function ProbeFormCard(){
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
                    probeSelection={probeType    
                }/>
            <ProbeFormTypeDecider probe={probeType} /> 
        <DialogFooter>
            <Button type="submit">Send Probe<SquarePlay/></Button>
        </DialogFooter>
        </DialogContent>
        </Dialog>
        
    )
}

/*
<Dialog>
      <DialogTrigger asChild>
        <Button variant="outline">Edit Profile</Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Edit profile</DialogTitle>
          <DialogDescription>
            Make changes to your profile here. Click save when you're done.
          </DialogDescription>
        </DialogHeader>
        <div className="grid gap-4 py-4">
          <div className="grid grid-cols-4 items-center gap-4">
            <Label htmlFor="name" className="text-right">
              Name
            </Label>
            <Input id="name" value="Pedro Duarte" className="col-span-3" />
          </div>
          <div className="grid grid-cols-4 items-center gap-4">
            <Label htmlFor="username" className="text-right">
              Username
            </Label>
            <Input id="username" value="@peduarte" className="col-span-3" />
          </div>
        </div>
        <DialogFooter>
          <Button type="submit">Save changes</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
} */