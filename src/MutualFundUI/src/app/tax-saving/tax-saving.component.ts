import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-tax-saving',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './tax-saving.component.html',
  styleUrls: ['./tax-saving.component.css']
})
export class TaxSavingComponent implements OnInit {
  annualIncome = 1000000;
  elssInvestment = 150000;
  taxSlab = 0;
  taxSaved = 0;
  effectiveCost = 0;
  elssFunds: any[] = [];

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.calcTax();
    this.apiService.listFunds('Equity').subscribe({
      next: (funds) => { this.elssFunds = funds.slice(0, 5); }
    });
  }

  calcTax() {
    if (this.annualIncome <= 250000) this.taxSlab = 0;
    else if (this.annualIncome <= 500000) this.taxSlab = 5;
    else if (this.annualIncome <= 1000000) this.taxSlab = 20;
    else this.taxSlab = 30;

    const investment = Math.min(this.elssInvestment, 150000);
    this.taxSaved = investment * (this.taxSlab / 100);
    this.effectiveCost = investment - this.taxSaved;
  }
}
