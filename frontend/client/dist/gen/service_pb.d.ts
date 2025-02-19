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
 * @generated from message StartProbingRequest
 */
export declare class StartProbingRequest extends Message<StartProbingRequest> {
    /**
     * @generated from field: string requestJson = 1;
     */
    requestJson: string;
    constructor(data?: PartialMessage<StartProbingRequest>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "StartProbingRequest";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): StartProbingRequest;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): StartProbingRequest;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): StartProbingRequest;
    static equals(a: StartProbingRequest | PlainMessage<StartProbingRequest> | undefined, b: StartProbingRequest | PlainMessage<StartProbingRequest> | undefined): boolean;
}
/**
 * @generated from message StopProbingRequest
 */
export declare class StopProbingRequest extends Message<StopProbingRequest> {
    /**
     * @generated from field: string requestJson = 1;
     */
    requestJson: string;
    constructor(data?: PartialMessage<StopProbingRequest>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "StopProbingRequest";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): StopProbingRequest;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): StopProbingRequest;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): StopProbingRequest;
    static equals(a: StopProbingRequest | PlainMessage<StopProbingRequest> | undefined, b: StopProbingRequest | PlainMessage<StopProbingRequest> | undefined): boolean;
}
/**
 * @generated from message TracerouteTarget
 */
export declare class TracerouteTarget extends Message<TracerouteTarget> {
    /**
     * @generated from field: string target = 1;
     */
    target: string;
    constructor(data?: PartialMessage<TracerouteTarget>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "TracerouteTarget";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): TracerouteTarget;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): TracerouteTarget;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): TracerouteTarget;
    static equals(a: TracerouteTarget | PlainMessage<TracerouteTarget> | undefined, b: TracerouteTarget | PlainMessage<TracerouteTarget> | undefined): boolean;
}
/**
 * @generated from message DnsTarget
 */
export declare class DnsTarget extends Message<DnsTarget> {
    /**
     * @generated from field: string target = 1;
     */
    target: string;
    constructor(data?: PartialMessage<DnsTarget>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "DnsTarget";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): DnsTarget;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): DnsTarget;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): DnsTarget;
    static equals(a: DnsTarget | PlainMessage<DnsTarget> | undefined, b: DnsTarget | PlainMessage<DnsTarget> | undefined): boolean;
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
 * @generated from message PingResultPublic
 */
