import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-what-if',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container" style="margin-top:24px; max-width:800px">
      <div class="page-header">
        <h1>What If I Had Started Earlier?</h1>
        <p>See how much you'd have today if you started investing in the past</p>
      </div>

      <div class="grid-2">
        <div class="card">
          <h3 class="card-title">Set Parameters</h3>
          <div class="form-group">
            <label>Monthly SIP (Rs.)</label>
            <input type="number" [(ngModel)]="monthly" (ngModelChange)="calculate()">
          </div>
          <div class="form-group">
            <label>If I had started {{ yearsAgo }} years ago</label>
            <input type="range" [(ngModel)]="yearsAgo" min="1" max="25" class="range-slider" (ngModelChange)="calculate()">
          </div>
          <div class="form-group">
            <label>Assumed Market Return: {{ returnRate }}%</label>
            <input type="range" [(ngModel)]="returnRate" min="8" max="20" step="0.5" class="range-slider" (ngModelChange)="calculate()">
          </div>
          <div class="presets">
            <button (click)="setPreset(5000, 3)">Rs.5K, 3 yrs ago</button>
            <button (click)="setPreset(10000, 5)">Rs.10K, 5 yrs ago</button>
            <button (click)="setPreset(10000, 10)">Rs.10K, 10 yrs ago</button>
            <button (click)="setPreset(25000, 15)">Rs.25K, 15 yrs ago</button>
          </div>
        </div>

        <div class="card result-card">
          <div class="whatif-result">
            <p class="whatif-intro">If you had started a SIP of Rs.{{ monthly | number:'1.0-0' }}/month</p>
            <p class="whatif-ago">{{ yearsAgo }} years ago...</p>
            <div class="whatif-value">
              <span class="label">You'd have today</span>
              <span class="value">Rs. {{ futureValue | number:'1.0-0' }}</span>
            </div>
            <div class="whatif-details">
              <div class="detail-row">
                <span>Total invested</span>
                <span>Rs. {{ totalInvested | number:'1.0-0' }}</span>
              </div>
              <div class="detail-row gain">
                <span>Market would've added</span>
                <span>Rs. {{ gained | number:'1.0-0' }}</span>
              </div>
            </div>
            <div class="whatif-moral">
              <p>The best time to start was {{ yearsAgo }} years ago.</p>
              <p><strong>The second best time is today.</strong></p>
            </div>
            <div class="start-today">
              <p>If you start today with Rs.{{ monthly | number:'1.0-0' }}/month:</p>
              <p>In {{ yearsAgo }} years you'll have <strong>Rs. {{ futureIfStartNow | number:'1.0-0' }}</strong></p>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .card-title { font-size: 18px; font-weight: 700; margin-bottom: 20px; }
    .range-slider { width: 100%; margin-top: 8px; accent-color: #1e40af; }
    .presets { display: grid; grid-template-columns: 1fr 1fr; gap: 8px; margin-top: 16px; }
    .presets button { padding: 10px; background: white; border: 1px solid #e5e7eb; border-radius: 8px; font-size: 12px; cursor: pointer; }
    .presets button:hover { border-color: #1e40af; background: #eff6ff; }
    .result-card { background: linear-gradient(135deg, #f0fdf4, #ecfdf5); border: 1px solid #a7f3d0; }
    .whatif-result { text-align: center; }
    .whatif-intro { font-size: 14px; color: #6b7280; }
    .whatif-ago { font-size: 20px; font-weight: 700; color: #374151; margin-bottom: 16px; }
    .whatif-value { padding: 20px; background: white; border-radius: 12px; margin-bottom: 16px; border: 1px solid #d1fae5; }
    .whatif-value .label { display: block; font-size: 12px; color: #6b7280; text-transform: uppercase; }
    .whatif-value .value { display: block; font-size: 32px; font-weight: 800; color: #059669; }
    .whatif-details { margin-bottom: 16px; }
    .detail-row { display: flex; justify-content: space-between; padding: 6px 0; font-size: 13px; color: #4b5563; }
    .detail-row.gain { color: #059669; font-weight: 600; }
    .whatif-moral { background: #fffbeb; border: 1px solid #fcd34d; border-radius: 8px; padding: 12px; margin-bottom: 12px; font-size: 13px; color: #92400e; }
    .whatif-moral p { margin-bottom: 4px; }
    .start-today { background: white; border: 1px solid #e5e7eb; border-radius: 8px; padding: 12px; font-size: 13px; color: #4b5563; }
    .start-today strong { color: #1e40af; }
  `]
})
export class WhatIfComponent {
  monthly = 10000;
  yearsAgo = 5;
  returnRate = 12;
  futureValue = 0;
  totalInvested = 0;
  gained = 0;
  futureIfStartNow = 0;

  constructor() { this.calculate(); }

  calculate() {
    const P = this.monthly;
    const n = this.yearsAgo * 12;
    const r = this.returnRate / 100 / 12;
    this.futureValue = P * (((Math.pow(1 + r, n) - 1) / r) * (1 + r));
    this.totalInvested = P * n;
    this.gained = this.futureValue - this.totalInvested;
    this.futureIfStartNow = this.futureValue; // Same calculation for future
  }

  setPreset(monthly: number, years: number) {
    this.monthly = monthly;
    this.yearsAgo = years;
    this.calculate();
  }
}
