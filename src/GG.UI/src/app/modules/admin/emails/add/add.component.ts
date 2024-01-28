import { Component, OnInit, OnDestroy, ViewEncapsulation } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { RouterLink } from '@angular/router';
import { EmailTemplateService, EmailTemplate } from 'app/core/email-template';
import { QuillEditorComponent } from 'ngx-quill';
import { FormsModule, ReactiveFormsModule, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import {  Subject, takeUntil } from 'rxjs';

@Component({
    selector: 'email-add',
    templateUrl: './add.component.html',
    encapsulation: ViewEncapsulation.None,
    standalone: true,
    imports: [MatIconModule, RouterLink, MatButtonModule, MatInputModule, QuillEditorComponent, FormsModule, ReactiveFormsModule],
})
export class EmailAddComponent implements OnInit, OnDestroy {
    email: EmailTemplate;
    composeForm: UntypedFormGroup;

    quillModules: any = {
        toolbar: [
            ['bold', 'italic', 'underline'],
            [{ align: [] }, { list: 'ordered' }, { list: 'bullet' }],
            ['clean'],
        ],
    };

    // Private
    private _unsubscribeAll: Subject<any> = new Subject<any>();

    constructor(private _emailTemplateService: EmailTemplateService,
        private _formBuilder: UntypedFormBuilder,
        private _router: Router) {
        this.email = new EmailTemplate();
    }

    ngOnInit(): void {
        this.composeForm = this._formBuilder.group({
            name: ['', [Validators.required]],
            html: ['', [Validators.required]],
        });
    }

    ngOnDestroy(): void
    {
        // Unsubscribe from all subscriptions
        this._unsubscribeAll.next(null);
        this._unsubscribeAll.complete();
    }

    save(): void {
        if (this.composeForm.invalid) {
            return;
        }

        this.email.name = this.composeForm.value.name;
        this.email.html = this.composeForm.value.html;

        this._emailTemplateService.add({
            ...this.email,
        }).subscribe((id: string)=> {
            this.email.id = id;
            this._router.navigateByUrl(`/emails/details/${this.email.id}`);
        });     
    }
}
