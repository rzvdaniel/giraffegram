import { inject } from '@angular/core';
import { Routes } from '@angular/router';
import { ApiKeyListComponent } from 'app/modules/admin/apikeys/list/list.component';
import { ApiKeyAddComponent } from 'app/modules/admin/apikeys/add/add.component';
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
            {
                path     : 'add',
                component: ApiKeyAddComponent
            },
        ],
    },
] as Routes;
