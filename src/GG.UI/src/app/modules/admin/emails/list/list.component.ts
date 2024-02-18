import { Component, OnInit, OnDestroy, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { DatePipe, SlicePipe } from '@angular/common';
import { EmailTemplateService, EmailTemplate } from 'app/core/email-template';
import { FuseConfirmationService } from '@fuse/services/confirmation';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { FormsModule, ReactiveFormsModule, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { debounceTime, map, merge, Observable, Subject, switchMap, takeUntil } from 'rxjs';

import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar } from '@angular/material/snack-bar';

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
    emailTemplatesTableColumns: string[] = ['name', 'html', 'created', 'updated', 'action'];
    searchInputControl: UntypedFormControl = new UntypedFormControl();
    emailTemplatesDataSource: MatTableDataSource<any> = new MatTableDataSource();

    // Private
    private _unsubscribeAll: Subject<any> = new Subject<any>();

    constructor(
        private _emailTemplateService: EmailTemplateService,
        private _fuseConfirmationService: FuseConfirmationService,
        private _snackBar: MatSnackBar,
        private _router: Router)
    {
    }

    ngOnInit(): void
    {
        this._emailTemplateService.templates$
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe((emailTemplates: EmailTemplate[]) => {
                this.emailTemplatesDataSource.data = emailTemplates;
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
    
    deleteEmailTemplate(emailTemplate: EmailTemplate) {

        // Open the confirmation dialog
        const confirmation = this._fuseConfirmationService.open({
            title: 'Delete email template',
            message: 'Are you sure you want to delete this email template? This action cannot be undone!',
            actions: {
                confirm: {
                    label: 'Delete',
                },
            },
        });

        // Subscribe to the confirmation dialog closed action
        confirmation.afterClosed().subscribe((result) => {
            if (result === 'confirmed') {
                const id = emailTemplate.id;

                this._emailTemplateService.delete(id).subscribe(
                    {
                        complete: () => {
                            this._snackBar.open("Email template deleted successfully!", "Close", {
                                duration: 5000
                            });
                        },
                        error: () => {
                            this._snackBar.open("Something went wrong when deleting email template!", "Close", {
                                duration: 5000,
                                panelClass: ["red-snackbar"]
                            });
                        }
                    });
            }
        });
    }
}
