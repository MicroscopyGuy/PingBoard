import { useCallback, useState } from 'react';
import './AnomaliesTable.css';
import { ServerEvent_PingAnomaly } from 'client/dist/gen/service_pb';
import { useServerEventListener } from './ServerEventListener';


type AnomaliesTablePageControlProps = {
    pageNumber : number,
    saveUpdatedPage: (newPageNumber: number) => void
}

/**
 * 
 * @param param0 AnomaliesTablePageControlProps: {pageNumber, saveUpdatedPage}
 * @returns A text box which displays the current page number of the paginated anomalies view
 */
function AnomaliesTablePageBox({pageNumber, saveUpdatedPage}:AnomaliesTablePageControlProps){

    // make sure to add feature to limit textbox value based on number of anomalies in the database
    return <input className = "page-box nav anomalies-table"
            type = "text"
            value = {pageNumber}
            min = {1}
            style={{maxWidth: "60px", 
                    alignSelf: "stretch", 
                    textAlign:'center', 
                    borderRadius: '5px',
                    border:'transparent',
                    width: '30px',
                    fontSize:"18px"}}
            />

}

/**
 * 
 * @param param0 AnomaliesTablePageControlProps: {pageNumber, saveUpdatedPage}
 * @returns A button used to move through the paginated anomalies in reverse order
 */
function AnomaliesTableLeftPage({pageNumber, saveUpdatedPage}:AnomaliesTablePageControlProps){
    return(
        <button disabled={ pageNumber == 1 } onClick={ () => saveUpdatedPage(pageNumber-1) }>
            <img src="/public/chevron-left.svg" className="button nav anomalies-table"/>
        </button>
    );
    
}

function AnomaliesTableRightPage({pageNumber, saveUpdatedPage}:AnomaliesTablePageControlProps){
    // need to eventually make some sort of API to get the count of pages that are anomalies,
    // use that information to disable the right page button when at the end of results
    return(
        <button onClick={ () => saveUpdatedPage(pageNumber+1)}>
            <img src="/public/chevron-right.svg" className="button nav anomalies-table" style={{justifyContent: "center"}}/>
        </button>
    );
}


function AnomaliesTableNewestButton({pageNumber, saveUpdatedPage}:AnomaliesTablePageControlProps){
    return(
        <button disabled={ pageNumber == 1 } onClick={ () => saveUpdatedPage(1)}>
            <img src="/public/double-chevron-left.svg" className="button nav anomalies-table"/>
        </button>
    );
}

function AnomaliesTableOldestButton({pageNumber, saveUpdatedPage}:AnomaliesTablePageControlProps){
    // simply go 5 pages over for now, but this needs to eventually go to the last page. 
    // Will need to use the same API that the AnomaliesTableRightPage button needs in order to detect when it should be disabled.
    return(
        <button onClick={ () => saveUpdatedPage(pageNumber+5)}>
            <img src="/public/double-chevron-right.svg" className="button nav anomalies-table"/>
        </button>
    );
}


// will need callback to parent, parent will need to request new records
function AnomaliesTableNavigation(){
    const [pageNumber, setPageNumber] = useState<number>(1);

    function saveUpdatedPage(num: number){
        setPageNumber(num);
    } 

    return( 
        <div className="nav anomalies-table" style={{display:'flex'}}>
            <AnomaliesTableNewestButton pageNumber={pageNumber} saveUpdatedPage = {saveUpdatedPage}/>
            <AnomaliesTableLeftPage pageNumber={pageNumber} saveUpdatedPage = {saveUpdatedPage}/>
            <AnomaliesTablePageBox pageNumber={pageNumber} saveUpdatedPage = {saveUpdatedPage}/>
            <AnomaliesTableRightPage pageNumber={pageNumber} saveUpdatedPage = {saveUpdatedPage}/>
            <AnomaliesTableOldestButton pageNumber={pageNumber} saveUpdatedPage = {saveUpdatedPage}/>
        </div>
    )
}


function ArrayToTableData(array: any[]){
    return <tr>{ array.map((e, index) => <td key={index}>{e}</td>) }</tr>;
}

function AnomaliesTableOutput(){
    const [anomalyInfo, setAnomalyInfo] = useState<Array<string>>([]); // data structure to be determined later
  
    function onPingAnomaly(description : string){
        const newData = [...anomalyInfo, description ];
        setAnomalyInfo(newData);
        console.log("New PingAnomaly data:");
        console.log(newData);
      return; 
    }
    
    const eventHandler = useCallback((e: CustomEvent<ServerEvent_PingAnomaly>) => {
      onPingAnomaly(e.detail.anomalyDescription);
      console.log("AnomaliesTableOutput: PingAnomaly event");
      console.log(e);
    }, [setAnomalyInfo]);
  
    useServerEventListener("pinganomaly", eventHandler);

    const sampleData = [
        ["1","2024-06-07 08:31:14.898", "2024-06-07 08:31:15.776", "8.8.8.8", "3", "4", "5", "1", "0"],
        ["2","2024-06-07 08:31:15.912", "2024-06-07 08:31:16.789", "8.8.8.8", "3", "3.875", "6", "1.286", "0"],
        ["3","2024-06-07 08:31:16.912", "2024-06-07 08:31:17.788", "8.8.8.8", "3", "4", "5", "1.143", "0"]
    ];

    return (
        <table className="styled-table anomalies">
            <thead>
            <tr>
                <th>Number</th>
                <th>StartTime</th>
                <th>EndTime</th>
                <th>Target</th>
                <th>MinimumPing</th>
                <th>AveragePing</th>
                <th>MaximumPing</th>
                <th>Jitter</th>
                <th>PacketLoss</th>
            </tr>
            </thead>
            <tbody>
            {ArrayToTableData(sampleData[0])}
            {ArrayToTableData(sampleData[1])}
            {ArrayToTableData(sampleData[2])}
            </tbody>
        </table>
    )
}

export function AnomaliesTable(){
    return (
        <div className="flex-container anomalies-table">
        <AnomaliesTableOutput />
        <AnomaliesTableNavigation />
        </div>
    )
}  
  