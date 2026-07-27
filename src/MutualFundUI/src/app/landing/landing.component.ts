import { Component, OnInit, OnDestroy, HostListener, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.css']
})
export class LandingComponent implements OnInit, OnDestroy {
  private countUpDone = false;
  private comparisonCountDone = false;
  private queryInterval: any;
  currentQueryIndex = 0;

  // Navigation
  navLinks = ['Home', 'Features', 'Mutual Funds', 'Tools', 'About'];

  // Trust strip items
  trustStrip = ['Live AMFI NAV', 'Direct Growth', 'AI Powered', 'No Hidden Fees', 'Risk Assessment'];

  // Hero trust badges
  trustBadges = ['33+ AMFI Funds', 'Daily NAV Updates', 'AI Powered', 'Direct Growth Plans'];

  // Stats with count-up
  stats = [
    { value: 33, suffix: '+', label: 'Mutual Funds', current: 0 },
    { value: 24, suffix: '/7', label: 'AI Advisor', current: 0 },
    { value: 365, suffix: '', label: 'Daily NAV Sync', current: 0 },
    { value: 50, suffix: '%', label: 'Stress Testing', current: 0 }
  ];

  // Products
  products = [
    {
      title: 'Mutual Funds',
      subtitle: 'Direct Growth Plans',
      desc: 'Invest in top-rated funds across Equity, Debt, Gold, Hybrid & International — all Direct Growth for maximum returns.',
      tags: ['Large Cap', 'Mid Cap', 'Small Cap', 'Debt', 'Gold', 'International'],
      color: '#6366f1',
      iconType: 'chart'
    },
    {
      title: 'AI-Powered Advice',
      subtitle: 'Ask WealthAI Anything',
      desc: '',
      tags: [],
      color: '#14b8a6',
      iconType: 'brain'
    },
    {
      title: 'Portfolio Analytics',
      subtitle: 'Deep Insights',
      desc: 'Diversification score, risk alignment, fund overlap detection, rebalancing suggestions & stress testing.',
      tags: ['Analysis', 'Stress Test', 'What-If', 'Rebalance'],
      color: '#10b981',
      iconType: 'analytics'
    }
  ];

  // AI rotating queries
  aiQueries = [
    'What SIP should I start?',
    'Explain NAV',
    'Compare SBI vs Nippon',
    'Should I invest ₹10,000?'
  ];

  // Steps
  steps = [
    { num: 1, title: 'Create Profile', desc: 'Income, savings, and goals', iconLetter: 'P' },
    { num: 2, title: 'Risk Assessment', desc: '15 questions, 5 minutes', iconLetter: 'R' },
    { num: 3, title: 'AI Recommends', desc: 'Personalized allocation', iconLetter: 'A' },
    { num: 4, title: 'Track & Grow', desc: 'Monitor and rebalance', iconLetter: 'T' }
  ];

  // AI Feature
  aiFeature = {
    title: 'WealthAI Advisor',
    desc: 'GPT-powered financial assistant that explains investments in simple language. Ask about SIPs, NAV, risk profiles, fund comparisons — get instant, personalized answers.',
    queries: ['What SIP amount is right for me?', 'Explain expense ratio', 'Is my portfolio diversified?']
  };

  // Features grid
  features = [
    { title: 'Goal Planning', desc: 'Set wealth, retirement, education goals with SIP tracking', iconLetter: 'G' },
    { title: 'Live AMFI Data', desc: 'Real-time NAV synced daily from AMFI India', iconLetter: 'N' },
    { title: 'Tax Optimization', desc: 'ELSS recommendations & Section 80C calculator', iconLetter: 'T' },
    { title: 'SIP Calculator', desc: 'Project returns over your investment horizon', iconLetter: 'S' },
    { title: 'Stress Testing', desc: 'Simulate -10% to -50% market crashes', iconLetter: '!' }
  ];

  // Comparison
  directAmount = 0;
  regularAmount = 0;

  // Why WealthAI - 8 points with icon letters
  whyPoints = [
    { text: 'AI-powered recommendations', letter: 'AI' },
    { text: 'Daily AMFI NAV sync', letter: 'N' },
    { text: 'Personalized risk profiling', letter: 'R' },
    { text: 'Portfolio health score', letter: 'H' },
    { text: 'Stress testing & what-if', letter: 'S' },
    { text: 'Goal-based planning', letter: 'G' },
    { text: 'Tax optimization tools', letter: 'T' },
    { text: 'No commission bias', letter: '0' }
  ];

