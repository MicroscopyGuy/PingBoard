import { PingAnomalies, PingStatusMessage, PingTarget } from "./service_pb.js";
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
            readonly I: typeof PingTarget;
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
         * @generated from rpc PingBoardService.GetAnomalies
         */
        readonly getAnomalies: {
            readonly name: "GetAnomalies";
            readonly I: typeof PingTarget;
            readonly O: typeof PingAnomalies;
            readonly kind: MethodKind.ServerStreaming;
        };
        /**
         * @generated from rpc PingBoardService.GetPingingStatus
         */
        readonly getPingingStatus: {
            readonly name: "GetPingingStatus";
            readonly I: typeof Empty;
            readonly O: typeof PingStatusMessage;
            readonly kind: MethodKind.ServerStreaming;
        };
    };
};
