import type { BinaryReadOptions, FieldList, JsonReadOptions, JsonValue, PartialMessage, PlainMessage } from "@bufbuild/protobuf";
import { Message, proto3, Timestamp } from "@bufbuild/protobuf";
/**
 * @generated from enum Quantum
 */
export declare enum Quantum {
    /**
     * @generated from enum value: Length = 0;
     */
    Length = 0
}
/**
 * @generated from enum Metric
 */
export declare enum Metric {
    /**
     * @generated from enum value: LATENCY = 0;
     */
    LATENCY = 0,
    /**
     * @generated from enum value: PACKET_LOSS = 1;
     */
    PACKET_LOSS = 1,
    /**
     * @generated from enum value: JITTER = 2;
     */
    JITTER = 2
}
/**
 * @generated from enum Statistic
 */
export declare enum Statistic {
    /**
     * @generated from enum value: MIN = 0;
     */
    MIN = 0,
    /**
     * @generated from enum value: AVG = 1;
     */
    AVG = 1,
    /**
     * @generated from enum value: MAX = 2;
     */
    MAX = 2,
    /**
     * @generated from enum value: P90 = 3;
     */
    P90 = 3,
    /**
     * @generated from enum value: P99 = 4;
     */
    P99 = 4,
    /**
     * @generated from enum value: SUM = 5;
     */
    SUM = 5,
    /**
     * @generated from enum value: COUNT = 6;
     */
    COUNT = 6
}
/**
 * @generated from message StartPingingRequest
 */
export declare class StartPingingRequest extends Message<StartPingingRequest> {
    /**
     * @generated from field: PingTarget target = 1;
     */
    target?: PingTarget;
    constructor(data?: PartialMessage<StartPingingRequest>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "StartPingingRequest";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): StartPingingRequest;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): StartPingingRequest;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): StartPingingRequest;
    static equals(a: StartPingingRequest | PlainMessage<StartPingingRequest> | undefined, b: StartPingingRequest | PlainMessage<StartPingingRequest> | undefined): boolean;
}
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
 * @generated from message ShowPingsRequest
 */
export declare class ShowPingsRequest extends Message<ShowPingsRequest> {
    /**
     * @generated from field: PingTarget target = 1;
     */
    target?: PingTarget;
    /**
     * @generated from field: google.protobuf.Timestamp startingTime = 2;
     */
    startingTime?: Timestamp;
    /**
     * @generated from field: google.protobuf.Timestamp endingTime = 3;
     */
    endingTime?: Timestamp;
    constructor(data?: PartialMessage<ShowPingsRequest>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "ShowPingsRequest";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): ShowPingsRequest;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): ShowPingsRequest;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): ShowPingsRequest;
    static equals(a: ShowPingsRequest | PlainMessage<ShowPingsRequest> | undefined, b: ShowPingsRequest | PlainMessage<ShowPingsRequest> | undefined): boolean;
}
/**
 * @generated from message ShowPingsResponse
 */
export declare class ShowPingsResponse extends Message<ShowPingsResponse> {
    /**
     * @generated from field: repeated PingGroupSummaryPublic pings = 1;
     */
    pings: PingGroupSummaryPublic[];
    constructor(data?: PartialMessage<ShowPingsResponse>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "ShowPingsResponse";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): ShowPingsResponse;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): ShowPingsResponse;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): ShowPingsResponse;
    static equals(a: ShowPingsResponse | PlainMessage<ShowPingsResponse> | undefined, b: ShowPingsResponse | PlainMessage<ShowPingsResponse> | undefined): boolean;
}
/**
 * @generated from message ListPingsRequest
 */
export declare class ListPingsRequest extends Message<ListPingsRequest> {
    /**
     * @generated from field: google.protobuf.Timestamp startingTime = 1;
     */
    startingTime?: Timestamp;
    /**
     * @generated from field: google.protobuf.Timestamp endingTime = 2;
     */
    endingTime?: Timestamp;
    /**
     * @generated from field: PingTarget pingTarget = 4;
     */
    pingTarget?: PingTarget;
    /**
     * @generated from field: Metric metric = 5;
     */
    metric: Metric;
    /**
     * @generated from field: Statistic statistic = 6;
     */
    statistic: Statistic;
    /**
     * @generated from field: optional Quantum quantum = 7;
     */
    quantum?: Quantum;
    constructor(data?: PartialMessage<ListPingsRequest>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "ListPingsRequest";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): ListPingsRequest;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): ListPingsRequest;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): ListPingsRequest;
    static equals(a: ListPingsRequest | PlainMessage<ListPingsRequest> | undefined, b: ListPingsRequest | PlainMessage<ListPingsRequest> | undefined): boolean;
}
/**
 * @generated from message ListPingsDatapoint
 */
