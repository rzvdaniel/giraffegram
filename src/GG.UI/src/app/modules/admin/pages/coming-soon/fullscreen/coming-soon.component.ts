import { NgIf } from '@angular/common';
import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormsModule, NgForm, ReactiveFormsModule, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { fuseAnimations } from '@fuse/animations';
import { FuseAlertComponent, FuseAlertType } from '@fuse/components/alert';
import { AuthService } from 'app/core/auth/auth.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, Observable, of, switchMap, throwError, tap } from 'rxjs';

@Component({
    selector     : 'coming-soon-classic',
    templateUrl  : './coming-soon.component.html',
    encapsulation: ViewEncapsulation.None,
    animations   : fuseAnimations,
    standalone   : true,
    imports      : [NgIf, FuseAlertComponent, FormsModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatIconModule, MatButtonModule, MatProgressSpinnerModule],
})
export class ComingSoonFullscreenComponent implements OnInit
{
    @ViewChild('comingSoonNgForm') comingSoonNgForm: NgForm;

    alert: { type: FuseAlertType; message: string } = {
        type   : 'success',
        message: '',
    };
    comingSoonForm: UntypedFormGroup;
    showAlert: boolean = false;

    private subscribeUrl = 'https://comingsoonapi.giraffegram.com/EmailSubscription';

    httpOptions = {
        headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    };

    /**
     * Constructor
     */
    constructor(
        private _authService: AuthService,
        private _formBuilder: UntypedFormBuilder,
        private http: HttpClient,
    )
    {
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Lifecycle hooks
    // -----------------------------------------------------------------------------------------------------

    /**
     * On init
     */
    ngOnInit(): void
    {
        // Create the form
        this.comingSoonForm = this._formBuilder.group({
            email: ['', [Validators.required, Validators.email]],
        });
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    public registerEmail(): Observable<string> {
        let email = this.comingSoonForm.get('email').value;
        let emailJson = JSON.stringify({'email': email});
        return this.http.post<string>(this.subscribeUrl, emailJson, this.httpOptions).pipe(
            tap((email: string) => {
                // Set the alert
                this.alert = {
                    type   : 'success',
                    message: 'You have been registered to the list.',
                };
            }),
            catchError(this.handleError<string>())
        );
    }

    /**
     * Register
     */
    register(): void
    {
        // Return if the form is invalid
        if ( this.comingSoonForm.invalid )
        {
            return;
        }

        // Disable the form
        this.comingSoonForm.disable();

        // Hide the alert
        this.showAlert = false;

        // Do your action here...
        this.registerEmail().subscribe(result => {
            // Re-enable the form
            this.comingSoonForm.enable();

            // Reset the form
            this.comingSoonNgForm.resetForm();

            // Show the alert
            this.showAlert = true;
        });
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

        // Set the alert
        this.alert = {
            type   : 'error',
            message: 'Something went wrong, please try again.',
        };
        
        // Let the app keep running by returning an empty result.
        return of(result as T);
        };
    }  
}
