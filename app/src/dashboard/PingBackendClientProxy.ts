

// caller will be presented with something.apiName(request)
class PingBackendClientProxy{
    constructor(){
        return new Proxy({}, {
            get(self, apiName, _){
                if (typeof apiName !== "string"){
                    return Reflect.get(self, apiName);
                }

                return async (requestObj: any) => await (window as any).apiBridge.makeApiRequest(apiName, requestObj);
            }
        })
    }
}

export default PingBackendClientProxy;