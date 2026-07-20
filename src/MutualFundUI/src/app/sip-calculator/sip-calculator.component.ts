import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-sip-calculator',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container" style="margin-top:24px; max-width:800px">
      <div class="page-header">
        <h1>SIP Calculator</h1>
        <p>Calculate the future value of your Systematic Investment Plan</p>
      </div>

      <div class="grid-2">
        <!-- Input Card -->
        <div class="card">
          <h3 class="card-title">Enter Details</h3>

          <div class="form-group">
            <label>Monthly SIP Amount (Rs.)</label>
            <input type="number" [(ngModel)]="monthlyAmount" placeholder="10000" (ngModelChange)="calculate()">
          </div>

          <div class="form-group">
            <label>Investment Duration (Years)</label>
            <input type="number" [(ngModel)]="years" placeholder="10" (ngModelChange)="calculate()">
            <input type="range" [(ngModel)]="years" min="1" max="40" class="range-slider" (ngModelChange)="calculate()">
            <span class="range-value">{{ years }} years</span>
          </div>

          <div class="form-group">
            <label>Expected Annual Return (%)</label>
            <input type="number" [(ngModel)]="expectedReturn" placeholder="12" step="0.5" (ngModelChange)="calculate()">
            <input type="range" [(ngModel)]="expectedReturn" min="4" max="30" step="0.5" class="range-slider" (ngModelChange)="calculate()">
            <span class="range-value">{{ expectedReturn }}% per year</span>
          </div>

          <button class="btn btn-primary" style="width:100%" (click)="calculate()">Calculate</button>
        </div>

        <!-- Result Card -->
        <div class="card result-card" *ngIf="calculated">
          <h3 class="card-title">Results</h3>

          <div class="result-main">
            <span class="result-label">Future Value</span>
            <span class="result-value">Rs. {{ futureValue | number:'1.0-0' }}</span>
          </div>

          <div class="result-breakdown">
            <div class="breakdown-item">
              <span class="breakdown-label">Total Invested</span>
              <span class="breakdown-value">Rs. {{ totalInvested | number:'1.0-0' }}</span>
              <div class="breakdown-bar">
                <div class="bar-fill invested" [style.width.%]="investedPercentage"></div>
              </div>
            </div>
            <div class="breakdown-item">
              <span class="breakdown-label">Wealth Gained</span>
              <span class="breakdown-value gain">Rs. {{ wealthGained | number:'1.0-0' }}</span>
              <div class="breakdown-bar">
                <div class="bar-fill gained" [style.width.%]="gainedPercentage"></div>
              </div>
            </div>
          </div>

          <div class="result-stats">
            <div class="stat-mini">
              <span class="stat-mini-label">Total Months</span>
              <span class="stat-mini-value">{{ years * 12 }}</span>
            </div>
            <div class="stat-mini">
              <span class="stat-mini-label">Growth Multiple</span>
              <span class="stat-mini-value">{{ growthMultiple }}x</span>
            </div>
            <div class="stat-mini">
              <span class="stat-mini-label">Gain %</span>
              <span class="stat-mini-value gain">{{ gainPercentage }}%</span>
            </div>
          </div>

          <p class="disclaimer-text">
            This is an estimate based on assumed constant returns. Actual mutual fund returns vary and are not guaranteed.
          </p>
        </div>
      </div>

      <!-- Quick Examples -->
      <div class="card mt-4">
        <h3 class="card-title">Quick Examples</h3>
        <div class="examples-grid">
          <button class="example-btn" (click)="setExample(5000, 5, 12)">
            <strong>Rs.5,000/mo</strong> for 5 years at 12%
          </button>
          <button class="example-btn" (click)="setExample(10000, 10, 12)">
            <strong>Rs.10,000/mo</strong> for 10 years at 12%
          </button>
          <button class="example-btn" (click)="setExample(25000, 15, 14)">
            <strong>Rs.25,000/mo</strong> for 15 years at 14%
          </button>
          <button class="example-btn" (click)="setExample(50000, 20, 12)">
            <strong>Rs.50,000/mo</strong> for 20 years at 12%
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .card-title { font-size: 16px; font-weight: 700; margin-bottom: 20px; color: #111827; }
    .range-slider { width: 100%; margin-top: 8px; accent-color: #1e40af; }
    .range-value { font-size: 12px; color: #6b7280; display: block; margin-top: 4px; }
    .result-card { background: linear-gradient(135deg, #f8fafc, #eff6ff); border: 1px solid #dbeafe; }
    .result-main { text-align: center; padding: 24px 0; margin-bottom: 20px; border-bottom: 1px solid #e5e7eb; }
    .result-label { display: block; font-size: 13px; color: #6b7280; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px; }
    .result-value { display: block; font-size: 36px; font-weight: 800; color: #1e40af; margin-top: 4px; }
    .result-breakdown { margin-bottom: 20px; }
    .breakdown-item { margin-bottom: 16px; }
    .breakdown-label { font-size: 13px; color: #6b7280; }
    .breakdown-value { float: right; font-size: 13px; font-weight: 700; color: #374151; }
    .breakdown-value.gain { color: #059669; }
    .breakdown-bar { height: 8px; background: #e5e7eb; border-radius: 4px; margin-top: 6px; clear: both; }
    .bar-fill { height: 100%; border-radius: 4px; transition: width 0.5s ease; }
    .bar-fill.invested { background: #93c5fd; }
    .bar-fill.gained { background: #34d399; }
    .result-stats { display: grid; grid-template-columns: 1fr 1fr 1fr; gap: 12px; margin-bottom: 16px; }
    .stat-mini { text-align: center; padding: 12px; background: white; border-radius: 8px; border: 1px solid #e5e7eb; }
    .stat-mini-label { display: block; font-size: 11px; color: #6b7280; margin-bottom: 2px; }
    .stat-mini-value { display: block; font-size: 16px; font-weight: 700; color: #1f2937; }
    .stat-mini-value.gain { color: #059669; }
    .disclaimer-text { font-size: 11px; color: #9ca3af; font-style: italic; text-align: center; }
    .examples-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }
    .example-btn { padding: 14px; background: white; border: 1px solid #e5e7eb; border-radius: 8px; cursor: pointer; font-size: 13px; color: #374151; transition: all 0.2s; text-align: left; }
    .example-btn:hover { border-color: #1e40af; background: #eff6ff; }
    .example-btn strong { display: block; color: #1e40af; margin-bottom: 2px; }
  `]
})
export class SipCalculatorComponent {
  monthlyAmount = 10000;
  years = 10;
  expectedReturn = 12;
  calculated = true;

  futureValue = 0;
  totalInvested = 0;
  wealthGained = 0;
  investedPercentage = 0;
  gainedPercentage = 0;
  growthMultiple = '';
  gainPercentage = '';

  constructor() {
    this.calculate();
  }

  calculate() {
    if (!this.monthlyAmount || !this.years || !this.expectedReturn) return;

    const P = this.monthlyAmount;
    const n = this.years * 12; // total months
    const r = this.expectedReturn / 100 / 12; // monthly rate

    // SIP Future Value formula: FV = P * [((1 + r)^n - 1) / r] * (1 + r)
    this.futureValue = P * (((Math.pow(1 + r, n) - 1) / r) * (1 + r));
    this.totalInvested = P * n;
    this.wealthGained = this.futureValue - this.totalInvested;

    this.investedPercentage = (this.totalInvested / this.futureValue) * 100;
    this.gainedPercentage = (this.wealthGained / this.futureValue) * 100;
    this.growthMultiple = (this.futureValue / this.totalInvested).toFixed(1);
    this.gainPercentage = ((this.wealthGained / this.totalInvested) * 100).toFixed(0);

    this.calculated = true;
  }

  setExample(amount: number, years: number, returnRate: number) {
    this.monthlyAmount = amount;
    this.years = years;
    this.expectedReturn = returnRate;
    this.calculate();
  }
}
