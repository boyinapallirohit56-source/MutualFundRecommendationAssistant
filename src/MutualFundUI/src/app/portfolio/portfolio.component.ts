import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-portfolio',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container" style="margin-top:24px">
      <div class="page-header">
        <h1>Portfolio</h1>
        <p>Manage and analyze your mutual fund holdings</p>
      </div>

      <!-- Add Holding Form -->
      <div class="card">
        <h3 style="font-size:16px; margin-bottom:16px">Add New Holding</h3>
        <div class="grid-2">
          <div class="form-group">
            <label>Fund Name</label>
            <input type="text" [(ngModel)]="newHolding.fundName" placeholder="e.g., SBI Bluechip Fund">
          </div>
          <div class="form-group">
            <label>Units</label>
            <input type="number" [(ngModel)]="newHolding.units" placeholder="100">
          </div>
          <div class="form-group">
            <label>Purchase NAV (Rs)</label>
            <input type="number" [(ngModel)]="newHolding.purchaseNAV" placeholder="45.50">
          </div>
          <div class="form-group">
            <label>Invested Amount (Rs)</label>
            <input type="number" [(ngModel)]="newHolding.investedAmount" placeholder="50000">
          </div>
          <div class="form-group">
            <label>Purchase Date</label>
            <input type="date" [(ngModel)]="newHolding.purchaseDate">
          </div>
        </div>
        <button class="btn btn-primary mt-4" (click)="addHolding()" [disabled]="adding">
          {{ adding ? 'Adding...' : 'Add Holding' }}
        </button>
      </div>

      <!-- Portfolio Summary -->
      <div class="card" *ngIf="portfolio">
        <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:16px">
          <h3 style="font-size:16px">Portfolio Summary</h3>
          <button class="btn btn-secondary" (click)="analyzePortfolio()">Analyze</button>
        </div>
        <div class="grid-2" style="grid-template-columns:repeat(4,1fr)">
          <div class="stat-box">
            <span class="stat-label">Invested</span>
            <span class="stat-value">Rs. {{ portfolio.totalInvested | number:'1.0-0' }}</span>
          </div>
          <div class="stat-box">
            <span class="stat-label">Current Value</span>
            <span class="stat-value">Rs. {{ portfolio.currentValue | number:'1.0-0' }}</span>
          </div>
          <div class="stat-box">
            <span class="stat-label">Returns</span>
            <span class="stat-value" [style.color]="portfolio.totalReturns >= 0 ? '#10b981' : '#ef4444'">
              Rs. {{ portfolio.totalReturns | number:'1.0-0' }}
            </span>
          </div>
          <div class="stat-box">
            <span class="stat-label">Returns %</span>
            <span class="stat-value" [style.color]="portfolio.returnsPercentage >= 0 ? '#10b981' : '#ef4444'">
              {{ portfolio.returnsPercentage }}%
            </span>
          </div>
        </div>
      </div>

      <!-- Holdings List -->
      <div class="card" *ngIf="portfolio?.holdings?.length">
        <h3 style="font-size:16px; margin-bottom:16px">Holdings ({{ portfolio.holdings.length }})</h3>
        <div class="holding-item" *ngFor="let h of portfolio.holdings">
          <div class="holding-info">
            <strong>{{ h.fundName }}</strong>
            <span class="holding-category">{{ h.category || 'Unknown' }}</span>
          </div>
          <div class="holding-numbers">
            <span>Invested: Rs.{{ h.investedAmount | number:'1.0-0' }}</span>
            <span>Current: Rs.{{ h.currentValue | number:'1.0-0' }}</span>
            <span [style.color]="h.returnsPercentage >= 0 ? '#10b981' : '#ef4444'">
              {{ h.returnsPercentage > 0 ? '+' : '' }}{{ h.returnsPercentage }}%
            </span>
            <button class="btn-remove" (click)="removeHolding(h.id)">Remove</button>
          </div>
        </div>
      </div>

      <!-- Analysis Results -->
      <div *ngIf="analysis" class="card">
        <h3 style="font-size:16px; margin-bottom:16px">Portfolio Analysis</h3>
        <div class="grid-2" style="margin-bottom:16px">
          <div class="stat-box">
            <span class="stat-label">Portfolio Score</span>
            <span class="stat-value" style="font-size:32px; color:#2563eb">{{ analysis.portfolioScore }}/100</span>
          </div>
          <div class="stat-box">
            <span class="stat-label">Diversification</span>
            <span class="stat-value">{{ analysis.diversification.rating }} ({{ analysis.diversification.score }}/100)</span>
          </div>
        </div>

        <h4 style="font-size:14px; margin-bottom:8px">Risk Alignment</h4>
        <p style="font-size:13px; color:#4b5563; margin-bottom:16px">{{ analysis.riskAnalysis.explanation }}</p>

        <h4 style="font-size:14px; margin-bottom:8px">Insights</h4>
        <ul style="font-size:13px; color:#4b5563; padding-left:20px">
          <li *ngFor="let insight of analysis.insights" style="margin-bottom:6px">{{ insight }}</li>
        </ul>

        <div *ngIf="analysis.rebalancingSuggestions?.length" style="margin-top:16px">
          <h4 style="font-size:14px; margin-bottom:8px">Rebalancing Suggestions</h4>
          <div class="rebalance-item" *ngFor="let s of analysis.rebalancingSuggestions">
            <span>{{ s.action }} <strong>{{ s.assetClass }}</strong></span>
            <span>{{ s.currentPercentage }}% → {{ s.targetPercentage }}% ({{ s.action === 'Increase' ? '+' : '-' }}{{ s.differencePercentage }}%)</span>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div class="card text-center" *ngIf="!portfolio && !loading">
        <p style="color:#6b7280">No portfolio yet. Add your first holding above.</p>
      </div>
    </div>
  `,
  styles: [`
    h3 { font-weight: 600; }
    .stat-box { text-align: center; padding: 12px; background: #f9fafb; border-radius: 8px; }
    .stat-label { display: block; font-size: 12px; color: #6b7280; margin-bottom: 4px; }
    .stat-value { display: block; font-size: 18px; font-weight: 600; }
    .holding-item { display: flex; justify-content: space-between; align-items: center; padding: 12px 0; border-bottom: 1px solid #f3f4f6; }
    .holding-item:last-child { border-bottom: none; }
    .holding-info { display: flex; flex-direction: column; gap: 4px; }
    .holding-category { font-size: 12px; color: #6b7280; }
    .holding-numbers { display: flex; gap: 16px; align-items: center; font-size: 13px; }
    .btn-remove { background: none; border: none; color: #ef4444; cursor: pointer; font-size: 12px; }
    .btn-remove:hover { text-decoration: underline; }
    .rebalance-item { display: flex; justify-content: space-between; font-size: 13px; padding: 8px 12px; background: #f9fafb; border-radius: 6px; margin-bottom: 6px; }
  `]
})
export class PortfolioComponent implements OnInit {
  portfolio: any = null;
  analysis: any = null;
  loading = true;
  adding = false;

  newHolding = {
    fundName: '',
    units: 0,
    purchaseNAV: 0,
    investedAmount: 0,
    purchaseDate: ''
  };

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadPortfolio();
  }

  loadPortfolio() {
    this.apiService.getPortfolio().subscribe({
      next: (res) => { this.portfolio = res; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  addHolding() {
    this.adding = true;
    this.apiService.addHolding(this.newHolding).subscribe({
      next: () => {
        this.adding = false;
        this.newHolding = { fundName: '', units: 0, purchaseNAV: 0, investedAmount: 0, purchaseDate: '' };
        this.loadPortfolio();
      },
      error: () => { this.adding = false; }
    });
  }

  removeHolding(id: number) {
    this.apiService.removeHolding(id).subscribe({ next: () => this.loadPortfolio() });
  }

  analyzePortfolio() {
    this.apiService.analyzePortfolio().subscribe({
      next: (res) => { this.analysis = res; }
    });
  }
}