  // Trust section
  trustItems = [
    { label: 'Secure Login', letter: 'S', color: '#6366f1' },
    { label: 'AMFI Data', letter: 'A', color: '#14b8a6' },
    { label: 'AI Powered', letter: 'AI', color: '#8b5cf6' },
    { label: 'Direct Plans', letter: 'D', color: '#10b981' },
    { label: 'Built for India', letter: 'IN', color: '#f59e0b' }
  ];

  // Teasers
  screenTeasers = [
    { title: 'Dashboard Analytics', desc: 'Risk score, allocation chart, SIP schedule', letter: 'D' },
    { title: 'AI Advisor Chat', desc: 'Ask anything about mutual funds', letter: 'C' },
    { title: 'Portfolio Insights', desc: 'Diversification, overlap, rebalancing', letter: 'P' },
    { title: 'SIP & Goal Calculators', desc: 'Plan your financial future', letter: 'S' },
    { title: 'Stress Testing', desc: 'Simulate market crashes', letter: '!' },
    { title: 'Smart Reports', desc: 'Risk, portfolio & recommendation reports', letter: 'R' }
  ];

  // Testimonials
  testimonials = [
    { text: 'The AI Advisor explained mutual funds better than hours of YouTube videos.', name: 'Rahul S.', role: 'Software Developer', initials: 'RS', color: '#6366f1' },
    { text: 'Portfolio stress testing showed me exactly where my risk was. Incredible tool.', name: 'Priya P.', role: 'Doctor', initials: 'PP', color: '#14b8a6' },
    { text: 'Finally understood my risk profile and got recommendations that actually made sense.', name: 'Arjun K.', role: 'Product Manager', initials: 'AK', color: '#10b981' }
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

  // Scroll animation tracking
  animatedSections: Set<string> = new Set();

  constructor(private el: ElementRef, private router: Router) {}

  ngOnInit() {
    // Rotate AI queries every 3 seconds
    this.queryInterval = setInterval(() => {
      this.currentQueryIndex = (this.currentQueryIndex + 1) % this.aiQueries.length;
    }, 3000);
  }

  ngOnDestroy() {
    if (this.queryInterval) clearInterval(this.queryInterval);
  }

  @HostListener('window:scroll')
  onScroll() {
    // Stats count-up
    if (!this.countUpDone) {
      const statsEl = this.el.nativeElement.querySelector('.stats-bar');
      if (statsEl) {
        const rect = statsEl.getBoundingClientRect();
        if (rect.top < window.innerHeight * 0.85) {
          this.countUpDone = true;
          this.animateCountUp();
        }
      }
    }

    // Comparison count-up
    if (!this.comparisonCountDone) {
      const compEl = this.el.nativeElement.querySelector('.comparison-card');
      if (compEl) {
        const rect = compEl.getBoundingClientRect();
        if (rect.top < window.innerHeight * 0.85) {
          this.comparisonCountDone = true;
          this.animateComparison();
        }
      }
    }

    // Scroll-reveal animations
    const reveals = this.el.nativeElement.querySelectorAll('.reveal');
    reveals.forEach((el: HTMLElement) => {
      const rect = el.getBoundingClientRect();
      if (rect.top < window.innerHeight * 0.88) {
        el.classList.add('revealed');
      }
    });
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

  animateComparison() {
    const targetDirect = 24.6;
    const targetRegular = 22.9;
    const duration = 1500;
    const steps = 40;
    const incDirect = targetDirect / steps;
    const incRegular = targetRegular / steps;
    let current = 0;
    const interval = setInterval(() => {
      current++;
      if (current >= steps) {
        this.directAmount = targetDirect;
        this.regularAmount = targetRegular;
        clearInterval(interval);
      } else {
        this.directAmount = Math.round(incDirect * current * 10) / 10;
        this.regularAmount = Math.round(incRegular * current * 10) / 10;
      }
    }, duration / steps);
  }

  toggleFaq(index: number) {
    this.faqs[index].open = !this.faqs[index].open;
  }

  explorePlatform() {
    this.router.navigate(['/register']);
  }

  scrollToFeatures() {
    const el = this.el.nativeElement.querySelector('#features');
    if (el) {
      el.scrollIntoView({ behavior: 'smooth' });
    }
  }
}
