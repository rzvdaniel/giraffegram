import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { APP_INITIALIZER, EnvironmentProviders, Provider } from '@angular/core';
import { DevApiInterceptor } from 'app/backend';
/**
 * Fuse provider
 */
export const provideApp = (): Array<Provider | EnvironmentProviders> =>
{
    // Base providers
    const providers: Array<Provider | EnvironmentProviders> = [
        
    ];

    // Mock Api services
    //if ( config?.mockApi?.services )
    {
        providers.push(
            provideHttpClient(withInterceptors([DevApiInterceptor])),
            {
                provide   : APP_INITIALIZER,
                //deps      : [...config.mockApi.services],
                useFactory: () => (): any => null,
                multi     : true,
            },
        );
    }

    // Return the providers
    return providers;
};
