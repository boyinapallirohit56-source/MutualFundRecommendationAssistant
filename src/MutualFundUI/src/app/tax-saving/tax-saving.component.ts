import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-tax-saving',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container" style="margin-top:24px; max-width:800px">
      <div class="page-header">
        <h1>Tax Saving (Section 80C)</h1>
        <p>Save taxes by investing in ELSS mutual funds</p>
      </div>

      <!-- Tax Calculator -->
      <div class="card">
        <h3 style="font-size:16px; margin-bottom:16px">Tax Savings Calculator</h3>
        <div class="grid-2">
          <div class="form-group">
            <label>Your Annual Income (Rs.)</label>
            <input type="number" [(ngModel)]="annualIncome" (ngModelChange)="calcTax()">
          </div>
          <div class="form-group">
            <label>ELSS Investment (Rs.) (Max 1,50,000)</label>
            <input type="number" [(ngModel)]="elssInvestment" max="150000" (ngModelChange)="calcTax()">
            <input type="range" [(ngModel)]="elssInvestment" min="0" max="150000" step="5000" class="range-slider" (ngModelChange)="calcTax()">
          </div>
        </div>

        <div class="tax-result" *ngIf="taxSaved > 0">
          <div class="grid-2" style="grid-template-columns: 1fr 1fr 1fr">
            <div class="stat-box">
              <span class="stat-label">Tax Slab</span>
              <span class="stat-value">{{ taxSlab }}%</span>
            </div>
            <div class="stat-box highlight">
              <span class="stat-label">Tax Saved</span>
              <span class="stat-value">Rs. {{ taxSaved | number:'1.0-0' }}</span>
            </div>
            <div class="stat-box">
              <span class="stat-label">Effective Cost</span>
              <span class="stat-value">Rs. {{ effectiveCost | number:'1.0-0' }}</span>
            </div>
          </div>
          <p class="tax-tip mt-4">
            By investing Rs.{{ elssInvestment | number:'1.0-0' }} in ELSS, you save Rs.{{ taxSaved | number:'1.0-0' }} in taxes.
            Your actual investment cost is only Rs.{{ effectiveCost | number:'1.0-0' }}. Plus, your money grows at ~12-15% per year!
          </p>
        </div>
      </div>

      <!-- ELSS Info -->
      <div class="card">
        <h3 style="font-size:16px; margin-bottom:12px">Why ELSS?</h3>
        <div class="info-grid">
          <div class="info-item">
            <strong>3 Year Lock-in</strong>
            <p>Shortest lock-in among all 80C options (PPF is 15 years, FD is 5 years)</p>
          </div>
          <div class="info-item">
            <strong>Equity Returns</strong>
            <p>Invests in stocks, historically gives 12-15% returns (vs 7-8% for FD/PPF)</p>
          </div>
          <div class="info-item">
            <strong>Rs.1.5 Lakh Limit</strong>
            <p>Maximum deduction under Section 80C per financial year</p>
          </div>
          <div class="info-item">
            <strong>SIP Option</strong>
            <p>Invest Rs.12,500/month to max out your 80C limit through SIP</p>
          </div>
        </div>
      </div>

      <!-- ELSS Funds -->
      <div class="card">
        <h3 style="font-size:16px; margin-bottom:16px">Top ELSS Funds</h3>
        <div class="fund-item" *ngFor="let fund of elssFunds">
          <div class="fund-info">
            <strong>{{ fund.name }}</strong>
            <span>{{ fund.amc }}</span>
          </div>
          <div class="fund-metrics">
            <span class="metric">3Y: {{ fund.cagr3Y }}%</span>
            <span class="metric">Rating: {{ fund.rating }} ★</span>
          </div>
        </div>
        <p *ngIf="!elssFunds.length" style="color:#6b7280; font-size:13px">No ELSS funds found in database.</p>
      </div>
    </div>
  `,
  styles: [`
    .range-slider { width: 100%; margin-top: 8px; accent-color: #1e40af; }
    .tax-result { margin-top: 20px; }
    .stat-box.highlight { background: #ecfdf5; border-color: #a7f3d0; }
    .stat-box.highlight .stat-value { color: #059669; }
    .tax-tip { font-size: 13px; color: #4b5563; background: #f9fafb; padding: 12px; border-radius: 8px; line-height: 1.6; }
    .info-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
    .info-item { padding: 16px; background: #f9fafb; border-radius: 8px; border: 1px solid #f3f4f6; }
    .info-item strong { display: block; font-size: 14px; color: #1e40af; margin-bottom: 4px; }
    .info-item p { font-size: 13px; color: #6b7280; line-height: 1.5; }
    .fund-item { display: flex; justify-content: space-between; align-items: center; padding: 12px 0; border-bottom: 1px solid #f3f4f6; }
    .fund-item:last-child { border-bottom: none; }
    .fund-info strong { display: block; font-size: 14px; color: #111827; }
    .fund-info span { font-size: 12px; color: #6b7280; }
    .fund-metrics { display: flex; gap: 16px; }
    .metric { font-size: 13px; font-weight: 600; color: #059669; }
  `]
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
    // Load ELSS funds (equity funds that could be ELSS)
    this.apiService.listFunds('Equity').subscribe({
      next: (funds) => { this.elssFunds = funds.slice(0, 5); }
    });
  }

  calcTax() {
    // Determine tax slab (Old regime simplified)
    if (this.annualIncome <= 250000) this.taxSlab = 0;
    else if (this.annualIncome <= 500000) this.taxSlab = 5;
    else if (this.annualIncome <= 1000000) this.taxSlab = 20;
    else this.taxSlab = 30;

    const investment = Math.min(this.elssInvestment, 150000);
    this.taxSaved = investment * (this.taxSlab / 100);
    this.effectiveCost = investment - this.taxSaved;
  }
}
