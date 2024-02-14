import { inject } from '@angular/core';
import { CanActivateChildFn, CanActivateFn, Router } from '@angular/router';
import { ConfigService } from 'app/core/config/config.service';
import { of, switchMap } from 'rxjs';

export const ConfigGuard: CanActivateFn | CanActivateChildFn = (route, state) =>
{
    const router: Router = inject(Router);

    return inject(ConfigService).allowUserRegistration().pipe(
        switchMap((result) =>
        {
            if ( !result )
            {
                const notFoundUrl = router.parseUrl("404-not-found");

                return of(notFoundUrl);
            }

            return of(true);
        }),
    );
};
