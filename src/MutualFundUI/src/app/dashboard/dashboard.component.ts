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
