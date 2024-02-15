import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot, Routes } from '@angular/router';
import { ApiKeyListComponent } from 'app/modules/admin/apikeys/list/list.component';
import { ApiKeyDetailsComponent } from 'app/modules/admin/apikeys/details/details.component';
import { ApiKeyAddComponent } from 'app/modules/admin/apikeys/add/add.component';
import { ApiKeyComponent } from 'app/modules/admin/apikeys/apikey.component';
import { ApiKeyService } from 'app/core/api-key';

/**
 * Course resolver
 *
 * @param route
 * @param state
 */
const apiKeyResolver = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) =>
{
    const eapiKeyService = inject(ApiKeyService);
    const router = inject(Router);

    return eapiKeyService.getApiKeyById(route.paramMap.get('id')).pipe(
        // Error here means the requested course is not available
        catchError((error) =>
        {
            // Log the error
            console.error(error);

            // Get the parent url
            const parentUrl = state.url.split('/').slice(0, -1).join('/');

            // Navigate to there
            router.navigateByUrl(parentUrl);

            // Throw an error
            return throwError(error);
        }),
    );
};


export default [
    {
        path     : '',
        component: ApiKeyComponent,
        resolve  : {
        },
        children : [
            {
                path     : '',
                pathMatch: 'full',
                component: ApiKeyListComponent,
                resolve  : {
                    apiKeys    : () => inject(ApiKeyService).get()
                },
            },
            {
                path     : 'details/:id',
                component: ApiKeyDetailsComponent,
                resolve  : {
                    course: apiKeyResolver,
                },
            },
            {
                path     : 'add',
                component: ApiKeyAddComponent
            },
        ],
    },
] as Routes;
