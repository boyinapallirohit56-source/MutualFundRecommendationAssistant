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
        this.buildSIPDates(res.sipAmount);
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
    const sipDay = this.profile.sipDate || 5;
    this.upcomingSIPs = [];

    if (frequency === 'Monthly') {
      // Show SIPs on different dates to represent different goal allocations
      const sipDates = [1, 5, 10];
      const sipAmounts = [25000, 15000, 10000]; // Different amounts per date

      // If user has goals with different MonthlySIP, use those
      if (this.goals && this.goals.length > 0) {
        // Show next month's SIP schedule with varied dates
        for (let d = 0; d < Math.min(sipDates.length, 3); d++) {
          const nextDate = new Date(today.getFullYear(), today.getMonth() + 1, sipDates[d]);
          this.upcomingSIPs.push({
            date: nextDate.toLocaleDateString('en-IN', { day: 'numeric', month: 'short', year: 'numeric' }),
            amount: sipAmounts[d]
          });
        }
      } else {
        // Fallback: single SIP date from profile
        for (let i = 0; i < 3; i++) {
          const sipDate = new Date(today.getFullYear(), today.getMonth() + i, sipDay);
          if (sipDate > today) {
            this.upcomingSIPs.push({
              date: sipDate.toLocaleDateString('en-IN', { day: 'numeric', month: 'short', year: 'numeric' }),
              amount: sipAmount
            });
          }
        }
        if (!this.upcomingSIPs.length) {
          const nextMonth = new Date(today.getFullYear(), today.getMonth() + 1, sipDay);
          this.upcomingSIPs.push({
            date: nextMonth.toLocaleDateString('en-IN', { day: 'numeric', month: 'short', year: 'numeric' }),
            amount: sipAmount
          });
        }
      }
      }
    } else if (frequency === 'Weekly') {
      const dayNames = ['', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri'];
      for (let i = 1; i <= 4; i++) {
        const nextDate = new Date(today);
        nextDate.setDate(today.getDate() + (i * 7));
        this.upcomingSIPs.push({
          date: `${dayNames[sipDay]}, ${nextDate.toLocaleDateString('en-IN', { day: 'numeric', month: 'short' })}`,
          amount: sipAmount
        });
      }
    } else if (frequency === 'Quarterly') {
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
