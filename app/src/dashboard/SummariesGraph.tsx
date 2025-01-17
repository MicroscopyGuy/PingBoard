import "@cloudscape-design/global-styles/index.css"
import LineChart from "@cloudscape-design/components/line-chart";
import Box from "@cloudscape-design/components/box";
import Button from "@cloudscape-design/components/button";
import Link from "@cloudscape-design/components/link";
import { PingTarget, ServerEvent_PingInfo } from "client/dist/gen/service_pb";
import {useState, useEffect, useCallback} from 'react';
import { useBackendClient } from './PingBackendContext'
import { useServerEventListener } from "./ServerEventListener";
import { Timestamp } from "@bufbuild/protobuf";

/* @DEPRECATED: The PingGroupSummary model type no longer exists in the backend,
                the graph will instead show individual pings, to be done shortly.
*/

export const numberFormatter = (value: number): string => {
    if (Math.abs(value) >= 1_000_000_000) {
      return (value / 1_000_000_000).toFixed(1).replace(/\.0$/, '') + 'G';
    }
    if (Math.abs(value) >= 1_000_000) {
      return (value / 1_000_000).toFixed(1).replace(/\.0$/, '') + 'M';
    }
    if (Math.abs(value) >= 1_000) {
      return (value / 1_000).toFixed(1).replace(/\.0$/, '') + 'K';
    }
    return '' + value;
  };


var d = new Date();

Object.defineProperty(Date.prototype, 'subtractMinutes', {
    value: function (this : Date, minutes: number): Date {
      var newTicks = this!.getTime() - 60000*minutes;
      var newDate = new Date();
      newDate.setTime(newTicks);
      return newDate;
    }
});

Object.defineProperty(Date.prototype, 'addMinutes', {
  value: function (this : Date, minutes: number): Date {
    var newTicks = this!.getTime() + 60000*minutes;
    var newDate = new Date();
    newDate.setTime(newTicks);
    return newDate;
  }
});

Object.defineProperty(Date.prototype, 'toTimestamp', {
  value: function (this : Date): Timestamp {
    var ticks = this!.getTime();
    var tickDate = new Date()
    tickDate.setTime(ticks);

    return Timestamp.fromDate(tickDate);
  }
});

declare global{
  interface Date{
    subtractMinutes: (num: number) => Date;
    addMinutes: (num: number) => Date;
    toTimestamp: () => Timestamp;
  }
}


export default function SummariesGraph(){
  return <SummariesGraphManager/>
}

function SummariesGraphManager(){
  /*
  //const [pingInfo, setPingInfo] = useState<Array<PingGroupSummaryPublic>>([]);
  const [apiError, setApiError] = useState<Error>(undefined);
  const [loading, setLoading] = useState<boolean>(false);
  const [pingTarget, setPingTarget] = useState<string>("8.8.8.8");
  
  
  const client = useBackendClient();

  const loadPingInfo = useCallback((request: ShowPingsRequest) => {
    console.log("loadPingInfo entered");
    console.log(request);

    client?.showPings(request)
        .then((response: ShowPingsResponse) => {
            setLoading(true);
            console.log(response!.pings);
            setPingInfo(response!.pings);
            setApiError(undefined);  
        })
        .catch((error: Error) => {
            console.log(error);
            setApiError(error);
        })
        .finally(() => {
            setLoading(false);
        });
  }, [client, setPingInfo]);

  const eventHandler = useCallback((e: CustomEvent<ServerEvent_PingInfo>) => {
    var request = new ShowPingsRequest({ 
      target: new PingTarget({target: "8.8.8.8"}), 
      startingTime: Timestamp.fromDate(new Date().subtractMinutes(5)), 
      endingTime: new Date().toTimestamp()
    });
    loadPingInfo(request);
  
  }, [loadPingInfo, pingTarget]); 

  useServerEventListener("pinginfo", eventHandler);
  console.log('render');

  return <SummariesGraphDisplay pingTarget={pingTarget} data = {pingInfo} /> */
  return <div></div>
}

type showPingData = ShowPingsResponse['pings'];

type SummariesGraphDisplayProps = {
  pingTarget : string,
  data: Array<PingGroupSummaryPublic>;
};


function SummariesGraphDisplay(p : SummariesGraphDisplayProps){
  const graphData = p.data.map((datapoint) => ({
      x: new Date(datapoint.start.toDate()), 
      y: datapoint.maximumPing
    })
  );

  console.log(graphData);
  return (
    <LineChart
      series={[
        {
          title: "Site 1",
          type: "line",
          data: graphData,
          valueFormatter: function s(e: number) {
            return Math.abs(e) >= 1e9
              ? (e / 1e9).toFixed(1).replace(/\.0$/, "") +
                  "G"
              : Math.abs(e) >= 1e6
              ? (e / 1e6).toFixed(1).replace(/\.0$/, "") +
                "M"
              : Math.abs(e) >= 1e3
              ? (e / 1e3).toFixed(1).replace(/\.0$/, "") +
                "K"
              : e.toFixed(2);
          }
        },
        //{
          //title: "Peak hours",
          //type: "threshold",
          //x: new Date(1601025000000)
        //}
      ]}
      xDomain={
        graphData.length > 0 
          ? [new Date(graphData[0].x), new Date(graphData[-1].x)]
          : [new Date().subtractMinutes(5), new Date()]
      }
      yDomain={[0, 1500]} // this should dynamically pull from the timeOut property
      i18nStrings={{
        xTickFormatter: e =>
          e
            .toLocaleDateString("en-US", {
              month: "short",
              day: "numeric",
              hour: "numeric",
              minute: "numeric",
              hour12: !1
            })
            .split(",")
            .join("\n"),
        yTickFormatter: function s(e) {
          return e + "ms";
        }
      }}
      detailPopoverSeriesContent={({ series, x, y }) => ({
        key: (
          <Link external="true" href="#">
            {series.title}
          </Link>
        ),
        value: numberFormatter(y)
      })}
      ariaLabel="Single data series line chart"
      height={300}
      hideFilter
      hideLegend
      xScaleType="time"
      xTitle="Time (EST)"
      yTitle="MaximumPing"
      empty={
        <Box textAlign="center" color="inherit">
          <b>No data available</b>
          <Box variant="p" color="inherit">
            There is no data available
          </Box>
        </Box>
      }
      noMatch={
        <Box textAlign="center" color="inherit">
          <b>No matching data</b>
          <Box variant="p" color="inherit">
            There is no matching data to display
          </Box>
          <Button>Clear filter</Button>
        </Box>
      }
    />
  );
}