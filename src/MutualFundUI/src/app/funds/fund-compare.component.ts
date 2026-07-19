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

      <div class="card" *ngIf="loading" class="text-center">
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
                  [class.winner]="comparison.metricWinners['CAGR1Y'] === fund.name">
                {{ fund.cagr1Y || 'N/A' }}%
              </td>
            </tr>
            <tr>
              <td>3Y Returns</td>
              <td *ngFor="let fund of comparison.funds"
                  [class.winner]="comparison.metricWinners['CAGR3Y'] === fund.name">
                {{ fund.cagr3Y || 'N/A' }}%
              </td>
            </tr>
            <tr>
              <td>5Y Returns</td>
              <td *ngFor="let fund of comparison.funds"
                  [class.winner]="comparison.metricWinners['CAGR5Y'] === fund.name">
                {{ fund.cagr5Y || 'N/A' }}%
              </td>
            </tr>
            <tr>
              <td>Expense Ratio</td>
              <td *ngFor="let fund of comparison.funds"
                  [class.winner]="comparison.metricWinners['ExpenseRatio'] === fund.name">
                {{ fund.expenseRatio || 'N/A' }}%
              </td>
            </tr>
            <tr>
              <td>AUM (Cr)</td>
              <td *ngFor="let fund of comparison.funds"
                  [class.winner]="comparison.metricWinners['AUM'] === fund.name">
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
                  [class.winner]="comparison.metricWinners['Rating'] === fund.name">
                {{ fund.rating || 'N/A' }} ★
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <button class="btn btn-secondary mt-4" (click)="router.navigate(['/funds'])">Back to Funds</button>
    </div>
  `,
  styles: [`
    .compare-table { width: 100%; border-collapse: collapse; font-size: 14px; }
    .compare-table th, .compare-table td { padding: 12px 16px; border-bottom: 1px solid #f3f4f6; text-align: left; }
    .compare-table th { background: #f9fafb; font-weight: 600; font-size: 13px; }
    .compare-table td:first-child { font-weight: 500; color: #374151; }
    .winner { background: #ecfdf5; color: #059669; font-weight: 600; }
  `]
})
export class FundCompareComponent implements OnInit {
  comparison: any = null;
  loading = true;

  constructor(private apiService: ApiService, private route: ActivatedRoute, public router: Router) {}

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      const ids = params['ids']?.split(',').map(Number) || [];
      if (ids.length >= 2) {
        this.apiService.compareFunds(ids).subscribe({
          next: (res) => { this.comparison = res; this.loading = false; },
          error: () => { this.loading = false; }
        });
      } else {
        this.loading = false;
      }
    });
  }
}
