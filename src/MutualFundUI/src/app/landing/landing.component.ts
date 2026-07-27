import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.css']
})
export class LandingComponent implements OnInit, OnDestroy {
  private tickerInterval: any;
  private countUpDone = false;

  // Market Ticker
  marketIndices = [
    { name: 'NIFTY 50', value: '24,856.15', change: '0.84%', isPositive: true, color: '#10b981' },
    { name: 'SENSEX', value: '81,432.60', change: '0.72%', isPositive: true, color: '#10b981' },
    { name: 'NIFTY IT', value: '29,441.90', change: '2.34%', isPositive: true, color: '#10b981' },
    { name: 'GOLD', value: '7,245.00', change: '0.45%', isPositive: true, color: '#eab308' },
    { name: 'USD/INR', value: '83.42', change: '0.12%', isPositive: false, color: '#6366f1' },
    { name: 'BANKNIFTY', value: '55,230.80', change: '1.15%', isPositive: true, color: '#10b981' }
  ];

  // Stats with count-up
  stats = [
    { value: 33, suffix: '+', label: 'AMFI Funds', current: 0 },
    { value: 4, suffix: '', label: 'Risk Profiles', current: 0 },
    { value: 15, suffix: '', label: 'Assessment Questions', current: 0 },
    { value: 6, suffix: '', label: 'Asset Classes', current: 0 }
  ];

  // Navigation
  navLinks = ['Home', 'Features', 'Mutual Funds', 'Tools', 'About'];

  // Trust badges
  trustBadges = [
    '33+ AMFI Funds',
    'Daily NAV Updates',
    'AI Powered',
    'Direct Growth Plans'
  ];

  // Products
  products = [
    {
      icon: '&#128201;', title: 'Mutual Funds', subtitle: 'Direct Growth Plans',
      desc: 'Invest in top-rated funds across Equity, Debt, Gold, Hybrid & International — all Direct Growth for maximum returns.',
      tags: ['Large Cap', 'Mid Cap', 'Small Cap', 'Debt', 'Gold', 'International'], color: '#6366f1'
    },
    {
      icon: '&#129504;', title: 'AI-Powered Advice', subtitle: 'Ask WealthAI Anything',
      desc: '',
      queries: ['"What SIP should I start?"', '"What is NAV?"', '"Compare SBI vs Nippon"', '"Can I retire in 20 years?"'],
      tags: [], color: '#14b8a6'
    },
    {
      icon: '&#128202;', title: 'Portfolio Analytics', subtitle: 'Deep Insights',
      desc: 'Diversification score, risk alignment, fund overlap detection, rebalancing suggestions & stress testing.',
      tags: ['Analysis', 'Stress Test', 'What-If', 'Rebalance'], color: '#10b981'
    }
  ];

  // Steps
  steps = [
    { icon: '&#128100;', title: 'Create Profile', desc: 'Income, savings, and goals' },
    { icon: '&#128203;', title: 'Risk Assessment', desc: '15 questions, 5 minutes' },
    { icon: '&#129302;', title: 'AI Recommends', desc: 'Personalized allocation' },
    { icon: '&#128200;', title: 'Track & Grow', desc: 'Monitor and rebalance' }
  ];

  // Features - AI highlighted
  aiFeature = {
    icon: '&#129302;', title: 'WealthAI Advisor',
    desc: 'GPT-powered financial assistant that explains investments in simple language. Ask about SIPs, NAV, risk profiles, fund comparisons — get instant, personalized answers.',
    queries: ['What SIP amount is right for me?', 'Explain expense ratio', 'Is my portfolio diversified?']
  };

  features = [
    { icon: '&#127919;', title: 'Goal Planning', desc: 'Set wealth, retirement, education goals with SIP tracking' },
    { icon: '&#9889;', title: 'Live AMFI Data', desc: 'Real-time NAV synced daily from AMFI India' },
    { icon: '&#128176;', title: 'Tax Optimization', desc: 'ELSS recommendations & Section 80C calculator' },
    { icon: '&#128200;', title: 'SIP Calculator', desc: 'Project returns over your investment horizon' },
    { icon: '&#9888;&#65039;', title: 'Stress Testing', desc: 'Simulate -10% to -50% market crashes' }
  ];

  // Why WealthAI
  whyPoints = [
    'AI-powered fund recommendations',
    'Daily AMFI NAV sync',
    'Personalized risk profiling',
    'Portfolio health score',
    'Stress testing & what-if analysis',
    'Goal-based planning',
    'Tax optimization tools',
    'No commission bias — Direct plans only'
  ];

