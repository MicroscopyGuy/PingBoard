"use strict";
// @generated by protoc-gen-connect-es v1.4.0 with parameter "target=ts"
// @generated from file service.proto (syntax proto3)
/* eslint-disable */
// @ts-nocheck
Object.defineProperty(exports, "__esModule", { value: true });
exports.PingBoardService = void 0;
const service_pb_js_1 = require("./service_pb.js");
const protobuf_1 = require("@bufbuild/protobuf");
/**
 * @generated from service PingBoardService
 */
exports.PingBoardService = {
    typeName: "PingBoardService",
    methods: {
        /**
         * @generated from rpc PingBoardService.StartPinging
         */
        startPinging: {
            name: "StartPinging",
            I: service_pb_js_1.PingTarget,
            O: protobuf_1.Empty,
            kind: protobuf_1.MethodKind.Unary,
        },
        /**
         * @generated from rpc PingBoardService.StopPinging
         */
        stopPinging: {
            name: "StopPinging",
            I: protobuf_1.Empty,
            O: protobuf_1.Empty,
            kind: protobuf_1.MethodKind.Unary,
        },
    }
};
