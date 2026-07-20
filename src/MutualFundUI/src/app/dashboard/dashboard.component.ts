import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ApiService } from '../shared/services/api.service';
import { AuthService } from '../shared/services/auth.service';
import { PieChartComponent, ChartData } from '../shared/components/pie-chart.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, PieChartComponent],
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
        <!-- Row 1: Risk Score + Quick Actions -->
        <div class="grid-2">
          <div class="card text-center">
            <p style="font-size:13px; color:#6b7280">Your Risk Score</p>
            <h2 style="font-size:48px; color:#2563eb; margin:8px 0">{{ assessment.normalizedScore }}</h2>
            <span class="profile-badge">{{ assessment.riskProfile }}</span>
          </div>
          <div class="card">
            <h3 style="font-size:16px; margin-bottom:16px">Quick Actions</h3>
            <div style="display:flex; flex-direction:column; gap:10px">
              <button class="btn btn-secondary" (click)="router.navigate(['/risk-assessment'])">Retake Assessment</button>
              <button class="btn btn-secondary" (click)="router.navigate(['/onboarding'])">Update Profile</button>
              <button class="btn btn-primary" (click)="regenerate()">Regenerate Recommendations</button>
            </div>
          </div>
        </div>

        <!-- Row 2: Pie Chart + Allocation Details -->
        <div class="grid-2" *ngIf="recommendation">
          <div class="card text-center">
            <h3 style="font-size:16px; margin-bottom:16px">Allocation Pie Chart</h3>
            <app-pie-chart [data]="chartData" [size]="220" [showLegend]="true"></app-pie-chart>
          </div>
          <div class="card">
            <h3 style="font-size:16px; margin-bottom:16px">Recommended Allocation</h3>
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
        </div>

        <!-- Row 3: Goal Progress + SIP Dates -->
        <div class="grid-2">
          <div class="card" *ngIf="profile">
            <h3 style="font-size:16px; margin-bottom:16px">Goal Progress</h3>
            <div class="goal-item" *ngFor="let goal of goals">
              <div class="goal-header">
                <span>{{ goal.name }}</span>
                <span class="goal-pct">{{ goal.progress }}%</span>
              </div>
              <div class="goal-bar-bg">
                <div class="goal-bar" [style.width.%]="goal.progress"></div>
              </div>
            </div>
            <p *ngIf="!goals.length" style="color:#6b7280; font-size:13px">No goals set yet. Update your profile to add goals.</p>
          </div>

          <div class="card">
            <h3 style="font-size:16px; margin-bottom:16px">Upcoming SIP Dates</h3>
            <div class="sip-item" *ngFor="let sip of upcomingSIPs">
              <span class="sip-date">{{ sip.date }}</span>
              <span class="sip-amount">Rs. {{ sip.amount | number:'1.0-0' }}</span>
            </div>
            <p *ngIf="!upcomingSIPs.length" style="color:#6b7280; font-size:13px">No SIP configured. Set your SIP amount in profile.</p>
          </div>
        </div>

        <!-- Row 4: Recent Activity -->
        <div class="card">
          <h3 style="font-size:16px; margin-bottom:16px">Recent Activity</h3>
          <div class="activity-item" *ngFor="let activity of recentActivity">
            <div class="activity-dot" [style.background]="activity.color"></div>
            <div class="activity-content">
              <span class="activity-text">{{ activity.text }}</span>
              <span class="activity-time">{{ activity.time }}</span>
            </div>
          </div>
          <p *ngIf="!recentActivity.length" style="color:#6b7280; font-size:13px">No recent activity.</p>
        </div>

        <!-- Row 5: Portfolio Summary -->
        <div class="card" *ngIf="portfolio">
          <h3 style="font-size:16px; margin-bottom:16px">Portfolio Summary</h3>
          <div class="grid-2" style="grid-template-columns:repeat(4,1fr)">
            <div class="stat-box">
              <span class="stat-label">Invested</span>
              <span class="stat-value">Rs. {{ portfolio.totalInvested | number:'1.0-0' }}</span>
            </div>
            <div class="stat-box">
              <span class="stat-label">Current Value</span>
              <span class="stat-value">Rs. {{ portfolio.currentValue | number:'1.0-0' }}</span>
            </div>
            <div class="stat-box">
              <span class="stat-label">Returns</span>
              <span class="stat-value" [style.color]="portfolio.totalReturns >= 0 ? '#10b981' : '#ef4444'">
                {{ portfolio.totalReturns >= 0 ? '+' : '' }}{{ portfolio.returnsPercentage }}%
              </span>
            </div>
            <div class="stat-box">
              <span class="stat-label">Holdings</span>
              <span class="stat-value">{{ portfolio.totalHoldings }}</span>
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
    .goal-item { margin-bottom: 14px; }
    .goal-header { display: flex; justify-content: space-between; font-size: 13px; margin-bottom: 4px; }
    .goal-pct { font-weight: 600; color: #2563eb; }
    .goal-bar-bg { height: 6px; background: #e5e7eb; border-radius: 3px; }
    .goal-bar { height: 100%; background: #10b981; border-radius: 3px; transition: width 0.5s; }
    .sip-item { display: flex; justify-content: space-between; padding: 10px 0; border-bottom: 1px solid #f3f4f6; font-size: 14px; }
    .sip-item:last-child { border-bottom: none; }
    .sip-date { color: #374151; }
    .sip-amount { font-weight: 600; color: #2563eb; }
    .activity-item { display: flex; align-items: center; gap: 12px; padding: 10px 0; border-bottom: 1px solid #f3f4f6; }
    .activity-item:last-child { border-bottom: none; }
    .activity-dot { width: 8px; height: 8px; border-radius: 50%; flex-shrink: 0; }
    .activity-content { display: flex; justify-content: space-between; flex: 1; }
    .activity-text { font-size: 13px; color: #374151; }
    .activity-time { font-size: 12px; color: #6b7280; }
    .stat-box { text-align: center; padding: 12px; background: #f9fafb; border-radius: 8px; }
    .stat-label { display: block; font-size: 12px; color: #6b7280; margin-bottom: 4px; }
    .stat-value { display: block; font-size: 16px; font-weight: 600; }
  `]
})
export class DashboardComponent implements OnInit {
  userName = '';
  assessment: any = null;
  recommendation: any = null;
  profile: any = null;
  portfolio: any = null;
  loading = true;
  chartData: ChartData[] = [];
  goals: any[] = [];
  upcomingSIPs: any[] = [];
  recentActivity: any[] = [];

  constructor(
    private apiService: ApiService,
    private authService: AuthService,
    public router: Router
  ) {}

  ngOnInit() {
    const user = this.authService.getUser();
    this.userName = user?.name || 'User';

    // Load all dashboard data
    this.loadAssessment();
    this.loadProfile();
    this.loadPortfolio();
  }

  loadAssessment() {
    this.apiService.getLatestAssessment().subscribe({
      next: (res) => {
        this.assessment = res;
        this.loadRecommendation();
        this.addActivity('Completed risk assessment', res.riskProfile, '#2563eb');
      },
      error: () => { this.loading = false; }
    });
  }

  loadRecommendation() {
    this.apiService.getLatestRecommendation().subscribe({
      next: (res) => {
        this.recommendation = res;
        this.buildChartData(res.allocations);
        this.addActivity('Received fund recommendation', res.riskProfile + ' profile', '#10b981');
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  loadProfile() {
    this.apiService.getProfile().subscribe({
      next: (res) => {
        this.profile = res;
        this.buildGoals(res.goals);
        this.buildSIPDates(res.sipAmount);
      }
    });
  }

  loadPortfolio() {
    this.apiService.getPortfolio().subscribe({
      next: (res) => {
        this.portfolio = res;
        if (res.totalHoldings > 0) {
          this.addActivity('Portfolio updated', res.totalHoldings + ' holdings', '#f59e0b');
        }
      }
    });
  }

  buildChartData(allocations: any[]) {
    this.chartData = allocations
      .filter((a: any) => a.percentage > 0)
      .map((a: any) => ({
        label: a.assetClass,
        value: a.percentage,
        color: this.getColor(a.assetClass)
      }));
  }

  buildGoals(goalsString: string) {
    if (!goalsString) return;
    const goalNames = goalsString.split(',').map((g: string) => g.trim()).filter((g: string) => g);
    // Simulate progress based on profile completion
    this.goals = goalNames.map((name: string, index: number) => ({
      name,
      progress: Math.min(95, 20 + (index * 15) + Math.floor(Math.random() * 20))
    }));
  }

  buildSIPDates(sipAmount: number) {
    if (!sipAmount || sipAmount <= 0) return;
    const today = new Date();
    this.upcomingSIPs = [];
    for (let i = 0; i < 3; i++) {
      const sipDate = new Date(today.getFullYear(), today.getMonth() + i, 5);
      if (sipDate > today) {
        this.upcomingSIPs.push({
          date: sipDate.toLocaleDateString('en-IN', { day: 'numeric', month: 'short', year: 'numeric' }),
          amount: sipAmount
        });
      }
    }
    // If no future dates yet, show next month
    if (!this.upcomingSIPs.length) {
      const nextMonth = new Date(today.getFullYear(), today.getMonth() + 1, 5);
      this.upcomingSIPs.push({
        date: nextMonth.toLocaleDateString('en-IN', { day: 'numeric', month: 'short', year: 'numeric' }),
        amount: sipAmount
      });
    }
  }

  addActivity(text: string, detail: string, color: string) {
    this.recentActivity.push({
      text: `${text} — ${detail}`,
      time: 'Recently',
      color
    });
  }

  regenerate() {
    this.apiService.generateRecommendation().subscribe({
      next: (res) => {
        this.recommendation = res;
        this.buildChartData(res.allocations);
      }
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
