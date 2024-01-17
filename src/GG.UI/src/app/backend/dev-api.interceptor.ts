import { HttpEvent, HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';

export default (request: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> =>
{
    const url = 'https://localhost:7160';
    request = request.clone({
      url: `${url}/${request.url}`
    });
    return next(request);
};
