import { Component } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { LoanCalculatorModule } from './loan/loan.module';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: true,
  imports: [MatToolbarModule, LoanCalculatorModule],
})
export class AppComponent {
  title = 'loancalculator';
}
