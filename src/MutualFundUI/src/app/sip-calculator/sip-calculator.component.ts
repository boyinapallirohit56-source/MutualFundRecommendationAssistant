import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-sip-calculator',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container" style="margin-top:24px; max-width:900px">
      <div class="page-header">
        <h1>Investment Calculators</h1>
        <p>Plan your investments with these tools</p>
      </div>

      <!-- Tab Navigation -->
      <div class="calc-tabs">
        <button [class.active]="activeTab === 'sip'" (click)="activeTab = 'sip'">SIP Calculator</button>
        <button [class.active]="activeTab === 'goal'" (click)="activeTab = 'goal'">Goal Planner</button>
        <button [class.active]="activeTab === 'emi'" (click)="activeTab = 'emi'">EMI vs SIP</button>
      </div>

      <!-- SIP Calculator -->
      <div *ngIf="activeTab === 'sip'" class="animate-in">
        <div class="grid-2">
          <div class="card">
            <h3 class="card-title">SIP Calculator</h3>
            <p class="card-desc">How much will my SIP grow to?</p>
            <div class="form-group">
              <label>Monthly Investment (Rs.)</label>
              <input type="number" [(ngModel)]="sip.monthly" (ngModelChange)="calcSIP()">
              <input type="range" [(ngModel)]="sip.monthly" min="500" max="100000" step="500" class="range-slider" (ngModelChange)="calcSIP()">
            </div>
            <div class="form-group">
              <label>Duration: {{ sip.years }} years</label>
              <input type="range" [(ngModel)]="sip.years" min="1" max="40" class="range-slider" (ngModelChange)="calcSIP()">
            </div>
            <div class="form-group">
              <label>Expected Return: {{ sip.returnRate }}%</label>
              <input type="range" [(ngModel)]="sip.returnRate" min="4" max="25" step="0.5" class="range-slider" (ngModelChange)="calcSIP()">
            </div>
          </div>
          <div class="card result-card">
            <div class="result-main">
              <span class="result-label">Future Value</span>
              <span class="result-value">Rs. {{ sip.futureValue | number:'1.0-0' }}</span>
            </div>
            <div class="result-row"><span>Total Invested</span><span>Rs. {{ sip.totalInvested | number:'1.0-0' }}</span></div>
            <div class="result-row gain"><span>Wealth Gained</span><span>Rs. {{ sip.gained | number:'1.0-0' }}</span></div>
            <div class="result-row"><span>Growth</span><span>{{ sip.growthX }}x ({{ sip.gainPct }}%)</span></div>
            <div class="visual-bar mt-4">
              <div class="bar-invested" [style.width.%]="sip.investedPct"></div>
              <div class="bar-gained" [style.width.%]="sip.gainedPct"></div>
            </div>
            <div class="bar-legend">
              <span class="legend-invested">Invested ({{ sip.investedPct | number:'1.0-0' }}%)</span>
              <span class="legend-gained">Gained ({{ sip.gainedPct | number:'1.0-0' }}%)</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Goal Planner (Reverse SIP) -->
      <div *ngIf="activeTab === 'goal'" class="animate-in">
        <div class="grid-2">
          <div class="card">
            <h3 class="card-title">Goal Planner</h3>
            <p class="card-desc">How much should I invest monthly to reach my goal?</p>
            <div class="form-group">
              <label>Target Amount (Rs.)</label>
              <input type="number" [(ngModel)]="goal.target" (ngModelChange)="calcGoal()">
            </div>
            <div class="form-group">
              <label>Time to Achieve: {{ goal.years }} years</label>
              <input type="range" [(ngModel)]="goal.years" min="1" max="35" class="range-slider" (ngModelChange)="calcGoal()">
            </div>
            <div class="form-group">
              <label>Expected Return: {{ goal.returnRate }}%</label>
              <input type="range" [(ngModel)]="goal.returnRate" min="6" max="20" step="0.5" class="range-slider" (ngModelChange)="calcGoal()">
            </div>
            <div class="goal-presets">
              <button (click)="setGoal('Retirement', 10000000, 25)">Retirement (1Cr)</button>
              <button (click)="setGoal('Child Education', 5000000, 15)">Education (50L)</button>
              <button (click)="setGoal('Home', 3000000, 10)">Home (30L)</button>
              <button (click)="setGoal('Car', 1000000, 5)">Car (10L)</button>
            </div>
          </div>
          <div class="card result-card">
            <div class="result-main">
              <span class="result-label">Monthly SIP Needed</span>
              <span class="result-value">Rs. {{ goal.monthlySIP | number:'1.0-0' }}</span>
            </div>
            <div class="result-row"><span>Target Amount</span><span>Rs. {{ goal.target | number:'1.0-0' }}</span></div>
            <div class="result-row"><span>Total You'll Invest</span><span>Rs. {{ goal.totalInvested | number:'1.0-0' }}</span></div>
            <div class="result-row gain"><span>Market Will Add</span><span>Rs. {{ goal.marketGain | number:'1.0-0' }}</span></div>
            <div class="result-row"><span>Time</span><span>{{ goal.years }} years ({{ goal.years * 12 }} months)</span></div>
            <p class="tip-text mt-4" *ngIf="goal.goalName">
              To reach your {{ goal.goalName }} goal of Rs.{{ goal.target | number:'1.0-0' }} in {{ goal.years }} years, start a SIP of Rs.{{ goal.monthlySIP | number:'1.0-0' }}/month today.
            </p>
          </div>
        </div>
      </div>

      <!-- EMI vs SIP Comparison -->
      <div *ngIf="activeTab === 'emi'" class="animate-in">
        <div class="grid-2">
          <div class="card">
            <h3 class="card-title">EMI vs SIP</h3>
            <p class="card-desc">What if you invested your idle money instead of keeping it in savings?</p>
            <div class="form-group">
              <label>Monthly Amount (Rs.)</label>
              <input type="number" [(ngModel)]="emi.monthly" (ngModelChange)="calcEMI()">
            </div>
            <div class="form-group">
              <label>Duration: {{ emi.years }} years</label>
              <input type="range" [(ngModel)]="emi.years" min="1" max="20" class="range-slider" (ngModelChange)="calcEMI()">
            </div>
            <div class="form-group">
              <label>Savings Account Rate: {{ emi.savingsRate }}%</label>
              <input type="range" [(ngModel)]="emi.savingsRate" min="2" max="7" step="0.5" class="range-slider" (ngModelChange)="calcEMI()">
            </div>
            <div class="form-group">
              <label>Mutual Fund Return: {{ emi.mfRate }}%</label>
              <input type="range" [(ngModel)]="emi.mfRate" min="8" max="20" step="0.5" class="range-slider" (ngModelChange)="calcEMI()">
            </div>
          </div>
          <div class="card result-card">
            <div class="comparison-boxes">
              <div class="comp-box savings">
                <span class="comp-label">Savings Account</span>
                <span class="comp-value">Rs. {{ emi.savingsValue | number:'1.0-0' }}</span>
                <span class="comp-rate">at {{ emi.savingsRate }}% per year</span>
              </div>
              <div class="comp-vs">VS</div>
              <div class="comp-box mutual-fund">
                <span class="comp-label">Mutual Fund SIP</span>
                <span class="comp-value">Rs. {{ emi.mfValue | number:'1.0-0' }}</span>
                <span class="comp-rate">at {{ emi.mfRate }}% per year</span>
              </div>
            </div>
            <div class="opportunity-cost">
              <span>You're missing out on</span>
              <strong>Rs. {{ emi.difference | number:'1.0-0' }}</strong>
              <span>by keeping money in savings</span>
            </div>
            <p class="tip-text mt-4">
              Investing Rs.{{ emi.monthly | number:'1.0-0' }}/month in mutual funds instead of savings could give you {{ emi.extraPercent }}% more wealth over {{ emi.years }} years.
            </p>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .calc-tabs { display: flex; gap: 4px; margin-bottom: 24px; background: #f3f4f6; padding: 4px; border-radius: 10px; }
    .calc-tabs button { flex: 1; padding: 10px; border: none; background: transparent; border-radius: 8px; font-size: 14px; font-weight: 600; cursor: pointer; color: #6b7280; transition: all 0.2s; }
    .calc-tabs button.active { background: white; color: #1e40af; box-shadow: 0 2px 4px rgba(0,0,0,0.08); }
    .card-title { font-size: 18px; font-weight: 700; color: #111827; margin-bottom: 4px; }
    .card-desc { font-size: 13px; color: #6b7280; margin-bottom: 20px; }
    .range-slider { width: 100%; margin-top: 8px; accent-color: #1e40af; }
    .result-card { background: linear-gradient(135deg, #f8fafc, #eff6ff); border: 1px solid #dbeafe; }
    .result-main { text-align: center; padding: 24px 0 20px; margin-bottom: 16px; border-bottom: 1px solid #e5e7eb; }
    .result-label { display: block; font-size: 12px; color: #6b7280; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px; }
    .result-value { display: block; font-size: 32px; font-weight: 800; color: #1e40af; margin-top: 4px; }
    .result-row { display: flex; justify-content: space-between; padding: 8px 0; font-size: 14px; color: #4b5563; border-bottom: 1px solid #f3f4f6; }
    .result-row.gain { color: #059669; font-weight: 600; }
    .visual-bar { display: flex; height: 12px; border-radius: 6px; overflow: hidden; background: #e5e7eb; }
    .bar-invested { background: #93c5fd; }
    .bar-gained { background: #34d399; }
    .bar-legend { display: flex; justify-content: space-between; margin-top: 6px; font-size: 11px; }
    .legend-invested { color: #2563eb; }
    .legend-gained { color: #059669; }
    .goal-presets { display: grid; grid-template-columns: 1fr 1fr; gap: 8px; margin-top: 16px; }
    .goal-presets button { padding: 10px; background: white; border: 1px solid #e5e7eb; border-radius: 8px; font-size: 12px; cursor: pointer; transition: all 0.2s; }
    .goal-presets button:hover { border-color: #1e40af; background: #eff6ff; }
    .tip-text { font-size: 13px; color: #4b5563; background: white; padding: 12px; border-radius: 8px; border: 1px solid #e5e7eb; line-height: 1.6; }
    .comparison-boxes { display: flex; align-items: center; gap: 12px; margin-bottom: 20px; }
    .comp-box { flex: 1; text-align: center; padding: 20px 12px; border-radius: 12px; }
    .comp-box.savings { background: #fef3c7; border: 1px solid #fcd34d; }
    .comp-box.mutual-fund { background: #d1fae5; border: 1px solid #6ee7b7; }
    .comp-label { display: block; font-size: 11px; font-weight: 600; text-transform: uppercase; color: #6b7280; }
    .comp-value { display: block; font-size: 20px; font-weight: 800; color: #111827; margin: 4px 0; }
    .comp-rate { display: block; font-size: 11px; color: #6b7280; }
    .comp-vs { font-size: 14px; font-weight: 800; color: #6b7280; }
    .opportunity-cost { text-align: center; padding: 16px; background: #fef2f2; border-radius: 10px; border: 1px solid #fecaca; }
    .opportunity-cost span { display: block; font-size: 13px; color: #6b7280; }
    .opportunity-cost strong { display: block; font-size: 24px; color: #dc2626; font-weight: 800; margin: 4px 0; }
  `]
})
export class SipCalculatorComponent {
  activeTab = 'sip';

  // SIP Calculator
  sip = { monthly: 10000, years: 10, returnRate: 12, futureValue: 0, totalInvested: 0, gained: 0, growthX: '', gainPct: '', investedPct: 0, gainedPct: 0 };

  // Goal Planner (Reverse SIP)
  goal = { target: 10000000, years: 20, returnRate: 12, monthlySIP: 0, totalInvested: 0, marketGain: 0, goalName: 'Retirement' };

  // EMI vs SIP
  emi = { monthly: 5000, years: 5, savingsRate: 3.5, mfRate: 12, savingsValue: 0, mfValue: 0, difference: 0, extraPercent: '' };

  constructor() {
    this.calcSIP();
    this.calcGoal();
    this.calcEMI();
  }

  calcSIP() {
    const P = this.sip.monthly;
    const n = this.sip.years * 12;
    const r = this.sip.returnRate / 100 / 12;
    this.sip.futureValue = P * (((Math.pow(1 + r, n) - 1) / r) * (1 + r));
    this.sip.totalInvested = P * n;
    this.sip.gained = this.sip.futureValue - this.sip.totalInvested;
    this.sip.growthX = (this.sip.futureValue / this.sip.totalInvested).toFixed(1);
    this.sip.gainPct = ((this.sip.gained / this.sip.totalInvested) * 100).toFixed(0);
    this.sip.investedPct = (this.sip.totalInvested / this.sip.futureValue) * 100;
    this.sip.gainedPct = (this.sip.gained / this.sip.futureValue) * 100;
  }

  calcGoal() {
    const FV = this.goal.target;
    const n = this.goal.years * 12;
    const r = this.goal.returnRate / 100 / 12;
    // Reverse SIP formula: P = FV * r / [((1+r)^n - 1) * (1+r)]
    this.goal.monthlySIP = FV * r / (((Math.pow(1 + r, n) - 1)) * (1 + r));
    this.goal.totalInvested = this.goal.monthlySIP * n;
    this.goal.marketGain = FV - this.goal.totalInvested;
  }

  setGoal(name: string, target: number, years: number) {
    this.goal.goalName = name;
    this.goal.target = target;
    this.goal.years = years;
    this.calcGoal();
  }

  calcEMI() {
    const P = this.emi.monthly;
    const n = this.emi.years * 12;
    const rSav = this.emi.savingsRate / 100 / 12;
    const rMf = this.emi.mfRate / 100 / 12;
    this.emi.savingsValue = P * (((Math.pow(1 + rSav, n) - 1) / rSav) * (1 + rSav));
    this.emi.mfValue = P * (((Math.pow(1 + rMf, n) - 1) / rMf) * (1 + rMf));
    this.emi.difference = this.emi.mfValue - this.emi.savingsValue;
    this.emi.extraPercent = ((this.emi.difference / this.emi.savingsValue) * 100).toFixed(0);
  }
}
