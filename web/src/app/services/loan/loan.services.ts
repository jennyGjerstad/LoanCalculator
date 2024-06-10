import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { LoanCalculationRequest, LoanType, PaymentPlan, PaymentScheme } from "../../models/loan/loan.models"
import { API_URL } from "../../../configuration/configuration";

@Injectable({
    providedIn: "root",
})
export class LoanService {
    private loanUrl: string = "";
    constructor(
        @Inject(API_URL) private readonly apiUrl: string,
        private http: HttpClient
    ) { this.loanUrl = `${this.apiUrl}/loans` }

    getLoans(): Observable<LoanType[]> {
        return this.http.get<LoanType[]>(`${this.loanUrl}`);
    }

    getLoan(loanId: number): Observable<LoanType> {
        return this.http.get<LoanType>(`${this.loanUrl}/${loanId}`);
    }

    getLoanTable(request: LoanCalculationRequest): Observable<PaymentPlan> {
        const headers = new HttpHeaders({ 'Content-Type': 'application/json'});
        return this.http.post<PaymentPlan>(`${this.loanUrl}/table`, request, { headers });
    }

    getPaymentSchemes(loanTypeId: number): Observable<PaymentScheme[]> {
        return this.http.get<PaymentScheme[]>(`${this.loanUrl}/${loanTypeId}/paymentschemes`);
    }
}