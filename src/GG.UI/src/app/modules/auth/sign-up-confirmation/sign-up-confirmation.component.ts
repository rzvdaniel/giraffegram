import { Component, ViewEncapsulation, OnDestroy, OnInit, } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { fuseAnimations } from '@fuse/animations';
import { I18nPluralPipe, NgIf } from '@angular/common';
import { finalize, Subject, takeUntil, takeWhile, tap, timer } from 'rxjs';

@Component({
    selector     : 'sign-up-confirmation',
    templateUrl  : './sign-up-confirmation.component.html',
    encapsulation: ViewEncapsulation.None,
    animations   : fuseAnimations,
    standalone   : true,
    imports      : [NgIf, RouterLink, I18nPluralPipe],
})
export class SignUpConfirmationComponent implements OnInit, OnDestroy
{
    countdown: number = 5;
    countdownMapping: any = {
        '=1'   : '# second',
        'other': '# seconds',
    };
    private _unsubscribeAll: Subject<any> = new Subject<any>();

    /**
     * Constructor
     */
    constructor(private _router: Router)
    {
    }

    /**
     * On init
     */
    ngOnInit(): void
    {
        // Redirect after the countdown
        timer(1000, 1000)
            .pipe(
                finalize(() =>
                {
                    this._router.navigate(['sign-in']);
                }),
                takeWhile(() => this.countdown > 0),
                takeUntil(this._unsubscribeAll),
                tap(() => this.countdown--),
            )
            .subscribe();
    }

    /**
     * On destroy
     */
    ngOnDestroy(): void
    {
        // Unsubscribe from all subscriptions
        this._unsubscribeAll.next(null);
        this._unsubscribeAll.complete();
    }
}
