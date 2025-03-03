// src/index.ts
import { AnonymousAuthenticationProvider } from "@microsoft/kiota-abstractions";
import { FetchRequestAdapter } from "@microsoft/kiota-http-fetchlibrary";

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
import { apiClientProxifier, registerDefaultDeserializer, registerDefaultSerializer } from "@microsoft/kiota-abstractions";
import { FormParseNodeFactory, FormSerializationWriterFactory } from "@microsoft/kiota-serialization-form";
import { JsonParseNodeFactory, JsonSerializationWriterFactory } from "@microsoft/kiota-serialization-json";
import { MultipartSerializationWriterFactory } from "@microsoft/kiota-serialization-multipart";
import { TextParseNodeFactory, TextSerializationWriterFactory } from "@microsoft/kiota-serialization-text";
function createPingBoardClient(requestAdapter) {
  registerDefaultSerializer(JsonSerializationWriterFactory);
  registerDefaultSerializer(TextSerializationWriterFactory);
  registerDefaultSerializer(FormSerializationWriterFactory);
  registerDefaultSerializer(MultipartSerializationWriterFactory);
  registerDefaultDeserializer(JsonParseNodeFactory);
  registerDefaultDeserializer(TextParseNodeFactory);
  registerDefaultDeserializer(FormParseNodeFactory);
  const pathParameters = {
    "baseurl": requestAdapter.baseUrl
  };
  return apiClientProxifier(requestAdapter, pathParameters, PingBoardClientNavigationMetadata, void 0);
}
var PingBoardClientNavigationMetadata = {
  probes: {
    requestsMetadata: ProbesRequestBuilderRequestsMetadata
  }
};

// src/gen/types.ts
import { z } from "zod";
var types_default = z.object({
  ProbeAnomaly: z.union([
    z.object({
      ProbeId: z.string().uuid(),
      ProbeType: z.string(),
      Target: z.string(),
      EventId: z.string().uuid().optional(),
      EventTime: z.string().datetime({ offset: true }).optional()
    }),
    z.null()
  ]).optional(),
  ProbeError: z.union([
    z.object({
      ProbeId: z.string().uuid(),
      ProbeType: z.string(),
      ErrorDescription: z.string(),
      Target: z.string(),
      EventId: z.string().uuid().optional(),
      EventTime: z.string().datetime({ offset: true }).optional()
    }),
    z.null()
  ]).optional(),
  ProbeStatus: z.union([
    z.object({
      ProbeId: z.string().uuid(),
      Status: z.string(),
      Target: z.string(),
      ProbeType: z.string(),
      EventId: z.string().uuid().optional(),
      EventTime: z.string().datetime({ offset: true }).optional()
    }),
    z.null()
  ]).optional(),
  ProbeInfo: z.union([
    z.object({
      ProbeId: z.string().uuid(),
      Target: z.string(),
      ProbeType: z.string(),
      EventId: z.string().uuid().optional(),
      EventTime: z.string().datetime({ offset: true }).optional()
    }),
    z.null()
  ]).optional(),
  AdminError: z.union([
    z.object({
      ErrorDescription: z.string(),
      EventId: z.string().uuid().optional(),
      EventTime: z.string().datetime({ offset: true }).optional()
    }),
    z.null()
  ]).optional()
});

// src/index.ts
function createClient() {
  const authProvider = new AnonymousAuthenticationProvider();
  const adapter = new FetchRequestAdapter(authProvider);
  return createPingBoardClient(adapter);
}
var index_default = createClient;
export {
  index_default as default
};
//# sourceMappingURL=index.js.map