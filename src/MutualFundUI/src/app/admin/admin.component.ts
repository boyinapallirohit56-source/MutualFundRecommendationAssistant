import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container" style="margin-top:24px">
      <div class="page-header">
        <h1>Admin Portal</h1>
        <p>Manage users, questionnaire, funds, and view analytics</p>
      </div>

      <!-- Tab Navigation -->
      <div class="tabs">
        <button class="tab" [class.active]="activeTab === 'analytics'" (click)="activeTab = 'analytics'">Analytics</button>
        <button class="tab" [class.active]="activeTab === 'users'" (click)="activeTab = 'users'; loadUsers()">Users</button>
        <button class="tab" [class.active]="activeTab === 'funds'" (click)="activeTab = 'funds'; loadFunds()">Funds</button>
      </div>

      <!-- Analytics Tab -->
      <div *ngIf="activeTab === 'analytics' && analytics">
        <div class="grid-2" style="grid-template-columns:repeat(4,1fr); margin-bottom:20px">
          <div class="card stat-card">
            <span class="stat-number">{{ analytics.totalUsers }}</span>
            <span class="stat-text">Total Users</span>
          </div>
          <div class="card stat-card">
            <span class="stat-number">{{ analytics.activeUsers }}</span>
            <span class="stat-text">Active Users</span>
          </div>
          <div class="card stat-card">
            <span class="stat-number">{{ analytics.totalAssessments }}</span>
            <span class="stat-text">Assessments</span>
          </div>
          <div class="card stat-card">
            <span class="stat-number">{{ analytics.totalRecommendations }}</span>
            <span class="stat-text">Recommendations</span>
          </div>
        </div>

        <div class="grid-2">
          <div class="card">
            <h3 style="font-size:15px; margin-bottom:12px">Risk Profile Distribution</h3>
            <div class="dist-item" *ngFor="let item of riskDistItems">
              <span>{{ item.key }}</span>
              <span class="dist-count">{{ item.value }}</span>
            </div>
          </div>
          <div class="card">
            <h3 style="font-size:15px; margin-bottom:12px">Goal Distribution</h3>
            <div class="dist-item" *ngFor="let item of goalDistItems">
              <span>{{ item.key }}</span>
              <span class="dist-count">{{ item.value }}</span>
            </div>
          </div>
        </div>

        <div class="card" *ngIf="analytics.recentActivity?.length">
          <h3 style="font-size:15px; margin-bottom:12px">Recent Activity</h3>
          <div class="activity-item" *ngFor="let a of analytics.recentActivity">
            <span><strong>{{ a.userName }}</strong> {{ a.action }}</span>
            <span class="activity-time">{{ a.timestamp | date:'short' }}</span>
          </div>
        </div>
      </div>

      <!-- Users Tab -->
      <div *ngIf="activeTab === 'users'">
        <div class="card">
          <table class="admin-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th>Risk Profile</th>
                <th>Status</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let user of users">
                <td>{{ user.name }}</td>
                <td>{{ user.email }}</td>
                <td>{{ user.riskProfile || 'Not assessed' }}</td>
                <td>
                  <span class="status-badge" [class.active]="user.isActive" [class.inactive]="!user.isActive">
                    {{ user.isActive ? 'Active' : 'Inactive' }}
                  </span>
                </td>
                <td>
                  <button class="btn-action" (click)="toggleUserStatus(user)">
                    {{ user.isActive ? 'Deactivate' : 'Activate' }}
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Funds Tab -->
      <div *ngIf="activeTab === 'funds'">
        <div class="card">
          <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:16px">
            <h3 style="font-size:15px">Mutual Funds ({{ funds.length }})</h3>
          </div>
          <table class="admin-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Category</th>
                <th>AMC</th>
                <th>3Y CAGR</th>
                <th>Rating</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let fund of funds">
                <td>{{ fund.name }}</td>
                <td>{{ fund.subCategory }}</td>
                <td>{{ fund.amc }}</td>
                <td>{{ fund.cagr3Y }}%</td>
                <td>{{ fund.rating }} ★</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .tabs { display: flex; gap: 4px; margin-bottom: 20px; }
    .tab { padding: 10px 20px; border: none; background: #e5e7eb; border-radius: 8px; cursor: pointer; font-size: 14px; font-weight: 500; }
    .tab.active { background: #2563eb; color: white; }
    .stat-card { text-align: center; padding: 20px; }
    .stat-number { display: block; font-size: 32px; font-weight: 700; color: #2563eb; }
    .stat-text { display: block; font-size: 13px; color: #6b7280; margin-top: 4px; }
    .dist-item { display: flex; justify-content: space-between; padding: 8px 0; border-bottom: 1px solid #f3f4f6; font-size: 14px; }
    .dist-count { font-weight: 600; color: #2563eb; }
    .activity-item { display: flex; justify-content: space-between; padding: 8px 0; border-bottom: 1px solid #f3f4f6; font-size: 13px; }
    .activity-time { color: #6b7280; }
    .admin-table { width: 100%; border-collapse: collapse; font-size: 14px; }
    .admin-table th, .admin-table td { padding: 12px; border-bottom: 1px solid #f3f4f6; text-align: left; }
    .admin-table th { font-weight: 600; color: #374151; font-size: 13px; }
    .status-badge { padding: 4px 10px; border-radius: 12px; font-size: 12px; font-weight: 500; }
    .status-badge.active { background: #ecfdf5; color: #059669; }
    .status-badge.inactive { background: #fef2f2; color: #dc2626; }
    .btn-action { background: none; border: 1px solid #d1d5db; padding: 4px 12px; border-radius: 6px; font-size: 12px; cursor: pointer; }
    .btn-action:hover { border-color: #2563eb; color: #2563eb; }
  `]
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
