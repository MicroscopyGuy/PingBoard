import { AdditionalDataHolder, Parsable, BaseRequestBuilder, RequestConfiguration, RequestInformation } from '@microsoft/kiota-abstractions';

interface IProbeBehavior extends AdditionalDataHolder, Parsable {
    /**
     * Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.
     */
    additionalData?: Record<string, unknown>;
    /**
     * The packetPayload property
     */
    packetPayload?: string | null;
    /**
     * The target property
     */
    target?: IProbeBehavior_targetMember1 | IProbeBehavior_targetMember2 | null;
    /**
     * The timeoutMs property
     */
    timeoutMs?: number | null;
    /**
     * The ttl property
     */
    ttl?: number | null;
}
interface IProbeBehavior_targetMember1 extends AdditionalDataHolder, Parsable {
    /**
     * Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.
     */
    additionalData?: Record<string, unknown>;
    /**
     * The ipAddress property
     */
    ipAddress?: string | null;
    /**
     * The targetType property
     */
    targetType?: IProbeBehavior_targetMember1_targetType | null;
}
type IProbeBehavior_targetMember1_targetType = (typeof IProbeBehavior_targetMember1_targetTypeObject)[keyof typeof IProbeBehavior_targetMember1_targetTypeObject];
interface IProbeBehavior_targetMember2 extends AdditionalDataHolder, Parsable {
    /**
     * Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.
     */
    additionalData?: Record<string, unknown>;
    /**
     * The hostname property
     */
    hostname?: string | null;
    /**
     * The targetType property
     */
    targetType?: IProbeBehavior_targetMember2_targetType | null;
}
type IProbeBehavior_targetMember2_targetType = (typeof IProbeBehavior_targetMember2_targetTypeObject)[keyof typeof IProbeBehavior_targetMember2_targetTypeObject];
interface IProbeThresholds extends AdditionalDataHolder, Parsable {
    /**
     * Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.
     */
    additionalData?: Record<string, unknown>;
}
interface ProbeConfigAggregate extends AdditionalDataHolder, Parsable {
    /**
     * Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.
     */
    additionalData?: Record<string, unknown>;
    /**
     * The behavior property
     */
    behavior?: IProbeBehavior | null;
    /**
     * The probeType property
     */
    probeType?: string | null;
    /**
     * The schedule property
     */
    schedule?: ProbeSchedule | null;
    /**
     * The thresholds property
     */
    thresholds?: IProbeThresholds | null;
}
interface ProbeSchedule extends AdditionalDataHolder, Parsable {
    /**
     * Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.
     */
    additionalData?: Record<string, unknown>;
    /**
     * The spread property
     */
    spread?: string | null;
}
declare const IProbeBehavior_targetMember1_targetTypeObject: {
    readonly IpAddress: "ipAddress";
};
declare const IProbeBehavior_targetMember2_targetTypeObject: {
    readonly Hostname: "hostname";
};

/**
 * Builds and executes requests for operations under /Probes
 */
interface ProbesRequestBuilder extends BaseRequestBuilder<ProbesRequestBuilder> {
    /**
     * @param requestConfiguration Configuration for the request such as headers, query parameters, and middleware options.
     * @throws {ProblemDetails} error when the service returns a 422 status code
     * @throws {ProblemDetails} error when the service returns a 500 status code
     */
    delete(requestConfiguration?: RequestConfiguration<object> | undefined): Promise<void>;
    /**
     * @param body The request body
     * @param requestConfiguration Configuration for the request such as headers, query parameters, and middleware options.
     * @throws {ProblemDetails} error when the service returns a 409 status code
     */
    post(body: ProbeConfigAggregate, requestConfiguration?: RequestConfiguration<object> | undefined): Promise<void>;
    /**
     * @param requestConfiguration Configuration for the request such as headers, query parameters, and middleware options.
     * @returns {RequestInformation}
     */
    toDeleteRequestInformation(requestConfiguration?: RequestConfiguration<object> | undefined): RequestInformation;
    /**
     * @param body The request body
     * @param requestConfiguration Configuration for the request such as headers, query parameters, and middleware options.
     * @returns {RequestInformation}
     */
    toPostRequestInformation(body: ProbeConfigAggregate, requestConfiguration?: RequestConfiguration<object> | undefined): RequestInformation;
}

/**
 * The main entry point of the SDK, exposes the configuration and the fluent API.
 */
interface PingBoardClient extends BaseRequestBuilder<PingBoardClient> {
    /**
     * The Probes property
     */
    get probes(): ProbesRequestBuilder;
}

declare function createClient(): PingBoardClient;

export { createClient as default };
