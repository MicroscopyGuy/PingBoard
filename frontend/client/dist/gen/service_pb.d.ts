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
     * @generated from field: optional int32 terminatingIPStatus = 9;
     */
    terminatingIPStatus?: number;
    /**
     * @generated from field: optional int32 lastAbnormalStatus = 10;
     */
    lastAbnormalStatus?: number;
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
 * @generated from message PingAnomalies
 */
export declare class PingAnomalies extends Message<PingAnomalies> {
    /**
     * @generated from field: repeated PingGroupSummaryPublic pingAnomaly = 1;
     */
    pingAnomaly: PingGroupSummaryPublic[];
    constructor(data?: PartialMessage<PingAnomalies>);
    static readonly runtime: typeof proto3;
    static readonly typeName = "PingAnomalies";
    static readonly fields: FieldList;
    static fromBinary(bytes: Uint8Array, options?: Partial<BinaryReadOptions>): PingAnomalies;
    static fromJson(jsonValue: JsonValue, options?: Partial<JsonReadOptions>): PingAnomalies;
    static fromJsonString(jsonString: string, options?: Partial<JsonReadOptions>): PingAnomalies;
    static equals(a: PingAnomalies | PlainMessage<PingAnomalies> | undefined, b: PingAnomalies | PlainMessage<PingAnomalies> | undefined): boolean;
}
