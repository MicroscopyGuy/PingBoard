import { StringValue } from "@bufbuild/protobuf";
import {ProbeScheduleJson} from "../CommonInput/ProbeScheduleForm";
import {ProbeFormProps} from "../CommonInput/ProbeFormTypes"

type PingProbeSpecificJsonProperties = {
    target: string;
    maxTtl: string;
    timeoutMs: string;
    packetPayload: string;
    maxAllowedRtt: string;
};

export type PingProbeFormJson = PingProbeSpecificJsonProperties & ProbeScheduleJson;
export type PingProbeFormProps = PingProbeFormJson & ProbeFormProps;

export var emptyPingProbeForm = {
    "spread": "",
    "target": "",
    "maxTtl": "",
    "timeoutMs": "",
    "packetPayload": "",
    "maxAllowedRtt": ""
};



