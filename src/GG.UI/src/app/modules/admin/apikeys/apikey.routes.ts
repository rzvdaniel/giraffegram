import { inject } from '@angular/core';
import { Routes } from '@angular/router';
import { ApiKeyListComponent } from 'app/modules/admin/apikeys/list/list.component';
import { ApiKeyComponent } from 'app/modules/admin/apikeys/apikey.component';
import { ApiKeyService } from 'app/core/api-key';

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
        ],
    },
] as Routes;
