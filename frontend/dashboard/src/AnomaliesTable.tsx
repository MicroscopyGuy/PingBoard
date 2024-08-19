import { IconSize } from "@blueprintjs/icons";
import { Icon } from "@blueprintjs/core";
import { useState } from 'react';
import './AnomaliesTable.css';

function ArrayToTableData(array: any[]){
    return <tr>{ array.map((e, index) => <td key={index}>{e}</td>) }</tr>;
}

function AnomaliesTableOutput(){
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


type AnomaliesTablePageControlProps = {
    pageNumber : number,
    saveUpdatedPage: (newPageNumber: number) => void
}

function AnomaliesTablePageBox({pageNumber, saveUpdatedPage}:AnomaliesTablePageControlProps){

    // make sure to add feature to limit textbox value based on number of anomalies in the database
    return <input className = "indicator nav anomalies-table"
            type = "text"
            value = {pageNumber}
            min = {1}
            />

}

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
        <button onClick={ () => saveUpdatedPage(pageNumber+1)}><img src="/public/chevron-right.svg" className="button nav anomalies-table"/></button>
    );
}

/*
function AnomaliesTableCurrentButton(){

}

function AnomaliesTableOldestButton(){

}
*/


function AnomaliesTableNavigation(){
    const [pageNumber, setPageNumber] = useState<number>(1);

    function saveUpdatedPage(num: number){
        setPageNumber(num);
    } 

    return( 
        <div className="nav anomalies-table">
            <AnomaliesTableLeftPage pageNumber={pageNumber} saveUpdatedPage = {saveUpdatedPage}/>
            <AnomaliesTablePageBox pageNumber={pageNumber} saveUpdatedPage = {saveUpdatedPage}/>
            <AnomaliesTableRightPage pageNumber={pageNumber} saveUpdatedPage = {saveUpdatedPage}/>
        </div>
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
  