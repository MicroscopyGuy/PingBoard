import type { BinaryReadOptions, FieldList, JsonReadOptions, JsonValue, PartialMessage, PlainMessage } from "@bufbuild/protobuf";
import { Message, proto3, Timestamp } from "@bufbuild/protobuf";
/**
 * @generated from message PingTarget
 */
export declare class PingTarget extends Message<PingTarget> {
    /**
     * @generated from field: string target = 1;
     */
    target: string;
    constructor(data?: PartialMessage<PingTarget>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "PingTarget";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): PingTarget;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): PingTarget;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): PingTarget;
    static equals(a: PingTarget | PlainMessage<PingTarget> | undefined, b: PingTarget | PlainMessage<PingTarget> | undefined): boolean;
}
/**
 * @generated from message PingGroupSummaryPublic
 */
export declare class PingGroupSummaryPublic extends Message<PingGroupSummaryPublic> {
    /**
     * @generated from field: google.protobuf.Timestamp start = 1;
     */
    start?: Timestamp;
    /**
     * @generated from field: google.protobuf.Timestamp end = 2;
     */
    end?: Timestamp;
    /**
     * @generated from field: string target = 3;
     */
    target: string;
    /**
     * @generated from field: int32 minimumPing = 4;
     */
    minimumPing: number;
    /**
     * @generated from field: float averagePing = 5;
     */
    averagePing: number;
    /**
     * @generated from field: int32 maximumPing = 6;
     */
    maximumPing: number;
    /**
     * @generated from field: float jitter = 7;
     */
    jitter: number;
    /**
     * @generated from field: float packetLoss = 8;
     */
    packetLoss: number;
    /**
     * @generated from field: optional string terminatingIPStatusExplanation = 9;
     */
    terminatingIPStatusExplanation?: string;
    /**
     * @generated from field: optional string lastAbnormalStatusExplanation = 10;
     */
    lastAbnormalStatusExplanation?: string;
    constructor(data?: PartialMessage<PingGroupSummaryPublic>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "PingGroupSummaryPublic";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): PingGroupSummaryPublic;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): PingGroupSummaryPublic;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): PingGroupSummaryPublic;
    static equals(a: PingGroupSummaryPublic | PlainMessage<PingGroupSummaryPublic> | undefined, b: PingGroupSummaryPublic | PlainMessage<PingGroupSummaryPublic> | undefined): boolean;
}
/**
 * @generated from message ServerEvent
 */
export declare class ServerEvent extends Message<ServerEvent> {
    /**
     * @generated from field: google.protobuf.Timestamp eventTime = 1;
     */
    eventTime?: Timestamp;
    /**
     * @generated from oneof ServerEvent.ServerEvent
     */
    ServerEvent: {
        /**
         * @generated from field: ServerEvent.PingOnOffToggle pingOnOffToggle = 100;
         */
        value: ServerEvent_PingOnOffToggle;
        case: "pingOnOffToggle";
    } | {
        /**
         * @generated from field: ServerEvent.PingAnomaly pingAnomaly = 101;
         */
        value: ServerEvent_PingAnomaly;
        case: "pingAnomaly";
    } | {
        /**
         * @generated from field: ServerEvent.PingAgentError pingAgentError = 190;
         */
        value: ServerEvent_PingAgentError;
        case: "pingAgentError";
    } | {
        case: undefined;
        value?: undefined;
    };
    constructor(data?: PartialMessage<ServerEvent>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "ServerEvent";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): ServerEvent;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): ServerEvent;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): ServerEvent;
    static equals(a: ServerEvent | PlainMessage<ServerEvent> | undefined, b: ServerEvent | PlainMessage<ServerEvent> | undefined): boolean;
}
/**
 * @generated from message ServerEvent.PingOnOffToggle
 */
export declare class ServerEvent_PingOnOffToggle extends Message<ServerEvent_PingOnOffToggle> {
    /**
     * @generated from field: PingTarget pingTarget = 1;
     */
    pingTarget?: PingTarget;
    /**
     * @generated from field: bool active = 2;
     */
    active: boolean;
    constructor(data?: PartialMessage<ServerEvent_PingOnOffToggle>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "ServerEvent.PingOnOffToggle";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): ServerEvent_PingOnOffToggle;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): ServerEvent_PingOnOffToggle;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): ServerEvent_PingOnOffToggle;
    static equals(a: ServerEvent_PingOnOffToggle | PlainMessage<ServerEvent_PingOnOffToggle> | undefined, b: ServerEvent_PingOnOffToggle | PlainMessage<ServerEvent_PingOnOffToggle> | undefined): boolean;
}
/**
 * @generated from message ServerEvent.PingAnomaly
 */
export declare class ServerEvent_PingAnomaly extends Message<ServerEvent_PingAnomaly> {
    /**
     * @generated from field: PingTarget pingTarget = 1;
     */
    pingTarget?: PingTarget;
    /**
     * @generated from field: string anomalyDescription = 2;
     */
    anomalyDescription: string;
    constructor(data?: PartialMessage<ServerEvent_PingAnomaly>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "ServerEvent.PingAnomaly";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): ServerEvent_PingAnomaly;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): ServerEvent_PingAnomaly;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): ServerEvent_PingAnomaly;
    static equals(a: ServerEvent_PingAnomaly | PlainMessage<ServerEvent_PingAnomaly> | undefined, b: ServerEvent_PingAnomaly | PlainMessage<ServerEvent_PingAnomaly> | undefined): boolean;
}
/**
 * @generated from message ServerEvent.PingAgentError
 */
export declare class ServerEvent_PingAgentError extends Message<ServerEvent_PingAgentError> {
    /**
     * @generated from field: PingTarget pingTarget = 1;
     */
    pingTarget?: PingTarget;
    /**
     * @generated from field: string errorDescription = 2;
     */
    errorDescription: string;
    constructor(data?: PartialMessage<ServerEvent_PingAgentError>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "ServerEvent.PingAgentError";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): ServerEvent_PingAgentError;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): ServerEvent_PingAgentError;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): ServerEvent_PingAgentError;
    static equals(a: ServerEvent_PingAgentError | PlainMessage<ServerEvent_PingAgentError> | undefined, b: ServerEvent_PingAgentError | PlainMessage<ServerEvent_PingAgentError> | undefined): boolean;
}
