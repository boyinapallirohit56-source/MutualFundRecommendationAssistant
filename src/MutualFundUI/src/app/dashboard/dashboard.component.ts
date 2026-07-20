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
    <div class="container page-wrapper">
      <!-- Header with greeting -->
      <div class="dashboard-header">
        <div>
          <p class="greeting">Good {{ getTimeOfDay() }},</p>
          <h1>{{ userName }}</h1>
          <p class="header-sub">Here's your investment overview</p>
        </div>
        <div class="header-actions">
          <button class="btn btn-secondary" (click)="router.navigate(['/risk-assessment'])">Retake Assessment</button>
          <button class="btn btn-primary" (click)="regenerate()">Update Recommendations</button>
        </div>
      </div>

      <!-- Loading -->
      <div *ngIf="loading" class="loading-grid">
        <div class="skeleton-card" *ngFor="let i of [1,2,3,4]"></div>
      </div>

      <!-- No Assessment -->
      <div *ngIf="!loading && !assessment" class="card empty-state animate-in">
        <div class="empty-icon">&#128202;</div>
        <h3>Start Your Investment Journey</h3>
        <p>Complete the risk assessment to receive AI-powered mutual fund recommendations</p>
        <button class="btn btn-primary btn-lg mt-6" (click)="router.navigate(['/risk-assessment'])">Take Risk Assessment</button>
      </div>

      <!-- Dashboard Content -->
      <div *ngIf="!loading && assessment" class="animate-in">
        <!-- Top Stats -->
        <div class="grid-4 stagger mb-6">
          <div class="stat-card">
            <div class="stat-card-icon" style="background:rgba(99,102,241,0.1); color:#6366f1">&#128202;</div>
            <div class="stat-card-info">
              <span class="stat-card-label">Risk Score</span>
              <span class="stat-card-value">{{ assessment.normalizedScore }}/100</span>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-card-icon" style="background:rgba(16,185,129,0.1); color:#10b981">&#127919;</div>
            <div class="stat-card-info">
              <span class="stat-card-label">Profile</span>
              <span class="stat-card-value">{{ assessment.riskProfile }}</span>
            </div>
          </div>
          <div class="stat-card" *ngIf="portfolio">
            <div class="stat-card-icon" style="background:rgba(245,158,11,0.1); color:#f59e0b">&#128176;</div>
            <div class="stat-card-info">
              <span class="stat-card-label">Portfolio Value</span>
              <span class="stat-card-value">Rs.{{ portfolio.currentValue | number:'1.0-0' }}</span>
            </div>
          </div>
          <div class="stat-card" *ngIf="portfolio">
            <div class="stat-card-icon" [style.background]="portfolio.totalReturns >= 0 ? 'rgba(16,185,129,0.1)' : 'rgba(239,68,68,0.1)'" [style.color]="portfolio.totalReturns >= 0 ? '#10b981' : '#ef4444'">&#128200;</div>
            <div class="stat-card-info">
              <span class="stat-card-label">Returns</span>
              <span class="stat-card-value" [style.color]="portfolio.totalReturns >= 0 ? '#10b981' : '#ef4444'">{{ portfolio.returnsPercentage }}%</span>
            </div>
          </div>
        </div>

        <!-- Main Grid: Allocation + Details -->
        <div class="grid-2 mb-6" *ngIf="recommendation">
          <!-- Pie Chart -->
          <div class="card">
            <div class="card-header-row">
              <h3>Asset Allocation</h3>
              <span class="badge badge-primary">{{ assessment.riskProfile }}</span>
            </div>
            <div style="display:flex; justify-content:center; padding:20px 0">
              <app-pie-chart [data]="chartData" [size]="240" [showLegend]="true"></app-pie-chart>
            </div>
          </div>

          <!-- Allocation Bars -->
          <div class="card">
            <div class="card-header-row">
              <h3>Recommended Funds</h3>
            </div>
            <div class="allocation-list">
              <div class="alloc-item" *ngFor="let alloc of recommendation.allocations">
                <div class="alloc-top">
                  <div class="alloc-info">
                    <span class="alloc-dot" [style.background]="getColor(alloc.assetClass)"></span>
                    <span class="alloc-name">{{ alloc.assetClass }}</span>
                  </div>
                  <span class="alloc-pct">{{ alloc.percentage }}%</span>
                </div>
                <div class="alloc-bar-track">
                  <div class="alloc-bar-fill" [style.width.%]="alloc.percentage" [style.background]="getColor(alloc.assetClass)"></div>
                </div>
                <p class="alloc-funds" *ngIf="alloc.suggestedFunds">{{ alloc.suggestedFunds }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- AI Insight + Goals Row -->
        <div class="grid-2 mb-6">
          <!-- AI Explanation -->
          <div class="card ai-card" *ngIf="recommendation?.aiExplanation">
            <div class="card-header-row">
              <h3>
                <span style="margin-right:8px">&#129302;</span>AI Insight
              </h3>
            </div>
            <p class="ai-text">{{ recommendation.aiExplanation }}</p>
            <button class="btn btn-ghost btn-sm mt-4" (click)="router.navigate(['/chat'])">Ask AI a question &#8594;</button>
          </div>

          <!-- Goal Progress -->
          <div class="card" *ngIf="profile">
            <div class="card-header-row">
              <h3>Goal Progress</h3>
            </div>
            <div class="goals-list" *ngIf="goals.length">
              <div class="goal-row" *ngFor="let goal of goals">
                <div class="goal-info">
                  <span class="goal-name">{{ goal.name }}</span>
                  <span class="goal-pct">{{ goal.progress }}%</span>
                </div>
                <div class="goal-bar-track">
                  <div class="goal-bar-fill" [style.width.%]="goal.progress"></div>
                </div>
              </div>
            </div>
            <p *ngIf="!goals.length" style="color:var(--text-tertiary); font-size:13px">Set goals in your profile to track progress.</p>
          </div>
        </div>

        <!-- Activity + SIP Row -->
        <div class="grid-2">
          <!-- Recent Activity -->
          <div class="card">
            <div class="card-header-row">
              <h3>Recent Activity</h3>
            </div>
            <div class="activity-list" *ngIf="recentActivity.length">
              <div class="activity-row" *ngFor="let a of recentActivity">
                <div class="activity-dot" [style.background]="a.color"></div>
                <span class="activity-text">{{ a.text }}</span>
                <span class="activity-time">{{ a.time }}</span>
              </div>
            </div>
            <p *ngIf="!recentActivity.length" style="color:var(--text-tertiary); font-size:13px">No activity yet.</p>
          </div>

          <!-- SIP Schedule -->
          <div class="card">
            <div class="card-header-row">
              <h3>Upcoming SIP</h3>
            </div>
            <div class="sip-list" *ngIf="upcomingSIPs.length">
              <div class="sip-row" *ngFor="let sip of upcomingSIPs">
                <span class="sip-date">{{ sip.date }}</span>
                <span class="sip-amount">Rs. {{ sip.amount | number:'1.0-0' }}</span>
              </div>
            </div>
            <p *ngIf="!upcomingSIPs.length" style="color:var(--text-tertiary); font-size:13px">Configure SIP in your profile.</p>
          </div>
        </div>
      </div>
    </div>
  `,
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
    .dashboard-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 32px; }
    .greeting { font-size: 14px; color: var(--text-tertiary); font-weight: 500; }
    .dashboard-header h1 { font-size: 32px; font-weight: 800; color: var(--text-primary); letter-spacing: -0.8px; }
    .header-sub { font-size: 14px; color: var(--text-secondary); margin-top: 4px; }
    .header-actions { display: flex; gap: 10px; }

    .loading-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 16px; }
    .skeleton-card { height: 100px; background: var(--surface-2); border-radius: var(--radius-lg); animation: pulse 1.5s infinite; }

    .empty-state { text-align: center; padding: 64px 32px; }
    .empty-icon { font-size: 48px; margin-bottom: 16px; opacity: 0.8; }
    .empty-state h3 { font-size: 20px; font-weight: 700; margin-bottom: 8px; }
    .empty-state p { color: var(--text-secondary); max-width: 400px; margin: 0 auto; }

    .stat-card { display: flex; align-items: center; gap: 14px; padding: 20px; background: var(--surface-0); border-radius: var(--radius-lg); border: 1px solid var(--border-light); box-shadow: var(--shadow-card); transition: all 0.2s; }
    .stat-card:hover { box-shadow: var(--shadow-card-hover); transform: translateY(-2px); }
    .stat-card-icon { width: 44px; height: 44px; border-radius: var(--radius-md); display: flex; align-items: center; justify-content: center; font-size: 20px; flex-shrink: 0; }
    .stat-card-label { display: block; font-size: 12px; color: var(--text-tertiary); font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px; }
    .stat-card-value { display: block; font-size: 18px; font-weight: 800; color: var(--text-primary); margin-top: 2px; letter-spacing: -0.3px; }

    .card-header-row { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
    .card-header-row h3 { font-size: 16px; font-weight: 700; color: var(--text-primary); }

    .allocation-list { display: flex; flex-direction: column; gap: 16px; }
    .alloc-item { }
    .alloc-top { display: flex; justify-content: space-between; align-items: center; margin-bottom: 6px; }
    .alloc-info { display: flex; align-items: center; gap: 8px; }
    .alloc-dot { width: 10px; height: 10px; border-radius: 3px; }
    .alloc-name { font-size: 14px; font-weight: 600; color: var(--text-primary); }
    .alloc-pct { font-size: 14px; font-weight: 800; color: var(--brand-accent); }
    .alloc-bar-track { height: 6px; background: var(--surface-2); border-radius: var(--radius-full); overflow: hidden; }
    .alloc-bar-fill { height: 100%; border-radius: var(--radius-full); transition: width 0.6s var(--ease-default); }
    .alloc-funds { font-size: 12px; color: var(--text-tertiary); margin-top: 4px; }

    .ai-card { background: linear-gradient(135deg, rgba(99,102,241,0.03), rgba(6,182,212,0.03)); border: 1px solid rgba(99,102,241,0.1); }
    .ai-text { font-size: 14px; color: var(--text-secondary); line-height: 1.8; }

    .goals-list { display: flex; flex-direction: column; gap: 16px; }
    .goal-row { }
    .goal-info { display: flex; justify-content: space-between; margin-bottom: 6px; }
    .goal-name { font-size: 13px; font-weight: 600; color: var(--text-primary); }
    .goal-pct { font-size: 13px; font-weight: 700; color: var(--brand-accent); }
    .goal-bar-track { height: 5px; background: var(--surface-2); border-radius: var(--radius-full); }
    .goal-bar-fill { height: 100%; background: linear-gradient(90deg, #10b981, #34d399); border-radius: var(--radius-full); transition: width 0.5s; }

    .activity-list { display: flex; flex-direction: column; gap: 12px; }
    .activity-row { display: flex; align-items: center; gap: 12px; }
    .activity-dot { width: 8px; height: 8px; border-radius: 50%; flex-shrink: 0; }
    .activity-text { flex: 1; font-size: 13px; color: var(--text-secondary); }
    .activity-time { font-size: 12px; color: var(--text-tertiary); }

    .sip-list { display: flex; flex-direction: column; gap: 12px; }
    .sip-row { display: flex; justify-content: space-between; align-items: center; padding: 10px 14px; background: var(--surface-1); border-radius: var(--radius-sm); }
    .sip-date { font-size: 13px; color: var(--text-secondary); font-weight: 500; }
    .sip-amount { font-size: 14px; font-weight: 700; color: var(--brand-accent); }

    @media (max-width: 768px) {
      .dashboard-header { flex-direction: column; gap: 16px; }
      .header-actions { width: 100%; }
      .loading-grid { grid-template-columns: 1fr 1fr; }
    }
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
      'Equity': '#6366f1',
      'Debt': '#10b981',
      'Hybrid': '#f59e0b',
      'Gold': '#eab308',
      'Liquid': '#06b6d4',
      'International': '#ec4899'
    };
    return colors[assetClass] || '#94a3b8';
  }

  getTimeOfDay(): string {
    const hour = new Date().getHours();
    if (hour < 12) return 'morning';
    if (hour < 17) return 'afternoon';
    return 'evening';
  }
}
