import { Month } from "../../enums/loan/month.enums";

// Requests
export type LoanCalculationRequest = {
    loanAmount: number;
    paybackTime: number;
    loanTypeId: number;
    paymentSchemeId: number;
}

export type Payment = {
    month: Month;
    totalAmount: number;
    interestAmount: number;
    principalAmount: number;
    outstandingAmount: number;
}

// General Types
export type RegisterItem = { id: number; }
export type NumberItem = { value: number; }
export type DisplayItem = { displayValue: string; }
export type DescriptionItem = { description: string; }

export type LoanType = RegisterItem & DisplayItem & NumberItem;
export type PaymentScheme = RegisterItem & DisplayItem & DescriptionItem;
export type PaymentPlan = Payment[];