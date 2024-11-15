import { ListAnomaliesRequest, ListAnomaliesResponse, ListPingsRequest, ListPingsResponse, ServerEvent, ShowPingsRequest, ShowPingsResponse, StartPingingRequest } from "./service_pb.js";
import { Empty, MethodKind } from "@bufbuild/protobuf";
/**
 * @generated from service PingBoardService
 */
export declare const PingBoardService: {
    readonly typeName: "PingBoardService";
    readonly methods: {
        /**
         * @generated from rpc PingBoardService.StartPinging
         */
        readonly startPinging: {
            readonly name: "StartPinging";
            readonly I: typeof StartPingingRequest;
            readonly O: typeof Empty;
            readonly kind: MethodKind.Unary;
        };
        /**
         * @generated from rpc PingBoardService.StopPinging
         */
        readonly stopPinging: {
            readonly name: "StopPinging";
            readonly I: typeof Empty;
            readonly O: typeof Empty;
            readonly kind: MethodKind.Unary;
        };
        /**
         * @generated from rpc PingBoardService.GetLatestServerEvent
         */
        readonly getLatestServerEvent: {
            readonly name: "GetLatestServerEvent";
            readonly I: typeof Empty;
            readonly O: typeof ServerEvent;
            readonly kind: MethodKind.ServerStreaming;
        };
        /**
         * @generated from rpc PingBoardService.ListPings
         */
        readonly listPings: {
            readonly name: "ListPings";
            readonly I: typeof ListPingsRequest;
            readonly O: typeof ListPingsResponse;
            readonly kind: MethodKind.Unary;
        };
        /**
         * @generated from rpc PingBoardService.ListAnomalies
         */
        readonly listAnomalies: {
            readonly name: "ListAnomalies";
            readonly I: typeof ListAnomaliesRequest;
            readonly O: typeof ListAnomaliesResponse;
            readonly kind: MethodKind.Unary;
        };
        /**
         * @generated from rpc PingBoardService.ShowPings
         */
        readonly showPings: {
            readonly name: "ShowPings";
            readonly I: typeof ShowPingsRequest;
            readonly O: typeof ShowPingsResponse;
            readonly kind: MethodKind.Unary;
        };
    };
};
