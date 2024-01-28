import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { DatePipe, SlicePipe } from '@angular/common';
import { EmailTemplateService, EmailTemplate } from 'app/core/email-template';
import { MatTableModule } from '@angular/material/table';
import { FormsModule, ReactiveFormsModule, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { debounceTime, map, merge, Observable, Subject, switchMap, takeUntil } from 'rxjs';
import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatButtonModule } from '@angular/material/button';

@Component({
    selector     : 'email-list',
    standalone   : true,
    templateUrl  : './list.component.html',
    styleUrls    : ['./list.component.scss'],
    encapsulation: ViewEncapsulation.None,
    imports        : [MatTableModule, MatIconModule, MatSlideToggleModule, MatButtonModule, DatePipe, SlicePipe],
})
export class EmailListComponent implements OnInit
{
    emailTemplates: EmailTemplate[];
    emailTemplatesTableColumns: string[] = ['name', 'text', 'html', 'created', 'updated'];
    searchInputControl: UntypedFormControl = new UntypedFormControl();

    constructor(private _emailTemplateService: EmailTemplateService, private _router: Router)
    {
    }

    ngOnInit(): void
    {
        this._emailTemplateService.templates$
            .subscribe((emailTemplates: EmailTemplate[]) => {
                this.emailTemplates = emailTemplates;
            });

        // Subscribe to search input field value changes
        // this.searchInputControl.valueChanges
        //     .pipe(
        //         takeUntil(this._unsubscribeAll),
        //         debounceTime(300),
        //         switchMap((query) =>
        //         {
        //             this.closeDetails();
        //             this.isLoading = true;
        //             return this._inventoryService.getProducts(0, 10, 'name', 'asc', query);
        //         }),
        //         map(() =>
        //         {
        //             this.isLoading = false;
        //         }),
        //     )
        //     .subscribe();
    }

    /**
     * Create product
     */
    createEmailTemplate(): void
    {
        // Create the product
        // this._inventoryService.createProduct().subscribe((newProduct) =>
        // {
        //     // Go to new product
        //     this.selectedProduct = newProduct;

        //     // Fill the form
        //     this.selectedProductForm.patchValue(newProduct);

        //     // Mark for check
        //     this._changeDetectorRef.markForCheck();
        // });
    }

    viewEmailDetails(row: EmailTemplate): void {
        this._router.navigateByUrl(`/emails/details/${row.id}`);
    }

    /**
     * Close the details
     */
    // closeDetails(): void
    // {
    //     this.selectedProduct = null;
    // }
       
}
