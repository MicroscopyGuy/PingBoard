"use client"
 
import * as React from "react"
import { Check, ChevronsUpDown } from "lucide-react"
 
import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"

import { Probe } from './ProbeFormTypes'; 
import { DropdownMenu, 
    DropdownMenuTrigger, 
    DropdownMenuContent, 
    DropdownMenuLabel, 
    DropdownMenuSeparator,
    DropdownMenuItem}
    from '@/components/ui/dropdown-menu'



type ProbeSelectionDropDownProps = {
    onSelection: (probe: Probe) => void;
    probes: Probe[];
    probeSelection: Probe;
}


export function ProbeSelectionDropDown(props: ProbeSelectionDropDownProps) {
  const [open, setOpen] = React.useState(false)
  const [value, setValue] = React.useState("")
 
  return (
    <DropdownMenu>
        <DropdownMenuTrigger>Select Probe</DropdownMenuTrigger>
        <DropdownMenuContent>
            {props.probes.map((probe: Probe) => {
                return(
                    <>
                        <DropdownMenuSeparator />
                        <DropdownMenuItem onClick={() => props.onSelection(probe)}>
                            {probe.label}
                        </DropdownMenuItem>
                    </>           
                )
            })}
        </DropdownMenuContent>            
    </DropdownMenu>
  )
}