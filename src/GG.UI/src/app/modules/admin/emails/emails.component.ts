import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { EmailTemplateService, EmailTemplate } from 'app/core/email-template';
import { MatTableModule } from '@angular/material/table';

@Component({
    selector     : 'emails',
    standalone   : true,
    templateUrl  : './emails.component.html',
    encapsulation: ViewEncapsulation.None,
    imports        : [MatTableModule],
})
export class EmailsComponent implements OnInit
{
    emailTemplates: EmailTemplate[];
    emailTemplatesTableColumns: string[] = ['name', 'text', 'html', 'created', 'updated'];

    constructor(private _emailTemplateService: EmailTemplateService)
    {
    }

    ngOnInit(): void
    {
        this._emailTemplateService.templates$
            .subscribe((emailTemplates: EmailTemplate[]) => {
                this.emailTemplates = emailTemplates;
            });
    }
    
}
