import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-fund-compare',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container" style="margin-top:24px">
      <div class="page-header">
        <h1>Fund Comparison</h1>
        <p>Side-by-side comparison of selected funds</p>
      </div>

      <div class="card text-center" *ngIf="loading">
        <p>Loading comparison...</p>
      </div>

      <div class="card" *ngIf="comparison && !loading">
        <table class="compare-table">
          <thead>
            <tr>
              <th>Metric</th>
              <th *ngFor="let fund of comparison.funds">{{ fund.name }}</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>Category</td>
              <td *ngFor="let fund of comparison.funds">{{ fund.subCategory }}</td>
            </tr>
            <tr>
              <td>AMC</td>
              <td *ngFor="let fund of comparison.funds">{{ fund.amc }}</td>
            </tr>
            <tr>
              <td>NAV</td>
              <td *ngFor="let fund of comparison.funds">Rs. {{ fund.nav || 'N/A' }}</td>
            </tr>
            <tr>
              <td>1Y Returns</td>
              <td *ngFor="let fund of comparison.funds"
                  [class]="getCellClass(fund.cagr1Y, 'cagr1Y', 'higher')">
                {{ fund.cagr1Y || 'N/A' }}%
              </td>
            </tr>
            <tr>
              <td>3Y Returns</td>
              <td *ngFor="let fund of comparison.funds"
                  [class]="getCellClass(fund.cagr3Y, 'cagr3Y', 'higher')">
                {{ fund.cagr3Y || 'N/A' }}%
              </td>
            </tr>
            <tr>
              <td>5Y Returns</td>
              <td *ngFor="let fund of comparison.funds"
                  [class]="getCellClass(fund.cagr5Y, 'cagr5Y', 'higher')">
                {{ fund.cagr5Y || 'N/A' }}%
              </td>
            </tr>
            <tr>
              <td>Expense Ratio</td>
              <td *ngFor="let fund of comparison.funds"
                  [class]="getCellClass(fund.expenseRatio, 'expenseRatio', 'lower')">
                {{ fund.expenseRatio || 'N/A' }}%
              </td>
            </tr>
            <tr>
              <td>AUM (Cr)</td>
              <td *ngFor="let fund of comparison.funds"
                  [class]="getCellClass(fund.aum, 'aum', 'higher')">
                Rs. {{ fund.aum | number:'1.0-0' }}
              </td>
            </tr>
            <tr>
              <td>Fund Manager</td>
              <td *ngFor="let fund of comparison.funds">{{ fund.fundManager || 'N/A' }}</td>
            </tr>
            <tr>
              <td>Rating</td>
              <td *ngFor="let fund of comparison.funds"
                  [class]="getCellClass(fund.rating, 'rating', 'higher')">
                {{ fund.rating || 'N/A' }} ★
              </td>
            </tr>
          </tbody>
        </table>

        <!-- Legend -->
        <div class="legend">
          <span class="legend-item"><span class="dot best"></span> Best</span>
          <span class="legend-item"><span class="dot good"></span> Good</span>
          <span class="legend-item"><span class="dot worst"></span> Worst</span>
        </div>
      </div>

      <button class="btn btn-secondary mt-4" (click)="router.navigate(['/funds'])">Back to Funds</button>
    </div>
  `,
  styles: [`
    .compare-table { width: 100%; border-collapse: collapse; font-size: 14px; }
    .compare-table th, .compare-table td { padding: 12px 16px; border-bottom: 1px solid #f3f4f6; text-align: left; }
    .compare-table th { background: #f9fafb; font-weight: 600; font-size: 13px; }
    .compare-table td:first-child { font-weight: 500; color: #374151; }
    .cell-best { background: #ecfdf5; color: #059669; font-weight: 600; }
    .cell-good { background: #f0fdf4; color: #16a34a; }
    .cell-worst { background: #fef2f2; color: #dc2626; }
    .cell-neutral { }
    .legend { display: flex; gap: 16px; margin-top: 16px; padding-top: 12px; border-top: 1px solid #f3f4f6; }
    .legend-item { display: flex; align-items: center; gap: 6px; font-size: 12px; color: #6b7280; }
    .dot { width: 10px; height: 10px; border-radius: 3px; }
    .dot.best { background: #ecfdf5; border: 1px solid #059669; }
    .dot.good { background: #f0fdf4; border: 1px solid #16a34a; }
    .dot.worst { background: #fef2f2; border: 1px solid #dc2626; }
  `]
})
export class FundCompareComponent implements OnInit {
  comparison: any = null;
  loading = true;
  private metricRanks: { [key: string]: { best: number; worst: number } } = {};

  constructor(private apiService: ApiService, private route: ActivatedRoute, public router: Router) {}

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      const ids = params['ids']?.split(',').map(Number) || [];
      if (ids.length >= 2) {
        this.apiService.compareFunds(ids).subscribe({
          next: (res) => {
            this.comparison = res;
            this.calculateRanks(res.funds);
            this.loading = false;
          },
          error: () => { this.loading = false; }
        });
      } else {
        this.loading = false;
      }
    });
  }

  calculateRanks(funds: any[]) {
    const metrics = ['cagr1Y', 'cagr3Y', 'cagr5Y', 'expenseRatio', 'aum', 'rating'];
    for (const metric of metrics) {
      const values = funds.map(f => f[metric]).filter(v => v != null);
      if (values.length > 0) {
        this.metricRanks[metric] = {
          best: Math.max(...values),
          worst: Math.min(...values)
        };
      }
    }
    // For expense ratio, lower is better — so swap best/worst
    if (this.metricRanks['expenseRatio']) {
      const temp = this.metricRanks['expenseRatio'].best;
      this.metricRanks['expenseRatio'].best = this.metricRanks['expenseRatio'].worst;
      this.metricRanks['expenseRatio'].worst = temp;
    }
  }

  getCellClass(value: number | null, metric: string, betterDirection: 'higher' | 'lower'): string {
    if (value == null || !this.metricRanks[metric]) return 'cell-neutral';

    const { best, worst } = this.metricRanks[metric];

    if (betterDirection === 'higher') {
      if (value === Math.max(best, worst) && best !== worst) return 'cell-best';
      if (value === Math.min(best, worst) && best !== worst) return 'cell-worst';
    } else {
      // Lower is better (expense ratio)
      if (value === Math.min(best, worst) && best !== worst) return 'cell-best';
      if (value === Math.max(best, worst) && best !== worst) return 'cell-worst';
    }

    return 'cell-good';
  }
}
