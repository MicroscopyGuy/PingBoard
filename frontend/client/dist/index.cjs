"use strict";
var __defProp = Object.defineProperty;
var __getOwnPropDesc = Object.getOwnPropertyDescriptor;
var __getOwnPropNames = Object.getOwnPropertyNames;
var __hasOwnProp = Object.prototype.hasOwnProperty;
var __export = (target, all) => {
  for (var name in all)
    __defProp(target, name, { get: all[name], enumerable: true });
};
var __copyProps = (to, from, except, desc) => {
  if (from && typeof from === "object" || typeof from === "function") {
    for (let key of __getOwnPropNames(from))
      if (!__hasOwnProp.call(to, key) && key !== except)
        __defProp(to, key, { get: () => from[key], enumerable: !(desc = __getOwnPropDesc(from, key)) || desc.enumerable });
  }
  return to;
};
var __toCommonJS = (mod) => __copyProps(__defProp({}, "__esModule", { value: true }), mod);

// src/index.ts
var index_exports = {};
__export(index_exports, {
  default: () => index_default
});
module.exports = __toCommonJS(index_exports);
var import_kiota_abstractions2 = require("@microsoft/kiota-abstractions");
var import_kiota_http_fetchlibrary = require("@microsoft/kiota-http-fetchlibrary");

// src/models/index.ts
function createProblemDetailsFromDiscriminatorValue(parseNode) {
  return deserializeIntoProblemDetails;
}
function deserializeIntoProblemDetails(problemDetails = {}) {
  return {
    "detail": (n) => {
      problemDetails.detail = n.getStringValue();
    },
    "instance": (n) => {
      problemDetails.instance = n.getStringValue();
    },
    "status": (n) => {
      problemDetails.status = n.getNumberValue();
    },
    "title": (n) => {
      problemDetails.title = n.getStringValue();
    },
    "type": (n) => {
      problemDetails.type = n.getStringValue();
    }
  };
}
function serializeIProbeBehavior(writer, iProbeBehavior = {}) {
  if (iProbeBehavior) {
    writer.writeStringValue("packetPayload", iProbeBehavior.packetPayload);
    writer.writeObjectValue("target", iProbeBehavior.target, serializeIProbeBehavior_target);
    writer.writeNumberValue("timeoutMs", iProbeBehavior.timeoutMs);
    writer.writeNumberValue("ttl", iProbeBehavior.ttl);
    writer.writeAdditionalData(iProbeBehavior.additionalData);
  }
}
function serializeIProbeBehavior_target(writer, iProbeBehavior_target = {}) {
  serializeIProbeBehavior_targetMember1(writer, iProbeBehavior_target);
  serializeIProbeBehavior_targetMember2(writer, iProbeBehavior_target);
}
function serializeIProbeBehavior_targetMember1(writer, iProbeBehavior_targetMember1 = {}) {
  if (iProbeBehavior_targetMember1) {
    writer.writeStringValue("ipAddress", iProbeBehavior_targetMember1.ipAddress);
    writer.writeEnumValue("targetType", iProbeBehavior_targetMember1.targetType);
    writer.writeAdditionalData(iProbeBehavior_targetMember1.additionalData);
  }
}
function serializeIProbeBehavior_targetMember2(writer, iProbeBehavior_targetMember2 = {}) {
  if (iProbeBehavior_targetMember2) {
    writer.writeStringValue("hostname", iProbeBehavior_targetMember2.hostname);
    writer.writeEnumValue("targetType", iProbeBehavior_targetMember2.targetType);
    writer.writeAdditionalData(iProbeBehavior_targetMember2.additionalData);
  }
}
function serializeIProbeThresholds(writer, iProbeThresholds = {}) {
  if (iProbeThresholds) {
    writer.writeAdditionalData(iProbeThresholds.additionalData);
  }
}
function serializeProbeConfigAggregate(writer, probeConfigAggregate = {}) {
  if (probeConfigAggregate) {
    writer.writeObjectValue("behavior", probeConfigAggregate.behavior, serializeIProbeBehavior);
    writer.writeStringValue("probeType", probeConfigAggregate.probeType);
    writer.writeObjectValue("schedule", probeConfigAggregate.schedule, serializeProbeSchedule);
    writer.writeObjectValue("thresholds", probeConfigAggregate.thresholds, serializeIProbeThresholds);
    writer.writeAdditionalData(probeConfigAggregate.additionalData);
  }
}
function serializeProbeSchedule(writer, probeSchedule = {}) {
  if (probeSchedule) {
    writer.writeStringValue("spread", probeSchedule.spread);
    writer.writeAdditionalData(probeSchedule.additionalData);
  }
}

