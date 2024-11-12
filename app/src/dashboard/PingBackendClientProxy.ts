import type { Maybe } from "../types";
import { PingBoardService } from "client";
import { ServerEvent } from "client/dist/gen/service_pb";


// caller will be presented with something.apiName(request)
class PingBackendClientProxy {
  constructor() {
    return new Proxy(
      {},
      {
        get(self, apiName, _) {
          if (typeof apiName !== "string") {
            return Reflect.get(self, apiName);
          }

          if (apiName === "getLatestServerEvent") {
            return iterateServerEvents;
          }

          return async (requestObj: any) => {
            const response = (await (window as any).apiBridge.makeApiRequest(
              apiName,
              requestObj
            )) as Maybe<any>;

            console.log("PingBackendClientProxy.ts: ");
            console.log(response);

            if (response.successful) {
              return PingBoardService.methods[apiName as keyof typeof PingBoardService.methods].O.fromJson(response.result);
            } else if (response.successful === false) {
              throw response.error; 
            }
          };
        },
      }
    );
  }
}

async function* iterateServerEvents(abortSignal: AbortController["signal"]) {
  while (!abortSignal.aborted) {
    const se = await new Promise<any>((resolve) => {
      window.eventSubscriber.getServerEvents((se) => {
        resolve(se);
      });
    });
    yield ServerEvent.fromJson(se);
  }
}

export default PingBackendClientProxy;
