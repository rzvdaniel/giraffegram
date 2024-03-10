import { inject } from '@angular/core';
import { CanActivateChildFn, CanActivateFn, Router } from '@angular/router';
import { UserService } from 'app/core/user/user.service';
import { of, switchMap } from 'rxjs';

export const AdminGuard: CanActivateFn | CanActivateChildFn = (route, state) =>
{
    const router: Router = inject(Router);

    return inject(UserService).user$.pipe(
        switchMap((user) =>
        {
            // If the user is not admin...
            if ( !user.role.includes("Administrator") )
            {
                return of(router.parseUrl('404-not-found'));
            }

            // Allow the access
            return of(true);
        }),
    );
};
