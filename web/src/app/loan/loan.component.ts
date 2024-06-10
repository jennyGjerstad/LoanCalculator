import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { LoanType } from '../models/loan/loan.models';
import { LoanService } from '../services/loan/loan.services';

@Component({
  selector: 'loan',
  templateUrl: './loan.component.html',
})
export class LoanComponent implements OnInit, OnDestroy {
  protected loanTypes: LoanType[] = [];
  protected selectedType: LoanType | undefined = undefined;
  private readonly subscriptions: Subscription[] = [];

  constructor(private readonly loanService: LoanService) {}

  ngOnInit(): void {
    this.loanService.getLoans().subscribe({
      next: (loanTypes) => {
        this.loanTypes = [...loanTypes];
        if (loanTypes.length > 0) {
          this.selectedType = loanTypes[0];
        }
      },
      error: (error) => console.error(error),
    });
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((s) => s.unsubscribe());
  }
}
