import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { AuthUtils } from 'app/core/auth/auth.utils';
import { UserService } from 'app/core/user/user.service';
import { catchError, Observable, of, switchMap, throwError } from 'rxjs';
import { environment } from 'environment/environment';

@Injectable({providedIn: 'root'})
export class AuthService
{
    private _httpClient = inject(HttpClient);
    private _userService = inject(UserService);

    // -----------------------------------------------------------------------------------------------------
    // @ Accessors
    // -----------------------------------------------------------------------------------------------------

    /**
     * Setter & getter for access token
     */
    set accessToken(token: string)
    { 
        localStorage.setItem('accessToken', token);
    }

    get accessToken(): string
    {
        return localStorage.getItem('accessToken') ?? '';
    }

    set expiresIn(seconds: number)
    {
        let expiresInSeconds = seconds.toString();
        localStorage.setItem('expiresIn', expiresInSeconds);
    }

    get expiresIn(): number
    {
        let expiresIn = localStorage.getItem('expiresIn');
        var expiresInSeconds = parseInt(expiresIn) ?? 0;
        return expiresInSeconds;
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Forgot password
     *
     * @param email
     */
    forgotPassword(email: string): Observable<any>
    {
        return this._httpClient.post(`${environment.api}/api/user/forgot-password`, { email });
    }

    /**
     * Reset password
     *
     * @param password
     */
    resetPassword(email: string, password: string, token: string): Observable<any>
    {
        return this._httpClient.post(`${environment.api}/api/user/reset-password`, {email: email, password: password, token: token});
    }

    /**
     * Sign in
     *
     * @param credentials
     */
    signIn(credentials: { email: string; password: string }): Observable<any>
    {
        return this._httpClient.post(`${environment.api}/api/authorization/token`, 
            new HttpParams()
                .set('username', credentials.email)
                .set('password', credentials.password)
                .set('grant_type', 'password'), {
            headers: new HttpHeaders()
                .set('Content-Type', 'application/x-www-form-urlencoded')
        }).pipe(
            switchMap((response: any) => {
                // Store the access token in the local storage
                this.accessToken = response.access_token;
                this.expiresIn = response.expires_in;

                // Store the user on the user service
                this._userService.get().subscribe();

                // Return a new observable with the response
                return of(response);
            })
        );
    }

    /**
     * Sign out
     */
    signOut(): Observable<any>
    {
        // Remove the access token from the local storage
        localStorage.removeItem('accessToken');
        localStorage.removeItem('expiresIn');

        // Return the observable
        return of(true);
    }

    /**
     * Sign up
     *
     * @param user
     */
    signUp(user: { name: string; email: string; password: string; }): Observable<any>
    {
        return this._httpClient.post(`${environment.api}/api/user`, user);
    }

    /** 
     * Unlock session
     *
     * @param credentials
     */
    unlockSession(credentials: { email: string; password: string }): Observable<any>
    {
        return this._httpClient.post('api/auth/unlock-session', credentials);
    }

    /**
     * Check the authentication status
     */
    check(): Observable<boolean>
    {
        // Check the access token availability
        if ( !this.accessToken )
        {
            return of(false);
        }

        // Check the access token expire date
        if ( AuthUtils.isTokenExpired(this.expiresIn) )
        {
            return of(false);
        }

        // If the access token exists, and it didn't expire, we are authenticated
        return of(true);
    }
}
