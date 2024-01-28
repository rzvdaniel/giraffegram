import { Component, OnInit, OnDestroy, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { DatePipe, SlicePipe } from '@angular/common';
import { EmailTemplateService, EmailTemplate } from 'app/core/email-template';
import { MatTableModule } from '@angular/material/table';
import { FormsModule, ReactiveFormsModule, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { debounceTime, map, merge, Observable, Subject, switchMap, takeUntil } from 'rxjs';
import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatButtonModule } from '@angular/material/button';

@Component({
    selector     : 'email-list',
    standalone   : true,
    templateUrl  : './list.component.html',
    styleUrls    : ['./list.component.scss'],
    encapsulation: ViewEncapsulation.None,
    imports        : [MatTableModule, MatIconModule, MatSlideToggleModule, MatButtonModule, DatePipe, SlicePipe],
})
export class EmailListComponent implements OnInit, OnDestroy
{
    emailTemplates: EmailTemplate[];
    emailTemplatesTableColumns: string[] = ['name', 'html', 'created', 'updated'];
    searchInputControl: UntypedFormControl = new UntypedFormControl();

    // Private
    private _unsubscribeAll: Subject<any> = new Subject<any>();

    constructor(private _emailTemplateService: EmailTemplateService, private _router: Router)
    {
    }

    ngOnInit(): void
    {
        this._emailTemplateService.templates$
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe((emailTemplates: EmailTemplate[]) => {
                this.emailTemplates = emailTemplates;
            });
    }

    ngOnDestroy(): void
    {
        // Unsubscribe from all subscriptions
        this._unsubscribeAll.next(null);
        this._unsubscribeAll.complete();
    }

    /**
     * Create product
     */
    add(): void
    {
        this._router.navigateByUrl(`/emails/add`);
    }

    viewEmailDetails(row: EmailTemplate): void {
        this._router.navigateByUrl(`/emails/details/${row.id}`);
    }

    /**
     * Close the details
     */
    // closeDetails(): void
    // {
    //     this.selectedProduct = null;
    // }       
}
