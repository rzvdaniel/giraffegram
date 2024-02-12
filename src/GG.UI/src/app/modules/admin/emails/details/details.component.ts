import { Component, OnInit, OnDestroy, ViewEncapsulation, ViewChild } from '@angular/core';
import { NgIf } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { RouterLink } from '@angular/router';
import { EmailTemplateService, EmailTemplate } from 'app/core/email-template';
import { FormsModule, ReactiveFormsModule, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';

import { EditorState, Extension } from '@codemirror/state';
import { EditorView } from '@codemirror/view';
import { xcodeLight } from '@uiw/codemirror-theme-xcode'
import { html } from '@codemirror/lang-html';

@Component({
    selector: 'email-details',
    templateUrl: './details.component.html',
    styleUrls    : ['./details.component.scss'],
    encapsulation: ViewEncapsulation.None,
    standalone: true,
    imports: [MatIconModule, MatButtonModule, MatInputModule, RouterLink, NgIf, FormsModule, ReactiveFormsModule],
})
export class EmailDetailsComponent implements OnInit, OnDestroy {
    email: EmailTemplate;
    composeForm: UntypedFormGroup;
    mode: 'view' | 'edit';
    editorState: EditorState;
    editorView: EditorView;

    @ViewChild('htmlEditor') private htmlEditor: any;

    // Private
    private _unsubscribeAll: Subject<any> = new Subject<any>();

    constructor(private _emailTemplateService: EmailTemplateService, private _formBuilder: UntypedFormBuilder) {
    }

    /**
     * On init
     */
    ngOnInit(): void {
        // Make sure to start in 'view' mode
        this.mode = 'view';

        this.composeForm = this._formBuilder.group({
            name: ['', [Validators.required]],
            subject: ['', [Validators.required]],
            html: ['', [Validators.required]],
        });

        this._emailTemplateService.template$
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe((email: EmailTemplate) => {
                this.email = email;
                this.composeForm.patchValue(this.email);
            });
    }

    ngAfterViewInit(): void {
        let myTheme = EditorView.theme({
            '&': { maxHeight: '400px', minHeight: '400px' },
            '.cm-scroller': { overflow: 'auto' },
          });

        let myEditorElement = this.htmlEditor.nativeElement;
        let myExt: Extension = [xcodeLight, myTheme, html()];
        
        this.editorState = EditorState.create({
            doc: this.email.html,
            extensions: myExt,
        });

        let state = this.editorState;

        this.editorView = new EditorView({
            state,
            parent: myEditorElement,
        });
    }

    ngOnDestroy(): void {
        // Unsubscribe from all subscriptions
        this._unsubscribeAll.next(null);
        this._unsubscribeAll.complete();
    }

    edit(): void {
        this.mode = 'edit';
    }

    view(): void {
        this.mode = 'view';
    }

    save(): void {
        this.email.name = this.composeForm.value.name;
        this.email.subject = this.composeForm.value.subject;
        this.email.html = this.editorView.state.doc.toString();

        this._emailTemplateService.update({
            ...this.email,
        }).subscribe();
    }
}
