declare const createClient: (url: string) => import("@connectrpc/connect").PromiseClient<{
    readonly typeName: "PingBoardService";
    readonly methods: {
        readonly startPinging: {
            readonly name: "StartPinging";
            readonly I: typeof import("./gen/service_pb.js").PingTarget;
            readonly O: typeof import("@bufbuild/protobuf").Empty;
            readonly kind: import("@bufbuild/protobuf").MethodKind.Unary;
        };
        readonly stopPinging: {
            readonly name: "StopPinging";
            readonly I: typeof import("@bufbuild/protobuf").Empty;
            readonly O: typeof import("@bufbuild/protobuf").Empty;
            readonly kind: import("@bufbuild/protobuf").MethodKind.Unary;
        };
        readonly getLatestServerEvent: {
            readonly name: "GetLatestServerEvent";
            readonly I: typeof import("@bufbuild/protobuf").Empty;
            readonly O: typeof import("./gen/service_pb.js").ServerEvent;
            readonly kind: import("@bufbuild/protobuf").MethodKind.ServerStreaming;
        };
    };
}>;
export default createClient;
