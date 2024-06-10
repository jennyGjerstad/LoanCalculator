import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { LoanCalculatorModule } from './loan/loan.module';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
  standalone: true,
  imports: [MatCardModule, LoanCalculatorModule],
})
export class AppComponent {
  title = 'loancalculator';
}
