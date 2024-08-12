// @generated by protoc-gen-es v1.10.0 with parameter "target=ts"
// @generated from file service.proto (syntax proto3)
/* eslint-disable */
// @ts-nocheck
import { Message, proto3, Timestamp } from "@bufbuild/protobuf";
/**
 * @generated from message PingTarget
 */
export class PingTarget extends Message {
    constructor(data) {
        super();
        /**
         * @generated from field: string target = 1;
         */
        this.target = "";
        proto3.util.initPartial(data, this);
    }
    static fromBinary(bytes, options) {
        return new PingTarget().fromBinary(bytes, options);
    }
    static fromJson(jsonValue, options) {
        return new PingTarget().fromJson(jsonValue, options);
    }
    static fromJsonString(jsonString, options) {
        return new PingTarget().fromJsonString(jsonString, options);
    }
    static equals(a, b) {
        return proto3.util.equals(PingTarget, a, b);
    }
}
PingTarget.runtime = proto3;
PingTarget.typeName = "PingTarget";
PingTarget.fields = proto3.util.newFieldList(() => [
    { no: 1, name: "target", kind: "scalar", T: 9 /* ScalarType.STRING */ },
]);
/**
 * @generated from message PingGroupSummaryPublic
 */
export class PingGroupSummaryPublic extends Message {
    constructor(data) {
        super();
        /**
         * @generated from field: string target = 3;
         */
        this.target = "";
        /**
         * @generated from field: int32 minimumPing = 4;
         */
        this.minimumPing = 0;
        /**
         * @generated from field: float averagePing = 5;
         */
        this.averagePing = 0;
        /**
         * @generated from field: int32 maximumPing = 6;
         */
        this.maximumPing = 0;
        /**
         * @generated from field: float jitter = 7;
         */
        this.jitter = 0;
        /**
         * @generated from field: float packetLoss = 8;
         */
        this.packetLoss = 0;
        proto3.util.initPartial(data, this);
    }
    static fromBinary(bytes, options) {
        return new PingGroupSummaryPublic().fromBinary(bytes, options);
    }
    static fromJson(jsonValue, options) {
        return new PingGroupSummaryPublic().fromJson(jsonValue, options);
    }
    static fromJsonString(jsonString, options) {
        return new PingGroupSummaryPublic().fromJsonString(jsonString, options);
    }
    static equals(a, b) {
        return proto3.util.equals(PingGroupSummaryPublic, a, b);
    }
}
PingGroupSummaryPublic.runtime = proto3;
PingGroupSummaryPublic.typeName = "PingGroupSummaryPublic";
PingGroupSummaryPublic.fields = proto3.util.newFieldList(() => [
    { no: 1, name: "start", kind: "message", T: Timestamp },
    { no: 2, name: "end", kind: "message", T: Timestamp },
    { no: 3, name: "target", kind: "scalar", T: 9 /* ScalarType.STRING */ },
    { no: 4, name: "minimumPing", kind: "scalar", T: 5 /* ScalarType.INT32 */ },
    { no: 5, name: "averagePing", kind: "scalar", T: 2 /* ScalarType.FLOAT */ },
    { no: 6, name: "maximumPing", kind: "scalar", T: 5 /* ScalarType.INT32 */ },
    { no: 7, name: "jitter", kind: "scalar", T: 2 /* ScalarType.FLOAT */ },
    { no: 8, name: "packetLoss", kind: "scalar", T: 2 /* ScalarType.FLOAT */ },
    { no: 9, name: "terminatingIPStatus", kind: "scalar", T: 5 /* ScalarType.INT32 */, opt: true },
    { no: 10, name: "lastAbnormalStatus", kind: "scalar", T: 5 /* ScalarType.INT32 */, opt: true },
]);
/**
 * @generated from message PingAnomalies
 */
export class PingAnomalies extends Message {
    constructor(data) {
        super();
        /**
         * @generated from field: repeated PingGroupSummaryPublic pingAnomaly = 1;
         */
        this.pingAnomaly = [];
        proto3.util.initPartial(data, this);
    }
    static fromBinary(bytes, options) {
        return new PingAnomalies().fromBinary(bytes, options);
    }
    static fromJson(jsonValue, options) {
        return new PingAnomalies().fromJson(jsonValue, options);
    }
    static fromJsonString(jsonString, options) {
        return new PingAnomalies().fromJsonString(jsonString, options);
    }
    static equals(a, b) {
        return proto3.util.equals(PingAnomalies, a, b);
    }
}
PingAnomalies.runtime = proto3;
PingAnomalies.typeName = "PingAnomalies";
PingAnomalies.fields = proto3.util.newFieldList(() => [
    { no: 1, name: "pingAnomaly", kind: "message", T: PingGroupSummaryPublic, repeated: true },
]);
/**
 * @generated from message PingStatusMessage
 */
export class PingStatusMessage extends Message {
    constructor(data) {
        super();
        /**
         * @generated from field: bool active = 2;
         */
        this.active = false;
        proto3.util.initPartial(data, this);
    }
    static fromBinary(bytes, options) {
        return new PingStatusMessage().fromBinary(bytes, options);
    }
    static fromJson(jsonValue, options) {
        return new PingStatusMessage().fromJson(jsonValue, options);
    }
    static fromJsonString(jsonString, options) {
        return new PingStatusMessage().fromJsonString(jsonString, options);
    }
    static equals(a, b) {
        return proto3.util.equals(PingStatusMessage, a, b);
    }
}
PingStatusMessage.runtime = proto3;
PingStatusMessage.typeName = "PingStatusMessage";
PingStatusMessage.fields = proto3.util.newFieldList(() => [
    { no: 1, name: "pingTarget", kind: "message", T: PingTarget },
    { no: 2, name: "active", kind: "scalar", T: 8 /* ScalarType.BOOL */ },
]);