// src/probes/index.ts
var ProbesRequestBuilderUriTemplate = "{+baseurl}/Probes";
var ProbesRequestBuilderRequestsMetadata = {
  delete: {
    uriTemplate: ProbesRequestBuilderUriTemplate,
    responseBodyContentType: "application/problem+json",
    errorMappings: {
      422: createProblemDetailsFromDiscriminatorValue,
      500: createProblemDetailsFromDiscriminatorValue
    },
    adapterMethodName: "sendNoResponseContent"
  },
  post: {
    uriTemplate: ProbesRequestBuilderUriTemplate,
    responseBodyContentType: "application/problem+json",
    errorMappings: {
      409: createProblemDetailsFromDiscriminatorValue
    },
    adapterMethodName: "sendNoResponseContent",
    requestBodyContentType: "application/json",
    requestBodySerializer: serializeProbeConfigAggregate,
    requestInformationContentSetMethod: "setContentFromParsable"
  }
};

// src/pingBoardClient.ts
var import_kiota_abstractions = require("@microsoft/kiota-abstractions");
var import_kiota_serialization_form = require("@microsoft/kiota-serialization-form");
var import_kiota_serialization_json = require("@microsoft/kiota-serialization-json");
var import_kiota_serialization_multipart = require("@microsoft/kiota-serialization-multipart");
var import_kiota_serialization_text = require("@microsoft/kiota-serialization-text");
function createPingBoardClient(requestAdapter) {
  (0, import_kiota_abstractions.registerDefaultSerializer)(import_kiota_serialization_json.JsonSerializationWriterFactory);
  (0, import_kiota_abstractions.registerDefaultSerializer)(import_kiota_serialization_text.TextSerializationWriterFactory);
  (0, import_kiota_abstractions.registerDefaultSerializer)(import_kiota_serialization_form.FormSerializationWriterFactory);
  (0, import_kiota_abstractions.registerDefaultSerializer)(import_kiota_serialization_multipart.MultipartSerializationWriterFactory);
  (0, import_kiota_abstractions.registerDefaultDeserializer)(import_kiota_serialization_json.JsonParseNodeFactory);
  (0, import_kiota_abstractions.registerDefaultDeserializer)(import_kiota_serialization_text.TextParseNodeFactory);
  (0, import_kiota_abstractions.registerDefaultDeserializer)(import_kiota_serialization_form.FormParseNodeFactory);
  const pathParameters = {
    "baseurl": requestAdapter.baseUrl
  };
  return (0, import_kiota_abstractions.apiClientProxifier)(requestAdapter, pathParameters, PingBoardClientNavigationMetadata, void 0);
}
var PingBoardClientNavigationMetadata = {
  probes: {
    requestsMetadata: ProbesRequestBuilderRequestsMetadata
  }
};

// src/gen/types.ts
var import_zod = require("zod");
var types_default = import_zod.z.object({
  ProbeAnomaly: import_zod.z.union([
    import_zod.z.object({
      ProbeId: import_zod.z.string().uuid(),
      ProbeType: import_zod.z.string(),
      Target: import_zod.z.string(),
      EventId: import_zod.z.string().uuid().optional(),
      EventTime: import_zod.z.string().datetime({ offset: true }).optional()
    }),
    import_zod.z.null()
  ]).optional(),
  ProbeError: import_zod.z.union([
    import_zod.z.object({
      ProbeId: import_zod.z.string().uuid(),
      ProbeType: import_zod.z.string(),
      ErrorDescription: import_zod.z.string(),
      Target: import_zod.z.string(),
      EventId: import_zod.z.string().uuid().optional(),
      EventTime: import_zod.z.string().datetime({ offset: true }).optional()
    }),
    import_zod.z.null()
  ]).optional(),
  ProbeStatus: import_zod.z.union([
    import_zod.z.object({
      ProbeId: import_zod.z.string().uuid(),
      Status: import_zod.z.string(),
      Target: import_zod.z.string(),
      ProbeType: import_zod.z.string(),
      EventId: import_zod.z.string().uuid().optional(),
      EventTime: import_zod.z.string().datetime({ offset: true }).optional()
    }),
    import_zod.z.null()
  ]).optional(),
  ProbeInfo: import_zod.z.union([
    import_zod.z.object({
      ProbeId: import_zod.z.string().uuid(),
      Target: import_zod.z.string(),
      ProbeType: import_zod.z.string(),
      EventId: import_zod.z.string().uuid().optional(),
      EventTime: import_zod.z.string().datetime({ offset: true }).optional()
    }),
    import_zod.z.null()
  ]).optional(),
  AdminError: import_zod.z.union([
    import_zod.z.object({
      ErrorDescription: import_zod.z.string(),
      EventId: import_zod.z.string().uuid().optional(),
      EventTime: import_zod.z.string().datetime({ offset: true }).optional()
    }),
    import_zod.z.null()
  ]).optional()
});

// src/index.ts
function createClient() {
  const authProvider = new import_kiota_abstractions2.AnonymousAuthenticationProvider();
  const adapter = new import_kiota_http_fetchlibrary.FetchRequestAdapter(authProvider);
  return createPingBoardClient(adapter);
}
var index_default = createClient;
//# sourceMappingURL=index.cjs.map