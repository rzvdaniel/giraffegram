import { Component, OnInit, OnDestroy, ViewEncapsulation, ViewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { RouterLink } from '@angular/router';
import { EmailTemplateService, EmailTemplate } from 'app/core/email-template';
import { FormsModule, ReactiveFormsModule, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import {  Subject, takeUntil } from 'rxjs';

import { EditorState, Extension } from '@codemirror/state';
import { EditorView } from '@codemirror/view';
import { xcodeLight } from '@uiw/codemirror-theme-xcode'
import { html } from '@codemirror/lang-html';

@Component({
    selector: 'email-add',
    templateUrl: './add.component.html',
    encapsulation: ViewEncapsulation.None,
    standalone: true,
    imports: [MatIconModule, RouterLink, MatButtonModule, MatInputModule, FormsModule, ReactiveFormsModule],
})
export class EmailAddComponent implements OnInit, OnDestroy {
    email: EmailTemplate;
    composeForm: UntypedFormGroup;
    editorState: EditorState;
    editorView: EditorView;

    @ViewChild('htmlEditor') private htmlEditor: any;

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
            subject: ['', [Validators.required]]
        });
    }

    ngAfterViewInit(): void {
        let customTheme = EditorView.theme({
            '&': { maxHeight: '400px', minHeight: '400px' },
            '.cm-scroller': { overflow: 'auto' },
          });

        let editorExtentions: Extension = [xcodeLight, customTheme, html()];
        
        this.editorState = EditorState.create({
            doc: this.email.html,
            extensions: editorExtentions,
        });

        let state = this.editorState;

        this.editorView = new EditorView({
            state,
            parent: this.htmlEditor.nativeElement,
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
        this.email.subject = this.composeForm.value.subject;
        this.email.html = this.editorView.state.doc.toString();

        this._emailTemplateService.add({
            ...this.email,
        }).subscribe((id: string)=> {
            this.email.id = id;
            this._router.navigateByUrl(`/emails/details/${this.email.id}`);
        });     
    }
}
