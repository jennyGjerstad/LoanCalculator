import { AfterViewInit, Component, Input, OnChanges, OnDestroy, SimpleChanges, ViewChild } from '@angular/core';
import { LoanCalculationRequest, LoanType, Payment, PaymentPlan, PaymentScheme } from '../../models/loan/loan.models';
import { Subscription, firstValueFrom } from 'rxjs';
import { LoanService } from '../../services/loan/loan.services';
import { Month } from '../../enums/loan/month.enums';
import { FormControl, Validators } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'loan-calculator',
  templateUrl: './loan-calculator.component.html',
  styleUrl: './loan-calculator.component.scss'
})
export class LoanCalculatorComponent implements OnDestroy, OnChanges, AfterViewInit {
  @Input({ required: true}) loanType: LoanType | undefined;
  protected schemes: PaymentScheme[] = [];
  protected loanRequest?: LoanCalculationRequest;
  protected paymentPlan: PaymentPlan = [];
  protected paymentScheme: PaymentScheme | undefined = undefined;
  protected amountControl: FormControl<number> = new FormControl(0, { nonNullable: true, validators: [Validators.min(1), Validators.required]});
  protected yearsControl: FormControl<number> = new FormControl(0, { nonNullable: true, validators: [Validators.min(1), Validators.required]});
  protected get amount() { return this.amountControl.value; }
  protected get years() { return this.yearsControl.value; }
  protected months: typeof Month = Month;
  protected pageIndex: number = 0;

  protected dataSource = new MatTableDataSource<Payment>(this.paymentPlan);
  @ViewChild(MatPaginator) paginator: MatPaginator = null!;

  protected readonly displayedColumns = ['month', 'totalAmount', 'interestAmount', 'principalAmount', 'outstandingAmount'];

  private readonly subscriptions: Subscription[] = [];

  constructor(private readonly loanService: LoanService) { }

  async ngOnChanges(changes: SimpleChanges) {
    if ('loanType' in changes) {
      if (this.loanType) {
        this.schemes = await firstValueFrom(this.loanService.getPaymentSchemes(this.loanType.id));
      }
      if (this.schemes.length > 0) {
        this.paymentScheme = this.schemes[0];
      } else {
        this.paymentScheme = undefined;
        this.paymentPlan = [];
      }
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  async getTable() {
    if(this.amount > 0 && this.years > 0 && this.loanType && this.paymentScheme) {
      this.loanRequest = {
        loanAmount: this.amount,
        paybackTime: this.years,
        loanTypeId: this.loanType.id,
        paymentSchemeId: this.paymentScheme.id
      }
    }
    if (this.loanRequest) {
      try {
        this.pageIndex = 0;
        this.paginator.pageIndex = 0; // Reset paginator to the first page
        this.dataSource.paginator = this.paginator; // Update paginator
        this.paymentPlan = await firstValueFrom(this.loanService.getLoanTable(this.loanRequest));
        this.dataSource.data = this.paymentPlan;
      } catch (error){
        console.error('Error fetching loan table:', error);
      }
    }
  }

  onPageChange(pageEvent: PageEvent) {
    this.pageIndex = pageEvent.pageIndex;
  }

  onPaymentSchemeChange() {
    this.paymentPlan = [];
  }

  onKeydown(event: KeyboardEvent) {
    if (event.key === "Enter") {
      this.getTable();
    }
  }
}
