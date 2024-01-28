import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot, Routes } from '@angular/router';
import { EmailListComponent } from 'app/modules/admin/emails/list/list.component';
import { EmailDetailsComponent } from 'app/modules/admin/emails/details/details.component';
import { EmailAddComponent } from 'app/modules/admin/emails/add/add.component';
import { EmailComponent } from 'app/modules/admin/emails/email.component';
import { EmailTemplateService } from 'app/core/email-template';
import { catchError, throwError } from 'rxjs';

/**
 * Course resolver
 *
 * @param route
 * @param state
 */
const emailResolver = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) =>
{
    const emailsService = inject(EmailTemplateService);
    const router = inject(Router);

    return emailsService.getEmailById(route.paramMap.get('id')).pipe(
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
        component: EmailComponent,
        resolve  : {
        },
        children : [
            {
                path     : '',
                pathMatch: 'full',
                component: EmailListComponent,
                resolve  : {
                    emailtemplates    : () => inject(EmailTemplateService).get()
                },
            },
            {
                path     : 'details/:id',
                component: EmailDetailsComponent,
                resolve  : {
                    course: emailResolver,
                },
            },
            {
                path     : 'add',
                component: EmailAddComponent
            },
        ],
    },
] as Routes;
