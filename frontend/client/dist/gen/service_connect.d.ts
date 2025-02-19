import { ListAnomaliesRequest, ListAnomaliesResponse, ServerEvent, StartPingingRequest, StartProbingRequest, StopProbingRequest } from "./service_pb.js";
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
         * @generated from rpc PingBoardService.ListAnomalies
         */
        readonly listAnomalies: {
            readonly name: "ListAnomalies";
            readonly I: typeof ListAnomaliesRequest;
            readonly O: typeof ListAnomaliesResponse;
            readonly kind: MethodKind.Unary;
        };
        /**
         * @generated from rpc PingBoardService.StartProbing
         */
        readonly startProbing: {
            readonly name: "StartProbing";
            readonly I: typeof StartProbingRequest;
            readonly O: typeof Empty;
            readonly kind: MethodKind.Unary;
        };
        /**
         * @generated from rpc PingBoardService.StopProbing
         */
        readonly stopProbing: {
            readonly name: "StopProbing";
            readonly I: typeof StopProbingRequest;
            readonly O: typeof Empty;
            readonly kind: MethodKind.Unary;
        };
    };
};
