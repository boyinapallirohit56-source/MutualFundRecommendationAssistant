import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../shared/services/api.service';
import { AuthService } from '../shared/services/auth.service';

@Component({
  selector: 'app-fund-factsheet',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './fund-factsheet.component.html',
  styleUrls: ['./fund-factsheet.component.css']
})
export class FundFactsheetComponent implements OnInit {
  fund: any = null;
  addingToWatchlist = false;
  watchlistAdded = false;
  watchlistError = false;
  isAdmin = false;

  constructor(
    private apiService: ApiService,
    private authService: AuthService,
    private route: ActivatedRoute,
    public router: Router
  ) {}

  ngOnInit() {
    const user = this.authService.getUser();
    this.isAdmin = user?.role === 'Admin';

    this.route.params.subscribe(params => {
      const id = +params['id'];
      this.apiService.getFundFactsheet(id).subscribe({
        next: (res) => { this.fund = res; }
      });
    });
  }

  addToWatchlist() {
    if (!this.fund) return;
    this.addingToWatchlist = true;
    this.apiService.addToWatchlist(this.fund.id).subscribe({
      next: () => {
        this.addingToWatchlist = false;
        this.watchlistAdded = true;
      },
      error: () => {
        this.addingToWatchlist = false;
        this.watchlistError = true;
        setTimeout(() => this.watchlistError = false, 3000);
      }
    });
  }
}
