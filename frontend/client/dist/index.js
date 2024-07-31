"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const connect_1 = require("@connectrpc/connect");
const service_connect_1 = require("./gen/service_connect");
const connect_web_1 = require("@connectrpc/connect-web");
const createClient = (url) => {
    const transport = (0, connect_web_1.createGrpcWebTransport)({
        baseUrl: url
    });
    return (0, connect_1.createPromiseClient)(service_connect_1.PingBoardService, transport);
};
exports.default = createClient;
