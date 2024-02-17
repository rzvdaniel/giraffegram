import { Component, OnInit, OnDestroy, ViewEncapsulation, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { DatePipe, SlicePipe, NgIf } from '@angular/common';
import { ApiKeyService, ApiKeyDetails } from 'app/core/api-key';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { FormsModule, ReactiveFormsModule, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { debounceTime, map, merge, Observable, Subject, switchMap, takeUntil, catchError, throwError } from 'rxjs';
import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatButtonModule } from '@angular/material/button';
import { FuseConfirmationService } from '@fuse/services/confirmation';
import { FuseAlertComponent, FuseAlertType } from '@fuse/components/alert';

@Component({
    selector: 'apikey-list',
    standalone: true,
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss'],
    encapsulation: ViewEncapsulation.None,
    imports: [MatTableModule, MatIconModule, MatSlideToggleModule, MatButtonModule, DatePipe, SlicePipe, FuseAlertComponent, NgIf],
})
export class ApiKeyListComponent implements OnInit, OnDestroy {
    apiKeyTableColumns: string[] = ['name', 'key', 'created', 'updated', 'action'];
    searchInputControl: UntypedFormControl = new UntypedFormControl();
    apiKeysDataSource: MatTableDataSource<any> = new MatTableDataSource();

    private _unsubscribeAll: Subject<any> = new Subject<any>();

    showAlert: boolean = false;

    alert: { type: FuseAlertType; message: string } = {
        type: 'success',
        message: 'asd',
    };

    constructor(
        private _apiKeyService: ApiKeyService,
        private _changeDetectorRef: ChangeDetectorRef,
        private _fuseConfirmationService: FuseConfirmationService,
        private _router: Router) {
    }

    ngOnInit(): void {
        this._apiKeyService.apiKeys$
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe((apiKeys: ApiKeyDetails[]) => {
                this.apiKeysDataSource.data = apiKeys;
            });
    }

    ngOnDestroy(): void {
        // Unsubscribe from all subscriptions
        this._unsubscribeAll.next(null);
        this._unsubscribeAll.complete();
    }

    /**
     * Create product
     */
    add(): void {
        this._router.navigateByUrl(`/apikeys/add`);
    }

    viewApiKeyDetails(row: ApiKeyDetails): void {
        this._router.navigateByUrl(`/apikeys/details/${row.id}`);
    }

    deleteApiKey(apiKey: ApiKeyDetails) {

        // Hide the alert
        this.showAlert = false;

        // Open the confirmation dialog
        const confirmation = this._fuseConfirmationService.open({
            title: 'Delete api key',
            message: 'Are you sure you want to delete this api key? This action cannot be undone!',
            actions: {
                confirm: {
                    label: 'Delete',
                },
            },
        });

        // Subscribe to the confirmation dialog closed action
        confirmation.afterClosed().subscribe((result) => {
            if (result === 'confirmed') {
                const id = apiKey.id;

                this._apiKeyService.delete(id).subscribe(
                {
                    complete: () =>
                    {
                        this.showAlert = true;
                        this.alert = {
                            type: 'success',
                            message: 'Something went wrong when deleting api key.',
                        };
                    },
                    error:() =>
                    {
                        this.showAlert = true;
                        this.alert = {
                            type: 'error',
                            message: 'Something went wrong when deleting api key.',
                        };
                    }
                });
            }
        });
    }
}
