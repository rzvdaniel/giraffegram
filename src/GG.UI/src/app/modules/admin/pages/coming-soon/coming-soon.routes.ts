import { Routes } from '@angular/router';
import { ComingSoonFullscreenComponent } from 'app/modules/admin/pages/coming-soon/fullscreen/coming-soon.component';

export default [
    {
        path     : '',
        component: ComingSoonFullscreenComponent,
    },
    {
        path     : 'fullscreen',
        component: ComingSoonFullscreenComponent,
    }
] as Routes;
