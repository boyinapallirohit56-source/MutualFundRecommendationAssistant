import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-stress-test',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container" style="margin-top:24px; max-width:900px">
      <div class="page-header">
        <h1>Stress Simulator</h1>
        <p>See how your portfolio would react under different market conditions</p>
      </div>

      <div class="card" *ngIf="!result">
        <p style="color:#4b5563; margin-bottom:16px">
          This simulator estimates the impact of various market scenarios on your portfolio.
          Make sure you have holdings in your portfolio before running the test.
        </p>
        <button class="btn btn-primary" (click)="runTest()" [disabled]="loading">
          {{ loading ? 'Running Simulation...' : 'Run Stress Test' }}
        </button>
      </div>

      <!-- Results -->
      <div *ngIf="result">
        <div *ngIf="result.errorMessage" class="card">
          <p style="color:#ef4444">{{ result.errorMessage }}</p>
        </div>

        <div *ngIf="!result.errorMessage">
          <div class="scenario-card" *ngFor="let scenario of result.scenarios">
            <div class="scenario-header">
              <h3>{{ scenario.scenarioName }}</h3>
              <span class="market-change" [class.negative]="scenario.marketChange < 0" [class.positive]="scenario.marketChange > 0">
                {{ scenario.marketChange > 0 ? '+' : '' }}{{ scenario.marketChange }}%
              </span>
            </div>

            <div class="grid-2" style="grid-template-columns:repeat(4,1fr); margin:16px 0">
              <div class="stat-box">
                <span class="stat-label">Current Value</span>
                <span class="stat-value">Rs. {{ scenario.portfolioCurrentValue | number:'1.0-0' }}</span>
              </div>
              <div class="stat-box">
                <span class="stat-label">Post-Stress Value</span>
                <span class="stat-value">Rs. {{ scenario.portfolioPostStressValue | number:'1.0-0' }}</span>
              </div>
              <div class="stat-box">
                <span class="stat-label">Impact</span>
                <span class="stat-value" [style.color]="scenario.portfolioImpact >= 0 ? '#10b981' : '#ef4444'">
                  {{ scenario.portfolioImpact >= 0 ? '+' : '' }}Rs. {{ scenario.portfolioImpact | number:'1.0-0' }}
                </span>
              </div>
              <div class="stat-box">
                <span class="stat-label">Recovery (Est.)</span>
                <span class="stat-value">{{ scenario.estimatedRecoveryMonths }} months</span>
              </div>
            </div>

            <!-- Fund-wise breakdown -->
            <details>
              <summary style="cursor:pointer; font-size:13px; color:#2563eb">View fund-wise impact</summary>
              <div class="fund-impacts">
                <div class="fund-impact-item" *ngFor="let h of scenario.holdingImpacts">
                  <span>{{ h.fundName }} <small>({{ h.category }})</small></span>
                  <span [style.color]="h.impactPercentage >= 0 ? '#10b981' : '#ef4444'">
                    {{ h.impactPercentage >= 0 ? '+' : '' }}{{ h.impactPercentage }}%
                  </span>
                </div>
              </div>
            </details>
          </div>

          <p style="font-size:12px; color:#6b7280; margin-top:16px; font-style:italic">
            {{ result.disclaimer }}
          </p>

          <button class="btn btn-secondary mt-4" (click)="result = null">Run Again</button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .scenario-card { background: white; border-radius: 12px; padding: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.08); margin-bottom: 16px; }
    .scenario-header { display: flex; justify-content: space-between; align-items: center; }
    .scenario-header h3 { font-size: 16px; font-weight: 600; }
    .market-change { font-size: 18px; font-weight: 700; padding: 4px 12px; border-radius: 6px; }
    .negative { background: #fef2f2; color: #dc2626; }
    .positive { background: #ecfdf5; color: #059669; }
    .stat-box { text-align: center; padding: 10px; background: #f9fafb; border-radius: 8px; }
    .stat-label { display: block; font-size: 11px; color: #6b7280; margin-bottom: 4px; }
    .stat-value { display: block; font-size: 14px; font-weight: 600; }
    .fund-impacts { margin-top: 12px; }
    .fund-impact-item { display: flex; justify-content: space-between; font-size: 13px; padding: 6px 0; border-bottom: 1px solid #f3f4f6; }
  `]
})
export class StressTestComponent {
  result: any = null;
  loading = false;

  constructor(private apiService: ApiService) {}

  runTest() {
    this.loading = true;
    this.apiService.runStressTest().subscribe({
      next: (res) => { this.result = res; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }
}
