import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  activeTab = 'analytics';
  analytics: any = null;
  users: any[] = [];
  funds: any[] = [];
  riskDistItems: any[] = [];
  goalDistItems: any[] = [];

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadAnalytics();
  }

  loadAnalytics() {
    this.apiService.getAdminAnalytics().subscribe({
      next: (data) => {
        this.analytics = data;
        this.riskDistItems = Object.entries(data.riskProfileDistribution || {}).map(([key, value]) => ({ key, value }));
        this.goalDistItems = Object.entries(data.goalDistribution || {}).map(([key, value]) => ({ key, value }));
      }
    });
  }

  loadUsers() {
    this.apiService.getAdminUsers().subscribe({
      next: (users) => { this.users = users; }
    });
  }

  loadFunds() {
    this.apiService.listFunds().subscribe({
      next: (funds) => { this.funds = funds; }
    });
  }

  toggleUserStatus(user: any) {
    this.apiService.updateUserStatus(user.id, !user.isActive).subscribe({
      next: () => { user.isActive = !user.isActive; }
    });
  }
}
