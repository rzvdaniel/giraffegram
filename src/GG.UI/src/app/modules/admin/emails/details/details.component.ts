import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { NgIf } from '@angular/common';
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
    imports      : [MatIconModule, RouterLink, NgIf, MatButtonModule, QuillEditorComponent, FormsModule, ReactiveFormsModule],
})
export class EmailDetailsComponent implements OnInit
{
    email: EmailTemplate;
    composeForm: UntypedFormGroup;
    mode: 'view' | 'edit';

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
        // Make sure to start in 'view' mode
        this.mode = 'view';

        this.composeForm = this._formBuilder.group({
            html   : ['', [Validators.required]],
        });

        this._emailTemplateService.template$.pipe()
            .subscribe((email: EmailTemplate) =>
            {
                this.email = email[0];

                this.composeForm.patchValue(this.email);
            });   
    }

    edit() : void 
    {
        this.mode = 'edit';
    }

    view() : void 
    {
        this.mode = 'view';
    }

    save() : void 
    {
        this.email.html = this.composeForm.value.html;

        this._emailTemplateService.update({
            ...this.email,
        }).subscribe();
    }
}
