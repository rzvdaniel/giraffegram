import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { EmailTemplate } from 'app/core/email-template/email-template.types';
import { BehaviorSubject, map, Observable, of, switchMap, tap, throwError } from 'rxjs';
import { environment } from 'environments/environment';

@Injectable({providedIn: 'root'})
export class EmailTemplateService
{
    private _httpClient = inject(HttpClient);
    private _templates: BehaviorSubject<EmailTemplate[] | null> = new BehaviorSubject(null);
    private _template: BehaviorSubject<EmailTemplate | null> = new BehaviorSubject(null);

    // -----------------------------------------------------------------------------------------------------
    // @ Accessors
    // -----------------------------------------------------------------------------------------------------

    /**
     * Getter for templates
     */
    get templates$(): Observable<EmailTemplate[]>
    {
        return this._templates.asObservable();
    }

    /**
     * Getter for email
     */
    get template$(): Observable<EmailTemplate>
    {
        return this._template.asObservable();
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

    /**
     * Get email by id
     */
    getEmailById(id: string): Observable<EmailTemplate>
    {
        return this._httpClient.get<EmailTemplate>(`${environment.api}/api/emailtemplate/${id}`).pipe(
            map((template) =>
            {
                this._template.next(template);

                return template;
            }),
            switchMap((template) =>
            {
                if ( !template )
                {
                    return throwError('Could not find email with id of ' + id + '!');
                }

                return of(template);
            }),
        );
    }

    add(template: EmailTemplate): Observable<any>
    {
        return this._httpClient.post<string>(`${environment.api}/api/emailtemplate`, template).pipe(
            map((id) =>
            {
                return id;
            })
        );
    }

    update(template: EmailTemplate): Observable<any>
    {
        return this._httpClient.put<EmailTemplate>(`${environment.api}/api/emailtemplate/${template.id}`, template);
    }
}
