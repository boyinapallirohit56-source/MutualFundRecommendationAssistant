import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-watchlist',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container" style="margin-top:24px; max-width:800px">
      <div class="page-header">
        <h1>Watchlist</h1>
        <p>Funds you're tracking</p>
      </div>

      <div class="card" *ngIf="!items.length && !loading">
        <p class="text-center" style="color:#6b7280">
          Your watchlist is empty. Browse funds and add them here.
        </p>
        <div class="text-center mt-4">
          <button class="btn btn-primary" (click)="router.navigate(['/funds'])">Browse Funds</button>
        </div>
      </div>

      <div class="card" *ngIf="items.length">
        <div class="watchlist-item" *ngFor="let item of items">
          <div class="item-info" (click)="router.navigate(['/funds', item.mutualFundId])" style="cursor:pointer">
            <strong>{{ item.fundName }}</strong>
            <span class="item-meta">{{ item.amc }} | {{ item.category }}</span>
          </div>
          <div class="item-metrics">
            <div class="metric">
              <span class="metric-label">NAV</span>
              <span class="metric-value">{{ item.nav || 'N/A' }}</span>
            </div>
            <div class="metric">
              <span class="metric-label">3Y CAGR</span>
              <span class="metric-value" style="color:#10b981">{{ item.cagr3Y || 'N/A' }}%</span>
            </div>
            <div class="metric">
              <span class="metric-label">Rating</span>
              <span class="metric-value">{{ item.rating || 'N/A' }} ★</span>
            </div>
            <button class="btn-remove" (click)="remove(item.id)">Remove</button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .watchlist-item { display: flex; justify-content: space-between; align-items: center; padding: 16px 0; border-bottom: 1px solid #f3f4f6; }
    .watchlist-item:last-child { border-bottom: none; }
    .item-info { display: flex; flex-direction: column; gap: 4px; }
    .item-info strong { font-size: 14px; color: #2563eb; }
    .item-info strong:hover { text-decoration: underline; }
    .item-meta { font-size: 12px; color: #6b7280; }
    .item-metrics { display: flex; gap: 20px; align-items: center; }
    .metric { text-align: center; }
    .metric-label { display: block; font-size: 11px; color: #6b7280; }
    .metric-value { display: block; font-size: 13px; font-weight: 600; }
    .btn-remove { background: none; border: none; color: #ef4444; cursor: pointer; font-size: 12px; padding: 4px 8px; }
    .btn-remove:hover { text-decoration: underline; }
  `]
})
export class WatchlistComponent implements OnInit {
  items: any[] = [];
  loading = true;

  constructor(private apiService: ApiService, public router: Router) {}

  ngOnInit() {
    this.loadWatchlist();
  }

  loadWatchlist() {
    this.apiService.getWatchlist().subscribe({
      next: (items) => { this.items = items; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  remove(itemId: number) {
    this.apiService.removeFromWatchlist(itemId).subscribe({
      next: () => { this.items = this.items.filter(i => i.id !== itemId); }
    });
  }
}
