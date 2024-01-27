import { inject } from '@angular/core';
import { Routes } from '@angular/router';
import { EmailsComponent } from 'app/modules/admin/emails/emails.component';
import { EmailTemplateService } from 'app/core/email-template';

export default [
    {
        path     : '',
        component: EmailsComponent,
        resolve  : {
            emailtemplates    : () => inject(EmailTemplateService).get()
        },
    },
] as Routes;
