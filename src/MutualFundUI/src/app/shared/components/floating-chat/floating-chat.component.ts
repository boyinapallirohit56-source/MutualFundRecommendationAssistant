import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, NavigationEnd, RouterLink } from '@angular/router';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';

interface QuickPrompt {
  icon: string;
  text: string;
  question: string;
}

interface PagePromptConfig {
  [key: string]: QuickPrompt[];
}

@Component({
  selector: 'app-floating-chat',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './floating-chat.component.html',
  styleUrls: ['./floating-chat.component.css']
})
export class FloatingChatComponent implements OnInit, OnDestroy, AfterViewChecked {
  @ViewChild('messagesContainer') private messagesContainer!: ElementRef;

  isOpen = false;
  isMinimized = false;
  messages: any[] = [];
  userMessage = '';
  loading = false;
  currentPage = '';
  quickPrompts: QuickPrompt[] = [];
  hasNewMessage = false;
  private routerSub!: Subscription;
  private shouldScrollToBottom = false;

  // Page-specific quick prompts configuration
  private pagePrompts: PagePromptConfig = {
    'dashboard': [
      { icon: 'trending_up', text: 'Portfolio performance', question: 'How is my portfolio performing overall?' },
      { icon: 'target', text: 'Goal progress', question: 'Am I on track with my investment goals?' },
      { icon: 'lightbulb', text: 'What should I do next?', question: 'Based on my profile and portfolio, what should I do next?' }
    ],
    'portfolio': [
      { icon: 'pie_chart', text: 'Diversification check', question: 'Is my portfolio well-diversified?' },
      { icon: 'trending_down', text: 'Underperformers', question: 'Which funds in my portfolio are underperforming?' },
      { icon: 'balance', text: 'Rebalancing advice', question: 'Should I rebalance my portfolio? How?' }
    ],
    'funds': [
      { icon: 'search', text: 'Fund selection tips', question: 'What should I look for when selecting a mutual fund?' },
      { icon: 'category', text: 'Best category for me', question: 'Which fund category suits my risk profile best?' },
      { icon: 'star', text: 'Top rated funds', question: 'What makes a fund highly rated?' }
    ],
    'fund-compare': [
      { icon: 'compare_arrows', text: 'Compare metrics', question: 'Which metrics matter most when comparing funds?' },
      { icon: 'analytics', text: 'Sharpe ratio explained', question: 'What does the Sharpe ratio tell me about a fund?' },
      { icon: 'help', text: 'Which fund is better?', question: 'How do I decide which fund is better for me from this comparison?' }
    ],
    'funds-compare': [
      { icon: 'compare_arrows', text: 'Compare metrics', question: 'Which metrics matter most when comparing funds?' },
      { icon: 'analytics', text: 'Sharpe ratio explained', question: 'What does the Sharpe ratio tell me about a fund?' },
      { icon: 'help', text: 'Which fund is better?', question: 'How do I decide which fund is better for me from this comparison?' }
    ],
    'risk-assessment': [
      { icon: 'shield', text: 'Risk profile meaning', question: 'What does my risk profile mean for my investments?' },
      { icon: 'psychology', text: 'How risk is calculated', question: 'How is my investment risk score calculated?' },
      { icon: 'swap_vert', text: 'Change risk profile', question: 'Can my risk profile change over time? What affects it?' }
    ],
    'sip-calculator': [
      { icon: 'savings', text: 'Ideal SIP amount', question: 'How much should I invest monthly via SIP based on my income?' },
      { icon: 'timeline', text: 'Power of compounding', question: 'How does compounding work in SIP investments?' },
      { icon: 'date_range', text: 'SIP duration', question: 'How long should I continue my SIP for best results?' }
    ],
    'what-if': [
      { icon: 'science', text: 'Scenario planning', question: 'What happens to my portfolio if the market drops 20%?' },
      { icon: 'trending_up', text: 'Bull market impact', question: 'How much could my portfolio grow in a bull market?' },
      { icon: 'tips_and_updates', text: 'Best strategy', question: 'What is the best investment strategy for uncertain markets?' }
    ],
    'tax-saving': [
      { icon: 'receipt_long', text: 'ELSS benefits', question: 'How much tax can I save with ELSS mutual funds?' },
      { icon: 'account_balance', text: 'Section 80C', question: 'Explain Section 80C tax benefits for mutual fund investments.' },
      { icon: 'calculate', text: 'Tax on returns', question: 'How are mutual fund returns taxed in India?' }
    ],
    'financial-health': [
      { icon: 'health_and_safety', text: 'Improve my score', question: 'How can I improve my financial health score?' },
      { icon: 'emergency', text: 'Emergency fund', question: 'How much should I keep as an emergency fund?' },
      { icon: 'savings', text: 'Savings rate', question: 'What is an ideal savings rate for my income level?' }
    ],
    'stress-test': [
      { icon: 'warning', text: 'Market crash impact', question: 'How would a market crash affect my portfolio?' },
      { icon: 'shield', text: 'Protect portfolio', question: 'How can I protect my portfolio from market downturns?' },
      { icon: 'history', text: 'Historical crashes', question: 'How did Indian markets recover from past crashes?' }
    ],
    'reports': [
      { icon: 'summarize', text: 'Report insights', question: 'What are the key insights from my investment reports?' },
      { icon: 'assessment', text: 'Interpret data', question: 'How do I interpret the data in my portfolio report?' },
      { icon: 'download', text: 'Report types', question: 'What different reports are available and what do they show?' }
    ],
    'watchlist': [
      { icon: 'visibility', text: 'When to invest', question: 'When is a good time to invest in funds from my watchlist?' },
      { icon: 'notifications', text: 'What to track', question: 'What should I monitor for funds in my watchlist?' },
      { icon: 'add_circle', text: 'Add suggestions', question: 'Which funds should I add to my watchlist based on my profile?' }
    ]
  };

