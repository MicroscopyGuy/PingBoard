import {useState, useEffect, useContext} from 'react'
//import {emptyPingProbeForm, PingProbeFormJson} from './PingProbeFormTypes';
import PingProbeBehaviorForm from './PingProbeBehaviorForm';
import PingProbeThresholdsForm from './PingProbeThresholdsForm';
import { ProbeScheduleForm } from '../CommonInput/ProbeScheduleForm';
import { ProbeConfigChange, OnProbeFormChangeContext } from '../ProbeConfigFormManager';

export function PingProbeConfigForm(){
    return (
        <div className="probeConfigForm">
            <PingProbeBehaviorForm />
            <PingProbeThresholdsForm />
            <ProbeScheduleForm />
        </div>
    )

}