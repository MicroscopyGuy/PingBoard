import {
  ColumnDef,
  flexRender,
  getCoreRowModel,
  useReactTable,
} from "@tanstack/react-table"
 
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"

interface ProbeManagementTableProps {
  columns: ColumnDef<ProbeStatus>[]
  data: ProbeStatus[]
}

import { ProbeStatus } from './ProbeManagementTableColumns'

export function ProbeManagementTable(props: ProbeManagementTableProps){

  return (
    <Table className="w-[800px]">
      <TableHeader>
        <TableRow>
          {props.columns.map((columnObj: ColumnDef<ProbeStatus>)=>{
            return <TableHead className="text-right"> {columnObj.header as string} </TableHead>
          })}
        </TableRow>
      </TableHeader>
      <TableBody>
        <TableRow >
          {props.data.map((dataObj: ProbeStatus)=>{
            return (<>
              <TableCell className="text-right"> {dataObj.target as string} </TableCell>
              <TableCell className="text-right"> {dataObj.probetype as string} </TableCell>
              <TableCell className="text-right"> {dataObj.status as string} </TableCell>
            </>)
          })}
        </TableRow>
      </TableBody>
    </Table>
    );
  }