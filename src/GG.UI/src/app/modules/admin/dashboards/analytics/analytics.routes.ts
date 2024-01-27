import { inject } from '@angular/core';
import { Routes } from '@angular/router';
import { AnalyticsComponent } from 'app/modules/admin/dashboards/analytics/analytics.component';
import { AnalyticsService } from 'app/modules/admin/dashboards/analytics/analytics.service';
import { UserService } from 'app/core/user/user.service';

export default [
    {
        path     : '',
        component: AnalyticsComponent,
        resolve  : {
            data: () => inject(AnalyticsService).getData()
        },
    },
] as Routes;
