import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { EmailTemplate } from 'app/core/email-template/email-template.types';
import { BehaviorSubject, filter, map, Observable, of, switchMap, take, tap, throwError } from 'rxjs';
import { environment } from 'environment/environment';

@Injectable({providedIn: 'root'})
export class EmailTemplateService
{
    private _httpClient = inject(HttpClient);
    private _templates: BehaviorSubject<EmailTemplate[] | null> = new BehaviorSubject(null);

    // -----------------------------------------------------------------------------------------------------
    // @ Accessors
    // -----------------------------------------------------------------------------------------------------

    /**
     * Getter for brands
     */
    get templates$(): Observable<EmailTemplate[]>
    {
        return this._templates.asObservable();
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    get(): Observable<EmailTemplate[]>
    {
        return this._httpClient.get<EmailTemplate[]>(`${environment.api}/api/emailtemplate`).pipe(
            tap((templates) =>
            {
                this._templates.next(templates);
            }),
        );
    }
}
