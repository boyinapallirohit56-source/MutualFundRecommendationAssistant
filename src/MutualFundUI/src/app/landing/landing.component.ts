import { Component, HostListener, Input, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-landing-icon',
  standalone: true,
  imports: [CommonModule],
  template: `
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8"
      stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
      <path *ngFor="let path of paths[name] || []" [attr.d]="path"></path>
    </svg>
  `,
  styles: [':host { display: inline-flex; } svg { width: 1em; height: 1em; }']
})
export class LandingIconComponent {
  @Input() name = '';

  readonly paths: Record<string, string[]> = {
    trendingUp: ['M3 3v18h18', 'm19 9-5 5-4-4-3 3'],
    sparkles: ['m12 3-1.9 5.8a2 2 0 0 1-1.3 1.3L3 12l5.8 1.9a2 2 0 0 1 1.3 1.3L12 21l1.9-5.8a2 2 0 0 1 1.3-1.3L21 12l-5.8-1.9a2 2 0 0 1-1.3-1.3Z', 'M5 3v4', 'M3 5h4'],
    pieChart: ['M21 12a9 9 0 1 1-9-9v9Z', 'M12 3a9 9 0 0 1 9 9h-9Z'],
    userRound: ['M17 8a5 5 0 1 1-10 0 5 5 0 0 1 10 0Z', 'M20 21a8 8 0 0 0-16 0'],
    clipboardCheck: ['M7 3h10a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2Z', 'M9 5h6', 'm9 14 2 2 4-4'],
    chart: ['M3 3v18h18', 'M7 16v-5', 'M12 16V8', 'M17 16V8'],
    target: ['M22 12a10 10 0 1 1-20 0 10 10 0 0 1 20 0Z', 'M18 12a6 6 0 1 1-12 0 6 6 0 0 1 12 0Z', 'M14 12a2 2 0 1 1-4 0 2 2 0 0 1 4 0Z'],
    database: ['M21 5c0 1.7-4 3-9 3S3 6.7 3 5s4-3 9-3 9 1.3 9 3Z', 'M3 5v14c0 1.7 4 3 9 3s9-1.3 9-3V5', 'M3 12c0 1.7 4 3 9 3s9-1.3 9-3'],
    receipt: ['M4 2v20l2-2 2 2 2-2 2 2 2-2 2 2 2-2 2 2V2l-2 2-2-2-2 2-2-2-2 2-2-2-2 2Z', 'M10 8h6', 'M10 12h6'],
    calculator: ['M6 2h12a2 2 0 0 1 2 2v16a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2Z', 'M8 6h8', 'M8 10h.01', 'M12 10h.01', 'M16 10h.01', 'M8 14h.01', 'M12 14h.01', 'M16 14h.01', 'M8 18h.01', 'M12 18h.01', 'M16 18h.01'],
    shieldAlert: ['M20 13c0 5-3.5 7.5-8 9-4.5-1.5-8-4-8-9V5l8-3 8 3Z', 'M12 8v4', 'M12 16h.01'],
    lock: ['M5 11h14a2 2 0 0 1 2 2v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-7a2 2 0 0 1 2-2Z', 'M7 11V7a5 5 0 0 1 10 0v4'],
    mapPin: ['M20 10c0 5-8 12-8 12S4 15 4 10a8 8 0 1 1 16 0Z', 'M15 10a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z'],
    layoutDashboard: ['M4 3h5a1 1 0 0 1 1 1v7a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V4a1 1 0 0 1 1-1Z', 'M15 3h5a1 1 0 0 1 1 1v3a1 1 0 0 1-1 1h-5a1 1 0 0 1-1-1V4a1 1 0 0 1 1-1Z', 'M15 12h5a1 1 0 0 1 1 1v7a1 1 0 0 1-1 1h-5a1 1 0 0 1-1-1v-7a1 1 0 0 1 1-1Z', 'M4 16h5a1 1 0 0 1 1 1v3a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1v-3a1 1 0 0 1 1-1Z'],
    messageCircle: ['M21 15a4 4 0 0 1-4 4H8l-5 3V7a4 4 0 0 1 4-4h10a4 4 0 0 1 4 4Z', 'M8 9h8', 'M8 13h5'],
    fileChart: ['M14.5 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7.5Z', 'M14 2v6h6', 'M8 18v-2', 'M12 18v-4', 'M16 18v-6'],
    users: ['M13 7a4 4 0 1 1-8 0 4 4 0 0 1 8 0Z', 'M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2', 'M16 3.1a4 4 0 0 1 0 7.8', 'M22 21v-2a4 4 0 0 0-3-3.9'],
    badgeIndianRupee: ['M3 3h18v18H3Z', 'M8 7h8', 'M8 11h8', 'M8 7l5 10', 'M8 15h3a4 4 0 0 0 0-8'],
    percent: ['M19 5 5 19', 'M9 6.5a2.5 2.5 0 1 1-5 0 2.5 2.5 0 0 1 5 0Z', 'M20 17.5a2.5 2.5 0 1 1-5 0 2.5 2.5 0 0 1 5 0Z'],
    walletCards: ['M5 3h14a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2Z', 'M3 9h18', 'M15 13h2']
  };
}

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [CommonModule, RouterLink, LandingIconComponent],
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.css']
})
export class LandingComponent implements OnInit, OnDestroy {
  private tickerInterval: any;
  private countUpDone = false;

