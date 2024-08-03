import { createPromiseClient } from "@connectrpc/connect";
import { PingBoardService } from "./gen/service_connect.js";
import { createGrpcWebTransport } from "@connectrpc/connect-web";
const createClient = (url) => {
    const transport = createGrpcWebTransport({
        baseUrl: url
    });
    return createPromiseClient(PingBoardService, transport);
};
export default createClient;
