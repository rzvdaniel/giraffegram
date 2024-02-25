import { HttpClient } from '@angular/common/http';
import { inject, Injectable, Inject } from '@angular/core';
import { User } from 'app/core/user/user.types';
import { map, Observable, ReplaySubject, tap , catchError, throwError} from 'rxjs';
import { RUNTIME_CONFIG, RuntimeConfig } from 'app/runtime.config'

@Injectable({providedIn: 'root'})
export class UserService
{
    private _config: RuntimeConfig;
    private _httpClient = inject(HttpClient);
    private _user: ReplaySubject<User> = new ReplaySubject<User>(1);

    /**
     * Constructor
     */
    constructor(@Inject(RUNTIME_CONFIG) config: RuntimeConfig)
    {
        this._config = config;
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Accessors
    // -----------------------------------------------------------------------------------------------------

    /**
     * Setter & getter for user
     *
     * @param value
     */
    set user(value: User)
    {
        // Store the value
        this._user.next(value);
    }

    get user$(): Observable<User>
    {
        return this._user.asObservable();
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Get the current signed-in user data
     */
    get(): Observable<User>
    {
        return this._httpClient.post<User>(`${this._config.api}/api/user/userinfo`, null).pipe(
            tap((user) =>
            {
                this._user.next(user);
            })     
        );
    }

    /**
     * Update the user
     *
     * @param user
     */
    update(user: User): Observable<any>
    {
        return this._httpClient.patch<User>('api/common/user', {user}).pipe(
            map((response) =>
            {
                this._user.next(response);
            }),
        );
    }
}
