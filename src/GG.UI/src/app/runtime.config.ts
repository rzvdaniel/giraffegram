import { InjectionToken } from '@angular/core';

export class RuntimeConfig {
    api: string
    commingSoonApi: string
    allowUserRegistration: false
}
   
export let RUNTIME_CONFIG = new InjectionToken<RuntimeConfig>('RUNTIME_CONFIG')