import { Component, OnInit, OnDestroy, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { DatePipe, SlicePipe } from '@angular/common';
import { ApiKeyService, ApiKeyDetails } from 'app/core/api-key';
import { MatTableModule } from '@angular/material/table';
import { FormsModule, ReactiveFormsModule, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { debounceTime, map, merge, Observable, Subject, switchMap, takeUntil } from 'rxjs';
import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatButtonModule } from '@angular/material/button';

@Component({
    selector     : 'apikey-list',
    standalone   : true,
    templateUrl  : './list.component.html',
    styleUrls    : ['./list.component.scss'],
    encapsulation: ViewEncapsulation.None,
    imports        : [MatTableModule, MatIconModule, MatSlideToggleModule, MatButtonModule, DatePipe, SlicePipe],
})
export class ApiKeyListComponent implements OnInit, OnDestroy
{
    apiKeys: ApiKeyDetails[];
    apiKeyTableColumns: string[] = ['name', 'key', 'created', 'updated'];
    searchInputControl: UntypedFormControl = new UntypedFormControl();

    // Private
    private _unsubscribeAll: Subject<any> = new Subject<any>();

    constructor(private _apiKeyService: ApiKeyService, private _router: Router)
    {
    }

    ngOnInit(): void
    {
        this._apiKeyService.apiKeys$
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe((apiKeys: ApiKeyDetails[]) => {
                this.apiKeys = apiKeys;
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
        this._router.navigateByUrl(`/apikeys/add`);
    }

    viewApiKeyDetails(row: ApiKeyDetails): void {
        this._router.navigateByUrl(`/apikeys/details/${row.id}`);
    }  
}
