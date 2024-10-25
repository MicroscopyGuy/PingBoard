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
        readonly listPings: {
            readonly name: "ListPings";
            readonly I: typeof import("./gen/service_pb.js").ListPingsRequest;
            readonly O: typeof import("./gen/service_pb.js").ListPingsResponse;
            readonly kind: import("@bufbuild/protobuf").MethodKind.Unary;
        };
        readonly listAnomalies: {
            readonly name: "ListAnomalies";
            readonly I: typeof import("./gen/service_pb.js").ListAnomaliesRequest;
            readonly O: typeof import("./gen/service_pb.js").ListAnomaliesResponse;
            readonly kind: import("@bufbuild/protobuf").MethodKind.Unary;
        };
    };
}>;
export default createClient;