  activeSection = 'home';

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
    { value: 33, suffix: '+', label: 'AMFI Direct Funds', current: 0, icon: 'trendingUp' },
    { value: 15, suffix: '', label: 'Risk Assessment Questions', current: 0, icon: 'clipboardCheck' },
    { value: 4, suffix: '', label: 'Risk Profiles', current: 0, icon: 'users' },
    { value: 6, suffix: '', label: 'Supported Categories', current: 0, icon: 'pieChart' }
  ];

  // Navigation
  navLinks = [
    { label: 'Home', target: 'home' },
    { label: 'Features', target: 'features' },
    { label: 'How It Works', target: 'how-it-works' },
    { label: 'Why WealthAI', target: 'why-wealthai' },
    { label: 'FAQ', target: 'faq' }
  ];

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
      icon: 'trendingUp', title: 'Mutual Funds', subtitle: 'Direct Growth Plans',
      desc: 'Explore curated funds across Equity, Debt, Gold, Hybrid and International categories with official AMFI NAV data.',
      tags: ['Large Cap', 'Mid Cap', 'Small Cap', 'Debt', 'Gold', 'International'], color: '#6366f1', featured: false
    },
    {
      icon: 'sparkles', title: 'AI-Powered Advice', subtitle: 'Ask WealthAI Anything',
      desc: '',
      queries: ['"What SIP should I start?"', '"What is NAV?"', '"Compare SBI vs Nippon"', '"Can I retire in 20 years?"'],
      tags: [], color: '#14b8a6', featured: true
    },
    {
      icon: 'pieChart', title: 'Portfolio Analytics', subtitle: 'Deep Insights',
      desc: 'Review diversification, risk alignment, fund overlap, rebalancing opportunities and stress-test scenarios.',
      tags: ['Analysis', 'Stress Test', 'What-If', 'Rebalance'], color: '#10b981', featured: false
    }
  ];

  // Steps
  steps = [
    { icon: 'userRound', title: 'Create Profile', desc: 'Income, savings, and goals' },
    { icon: 'clipboardCheck', title: 'Risk Assessment', desc: '15 questions, 5 minutes' },
    { icon: 'sparkles', title: 'AI Recommends', desc: 'Personalized allocation' },
    { icon: 'chart', title: 'Track & Grow', desc: 'Monitor and rebalance' }
  ];

  // Features - AI highlighted
  aiFeature = {
    icon: 'sparkles', title: 'WealthAI Advisor',
    desc: 'GPT-powered financial assistant that explains investments in simple language. Ask about SIPs, NAV, risk profiles, and fund comparisons for clear, personalized guidance.',
    queries: ['What SIP amount is right for me?', 'Explain expense ratio', 'Is my portfolio diversified?']
  };

  features = [
    { icon: 'target', title: 'Goal Planning', desc: 'Set wealth, retirement, education goals with SIP tracking' },
    { icon: 'database', title: 'Official AMFI Data', desc: 'NAV data synced daily from the official AMFI feed' },
    { icon: 'receipt', title: 'Tax Optimization', desc: 'ELSS recommendations & Section 80C calculator' },
    { icon: 'calculator', title: 'SIP Calculator', desc: 'Project returns over your investment horizon' },
    { icon: 'shieldAlert', title: 'Stress Testing', desc: 'Explore hypothetical -10% to -50% market scenarios' }
  ];

  directGrowthPaths = [
    {
      title: 'Regular Plan',
      icon: 'users',
      recommended: false,
      steps: [
        { label: 'Investor chooses a regular plan', icon: 'userRound' },
        { label: 'Distributor commission is included', icon: 'badgeIndianRupee' },
        { label: 'Higher expense ratio', icon: 'percent' },
        { label: 'Less of the investment stays compounded', icon: 'walletCards' }
      ]
    },
    {
      title: 'Direct Growth Plan',
      icon: 'trendingUp',
      recommended: true,
      steps: [
        { label: 'Investor chooses the fund directly', icon: 'userRound' },
        { label: 'No distributor commission', icon: 'badgeIndianRupee' },
        { label: 'Lower expense ratio', icon: 'percent' },
        { label: 'More of the investment stays compounded', icon: 'walletCards' }
      ]
    }
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
    { icon: 'lock', label: 'Secure Login' },
    { icon: 'database', label: 'Official AMFI Data' },
    { icon: 'sparkles', label: 'AI Explanations' },
    { icon: 'trendingUp', label: 'Direct Growth Focus' },
    { icon: 'mapPin', label: 'Built for India' }
  ];

  // Screenshots/Teasers
  screenTeasers = [
    { icon: 'layoutDashboard', title: 'Dashboard Analytics', desc: 'Risk score, allocation chart, SIP schedule' },
    { icon: 'messageCircle', title: 'AI Advisor Chat', desc: 'Ask anything about mutual funds' },
    { icon: 'pieChart', title: 'Portfolio Insights', desc: 'Diversification, overlap, rebalancing' },
    { icon: 'calculator', title: 'SIP & Goal Calculators', desc: 'Plan your financial future' },
    { icon: 'shieldAlert', title: 'Stress Testing', desc: 'Explore hypothetical market declines' },
    { icon: 'fileChart', title: 'Smart Reports', desc: 'Risk, portfolio & recommendation reports' }
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
    this.updateActiveSection();

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

  scrollToSection(event: Event, target: string) {
    event.preventDefault();
    document.getElementById(target)?.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }

  private updateActiveSection() {
    const sectionIds = this.navLinks.map(link => link.target);
    const scrollPosition = window.scrollY + 180;
    let currentSection = sectionIds[0];

    for (const sectionId of sectionIds) {
      const section = document.getElementById(sectionId);
      if (section && section.offsetTop <= scrollPosition) {
        currentSection = sectionId;
      }
    }

    this.activeSection = currentSection;
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
