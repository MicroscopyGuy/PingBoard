import createClient from "client";

interface ApiBridge {
  makeApiRequest: <T extends keyof PromiseReturningBackendClient>(
    rpcName: T,
    path: Parameters<PromiseReturningBackendClient[T]>[0]
  ) => Promise<Maybe<Awaited<ReturnType<PromiseReturningBackendClient[T]>>>>;
}

type ServerEventCallback = (cb: ServerEvent) => void;
interface EventSubscriber {
  getServerEvents: (callback: ServerEventCallback) => void;
}

declare global {
  interface Window {
    apiBridge: ApiBridge;
    eventSubscriber: EventSubscriber;
  }
}