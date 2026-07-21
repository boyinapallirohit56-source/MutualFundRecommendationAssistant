import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-watchlist',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './watchlist.component.html',
  styleUrls: ['./watchlist.component.css']
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
