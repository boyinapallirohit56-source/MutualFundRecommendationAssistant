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
    <div class="auth-container">
      <div class="auth-card animate-in">
        <div class="auth-header">
          <a routerLink="/" class="back-brand">MF Advisor</a>
          <h1>Create Account</h1>
          <p>Start your investment journey today</p>
        </div>

        <div *ngIf="!registered">
          <div class="form-group mt-4">
            <label>Full Name</label>
            <input type="text" [(ngModel)]="name" placeholder="Your full name">
          </div>

          <div class="form-group">
            <label>Email</label>
            <input type="email" [(ngModel)]="email" placeholder="your@email.com">
          </div>

          <div class="form-group">
            <label>Password</label>
            <input type="password" [(ngModel)]="password" placeholder="Min 6 characters">
          </div>

          <p class="error-text" *ngIf="error">{{ error }}</p>

          <button class="btn btn-primary" style="width:100%" (click)="register()" [disabled]="loading">
            {{ loading ? 'Creating account...' : 'Create Account' }}
          </button>
        </div>

        <div *ngIf="registered" class="success-state">
          <div class="success-icon">&#9993;</div>
          <h3>Check Your Email!</h3>
          <p>We've sent a verification link to <strong>{{ email }}</strong>. Click the link to activate your account.</p>
          <button class="btn btn-secondary mt-4" (click)="router.navigate(['/login'])">Go to Login</button>
        </div>

        <p class="mt-4 text-center auth-link" *ngIf="!registered">
          Already have an account? <a routerLink="/login">Sign In</a>
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
    .success-state { text-align: center; padding: 20px 0; }
    .success-icon { font-size: 48px; margin-bottom: 16px; }
    .success-state h3 { font-size: 18px; margin-bottom: 8px; color: #111827; }
    .success-state p { color: #4b5563; font-size: 14px; line-height: 1.6; }
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
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Registration failed';
      }
    });
  }
}
