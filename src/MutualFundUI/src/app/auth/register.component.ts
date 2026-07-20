import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../shared/services/auth.service';

@Component({
  selector: 'app-register',
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
          <h2>Start your intelligent<br>investment journey</h2>
          <p>Create an account to receive AI-powered mutual fund recommendations tailored to your financial goals.</p>
          <div class="auth-features">
            <div class="auth-feature"><span>&#10003;</span> Free personalized risk assessment</div>
            <div class="auth-feature"><span>&#10003;</span> AI explains every recommendation</div>
            <div class="auth-feature"><span>&#10003;</span> No investment experience needed</div>
          </div>
        </div>
      </div>

      <div class="auth-right">
        <div class="auth-card animate-in">
          <div *ngIf="!registered">
            <h1>Create Account</h1>
            <p class="auth-subtitle">Begin your investment journey in 2 minutes</p>

            <div class="form-group mt-6">
              <label>Full Name</label>
              <input type="text" [(ngModel)]="name" placeholder="Your full name">
            </div>

            <div class="form-group">
              <label>Email Address</label>
              <input type="email" [(ngModel)]="email" placeholder="you&#64;example.com">
            </div>

            <div class="form-group">
              <label>Password</label>
              <input type="password" [(ngModel)]="password" placeholder="Min 6 characters">
            </div>

            <p class="error-text" *ngIf="error">{{ error }}</p>

            <button class="btn btn-primary btn-lg" style="width:100%" (click)="register()" [disabled]="loading">
              {{ loading ? 'Creating account...' : 'Create Account' }}
            </button>

            <div class="auth-divider">
              <span>Already have an account?</span>
            </div>

            <a routerLink="/login" class="btn btn-secondary btn-lg" style="width:100%">Sign In</a>
          </div>

          <!-- Success State -->
          <div *ngIf="registered" class="success-state animate-scale-in">
            <div class="success-icon-wrapper">
              <div class="success-icon">&#9993;</div>
            </div>
            <h2>Check Your Email</h2>
            <p>We've sent a verification link to <strong>{{ email }}</strong></p>
            <p class="success-hint">Click the link in the email to activate your account and start investing.</p>
            <button class="btn btn-primary btn-lg mt-6" (click)="router.navigate(['/login'])">Go to Sign In</button>
          </div>
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

    .auth-divider { text-align: center; margin: 20px 0; position: relative; }
    .auth-divider::before { content: ''; position: absolute; left: 0; right: 0; top: 50%; height: 1px; background: #e2e8f0; }
    .auth-divider span { position: relative; background: #f8fafc; padding: 0 16px; font-size: 13px; color: #94a3b8; }

    .success-state { text-align: center; padding: 20px 0; }
    .success-icon-wrapper { width: 80px; height: 80px; border-radius: 50%; background: rgba(99,102,241,0.08); display: flex; align-items: center; justify-content: center; margin: 0 auto 24px; }
    .success-icon { font-size: 36px; }
    .success-state h2 { font-size: 22px; font-weight: 800; color: #0f172a; margin-bottom: 8px; }
    .success-state p { color: #64748b; font-size: 14px; line-height: 1.6; }
    .success-hint { margin-top: 8px; font-size: 13px; color: #94a3b8; }

    @media (max-width: 768px) {
      .auth-page { flex-direction: column; }
      .auth-left { display: none; }
      .auth-right { padding: 32px 20px; }
    }
  `]
})
export class RegisterComponent {
  name = '';
  email = '';
  password = '';
  error = '';
  loading = false;
  registered = false;

  constructor(private authService: AuthService, public router: Router) {}

  register() {
    this.error = '';
    this.loading = true;
    this.authService.register(this.name, this.email, this.password).subscribe({
      next: () => {
        this.loading = false;
        this.registered = true;
      },
      error: (err: any) => {
        this.loading = false;
        this.error = err.error?.message || 'Registration failed';
      }
    });
  }
}
