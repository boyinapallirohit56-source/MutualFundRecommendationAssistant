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
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
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
  today = new Date();

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
        // Recalculate goal progress based on latest profile data, then load goals
        this.apiService.recalculateGoals().subscribe({
          next: () => this.loadGoals(),
          error: () => this.loadGoals()
        });
      }
    });
  }

  loadGoals() {
    this.apiService.getGoals().subscribe({
      next: (goals: any[]) => {
        this.goals = goals.map((g: any) => ({
          name: g.name,
          progress: g.progressPercentage
        }));
        // Build SIP dates AFTER goals are loaded (SIP display depends on goals)
        if (this.profile) {
          this.buildSIPDates(this.profile.sipAmount);
        }
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

  buildSIPDates(sipAmount: number) {
    if (!sipAmount || sipAmount <= 0) return;
    if (!this.profile) return;

    const today = new Date();
    const frequency = this.profile.sipFrequency || 'Monthly';
    this.upcomingSIPs = [];

    if (frequency === 'Monthly') {
      // Show SIPs split by goal — different dates for each goal
      if (this.goals && this.goals.length > 0) {
        const sipSchedule = [
          { day: 1, label: 'Wealth Creation', amount: 25000 },
          { day: 5, label: 'Retirement', amount: 15000 },
          { day: 10, label: 'Tax Saving', amount: 12500 },
          { day: 15, label: 'Emergency Fund', amount: 10000 }
        ];

        const nextMonth = today.getMonth() + 1;
        for (const sip of sipSchedule.slice(0, this.goals.length)) {
          const sipDate = new Date(today.getFullYear(), nextMonth, sip.day);
          this.upcomingSIPs.push({
            date: sipDate.toLocaleDateString('en-IN', { day: 'numeric', month: 'short', year: 'numeric' }),
            amount: sip.amount,
            label: sip.label
          });
        }
      } else {
        // Fallback: single SIP
        const sipDay = this.profile.sipDate || 5;
        const nextMonth = new Date(today.getFullYear(), today.getMonth() + 1, sipDay);
        this.upcomingSIPs.push({
          date: nextMonth.toLocaleDateString('en-IN', { day: 'numeric', month: 'short', year: 'numeric' }),
          amount: sipAmount
        });
      }
    } else if (frequency === 'Weekly') {
      const dayNames = ['', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri'];
      const sipDay = this.profile.sipDate || 5;
      for (let i = 1; i <= 4; i++) {
        const nextDate = new Date(today);
        nextDate.setDate(today.getDate() + (i * 7));
        this.upcomingSIPs.push({
          date: `${dayNames[sipDay]}, ${nextDate.toLocaleDateString('en-IN', { day: 'numeric', month: 'short' })}`,
          amount: sipAmount
        });
      }
    } else if (frequency === 'Quarterly') {
      const sipDay = this.profile.sipDate || 5;
      for (let i = 1; i <= 3; i++) {
        const sipDate = new Date(today.getFullYear(), today.getMonth() + (i * 3), sipDay);
        this.upcomingSIPs.push({
          date: sipDate.toLocaleDateString('en-IN', { day: 'numeric', month: 'short', year: 'numeric' }),
          amount: sipAmount
        });
      }
    }
  }

  addActivity(text: string, detail: string, color: string) {
    const now = new Date();
    this.recentActivity.push({
      text: `${text} — ${detail}`,
      time: this.getRelativeTime(now),
      color
    });
  }

  getRelativeTime(date: Date): string {
    const now = new Date();
    const diff = Math.floor((now.getTime() - date.getTime()) / 1000);
    if (diff < 60) return 'Just now';
    if (diff < 3600) return `${Math.floor(diff / 60)} mins ago`;
    if (diff < 86400) return `${Math.floor(diff / 3600)} hours ago`;
    return date.toLocaleDateString('en-IN', { day: 'numeric', month: 'short' });
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
