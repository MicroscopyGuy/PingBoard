import { createGrpcTransport } from "@connectrpc/connect-node";
import { createPromiseClient } from "@connectrpc/connect";
import { PingBoardService } from "client";

const createClient = (url: string) => {
    const transport = createGrpcTransport({
        httpVersion: "2",
        baseUrl: url, // not a real URL, socket is in nodeOptions.createConnection
    })
    return createPromiseClient(PingBoardService, transport);
}

type BackendClient = ReturnType<typeof createClient>;

export default createClient;
export type { BackendClient };