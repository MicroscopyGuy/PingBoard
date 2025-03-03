import { AnonymousAuthenticationProvider } from "@microsoft/kiota-abstractions";
import { FetchRequestAdapter } from "@microsoft/kiota-http-fetchlibrary";
import { createPingBoardClient } from "./pingBoardClient.js";

function createClient() {
    const authProvider = new AnonymousAuthenticationProvider();
    const adapter = new FetchRequestAdapter(authProvider);
    return createPingBoardClient(adapter);
}

export default createClient;
export * from "./gen/types.js"