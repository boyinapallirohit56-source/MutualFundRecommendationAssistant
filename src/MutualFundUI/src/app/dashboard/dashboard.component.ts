import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ApiService } from '../shared/services/api.service';
import { AuthService } from '../shared/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container" style="margin-top:24px">
      <div class="page-header">
        <h1>Dashboard</h1>
        <p>Welcome back, {{ userName }}</p>
      </div>

      <!-- Loading -->
      <div *ngIf="loading" class="card text-center">
        <p>Loading your data...</p>
      </div>

      <!-- No Assessment -->
      <div *ngIf="!loading && !assessment" class="card text-center">
        <h3>No assessment found</h3>
        <p style="color:#6b7280; margin-top:8px">Complete the risk assessment to get your recommendations</p>
        <button class="btn btn-primary mt-4" (click)="router.navigate(['/risk-assessment'])">Take Assessment</button>
      </div>

      <!-- Dashboard Content -->
      <div *ngIf="!loading && assessment">
        <!-- Risk Score Card -->
        <div class="grid-2">
          <div class="card text-center">
            <p style="font-size:13px; color:#6b7280">Your Risk Score</p>
            <h2 style="font-size:48px; color:#2563eb; margin:8px 0">{{ assessment.normalizedScore }}</h2>
            <span class="profile-badge">{{ assessment.riskProfile }}</span>
          </div>

          <!-- Quick Actions -->
          <div class="card">
            <h3 style="font-size:16px; margin-bottom:16px">Quick Actions</h3>
            <div style="display:flex; flex-direction:column; gap:10px">
              <button class="btn btn-secondary" (click)="router.navigate(['/risk-assessment'])">Retake Assessment</button>
              <button class="btn btn-secondary" (click)="router.navigate(['/onboarding'])">Update Profile</button>
              <button class="btn btn-primary" (click)="regenerate()">Regenerate Recommendations</button>
            </div>
          </div>
        </div>

        <!-- Recommendation -->
        <div class="card" *ngIf="recommendation">
          <h3 style="font-size:18px; margin-bottom:16px">Recommended Allocation</h3>

          <!-- Allocation Bars -->
          <div class="allocation-list">
            <div class="allocation-item" *ngFor="let alloc of recommendation.allocations">
              <div class="alloc-header">
                <span class="alloc-name">{{ alloc.assetClass }}</span>
                <span class="alloc-pct">{{ alloc.percentage }}%</span>
              </div>
              <div class="alloc-bar-bg">
                <div class="alloc-bar" [style.width.%]="alloc.percentage" [style.background]="getColor(alloc.assetClass)"></div>
              </div>
              <p class="alloc-funds" *ngIf="alloc.suggestedFunds">{{ alloc.suggestedFunds }}</p>
            </div>
          </div>
        </div>

        <!-- AI Explanation -->
        <div class="card" *ngIf="recommendation?.aiExplanation">
          <h3 style="font-size:16px; margin-bottom:12px">Why This Allocation?</h3>
          <p style="color:#4b5563; line-height:1.7">{{ recommendation.aiExplanation }}</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .profile-badge { display: inline-block; padding: 6px 16px; background: #eff6ff; color: #2563eb; border-radius: 20px; font-size: 14px; font-weight: 500; }
    .allocation-list { display: flex; flex-direction: column; gap: 16px; }
    .alloc-header { display: flex; justify-content: space-between; margin-bottom: 4px; }
    .alloc-name { font-weight: 500; font-size: 14px; }
    .alloc-pct { font-weight: 600; font-size: 14px; color: #2563eb; }
    .alloc-bar-bg { height: 10px; background: #e5e7eb; border-radius: 5px; }
    .alloc-bar { height: 100%; border-radius: 5px; transition: width 0.5s; }
    .alloc-funds { font-size: 12px; color: #6b7280; margin-top: 4px; }
  `]
})
export class DashboardComponent implements OnInit {
  userName = '';
  assessment: any = null;
  recommendation: any = null;
  loading = true;

  constructor(
    private apiService: ApiService,
    private authService: AuthService,
    public router: Router
  ) {}

  ngOnInit() {
    const user = this.authService.getUser();
    this.userName = user?.name || 'User';

    this.apiService.getLatestAssessment().subscribe({
      next: (res) => {
        this.assessment = res;
        this.loadRecommendation();
      },
      error: () => { this.loading = false; }
    });
  }

  loadRecommendation() {
    this.apiService.getLatestRecommendation().subscribe({
      next: (res) => {
        this.recommendation = res;
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  regenerate() {
    this.apiService.generateRecommendation().subscribe({
      next: (res) => { this.recommendation = res; }
    });
  }

  getColor(assetClass: string): string {
    const colors: any = {
      'Equity': '#2563eb',
      'Debt': '#10b981',
      'Hybrid': '#f59e0b',
      'Gold': '#eab308',
      'Liquid': '#6366f1',
      'International': '#ec4899'
    };
    return colors[assetClass] || '#6b7280';
  }
}
