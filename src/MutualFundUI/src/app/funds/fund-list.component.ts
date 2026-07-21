import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-fund-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './fund-list.component.html',
  styleUrls: ['./fund-list.component.css']
})
export class FundListComponent implements OnInit {
  funds: any[] = [];
  searchTerm = '';
  selectedCategory = '';
  selectedFunds: number[] = [];
  loading = false;

  constructor(private apiService: ApiService, private router: Router) {}

  ngOnInit() { this.loadFunds(); }

  loadFunds() {
    this.loading = true;
    this.apiService.listFunds(this.selectedCategory || undefined, this.searchTerm || undefined).subscribe({
      next: (funds) => { this.funds = funds; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  isSelected(id: number): boolean { return this.selectedFunds.includes(id); }

  toggleFund(id: number) {
    const index = this.selectedFunds.indexOf(id);
    if (index > -1) this.selectedFunds.splice(index, 1);
    else if (this.selectedFunds.length < 4) this.selectedFunds.push(id);
  }

  viewFund(id: number) { this.router.navigate(['/funds', id]); }

  goToCompare() {
    this.router.navigate(['/funds/compare'], { queryParams: { ids: this.selectedFunds.join(',') } });
  }
}
