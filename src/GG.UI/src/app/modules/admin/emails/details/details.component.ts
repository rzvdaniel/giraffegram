import { Component, OnInit, OnDestroy, ViewEncapsulation } from '@angular/core';
import { NgIf } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { RouterLink } from '@angular/router';
import { EmailTemplateService, EmailTemplate } from 'app/core/email-template';
import { QuillEditorComponent } from 'ngx-quill';
import { FormsModule, ReactiveFormsModule, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import {  Subject, takeUntil } from 'rxjs';

@Component({
    selector     : 'email-details',
    templateUrl  : './details.component.html',
    encapsulation: ViewEncapsulation.None,
    standalone   : true,
    imports      : [MatIconModule, MatButtonModule, MatInputModule, RouterLink, NgIf, QuillEditorComponent, FormsModule, ReactiveFormsModule],
})
export class EmailDetailsComponent implements OnInit, OnDestroy
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

    // Private
    private _unsubscribeAll: Subject<any> = new Subject<any>();

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
            name: ['', [Validators.required]],
            subject: ['', [Validators.required]],
            html   : ['', [Validators.required]],
        });

        this._emailTemplateService.template$
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe((email: EmailTemplate) =>
            {
                this.email = email;

                this.composeForm.patchValue(this.email);
            });   
    }

    ngOnDestroy(): void
    {
        // Unsubscribe from all subscriptions
        this._unsubscribeAll.next(null);
        this._unsubscribeAll.complete();
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
        this.email.name = this.composeForm.value.name;
        this.email.subject = this.composeForm.value.subject;
        this.email.html = this.composeForm.value.html;

        this._emailTemplateService.update({
            ...this.email,
        }).subscribe();
    }
}