export declare class PingResultPublic extends Message<PingResultPublic> {
    /**
     * @generated from field: google.protobuf.Timestamp start = 1;
     */
    start?: Timestamp;
    /**
     * @generated from field: google.protobuf.Timestamp end = 2;
     */
    end?: Timestamp;
    /**
     * @generated from field: int32 rtt = 3;
     */
    rtt: number;
    /**
     * @generated from field: string target = 4;
     */
    target: string;
    /**
     * @generated from field: string ipStatus = 5;
     */
    ipStatus: string;
    /**
     * @generated from field: int32 ttl = 6;
     */
    ttl: number;
    /**
     * @generated from field: string replyAddress = 7;
     */
    replyAddress: string;
    /**
     * @generated from field: string ipStatusShortMeaning = 8;
     */
    ipStatusShortMeaning: string;
    /**
     * @generated from field: string ipStatusOfficialMeaning = 9;
     */
    ipStatusOfficialMeaning: string;
    /**
     * @generated from field: string id = 10;
     */
    id: string;
    constructor(data?: PartialMessage<PingResultPublic>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "PingResultPublic";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): PingResultPublic;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): PingResultPublic;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): PingResultPublic;
    static equals(a: PingResultPublic | PlainMessage<PingResultPublic> | undefined, b: PingResultPublic | PlainMessage<PingResultPublic> | undefined): boolean;
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
     * @generated from field: repeated PingResultPublic anomalies = 1;
     */
    anomalies: PingResultPublic[];
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
         * @generated from field: ServerEvent.PingAgentError pingAgentError = 109;
         */
        value: ServerEvent_PingAgentError;
        case: "pingAgentError";
    } | {
        /**
         * @generated from field: ServerEvent.DnsOnOffToggle dnsOnOffToggle = 110;
         */
        value: ServerEvent_DnsOnOffToggle;
        case: "dnsOnOffToggle";
    } | {
        /**
         * @generated from field: ServerEvent.DnsAnomaly dnsAnomaly = 111;
         */
        value: ServerEvent_DnsAnomaly;
        case: "dnsAnomaly";
    } | {
        /**
         * @generated from field: ServerEvent.DnsInfo dnsInfo = 112;
         */
        value: ServerEvent_DnsInfo;
        case: "dnsInfo";
    } | {
        /**
         * @generated from field: ServerEvent.DnsAgentError dnsAgentError = 119;
         */
        value: ServerEvent_DnsAgentError;
        case: "dnsAgentError";
    } | {
        /**
         * @generated from field: ServerEvent.TracerouteOnOffToggle tracerouteOnOffToggle = 120;
         */
        value: ServerEvent_TracerouteOnOffToggle;
        case: "tracerouteOnOffToggle";
    } | {
        /**
         * @generated from field: ServerEvent.TracerouteAnomaly tracerouteAnomaly = 121;
         */
        value: ServerEvent_TracerouteAnomaly;
        case: "tracerouteAnomaly";
    } | {
        /**
         * @generated from field: ServerEvent.TracerouteInfo tracerouteInfo = 122;
         */
        value: ServerEvent_TracerouteInfo;
        case: "tracerouteInfo";
    } | {
        /**
         * @generated from field: ServerEvent.TracerouteAgentError tracerouteAgentError = 129;
         */
        value: ServerEvent_TracerouteAgentError;
        case: "tracerouteAgentError";
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
/**
 * **************** DNS related information *******************
 *
 * @generated from message ServerEvent.DnsOnOffToggle
 */
export declare class ServerEvent_DnsOnOffToggle extends Message<ServerEvent_DnsOnOffToggle> {
    /**
     * @generated from field: DnsTarget dnsTarget = 1;
     */
    dnsTarget?: DnsTarget;
    /**
     * @generated from field: bool active = 2;
     */
    active: boolean;
    constructor(data?: PartialMessage<ServerEvent_DnsOnOffToggle>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "ServerEvent.DnsOnOffToggle";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): ServerEvent_DnsOnOffToggle;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): ServerEvent_DnsOnOffToggle;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): ServerEvent_DnsOnOffToggle;
    static equals(a: ServerEvent_DnsOnOffToggle | PlainMessage<ServerEvent_DnsOnOffToggle> | undefined, b: ServerEvent_DnsOnOffToggle | PlainMessage<ServerEvent_DnsOnOffToggle> | undefined): boolean;
}
/**
 * @generated from message ServerEvent.DnsAnomaly
 */
export declare class ServerEvent_DnsAnomaly extends Message<ServerEvent_DnsAnomaly> {
    /**
     * @generated from field: DnsTarget pingTarget = 1;
     */
    pingTarget?: DnsTarget;
    /**
     * @generated from field: string anomalyDescription = 2;
     */
    anomalyDescription: string;
    constructor(data?: PartialMessage<ServerEvent_DnsAnomaly>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "ServerEvent.DnsAnomaly";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): ServerEvent_DnsAnomaly;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): ServerEvent_DnsAnomaly;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): ServerEvent_DnsAnomaly;
    static equals(a: ServerEvent_DnsAnomaly | PlainMessage<ServerEvent_DnsAnomaly> | undefined, b: ServerEvent_DnsAnomaly | PlainMessage<ServerEvent_DnsAnomaly> | undefined): boolean;
}
/**
 * @generated from message ServerEvent.DnsAgentError
 */
export declare class ServerEvent_DnsAgentError extends Message<ServerEvent_DnsAgentError> {
    /**
     * @generated from field: DnsTarget pingTarget = 1;
     */
    pingTarget?: DnsTarget;
    /**
     * @generated from field: string errorDescription = 2;
     */
    errorDescription: string;
    constructor(data?: PartialMessage<ServerEvent_DnsAgentError>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "ServerEvent.DnsAgentError";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): ServerEvent_DnsAgentError;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): ServerEvent_DnsAgentError;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): ServerEvent_DnsAgentError;
    static equals(a: ServerEvent_DnsAgentError | PlainMessage<ServerEvent_DnsAgentError> | undefined, b: ServerEvent_DnsAgentError | PlainMessage<ServerEvent_DnsAgentError> | undefined): boolean;
}
/**
 * @generated from message ServerEvent.DnsInfo
 */
