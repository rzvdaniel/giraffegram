import { HttpClient } from '@angular/common/http';
import { inject, Injectable, Inject } from '@angular/core';
import { EmailTemplate } from 'app/core/email-template/email-template.types';
import { BehaviorSubject, map, Observable, of, switchMap, tap, take, throwError } from 'rxjs';
import { RUNTIME_CONFIG, RuntimeConfig } from 'app/runtime.config'

@Injectable({providedIn: 'root'})
export class EmailTemplateService
{
    private _config: RuntimeConfig;
    private _httpClient = inject(HttpClient);
    private _templates: BehaviorSubject<EmailTemplate[] | null> = new BehaviorSubject(null);
    private _template: BehaviorSubject<EmailTemplate | null> = new BehaviorSubject(null);

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
        return this._httpClient.get<EmailTemplate[]>(`${this._config.api}/api/emailtemplate`).pipe(
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
        return this._httpClient.get<EmailTemplate>(`${this._config.api}/api/emailtemplate/${id}`).pipe(
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
        return this._httpClient.post<string>(`${this._config.api}/api/emailtemplate`, template).pipe(
            map((id) =>
            {
                return id;
            })
        );
    }

    update(template: EmailTemplate): Observable<any>
    {
        return this._httpClient.put<EmailTemplate>(`${this._config.api}/api/emailtemplate/${template.id}`, template);
    }

    delete(id: string): Observable<any> {
        return this.templates$.pipe(
            take(1),
            switchMap(templates => this._httpClient.delete(`${this._config.api}/api/emailtemplate/${id}`)
                .pipe(
                    tap(() => {
                        const index = templates.findIndex(item => item.id === id);
                        templates.splice(index, 1);
                        this._templates.next(templates);
                    })
                ))
            )
    }
}
