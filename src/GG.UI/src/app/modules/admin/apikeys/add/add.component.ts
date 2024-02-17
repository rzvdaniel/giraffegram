import { Component, OnInit, OnDestroy, ViewChild, ViewEncapsulation, } from '@angular/core';
import { NgIf } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import {MatTooltipModule} from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { ApiKey, ApiKeyService } from 'app/core/api-key';
import { FormsModule, NgForm, ReactiveFormsModule, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { FuseAlertComponent, FuseAlertType } from '@fuse/components/alert';
import { fuseAnimations } from '@fuse/animations';
import { of, Observable, Subject, tap, catchError, takeUntil } from 'rxjs';
import { ClipboardModule } from 'ngx-clipboard';

@Component({
    selector: 'apikey-add',
    templateUrl: './add.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations,
    standalone: true,
    imports: [MatIconModule, RouterLink, MatButtonModule, MatInputModule, MatTooltipModule, FormsModule, ReactiveFormsModule, NgIf, FuseAlertComponent, ClipboardModule],
})
export class ApiKeyAddComponent implements OnInit, OnDestroy {
    apiKey: ApiKey;
    composeForm: UntypedFormGroup;
    @ViewChild('composeNgForm') composeNgForm: NgForm;

    private _unsubscribeAll: Subject<any> = new Subject<any>();
    showAlert: boolean = false;

    alert: { type: FuseAlertType; message: string } = {
        type: 'success',
        message: '',
    };

    constructor(private _apiKeyService: ApiKeyService,
        private _formBuilder: UntypedFormBuilder,
        private _router: Router) {
        this.apiKey = new ApiKey();
    }

    ngOnInit(): void {
        this.composeForm = this._formBuilder.group({
            name: ['', [Validators.required]],
            key: [''],
        });
    }

    ngOnDestroy(): void {
        // Unsubscribe from all subscriptions
        this._unsubscribeAll.next(null);
        this._unsubscribeAll.complete();
    }

    save(): void {
        if (this.composeForm.invalid) {
            return;
        }

        this.apiKey.name = this.composeForm.value.name;
        this.composeForm.controls['key'].reset();

        this.composeForm.disable();

        this.showAlert = false;

        this.createApiKey().subscribe((apiKey: ApiKey) => {
            this.composeForm.enable();
            this.showAlert = true;
        });

        this._apiKeyService.apiKey$
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe((apiKey: ApiKey) => {
                this.apiKey = apiKey;
                this.composeForm.patchValue(this.apiKey);
            });
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    public createApiKey(): Observable<ApiKey> {

        return this._apiKeyService.add({
            ...this.apiKey,
        }).pipe(
            tap((apiKey: ApiKey) => {
                this.alert = {
                    type: 'success',
                    message: 'Successfully created the Api Key. Please save it in your favourite Password Manager because we will not show it second time',
                };
            }),
            catchError(this.handleError<ApiKey>())
        );
    }

    /**
     * Handle Http operation that failed.
     * Let the app continue.
     *
     * @param operation - name of the operation that failed
     * @param result - optional value to return as the observable result
     */
    private handleError<T>(operation = 'operation', result?: T) {
        return (error: any): Observable<T> => {
            // TODO: send the error to remote logging infrastructure
            console.error(error); // log to console instead

            this.alert = {
                type: 'error',
                message: 'Error creating the Api Key. Please try again later or contact support',
            };

            // Let the app keep running by returning an empty result.
            return of(result as T);
        };
    }
}
