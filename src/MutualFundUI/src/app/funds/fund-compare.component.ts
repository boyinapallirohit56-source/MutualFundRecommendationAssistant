import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-fund-compare',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './fund-compare.component.html',
  styleUrls: ['./fund-compare.component.css']
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
      if (value === Math.min(best, worst) && best !== worst) return 'cell-best';
      if (value === Math.max(best, worst) && best !== worst) return 'cell-worst';
    }
    return 'cell-good';
  }
}
