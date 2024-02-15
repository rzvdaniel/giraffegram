import { Component, OnInit, OnDestroy, ViewEncapsulation, ViewChild } from '@angular/core';
import { NgIf } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { RouterLink } from '@angular/router';
import { ApiKeyService, ApiKeyDetails } from 'app/core/api-key';
import { FormsModule, ReactiveFormsModule, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { FuseAlertComponent } from '@fuse/components/alert';

@Component({
    selector: 'apikey-details',
    templateUrl: './details.component.html',
    styleUrls    : ['./details.component.scss'],
    encapsulation: ViewEncapsulation.None,
    standalone: true,
    imports: [FuseAlertComponent, MatIconModule, MatButtonModule, MatInputModule, RouterLink, NgIf, FormsModule, ReactiveFormsModule],
})
export class ApiKeyDetailsComponent implements OnInit, OnDestroy {
    apiKey: ApiKeyDetails;
    composeForm: UntypedFormGroup;

    @ViewChild('htmlEditor') private htmlEditor: any;

    // Private
    private _unsubscribeAll: Subject<any> = new Subject<any>();

    constructor(private _apiKeyService: ApiKeyService, private _formBuilder: UntypedFormBuilder) {
    }

    /**
     * On init
     */
    ngOnInit(): void {

        this.composeForm = this._formBuilder.group({
            name: ['', [Validators.required]]
        });

        this._apiKeyService.apiKey$
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe((apiKey: ApiKeyDetails) => {
                this.apiKey = apiKey;
                this.composeForm.patchValue(this.apiKey);
            });
    }

    ngOnDestroy(): void {
        // Unsubscribe from all subscriptions
        this._unsubscribeAll.next(null);
        this._unsubscribeAll.complete();
    }

    save(): void {
        this.apiKey.name = this.composeForm.value.name;

        this._apiKeyService.update({
            ...this.apiKey,
        }).subscribe();
    }
}
