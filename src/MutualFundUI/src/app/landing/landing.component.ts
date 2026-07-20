import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="landing">
      <!-- Hero Section -->
      <nav class="landing-nav">
        <span class="brand">MF Advisor</span>
        <div class="nav-actions">
          <a routerLink="/login" class="btn btn-secondary">Sign In</a>
          <a routerLink="/register" class="btn btn-primary">Get Started</a>
        </div>
      </nav>

      <section class="hero">
        <div class="hero-content">
          <span class="hero-badge">AI-Powered Investment Platform</span>
          <h1>Smart Mutual Fund<br>Recommendations</h1>
          <p>
            Don't know where to invest? Answer a few questions about your financial
            situation and get personalized mutual fund recommendations with AI-powered
            explanations in simple language.
          </p>
          <div class="hero-actions">
            <a routerLink="/register" class="btn btn-primary btn-lg">Start Free Assessment</a>
            <a routerLink="/login" class="btn btn-outline btn-lg">I have an account</a>
          </div>
          <div class="hero-stats">
            <div class="hero-stat">
              <strong>15+</strong>
              <span>Risk Questions</span>
            </div>
            <div class="hero-stat">
              <strong>18+</strong>
              <span>Mutual Funds</span>
            </div>
            <div class="hero-stat">
              <strong>6</strong>
              <span>Asset Classes</span>
            </div>
            <div class="hero-stat">
              <strong>AI</strong>
              <span>Powered</span>
            </div>
          </div>
        </div>
      </section>

      <!-- Features Section -->
      <section class="features">
        <div class="features-container">
          <h2>How It Works</h2>
          <p class="features-subtitle">Four simple steps to your personalized investment plan</p>

          <div class="steps">
            <div class="step">
              <div class="step-num">1</div>
              <h3>Tell Us About You</h3>
              <p>Enter your financial details, income, savings, goals, and investment duration</p>
            </div>
            <div class="step">
              <div class="step-num">2</div>
              <h3>Take Risk Assessment</h3>
              <p>Answer 15 questions about your risk tolerance and investment behaviour</p>
            </div>
            <div class="step">
              <div class="step-num">3</div>
              <h3>Get Recommendations</h3>
              <p>Receive a personalized asset allocation with specific fund suggestions</p>
            </div>
            <div class="step">
              <div class="step-num">4</div>
              <h3>Understand Why</h3>
              <p>AI explains every recommendation in simple language you can understand</p>
            </div>
          </div>
        </div>
      </section>

      <!-- Feature Cards -->
      <section class="feature-cards">
        <div class="features-container">
          <h2>Everything You Need</h2>
          <div class="cards-grid">
            <div class="feature-card">
              <div class="fc-icon">&#128202;</div>
              <h3>Risk Assessment</h3>
              <p>15 carefully designed questions to determine your exact risk profile</p>
            </div>
            <div class="feature-card">
              <div class="fc-icon">&#127919;</div>
              <h3>Smart Allocation</h3>
              <p>AI-driven fund selection across Equity, Debt, Gold, and more</p>
            </div>
            <div class="feature-card">
              <div class="fc-icon">&#128200;</div>
              <h3>Portfolio Analysis</h3>
              <p>Upload your portfolio and get diversification score, overlap detection, and insights</p>
            </div>
            <div class="feature-card">
              <div class="fc-icon">&#129302;</div>
              <h3>AI Assistant</h3>
              <p>Ask anything about investing and get simple, jargon-free explanations</p>
            </div>
            <div class="feature-card">
              <div class="fc-icon">&#9888;&#65039;</div>
              <h3>Stress Testing</h3>
              <p>See how your portfolio would react in market crashes and corrections</p>
            </div>
            <div class="feature-card">
              <div class="fc-icon">&#128214;</div>
              <h3>Reports</h3>
              <p>Download detailed PDF reports of your assessment, recommendations, and analysis</p>
            </div>
          </div>
        </div>
      </section>

      <!-- CTA -->
      <section class="cta">
        <h2>Ready to Start Investing Smart?</h2>
        <p>Join thousands of first-time investors making informed decisions</p>
        <a routerLink="/register" class="btn btn-primary btn-lg">Create Free Account</a>
      </section>

      <!-- Footer -->
      <footer class="landing-footer">
        <p>Mutual Fund Advisor &copy; 2026 | For educational purposes only, not financial advice</p>
      </footer>
    </div>
  `,
  styles: [`
    .landing { overflow-x: hidden; }

    .landing-nav { display: flex; justify-content: space-between; align-items: center; padding: 20px 48px; background: white; border-bottom: 1px solid #f3f4f6; }
    .brand { font-size: 22px; font-weight: 800; background: linear-gradient(135deg, #1e40af, #0891b2); -webkit-background-clip: text; -webkit-text-fill-color: transparent; }
    .nav-actions { display: flex; gap: 12px; }

    .hero { background: linear-gradient(135deg, #1e3a8a 0%, #1e40af 50%, #0891b2 100%); padding: 80px 48px; text-align: center; color: white; }
    .hero-content { max-width: 720px; margin: 0 auto; }
    .hero-badge { display: inline-block; background: rgba(255,255,255,0.15); padding: 6px 16px; border-radius: 20px; font-size: 13px; font-weight: 600; margin-bottom: 24px; backdrop-filter: blur(4px); }
    .hero h1 { font-size: 48px; font-weight: 800; line-height: 1.15; margin-bottom: 20px; letter-spacing: -1px; }
    .hero p { font-size: 18px; color: rgba(255,255,255,0.85); max-width: 560px; margin: 0 auto 32px; line-height: 1.7; }
    .hero-actions { display: flex; gap: 16px; justify-content: center; margin-bottom: 48px; }
    .hero-actions .btn-outline { border-color: white; color: white; }
    .hero-actions .btn-outline:hover { background: white; color: #1e40af; }
    .hero-stats { display: flex; gap: 48px; justify-content: center; padding-top: 32px; border-top: 1px solid rgba(255,255,255,0.2); }
    .hero-stat { text-align: center; }
    .hero-stat strong { display: block; font-size: 24px; font-weight: 800; }
    .hero-stat span { font-size: 13px; color: rgba(255,255,255,0.7); }

    .features { padding: 80px 48px; background: white; text-align: center; }
    .features-container { max-width: 1000px; margin: 0 auto; }
    .features h2 { font-size: 32px; font-weight: 800; color: #111827; margin-bottom: 8px; }
    .features-subtitle { color: #6b7280; font-size: 16px; margin-bottom: 48px; }
    .steps { display: grid; grid-template-columns: repeat(4, 1fr); gap: 32px; }
    .step { padding: 24px; border-radius: 12px; background: #f9fafb; border: 1px solid #f3f4f6; }
    .step-num { width: 40px; height: 40px; background: linear-gradient(135deg, #1e40af, #3b82f6); color: white; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-weight: 700; font-size: 16px; margin: 0 auto 16px; }
    .step h3 { font-size: 15px; font-weight: 700; margin-bottom: 8px; color: #1f2937; }
    .step p { font-size: 13px; color: #6b7280; line-height: 1.6; }

    .feature-cards { padding: 80px 48px; background: #f8fafc; text-align: center; }
    .feature-cards h2 { font-size: 32px; font-weight: 800; color: #111827; margin-bottom: 40px; }
    .cards-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 24px; text-align: left; }
    .feature-card { background: white; padding: 28px; border-radius: 12px; box-shadow: 0 2px 8px rgba(0,0,0,0.06); border: 1px solid #f3f4f6; transition: all 0.2s; }
    .feature-card:hover { transform: translateY(-4px); box-shadow: 0 8px 16px rgba(0,0,0,0.1); }
    .fc-icon { font-size: 28px; margin-bottom: 12px; }
    .feature-card h3 { font-size: 16px; font-weight: 700; margin-bottom: 8px; color: #1f2937; }
    .feature-card p { font-size: 13px; color: #6b7280; line-height: 1.6; }

    .cta { padding: 80px 48px; text-align: center; background: linear-gradient(135deg, #1e3a8a, #0891b2); color: white; }
    .cta h2 { font-size: 32px; font-weight: 800; margin-bottom: 12px; }
    .cta p { font-size: 16px; color: rgba(255,255,255,0.8); margin-bottom: 28px; }

    .landing-footer { padding: 24px 48px; text-align: center; background: #1f2937; color: #9ca3af; font-size: 13px; }

    @media (max-width: 768px) {
      .hero h1 { font-size: 28px; }
      .hero { padding: 48px 24px; }
      .steps { grid-template-columns: 1fr 1fr; }
      .cards-grid { grid-template-columns: 1fr; }
      .hero-stats { flex-wrap: wrap; gap: 24px; }
      .landing-nav { padding: 16px 20px; }
      .features, .feature-cards, .cta { padding: 48px 24px; }
    }
  `]
})
export class LandingComponent {}
