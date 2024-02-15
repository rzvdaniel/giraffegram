import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ApiKey, ApiKeyDetails, ApiKeyUpdate } from 'app/core/api-key/api-key.types';
import { BehaviorSubject, map, Observable, of, switchMap, tap, catchError, throwError } from 'rxjs';
import { environment } from 'environments/environment';

@Injectable({ providedIn: 'root' })
export class ApiKeyService {
    private _httpClient = inject(HttpClient);
    private _apiKeys: BehaviorSubject<ApiKeyDetails[] | null> = new BehaviorSubject(null);
    private _apiKey: BehaviorSubject<ApiKeyDetails | null> = new BehaviorSubject(null);

    // -----------------------------------------------------------------------------------------------------
    // @ Accessors
    // -----------------------------------------------------------------------------------------------------

    /**
     * Getter for templates
     */
    get apiKeys$(): Observable<ApiKeyDetails[]> {
        return this._apiKeys.asObservable();
    }

    /**
     * Getter for email
     */
    get apiKey$(): Observable<ApiKeyDetails> {
        return this._apiKey.asObservable();
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    get(): Observable<ApiKeyDetails[]> {
        return this._httpClient.get<ApiKeyDetails[]>(`${environment.api}/api/apikey`).pipe(
            tap((apiKeys) => {
                this._apiKeys.next(apiKeys);
            }),
        );
    }

    /**
     * Get email by id
     */
    getApiKeyById(id: string): Observable<ApiKeyDetails> {
        return this._httpClient.get<ApiKeyDetails>(`${environment.api}/api/apikey/${id}`).pipe(
            map((apiKey) => {
                this._apiKey.next(apiKey);

                return apiKey;
            }),
            switchMap((apiKey) => {
                if (!apiKey) {
                    return throwError('Could not find api key with id of ' + id + '!');
                }

                return of(apiKey);
            }),
        );
    }

    add(apiKey: ApiKey): Observable<any> {
        this._apiKey.next(new ApiKey());

        return this._httpClient.post<ApiKey>(`${environment.api}/api/apikey`, apiKey)
            .pipe(
                map((apiKey) => {
                    this._apiKey.next(apiKey);
                    return apiKey;
                })
            );
    }

    update(apiKey: ApiKeyUpdate): Observable<any> {
        return this._httpClient.put<ApiKeyUpdate>(`${environment.api}/api/apikey/${apiKey.id}`, apiKey);
    }
}
