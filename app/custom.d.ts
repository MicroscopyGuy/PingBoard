interface ApiBridge {
    makeApiRequest: (method: 'GET', path: string) => Promise<string>;
}

declare global {
    interface Window { apiBridge: ApiBridge; }
}

export {};