  // Default prompts when no page-specific ones match
  private defaultPrompts: QuickPrompt[] = [
    { icon: 'smart_toy', text: 'What can you help with?', question: 'What can you help me with?' },
    { icon: 'savings', text: 'What is SIP?', question: 'What is SIP and how does it work?' },
    { icon: 'shield', text: 'My risk profile', question: 'What does my risk profile mean?' }
  ];

  constructor(
    private apiService: ApiService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    // Listen to route changes to update current page and prompts
    this.routerSub = this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      this.updateCurrentPage(event.urlAfterRedirects || event.url);
    });

    // Set initial page
    this.updateCurrentPage(this.router.url);
  }

  ngOnDestroy() {
    if (this.routerSub) {
      this.routerSub.unsubscribe();
    }
  }

  ngAfterViewChecked() {
    if (this.shouldScrollToBottom) {
      this.scrollToBottom();
      this.shouldScrollToBottom = false;
    }
  }

  private updateCurrentPage(url: string) {
    // Extract page name from URL (remove leading slash and query params)
    const path = url.split('?')[0].replace(/^\//, '');
    this.currentPage = path || 'dashboard';
    this.updateQuickPrompts();
  }

  private updateQuickPrompts() {
    // Check for exact match first, then partial match
    const prompts = this.pagePrompts[this.currentPage];
    if (prompts) {
      this.quickPrompts = prompts;
    } else {
      // Try matching with slashes converted to dashes (e.g., 'funds/compare' -> 'fund-compare')
      const dashedPage = this.currentPage.replace(/\//g, '-');
      if (this.pagePrompts[dashedPage]) {
        this.quickPrompts = this.pagePrompts[dashedPage];
      } else {
        // Try base route matching (e.g., 'funds/123' should match 'funds')
        const baseRoute = this.currentPage.split('/')[0];
        this.quickPrompts = this.pagePrompts[baseRoute] || this.defaultPrompts;
      }
    }
  }

  toggleChat() {
    this.isOpen = !this.isOpen;
    this.hasNewMessage = false;
    if (this.isOpen && this.messages.length === 0) {
      this.loadChatHistory();
    }
    if (this.isOpen) {
      this.shouldScrollToBottom = true;
    }
  }

  closeChat() {
    this.isOpen = false;
  }

  minimizeChat() {
    this.isMinimized = !this.isMinimized;
  }

  private loadChatHistory() {
    this.apiService.getChatHistory().subscribe({
      next: (history) => {
        this.messages = history;
        this.shouldScrollToBottom = true;
      }
    });
  }

  sendMessage() {
    if (!this.userMessage.trim() || this.loading) return;

    const msg = this.userMessage.trim();
    this.messages.push({ role: 'user', content: msg, createdAt: new Date() });
    this.userMessage = '';
    this.loading = true;
    this.shouldScrollToBottom = true;

    this.apiService.sendChatMessage(msg, this.currentPage).subscribe({
      next: (res) => {
        this.messages.push({ role: 'assistant', content: res.reply, createdAt: new Date() });
        this.loading = false;
        this.shouldScrollToBottom = true;
      },
      error: () => {
        this.messages.push({
          role: 'assistant',
          content: "I couldn't generate a response. Please try again.",
          createdAt: new Date()
        });
        this.loading = false;
        this.shouldScrollToBottom = true;
      }
    });
  }

  askQuickPrompt(prompt: QuickPrompt) {
    this.userMessage = prompt.question;
    this.sendMessage();
  }

  private scrollToBottom() {
    try {
      if (this.messagesContainer) {
        const el = this.messagesContainer.nativeElement;
        el.scrollTop = el.scrollHeight;
      }
    } catch (err) {}
  }

  getTimestamp(date: any): string {
    const d = new Date(date);
    return d.toLocaleTimeString('en-IN', { hour: 'numeric', minute: '2-digit', hour12: true });
  }

  isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }
}
