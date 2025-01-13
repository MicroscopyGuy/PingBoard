import './AnomaliesTable.css';
import { useCallback, useContext, useState, useEffect, useRef } from 'react';
import { ListAnomaliesRequest, ListAnomaliesResponse, PingGroupSummaryPublic, ServerEvent_PingAnomaly } from 'client/dist/gen/service_pb';
import { useServerEventListener } from './ServerEventListener';
import { DatabaseContext } from './PingBackendContext';

type AnomaliesTablePageViewProps = {
    pageNumber : number;
}

/**
 * 
 * @param param0 AnomaliesTablePageControlProps: {pageNumber, saveUpdatedPage}
 * @returns A text box which displays the current page number of the paginated anomalies view
 */
function AnomaliesTablePageBox({pageNumber}:AnomaliesTablePageViewProps){

    // make sure to add feature to limit textbox value based on number of anomalies in the database
    return <input className = "page-box nav anomalies-table"
            readOnly
            type="text"
            value={pageNumber}
            min={1}
            style={{maxWidth: "60px", 
                    alignSelf: "stretch", 
                    textAlign:'center', 
                    borderRadius: '5px',
                    border:'transparent',
                    width: '30px',
                    fontSize:"18px"}}/>

}

type AnomaliesPageSetter = (current: number) => number;
type AnomaliesTablePageControlProps = {
    saveUpdatedPage: (newPageNumber: AnomaliesPageSetter) => void
    disabled: boolean
}

/**
 * 
 * @param param0 AnomaliesTablePageControlProps: {pageNumber, saveUpdatedPage}
 * @returns A button used to move through the paginated anomalies in reverse order
 */
function AnomaliesTableLeftPage({saveUpdatedPage, disabled}:AnomaliesTablePageControlProps){
    return(
        <button disabled={ disabled } onClick={ () => saveUpdatedPage((current) => current-1)}>
            <img src="/public/chevron-left.svg" className="button nav anomalies-table"/>
        </button>
    );
    
}

function AnomaliesTableRightPage({saveUpdatedPage, disabled}:AnomaliesTablePageControlProps){
    // need to eventually make some sort of API to get the count of pages that are anomalies,
    // use that information to disable the right page button when at the end of results
    return(
        <button disabled={disabled} onClick={ () => saveUpdatedPage((current) => current+1)}>
            <img src="/public/chevron-right.svg" className="button nav anomalies-table" style={{justifyContent: "center"}}/>
        </button>
    );
}


function AnomaliesTableNewestButton({saveUpdatedPage, disabled}:AnomaliesTablePageControlProps){
    return(
        <button disabled={ disabled} onClick={ () => saveUpdatedPage((_) => 1)}>
            <img src="/public/double-chevron-left.svg" className="button nav anomalies-table"/>
        </button>
    );
}

// Will evaluate adding this functionality later, for now save the same page
function AnomaliesTableOldestButton({saveUpdatedPage}:AnomaliesTablePageControlProps){
    // simply go 5 pages over for now, but this needs to eventually go to the last page. 
    // Will need to use the same API that the AnomaliesTableRightPage button needs in order to detect when it should be disabled.
    return(
        <button disabled={true} onClick={() => saveUpdatedPage((_) => 999)}>
            <img src="/public/double-chevron-right.svg" className="button nav anomalies-table"/>
        </button>
    );
}


function IndicateEmptyAnomalies(){
    return <tr>
                <td colSpan={8}>
                    No results found.
                </td>
           </tr>
}

function DisplayAnomalies(anomalies: Array<PingGroupSummaryPublic>){
    return anomalies.map((a) => {
        return <tr key={a.start?.toDate().toString() + a.end?.toDate().toString()}> 
                    <td>{a.start?.toDate().toLocaleString()}</td>
                    <td>{a.end?.toDate().toLocaleString()}</td>
                    <td>{a.target}</td>
                    <td>{a.minimumPing + "ms"}</td>
                    <td>{a.averagePing.toFixed(3) + "ms"}</td>
                    <td>{a.maximumPing + "ms"}</td>
                    <td>{a.jitter.toFixed(3)+"ms"}</td>
                    <td>{a.packetLoss.toFixed(2) + "%"}</td>
               </tr>
    }) 
}

