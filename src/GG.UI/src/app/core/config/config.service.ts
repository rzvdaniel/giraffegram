import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { environment } from 'environments/environment';

@Injectable({providedIn: 'root'})
export class ConfigService
{
    allowUserRegistration(): Observable<boolean>
    {
        if ( !environment.allowUserRegistration)
        {
            return of(false);
        }

        return of(true);
    }
}