  // Trust section
  trustItems = [
    { icon: '&#128274;', label: 'Secure Login' },
    { icon: '&#128202;', label: 'AMFI Data' },
    { icon: '&#129302;', label: 'AI Recommendations' },
    { icon: '&#128200;', label: 'Direct Plans' },
    { icon: '&#127470;&#127475;', label: 'Built for India' }
  ];

  // Screenshots/Teasers
  screenTeasers = [
    { icon: '&#128202;', title: 'Dashboard Analytics', desc: 'Risk score, allocation chart, SIP schedule' },
    { icon: '&#129302;', title: 'AI Advisor Chat', desc: 'Ask anything about mutual funds' },
    { icon: '&#128200;', title: 'Portfolio Insights', desc: 'Diversification, overlap, rebalancing' },
    { icon: '&#128178;', title: 'SIP & Goal Calculators', desc: 'Plan your financial future' },
    { icon: '&#9888;&#65039;', title: 'Stress Testing', desc: 'Simulate market crashes' },
    { icon: '&#128203;', title: 'Smart Reports', desc: 'Risk, portfolio & recommendation reports' }
  ];

  // Testimonials
  testimonials = [
    { text: 'The AI Advisor explained mutual funds better than hours of YouTube videos.', name: 'Rahul S.', role: 'Software Developer', rating: 5 },
    { text: 'Portfolio stress testing showed me exactly where my risk was. Incredible tool.', name: 'Priya P.', role: 'Doctor', rating: 5 },
    { text: 'Finally understood my risk profile and got recommendations that actually made sense.', name: 'Arjun K.', role: 'Product Manager', rating: 5 }
  ];

  // FAQ
  faqs = [
    { q: 'Where does the NAV data come from?', a: 'We sync NAV data directly from AMFI (Association of Mutual Funds in India) — the official source used by all mutual fund platforms in India.', open: false },
    { q: 'Is WealthAI free to use?', a: 'Yes, WealthAI is completely free. No hidden charges, no subscription fees, no commission on investments.', open: false },
    { q: 'How accurate are the recommendations?', a: 'Recommendations are based on your risk profile, SEBI-defined allocation rules, and fund performance metrics (CAGR, Sharpe ratio, alpha). They are educational, not SEBI-certified advice.', open: false },
    { q: 'Can beginners use this platform?', a: 'Absolutely! The AI Advisor explains everything in simple language. The risk assessment is designed for first-time investors.', open: false },
    { q: 'What are Direct Growth plans?', a: 'Direct plans have no distributor commission (0.5-1.5% lower expense ratio). Growth option reinvests profits for compounding. This is the industry standard for DIY platforms.', open: false },
    { q: 'How is AI used in this platform?', a: 'AI powers the chat advisor (GPT-based), generates personalized allocation explanations, and provides portfolio insights. Risk scoring uses a rule-based algorithm, not AI.', open: false }
  ];

  ngOnInit() {
    this.tickerInterval = setInterval(() => {
      this.marketIndices = this.marketIndices.map(idx => ({
        ...idx,
        change: this.randomizeChange(idx.change),
        isPositive: Math.random() > 0.25
      }));
    }, 8000);
  }

  ngOnDestroy() {
    if (this.tickerInterval) clearInterval(this.tickerInterval);
  }

  @HostListener('window:scroll')
  onScroll() {
    if (this.countUpDone) return;
    const statsEl = document.querySelector('.stats-bar');
    if (statsEl) {
      const rect = statsEl.getBoundingClientRect();
      if (rect.top < window.innerHeight * 0.8) {
        this.countUpDone = true;
        this.animateCountUp();
      }
    }
  }

  animateCountUp() {
    this.stats.forEach(stat => {
      const duration = 1500;
      const steps = 40;
      const increment = stat.value / steps;
      let current = 0;
      const interval = setInterval(() => {
        current += increment;
        if (current >= stat.value) {
          stat.current = stat.value;
          clearInterval(interval);
        } else {
          stat.current = Math.floor(current);
        }
      }, duration / steps);
    });
  }

  toggleFaq(index: number) {
    this.faqs[index].open = !this.faqs[index].open;
  }

  private randomizeChange(current: string): string {
    const base = parseFloat(current.replace('%', ''));
    const variation = (Math.random() - 0.4) * 0.3;
    return Math.abs(base + variation).toFixed(2) + '%';
  }
}
