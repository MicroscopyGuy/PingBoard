import { ColumnDef } from "@tanstack/react-table"
 
// This type is used to define the shape of our data.
// You can use a Zod schema here if you want.
export type ProbeStatus = {
  id: string;
  target: string;
  probetype: string
  status: string;
}

export const columns: ColumnDef<ProbeStatus>[] = [
  {
    accessorKey: "target",
    header: "Target",
  },
  {
    accessorKey: "probetype",
    header: "Probe Type",
  },
  {
    accessorKey: "status",
    header: "Status",
  },
]