export declare class ListPingsDatapoint extends Message<ListPingsDatapoint> {
    /**
     * @generated from field: google.protobuf.Timestamp Timestamp = 1;
     */
    Timestamp?: Timestamp;
    /**
     * @generated from field: double Value = 2;
     */
    Value: number;
    constructor(data?: PartialMessage<ListPingsDatapoint>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "ListPingsDatapoint";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): ListPingsDatapoint;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): ListPingsDatapoint;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): ListPingsDatapoint;
    static equals(a: ListPingsDatapoint | PlainMessage<ListPingsDatapoint> | undefined, b: ListPingsDatapoint | PlainMessage<ListPingsDatapoint> | undefined): boolean;
}
/**
 * @generated from message ListPingsResponse
 */
export declare class ListPingsResponse extends Message<ListPingsResponse> {
    /**
     * @generated from field: repeated ListPingsDatapoint datapoints = 1;
     */
    datapoints: ListPingsDatapoint[];
    constructor(data?: PartialMessage<ListPingsResponse>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "ListPingsResponse";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): ListPingsResponse;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): ListPingsResponse;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): ListPingsResponse;
    static equals(a: ListPingsResponse | PlainMessage<ListPingsResponse> | undefined, b: ListPingsResponse | PlainMessage<ListPingsResponse> | undefined): boolean;
}
/**
 * @generated from message ListAnomaliesRequest
 */
export declare class ListAnomaliesRequest extends Message<ListAnomaliesRequest> {
    /**
     * @generated from field: uint32 numberRequested = 1;
     */
    numberRequested: number;
    /**
     * @generated from field: string paginationToken = 2;
     */
    paginationToken: string;
    /**
     * @generated from field: optional PingTarget pingTarget = 3;
     */
    pingTarget?: PingTarget;
    constructor(data?: PartialMessage<ListAnomaliesRequest>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "ListAnomaliesRequest";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): ListAnomaliesRequest;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): ListAnomaliesRequest;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): ListAnomaliesRequest;
    static equals(a: ListAnomaliesRequest | PlainMessage<ListAnomaliesRequest> | undefined, b: ListAnomaliesRequest | PlainMessage<ListAnomaliesRequest> | undefined): boolean;
}
/**
 * @generated from message ListAnomaliesResponse
 */
export declare class ListAnomaliesResponse extends Message<ListAnomaliesResponse> {
    /**
     * @generated from field: repeated PingGroupSummaryPublic anomalies = 1;
     */
    anomalies: PingGroupSummaryPublic[];
    /**
     * @generated from field: string paginationToken = 2;
     */
    paginationToken: string;
    constructor(data?: PartialMessage<ListAnomaliesResponse>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "ListAnomaliesResponse";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): ListAnomaliesResponse;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): ListAnomaliesResponse;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): ListAnomaliesResponse;
    static equals(a: ListAnomaliesResponse | PlainMessage<ListAnomaliesResponse> | undefined, b: ListAnomaliesResponse | PlainMessage<ListAnomaliesResponse> | undefined): boolean;
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
         * @generated from field: ServerEvent.PingInfo pingInfo = 102;
         */
        value: ServerEvent_PingInfo;
        case: "pingInfo";
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
/**
 * @generated from message ServerEvent.PingInfo
 */
export declare class ServerEvent_PingInfo extends Message<ServerEvent_PingInfo> {
    /**
     * @generated from field: PingTarget pingTarget = 1;
     */
    pingTarget?: PingTarget;
    constructor(data?: PartialMessage<ServerEvent_PingInfo>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "ServerEvent.PingInfo";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): ServerEvent_PingInfo;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): ServerEvent_PingInfo;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): ServerEvent_PingInfo;
    static equals(a: ServerEvent_PingInfo | PlainMessage<ServerEvent_PingInfo> | undefined, b: ServerEvent_PingInfo | PlainMessage<ServerEvent_PingInfo> | undefined): boolean;
}
