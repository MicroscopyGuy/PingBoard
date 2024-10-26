import { useCallback, useContext, useState, useEffect, useRef } from 'react';
import './AnomaliesTable.css';
import { PingGroupSummaryPublic, ServerEvent_PingAnomaly } from 'client/dist/gen/service_pb';
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
            type = "text"
            value = {pageNumber}
            min = {1}
            style={{maxWidth: "60px", 
                    alignSelf: "stretch", 
                    textAlign:'center', 
                    borderRadius: '5px',
                    border:'transparent',
                    width: '30px',
                    fontSize:"18px"}}/>

}

type AnomaliesTablePageControlProps = {
    pageNumber : number,
    saveUpdatedPage: (newPageNumber: number) => void
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

// Will evaluate adding this functionality later, for now save the same page
function AnomaliesTableOldestButton({pageNumber, saveUpdatedPage}:AnomaliesTablePageControlProps){
    // simply go 5 pages over for now, but this needs to eventually go to the last page. 
    // Will need to use the same API that the AnomaliesTableRightPage button needs in order to detect when it should be disabled.
    return(
        <button onClick={ () => saveUpdatedPage(pageNumber)}>
            <img src="/public/double-chevron-right.svg" className="button nav anomalies-table"/>
        </button>
    );
}


function DisplayAnomalies(anomalies: Array<PingGroupSummaryPublic>){
    return anomalies.map((a) => {
        return <tr key={a.start?.toDate().toString()}> 
                <td>{a.start?.toDate().toString()}</td>
                <td>{a.end?.toDate().toString()}</td>
                <td>{a.target}</td>
                <td>{a.minimumPing}</td>
                <td>{a.averagePing}</td>
                <td>{a.maximumPing}</td>
                <td>{a.jitter.toFixed(3)}</td>
                <td>{a.packetLoss}</td>
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
                {DisplayAnomalies(props.anomalies)}
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
        
        var request = {numberRequested : numRecordsToGet, paginationToken : pTokenCacheRef.current.get(pageNumber-1)};
        console.log(request);
        setLoading(true);
        client?.listAnomalies(request)
            .catch((error) => {
                console.log(error);
                setApiError(error);
            })
            .then((response) => {
                console.log(response!.anomalies.length);
                if (!apiError){
                    setAnomaliesData(response!.anomalies);
                    setEndOfResults(response!.paginationToken != "");
                    if (pageNumber == 1){
                        pTokenCacheRef.current = new Map<number, string>();
                    }
                    pTokenCacheRef.current.set(pageNumber, response!.paginationToken);
                } else{
                    setApiError(undefined);
                    // pagination token cache?    
                }
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
                <AnomaliesTableNewestButton pageNumber={pageNumber} saveUpdatedPage = {setPageNumber}/>
                <AnomaliesTableLeftPage pageNumber={pageNumber} saveUpdatedPage = {setPageNumber}/>
                <AnomaliesTablePageBox pageNumber={pageNumber}/>
                { endOfResults && <AnomaliesTableRightPage pageNumber={pageNumber} saveUpdatedPage = {setPageNumber}/> }
                { endOfResults && <AnomaliesTableOldestButton pageNumber={pageNumber} saveUpdatedPage = {setPageNumber}/> }
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
  