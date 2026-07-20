import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="auth-container">
      <div class="auth-card animate-in">
        <div class="auth-header">
          <h1>Reset Password</h1>
          <p>Enter your email and we'll send you a reset link</p>
        </div>

        <div *ngIf="!sent">
          <div class="form-group mt-4">
            <label>Email Address</label>
            <input type="email" [(ngModel)]="email" placeholder="your@email.com">
          </div>

          <button class="btn btn-primary" style="width:100%" (click)="submit()" [disabled]="loading || !email">
            {{ loading ? 'Sending...' : 'Send Reset Link' }}
          </button>
        </div>

        <div *ngIf="sent" class="success-state">
          <div class="success-icon">&#9989;</div>
          <h3>Check Your Email</h3>
          <p>If an account exists with <strong>{{ email }}</strong>, we've sent a password reset link.</p>
          <p class="hint">Check your spam folder if you don't see it within a few minutes.</p>
        </div>

        <p class="mt-4 text-center auth-link">
          Remember your password? <a routerLink="/login">Sign In</a>
        </p>
      </div>
    </div>
  `,
  styles: [`
    .auth-container { display: flex; justify-content: center; align-items: center; min-height: 100vh; padding: 20px; background: linear-gradient(135deg, #f8fafc, #eff6ff); }
    .auth-card { background: white; padding: 40px; border-radius: 16px; box-shadow: 0 8px 24px rgba(0,0,0,0.08); width: 100%; max-width: 420px; border: 1px solid #f3f4f6; }
    .auth-header h1 { font-size: 24px; font-weight: 800; color: #111827; margin-bottom: 6px; }
    .auth-header p { color: #6b7280; font-size: 14px; }
    .auth-link { font-size: 14px; color: #6b7280; }
    .auth-link a { color: #1e40af; text-decoration: none; font-weight: 600; }
    .success-state { text-align: center; padding: 20px 0; }
    .success-icon { font-size: 48px; margin-bottom: 16px; }
    .success-state h3 { font-size: 18px; margin-bottom: 8px; }
    .success-state p { color: #4b5563; font-size: 14px; line-height: 1.6; }
    .hint { margin-top: 12px; font-size: 13px; color: #9ca3af; }
  `]
})
export class ForgotPasswordComponent {
  email = '';
  loading = false;
  sent = false;

  constructor(private apiService: ApiService) {}

  submit() {
    this.loading = true;
    this.apiService.forgotPassword(this.email).subscribe({
      next: () => { this.sent = true; this.loading = false; },
      error: () => { this.sent = true; this.loading = false; } // Show success even on error for security
    });
  }
}
