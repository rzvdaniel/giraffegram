import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { EmailTemplateService, EmailTemplate } from 'app/core/email-template';
import { QuillEditorComponent } from 'ngx-quill';
import { FormsModule, ReactiveFormsModule, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';

@Component({
    selector     : 'email-details',
    templateUrl  : './details.component.html',
    encapsulation: ViewEncapsulation.None,
    standalone   : true,
    imports      : [MatIconModule, RouterLink, MatButtonModule, QuillEditorComponent, FormsModule, ReactiveFormsModule],
})
export class EmailDetailsComponent implements OnInit
{
    email: EmailTemplate;
    composeForm: UntypedFormGroup;

    quillModules: any = {
        toolbar: [
            ['bold', 'italic', 'underline'],
            [{align: []}, {list: 'ordered'}, {list: 'bullet'}],
            ['clean'],
        ],
    };
    /**
     * Constructor
     */
    constructor(private _emailTemplateService: EmailTemplateService, private _formBuilder: UntypedFormBuilder)
    {
    }

    /**
     * On init
     */
    ngOnInit(): void
    {
        // Get the course
        this._emailTemplateService.template$
        .pipe()
            .subscribe((email: EmailTemplate) =>
            {
                this.email = email[0];
            });

        // Create the form
        this.composeForm = this._formBuilder.group({
            body   : ['', [Validators.required]],
        });
    }
}
