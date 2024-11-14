import "@cloudscape-design/global-styles/index.css"
import LineChart from "@cloudscape-design/components/line-chart";
import Box from "@cloudscape-design/components/box";
import Button from "@cloudscape-design/components/button";
import Link from "@cloudscape-design/components/link";
import { PingGroupSummaryPublic, PingTarget, ServerEvent_PingInfo, ShowPingsRequest, ShowPingsResponse } from "client/dist/gen/service_pb";
import {useState, useEffect, useCallback} from 'react';
import { useBackendClient } from './PingBackendContext'
import { useServerEventListener } from "./ServerEventListener";

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

declare global{
  interface Date{
    subtractMinutes: (num: number) => Date;
    addMinutes: (num: number) => Date;
  }
}


function SummariesGraphManager(){
  const [pingInfo, setPingInfo] = useState([]);
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
    var request = { target: {target: pingTarget}, startingTime: new Date(), endingTime: new Date().subtractMinutes(5) } as any as ShowPingsRequest;
    client.showPings(request);
    loadPingInfo(request);
  }, [loadPingInfo, pingTarget]);

  useServerEventListener("pinginfo", eventHandler);
}

type SummariesGraphProps = {
  datapoints: Array<PingGroupSummaryPublic>
}

export default function SummariesGraph(){
  return (
    <LineChart
      series={[
        {
          title: "Site 1",
          type: "line",
          data: [
            { x: new Date(1601006400000), y: 58020 },
            { x: new Date(1601007300000), y: 102402 },
            { x: new Date(1601008200000), y: 104920 },
            { x: new Date(1601009100000), y: 94031 },
            { x: new Date(1601010000000), y: 125021 },
            { x: new Date(1601010900000), y: 159219 },
            { x: new Date(1601011800000), y: 193082 },
            { x: new Date(1601012700000), y: 162592 },
            { x: new Date(1601013600000), y: 274021 },
            { x: new Date(1601014500000), y: 264286 },
            { x: new Date(1601015400000), y: 289210 },
            { x: new Date(1601016300000), y: 256362 },
            { x: new Date(1601017200000), y: 257306 },
            { x: new Date(1601018100000), y: 186776 },
            { x: new Date(1601019000000), y: 294020 },
            { x: new Date(1601019900000), y: 385975 },
            { x: new Date(1601020800000), y: 486039 },
            { x: new Date(1601021700000), y: 490447 },
            { x: new Date(1601022600000), y: 361845 },
            { x: new Date(1601023500000), y: 339058 },
            { x: new Date(1601024400000), y: 298028 },
            { x: new Date(1601025300000), y: 231902 },
            { x: new Date(1601026200000), y: 224558 },
            { x: new Date(1601027100000), y: 253901 },
            { x: new Date(1601028000000), y: 102839 },
            { x: new Date(1601028900000), y: 234943 },
            { x: new Date(1601029800000), y: 204405 },
            { x: new Date(1601030700000), y: 190391 },
            { x: new Date(1601031600000), y: 183570 },
            { x: new Date(1601032500000), y: 162592 },
            { x: new Date(1601033400000), y: 148910 },
            { x: new Date(1601034300000), y: 229492 },
            { x: new Date(1601035200000), y: 293910 }
          ],
          valueFormatter: function s(e) {
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
        {
          title: "Peak hours",
          type: "threshold",
          x: new Date(1601025000000)
        }
      ]}
      xDomain={[
        new Date(1601006400000),
        new Date(1601035200000)
      ]}
      yDomain={[0, 500000]}
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
      xTitle="Time (UTC)"
      yTitle="Bytes transferred"
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