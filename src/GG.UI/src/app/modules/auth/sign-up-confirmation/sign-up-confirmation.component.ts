import { Component, ViewEncapsulation } from '@angular/core';
import { RouterLink } from '@angular/router';
import { fuseAnimations } from '@fuse/animations';

@Component({
    selector     : 'sign-up-confirmation',
    templateUrl  : './sign-up-confirmation.component.html',
    encapsulation: ViewEncapsulation.None,
    animations   : fuseAnimations,
    standalone   : true,
    imports      : [RouterLink],
})
export class SignUpConfirmationComponent
{
    /**
     * Constructor
     */
    constructor()
    {
    }
}
