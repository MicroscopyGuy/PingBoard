import { PingTarget, ServerEvent } from "./service_pb.js";
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
         * rpc GetAnomalies (PingTarget) returns (PingAnomalies);
         * rpc GetPingingStatus(google.protobuf.Empty) returns (stream PingStatusMessage);
         * rpc GetAnomalyNotification(google.protobuf.Empty) returns (stream AnomalyNotification);
         *
         * @generated from rpc PingBoardService.GetLatestServerEvent
         */
        readonly getLatestServerEvent: {
            readonly name: "GetLatestServerEvent";
            readonly I: typeof Empty;
            readonly O: typeof ServerEvent;
            readonly kind: MethodKind.ServerStreaming;
        };
    };
};