interface AnomaliesTableOutputProps{
    anomalies: Array<PingGroupSummaryPublic> ;
}

function AnomaliesTableOutput(props : AnomaliesTableOutputProps){
    return (
        <table className="styled-table anomalies">
            <thead>
            <tr>
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
                {props.anomalies.length > 0 
                    ? DisplayAnomalies(props.anomalies) 
                    : IndicateEmptyAnomalies() 
                }
            </tbody>
        </table>
    )
}

/**
 * @description Responsible for tracking the current state of the Anomalies table, including data, pagination tokens and current page
 */
function AnomaliesTableManager(){
    const [pageNumber, setPageNumber] = useState<number>(1);
    const [anomaliesData, setAnomaliesData] = useState<Array<PingGroupSummaryPublic>>([]);
    const [newAnomaly, setNewAnomaly] = useState<ServerEvent_PingAnomaly>();
    const [apiError, setApiError] = useState<Error>();
    const [loading, setLoading] = useState<boolean>();
    const [endOfResults, setEndOfResults] = useState<boolean>(false);
    const [numRecordsToGet, setNumRecordsToGet] = useState<number>(10); // make this more flexible in the future
    const pTokenCacheRef = useRef<Map<number, string>>(new Map<number, string>());
    const databaseContext = useContext(DatabaseContext);
    const client = databaseContext.client;

    const loadAnomalies = useCallback(() => {
        console.log("loadAnomalies entered");
        var request = new ListAnomaliesRequest({
            numberRequested: numRecordsToGet,
            paginationToken: pTokenCacheRef.current.get(pageNumber-1) ?? ""
        });
        console.log(request);
        setLoading(true);
        client?.listAnomalies(request)
            .then((response: ListAnomaliesResponse) => {
                console.log(response!.anomalies.length);
                setAnomaliesData(response!.anomalies);
                setEndOfResults(response?.paginationToken ? false : true);
                if (pageNumber == 1){
                    pTokenCacheRef.current = new Map<number, string>();
                }
                pTokenCacheRef.current.set(pageNumber, response!.paginationToken);
                setApiError(undefined);  
            })
            .catch((error: Error) => {
                console.log(error);
                setApiError(error);
            })
            .finally(() => {
                setLoading(false);
            });
    }, [client, setAnomaliesData, pageNumber, newAnomaly]);
    
    const eventHandler = useCallback((e: CustomEvent<ServerEvent_PingAnomaly>) => {
      setNewAnomaly(e.detail);
    }, [setNewAnomaly, loadAnomalies]);
  
    useEffect(() => {
        loadAnomalies();
    }, [loadAnomalies, newAnomaly, pageNumber]);

    useServerEventListener("pinganomaly", eventHandler);
    return (
        <>
            <AnomaliesTableOutput anomalies={anomaliesData}/>
            <div className="nav anomalies-table" style={{display:'flex'}}>
                <AnomaliesTableNewestButton disabled={pageNumber === 1} saveUpdatedPage={setPageNumber}/>
                <AnomaliesTableLeftPage disabled={pageNumber === 1} saveUpdatedPage={setPageNumber}/>
                <AnomaliesTablePageBox pageNumber={pageNumber}/>
                <AnomaliesTableRightPage disabled={endOfResults} saveUpdatedPage={setPageNumber}/>
                <AnomaliesTableOldestButton disabled={endOfResults} saveUpdatedPage={setPageNumber}/>
            </div>
        </>
    ) 
}



export function AnomaliesTable(){
    return (
        <div className="flex-container anomalies-table">
            <AnomaliesTableManager/>
        </div>
    )
}  
  