export declare class ServerEvent_DnsInfo extends Message<ServerEvent_DnsInfo> {
    /**
     * @generated from field: DnsTarget pingTarget = 1;
     */
    pingTarget?: DnsTarget;
    constructor(data?: PartialMessage<ServerEvent_DnsInfo>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "ServerEvent.DnsInfo";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): ServerEvent_DnsInfo;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): ServerEvent_DnsInfo;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): ServerEvent_DnsInfo;
    static equals(a: ServerEvent_DnsInfo | PlainMessage<ServerEvent_DnsInfo> | undefined, b: ServerEvent_DnsInfo | PlainMessage<ServerEvent_DnsInfo> | undefined): boolean;
}
/**
 * @generated from message ServerEvent.TracerouteOnOffToggle
 */
export declare class ServerEvent_TracerouteOnOffToggle extends Message<ServerEvent_TracerouteOnOffToggle> {
    /**
     * @generated from field: TracerouteTarget tracerouteTarget = 1;
     */
    tracerouteTarget?: TracerouteTarget;
    /**
     * @generated from field: bool active = 2;
     */
    active: boolean;
    constructor(data?: PartialMessage<ServerEvent_TracerouteOnOffToggle>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "ServerEvent.TracerouteOnOffToggle";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): ServerEvent_TracerouteOnOffToggle;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): ServerEvent_TracerouteOnOffToggle;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): ServerEvent_TracerouteOnOffToggle;
    static equals(a: ServerEvent_TracerouteOnOffToggle | PlainMessage<ServerEvent_TracerouteOnOffToggle> | undefined, b: ServerEvent_TracerouteOnOffToggle | PlainMessage<ServerEvent_TracerouteOnOffToggle> | undefined): boolean;
}
/**
 * @generated from message ServerEvent.TracerouteAnomaly
 */
export declare class ServerEvent_TracerouteAnomaly extends Message<ServerEvent_TracerouteAnomaly> {
    /**
     * @generated from field: TracerouteTarget tracerouteTarget = 1;
     */
    tracerouteTarget?: TracerouteTarget;
    /**
     * @generated from field: string anomalyDescription = 2;
     */
    anomalyDescription: string;
    constructor(data?: PartialMessage<ServerEvent_TracerouteAnomaly>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "ServerEvent.TracerouteAnomaly";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): ServerEvent_TracerouteAnomaly;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): ServerEvent_TracerouteAnomaly;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): ServerEvent_TracerouteAnomaly;
    static equals(a: ServerEvent_TracerouteAnomaly | PlainMessage<ServerEvent_TracerouteAnomaly> | undefined, b: ServerEvent_TracerouteAnomaly | PlainMessage<ServerEvent_TracerouteAnomaly> | undefined): boolean;
}
/**
 * @generated from message ServerEvent.TracerouteAgentError
 */
export declare class ServerEvent_TracerouteAgentError extends Message<ServerEvent_TracerouteAgentError> {
    /**
     * @generated from field: TracerouteTarget TracerouteTarget = 1;
     */
    TracerouteTarget?: TracerouteTarget;
    /**
     * @generated from field: string errorDescription = 2;
     */
    errorDescription: string;
    constructor(data?: PartialMessage<ServerEvent_TracerouteAgentError>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "ServerEvent.TracerouteAgentError";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): ServerEvent_TracerouteAgentError;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): ServerEvent_TracerouteAgentError;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): ServerEvent_TracerouteAgentError;
    static equals(a: ServerEvent_TracerouteAgentError | PlainMessage<ServerEvent_TracerouteAgentError> | undefined, b: ServerEvent_TracerouteAgentError | PlainMessage<ServerEvent_TracerouteAgentError> | undefined): boolean;
}
/**
 * @generated from message ServerEvent.TracerouteInfo
 */
export declare class ServerEvent_TracerouteInfo extends Message<ServerEvent_TracerouteInfo> {
    /**
     * @generated from field: TracerouteTarget tracerouteTarget = 1;
     */
    tracerouteTarget?: TracerouteTarget;
    constructor(data?: PartialMessage<ServerEvent_TracerouteInfo>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "ServerEvent.TracerouteInfo";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): ServerEvent_TracerouteInfo;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): ServerEvent_TracerouteInfo;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): ServerEvent_TracerouteInfo;
    static equals(a: ServerEvent_TracerouteInfo | PlainMessage<ServerEvent_TracerouteInfo> | undefined, b: ServerEvent_TracerouteInfo | PlainMessage<ServerEvent_TracerouteInfo> | undefined): boolean;
}
