import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-financial-health',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './financial-health.component.html',
  styleUrls: ['./financial-health.component.css']
})
export class FinancialHealthComponent implements OnInit {
  score: number | null = null;
  breakdown: any[] = [];
  loading = true;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.apiService.getProfile().subscribe({
      next: (profile) => {
        this.calculateScore(profile);
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  calculateScore(p: any) {
    this.breakdown = [];
    let total = 0;

    // 1. Savings Ratio (20 pts)
    const savingsRatio = p.monthlyIncome > 0 ? ((p.monthlyIncome - p.monthlyExpenses) / p.monthlyIncome) * 100 : 0;
    const savingsScore = Math.min(20, Math.round(savingsRatio / 2.5));
    this.breakdown.push({ name: 'Savings Ratio', score: savingsScore, max: 20, tip: savingsRatio >= 30 ? 'Great! You save more than 30% of income' : 'Try to save at least 20-30% of your income' });
    total += savingsScore;

    // 2. Debt Ratio (20 pts)
    const debtRatio = p.monthlyIncome > 0 ? (p.loans / p.monthlyIncome) * 100 : 0;
    const debtScore = debtRatio === 0 ? 20 : debtRatio < 30 ? 15 : debtRatio < 50 ? 10 : 5;
    this.breakdown.push({ name: 'Debt Management', score: debtScore, max: 20, tip: debtRatio === 0 ? 'Excellent! No debt' : debtRatio < 30 ? 'Debt is manageable' : 'Consider reducing your debt-to-income ratio' });
    total += debtScore;

    // 3. Emergency Fund (20 pts)
    const monthsCovered = p.monthlyExpenses > 0 ? p.savings / p.monthlyExpenses : 0;
    const emergencyScore = monthsCovered >= 6 ? 20 : monthsCovered >= 3 ? 14 : monthsCovered >= 1 ? 8 : 3;
    this.breakdown.push({ name: 'Emergency Fund', score: emergencyScore, max: 20, tip: monthsCovered >= 6 ? 'Great! 6+ months covered' : `Covers ${monthsCovered.toFixed(1)} months. Aim for 6 months of expenses.` });
    total += emergencyScore;

    // 4. Investment Rate (20 pts)
    const investRate = p.monthlyIncome > 0 ? (p.sipAmount / p.monthlyIncome) * 100 : 0;
    const investScore = investRate >= 20 ? 20 : investRate >= 10 ? 14 : investRate >= 5 ? 8 : 3;
    this.breakdown.push({ name: 'Investment Rate', score: investScore, max: 20, tip: investRate >= 20 ? 'Excellent! Investing 20%+ of income' : `Investing ${investRate.toFixed(0)}% of income. Aim for 15-20%.` });
    total += investScore;

    // 5. Planning (20 pts)
    const hasGoals = p.goals && p.goals.length > 0;
    const hasDuration = p.durationInYears > 0;
    const planScore = (hasGoals ? 10 : 0) + (hasDuration ? 10 : 0);
    this.breakdown.push({ name: 'Financial Planning', score: planScore, max: 20, tip: planScore === 20 ? 'Goals and timeline defined' : 'Set clear financial goals with timelines' });
    total += planScore;

    this.score = total;
  }

  getScoreClass(): string {
    if (!this.score) return 'poor';
    if (this.score >= 80) return 'excellent';
    if (this.score >= 60) return 'good';
    if (this.score >= 40) return 'fair';
    return 'poor';
  }

  getRating(): string {
    if (!this.score) return '';
    if (this.score >= 80) return 'Excellent';
    if (this.score >= 60) return 'Good';
    if (this.score >= 40) return 'Fair';
    return 'Needs Improvement';
  }

  getDescription(): string {
    if (!this.score) return '';
    if (this.score >= 80) return 'Your financial health is strong. Keep up the discipline!';
    if (this.score >= 60) return 'You are on a good track. Focus on the areas below to improve.';
    if (this.score >= 40) return 'There is room for improvement. Start with small changes.';
    return 'Your financial health needs attention. Consider reducing debt and increasing savings.';
  }
}
