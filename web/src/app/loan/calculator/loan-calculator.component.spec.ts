import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoanCalculatorComponent } from './loan-calculator.component';

describe('LoanCalculatorComponent', () => {
  let component: LoanCalculatorComponent;
  let fixture: ComponentFixture<LoanCalculatorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoanCalculatorComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LoanCalculatorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
