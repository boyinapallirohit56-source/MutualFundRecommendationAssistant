import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

interface MarketIndex {
  name: string;
  value: string;
  change: string;
  isPositive: boolean;
}

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.css']
})
export class LandingComponent implements OnInit, OnDestroy {
  // Live market ticker data (simulated - updates every 5s)
  marketIndices: MarketIndex[] = [
    { name: 'NIFTY 50', value: '24,856.15', change: '+1.24%', isPositive: true },
    { name: 'SENSEX', value: '81,432.60', change: '+0.98%', isPositive: true },
    { name: 'NIFTY IT', value: '29,441.90', change: '+2.34%', isPositive: true },
    { name: 'GOLD', value: '7,245.00', change: '+0.45%', isPositive: true },
    { name: 'FINNIFTY', value: '26,126.15', change: '+0.83%', isPositive: true },
    { name: 'BANKNIFTY', value: '55,230.80', change: '-0.12%', isPositive: false }
  ];

  private tickerInterval: any;

  // Stats
  stats = [
    { value: '33+', label: 'Direct Growth Funds' },
    { value: '4', label: 'Risk Profiles' },
    { value: '15', label: 'Assessment Questions' },
    { value: '6', label: 'Asset Classes' }
  ];

  // How it works steps
  steps = [
    { icon: '&#128100;', title: 'Create Profile', desc: 'Tell us about your income, savings, and investment goals' },
    { icon: '&#128203;', title: 'Risk Assessment', desc: 'Answer 15 questions to find your risk appetite' },
    { icon: '&#129302;', title: 'AI Recommends', desc: 'Get personalized fund allocation based on your profile' },
    { icon: '&#128200;', title: 'Track & Grow', desc: 'Monitor portfolio, get insights, and rebalance' }
  ];

  // Product offerings (like Groww's cards)
  products = [
    {
      icon: '&#128201;',
      title: 'Mutual Funds',
      subtitle: 'Direct Growth Plans',
      desc: 'Invest in top-rated funds across Equity, Debt, Gold, Hybrid & International',
      tags: ['Large Cap', 'Mid Cap', 'Small Cap', 'Debt', 'Gold'],
      color: '#6366f1'
    },
    {
      icon: '&#129504;',
      title: 'AI-Powered Advice',
      subtitle: 'Personalized for You',
      desc: 'ChatGPT-powered advisor that explains investments in simple language',
      tags: ['Risk Profiling', 'Fund Selection', 'Rebalancing'],
      color: '#06b6d4'
    },
    {
      icon: '&#128202;',
      title: 'Portfolio Analytics',
      subtitle: 'Deep Insights',
      desc: 'Diversification score, risk alignment, fund overlap detection & stress testing',
      tags: ['Analysis', 'Stress Test', 'What-If'],
      color: '#10b981'
    }
  ];

  // Features list
  features = [
    { icon: '&#127919;', title: 'Goal-Based Planning', desc: 'Set wealth creation, retirement, tax saving or education goals with SIP tracking' },
    { icon: '&#9889;', title: 'Live AMFI Data', desc: 'Real-time NAV updates synced directly from AMFI for accurate portfolio valuation' },
    { icon: '&#128176;', title: 'Tax Optimization', desc: 'ELSS recommendations and Section 80C calculator to maximize your tax savings' },
    { icon: '&#128200;', title: 'SIP Calculator', desc: 'Plan your systematic investments with projected returns over your time horizon' },
    { icon: '&#9888;&#65039;', title: 'Stress Testing', desc: 'Simulate market crashes (-10% to -50%) and see how your portfolio would react' },
    { icon: '&#128269;', title: 'Fund Comparison', desc: 'Compare up to 4 funds side-by-side on returns, risk, expense ratio & more' }
  ];

  // Trust signals
  trustPoints = [
    { icon: '&#128274;', text: 'Bank-grade Security' },
    { icon: '&#128176;', text: 'Zero Commission' },
    { icon: '&#128202;', text: 'Direct Growth Only' },
    { icon: '&#129302;', text: 'AI-Powered' }
  ];

  ngOnInit() {
    // Simulate live ticker updates
    this.tickerInterval = setInterval(() => {
      this.marketIndices = this.marketIndices.map(index => ({
        ...index,
        change: this.randomizeChange(index.change),
        isPositive: Math.random() > 0.3
      }));
    }, 8000);
  }

  ngOnDestroy() {
    if (this.tickerInterval) {
      clearInterval(this.tickerInterval);
    }
  }

  private randomizeChange(current: string): string {
    const base = parseFloat(current.replace('%', '').replace('+', ''));
    const variation = (Math.random() - 0.4) * 0.5;
    const newVal = Math.abs(base + variation).toFixed(2);
    return (base + variation >= 0 ? '+' : '-') + newVal + '%';
  }
}
