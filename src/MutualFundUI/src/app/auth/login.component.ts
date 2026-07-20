import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../shared/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="auth-page">
      <div class="auth-left">
        <div class="auth-left-content">
          <div class="auth-brand">
            <span class="brand-icon">&#9670;</span>
            <span>WealthAI</span>
          </div>
          <h2>Welcome back to your<br>investment dashboard</h2>
          <p>Track your portfolio, review recommendations, and chat with your AI advisor.</p>
          <div class="auth-features">
            <div class="auth-feature"><span>&#10003;</span> AI-powered fund recommendations</div>
            <div class="auth-feature"><span>&#10003;</span> Portfolio analysis & insights</div>
            <div class="auth-feature"><span>&#10003;</span> Stress testing & risk profiling</div>
          </div>
        </div>
      </div>

      <div class="auth-right">
        <div class="auth-card animate-in">
          <h1>Sign In</h1>
          <p class="auth-subtitle">Enter your credentials to access your account</p>

          <div class="form-group mt-6">
            <label>Email Address</label>
            <input type="email" [(ngModel)]="email" placeholder="you&#64;example.com">
          </div>

          <div class="form-group">
            <label>Password</label>
            <input type="password" [(ngModel)]="password" placeholder="Enter your password">
          </div>

          <div class="form-actions-row">
            <label class="remember-me">
              <input type="checkbox"> Remember me
            </label>
            <a routerLink="/forgot-password" class="forgot-link">Forgot password?</a>
          </div>

          <p class="error-text" *ngIf="error">{{ error }}</p>

          <button class="btn btn-primary btn-lg" style="width:100%" (click)="login()" [disabled]="loading">
            {{ loading ? 'Signing in...' : 'Sign In' }}
          </button>

          <div class="auth-divider">
            <span>Don't have an account?</span>
          </div>

          <a routerLink="/register" class="btn btn-secondary btn-lg" style="width:100%">Create Account</a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .auth-page { display: flex; min-height: 100vh; }

    .auth-left { flex: 1; background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%); padding: 48px; display: flex; align-items: center; justify-content: center; position: relative; overflow: hidden; }
    .auth-left::before { content: ''; position: absolute; top: -50%; right: -30%; width: 500px; height: 500px; background: radial-gradient(circle, rgba(99,102,241,0.15), transparent 70%); }
    .auth-left::after { content: ''; position: absolute; bottom: -30%; left: -20%; width: 400px; height: 400px; background: radial-gradient(circle, rgba(6,182,212,0.1), transparent 70%); }
    .auth-left-content { position: relative; color: white; max-width: 420px; }
    .auth-brand { display: flex; align-items: center; gap: 10px; font-size: 20px; font-weight: 800; margin-bottom: 40px; }
    .brand-icon { color: #818cf8; font-size: 24px; }
    .auth-left-content h2 { font-size: 32px; font-weight: 800; line-height: 1.2; letter-spacing: -0.8px; margin-bottom: 16px; }
    .auth-left-content p { color: #94a3b8; font-size: 15px; line-height: 1.7; margin-bottom: 32px; }
    .auth-features { display: flex; flex-direction: column; gap: 12px; }
    .auth-feature { display: flex; align-items: center; gap: 10px; font-size: 14px; color: #cbd5e1; }
    .auth-feature span { color: #6366f1; font-weight: 700; }

    .auth-right { flex: 1; display: flex; align-items: center; justify-content: center; padding: 48px; background: #f8fafc; }
    .auth-card { width: 100%; max-width: 400px; }
    .auth-card h1 { font-size: 28px; font-weight: 800; color: #0f172a; letter-spacing: -0.5px; }
    .auth-subtitle { color: #64748b; font-size: 14px; margin-top: 6px; }

    .form-actions-row { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
    .remember-me { display: flex; align-items: center; gap: 6px; font-size: 13px; color: #64748b; cursor: pointer; }
    .remember-me input { width: 14px; height: 14px; accent-color: #6366f1; }
    .forgot-link { font-size: 13px; color: #6366f1; text-decoration: none; font-weight: 600; }
    .forgot-link:hover { text-decoration: underline; }

    .auth-divider { text-align: center; margin: 20px 0; position: relative; }
    .auth-divider::before { content: ''; position: absolute; left: 0; right: 0; top: 50%; height: 1px; background: #e2e8f0; }
    .auth-divider span { position: relative; background: #f8fafc; padding: 0 16px; font-size: 13px; color: #94a3b8; }

    @media (max-width: 768px) {
      .auth-page { flex-direction: column; }
      .auth-left { display: none; }
      .auth-right { padding: 32px 20px; }
    }
  `]
})
export class LoginComponent {
  email = '';
  password = '';
  error = '';
  loading = false;

  constructor(private authService: AuthService, private router: Router) {}

  login() {
    this.error = '';
    this.loading = true;
    this.authService.login(this.email, this.password).subscribe({
      next: (res: any) => {
        this.loading = false;
        if (res.isEmailVerified === false) {
          this.error = res.message || 'Please verify your email before logging in.';
          return;
        }
        this.router.navigate(['/dashboard']);
      },
      error: () => {
        this.loading = false;
        this.error = 'Invalid email or password';
      }
    });
  }
}
