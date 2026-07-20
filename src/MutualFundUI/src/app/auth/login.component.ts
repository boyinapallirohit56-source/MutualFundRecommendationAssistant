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
    <div class="auth-container">
      <div class="auth-card animate-in">
        <div class="auth-header">
          <a routerLink="/" class="back-brand">MF Advisor</a>
          <h1>Welcome Back</h1>
          <p>Sign in to your investment dashboard</p>
        </div>

        <div class="form-group mt-4">
          <label>Email</label>
          <input type="email" [(ngModel)]="email" placeholder="your@email.com">
        </div>

        <div class="form-group">
          <label>Password</label>
          <input type="password" [(ngModel)]="password" placeholder="Enter your password">
        </div>

        <div style="text-align:right; margin-bottom:16px">
          <a routerLink="/forgot-password" class="forgot-link">Forgot password?</a>
        </div>

        <p class="error-text" *ngIf="error">{{ error }}</p>

        <button class="btn btn-primary" style="width:100%" (click)="login()" [disabled]="loading">
          {{ loading ? 'Signing in...' : 'Sign In' }}
        </button>

        <p class="mt-4 text-center auth-link">
          Don't have an account? <a routerLink="/register">Create one</a>
        </p>
      </div>
    </div>
  `,
  styles: [`
    .auth-container { display: flex; justify-content: center; align-items: center; min-height: 100vh; padding: 20px; background: linear-gradient(135deg, #f8fafc, #eff6ff); }
    .auth-card { background: white; padding: 40px; border-radius: 16px; box-shadow: 0 8px 24px rgba(0,0,0,0.08); width: 100%; max-width: 420px; border: 1px solid #f3f4f6; }
    .auth-header { margin-bottom: 8px; }
    .back-brand { font-size: 14px; font-weight: 700; color: #1e40af; text-decoration: none; display: block; margin-bottom: 20px; }
    .auth-header h1 { font-size: 24px; font-weight: 800; color: #111827; margin-bottom: 6px; }
    .auth-header p { color: #6b7280; font-size: 14px; }
    .auth-link { font-size: 14px; color: #6b7280; }
    .auth-link a { color: #1e40af; text-decoration: none; font-weight: 600; }
    .forgot-link { font-size: 13px; color: #1e40af; text-decoration: none; font-weight: 500; }
    .forgot-link:hover { text-decoration: underline; }
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
      next: (res) => {
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
