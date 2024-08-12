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
        readonly getAnomalies: {
            readonly name: "GetAnomalies";
            readonly I: typeof import("./gen/service_pb.js").PingTarget;
            readonly O: typeof import("./gen/service_pb.js").PingAnomalies;
            readonly kind: import("@bufbuild/protobuf").MethodKind.ServerStreaming;
        };
        readonly getPingingStatus: {
            readonly name: "GetPingingStatus";
            readonly I: typeof import("@bufbuild/protobuf").Empty;
            readonly O: typeof import("./gen/service_pb.js").PingStatusMessage;
            readonly kind: import("@bufbuild/protobuf").MethodKind.ServerStreaming;
        };
    };
}>;
export default createClient;
