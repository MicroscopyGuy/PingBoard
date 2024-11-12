import type { BackendClient } from './createClient';

type PromiseOrNever<T extends keyof BackendClient> =
  ReturnType<BackendClient[T]> extends Promise<any> ? T : never;
type PromiseReturningBackendClient = {
  [K in keyof BackendClient as PromiseOrNever<K>]: BackendClient[K];
};

declare type Maybe<T> =
  | {
      successful: true;
      result: T;
    }
  | {
      successful: false;
      error: Error;
    };
export { Maybe, PromiseOrNever, BackendClient, PromiseReturningBackendClient };