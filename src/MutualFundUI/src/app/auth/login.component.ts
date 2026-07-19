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
      <div class="auth-card">
        <h1>Welcome Back</h1>
        <p>Sign in to your account</p>

        <div class="form-group mt-4">
          <label>Email</label>
          <input type="email" [(ngModel)]="email" placeholder="Enter your email">
        </div>

        <div class="form-group">
          <label>Password</label>
          <input type="password" [(ngModel)]="password" placeholder="Enter your password">
        </div>

        <p class="error-text" *ngIf="error">{{ error }}</p>

        <button class="btn btn-primary" style="width:100%" (click)="login()" [disabled]="loading">
          {{ loading ? 'Signing in...' : 'Sign In' }}
        </button>

        <p class="mt-4 text-center">
          Don't have an account? <a routerLink="/register">Register</a>
        </p>
      </div>
    </div>
  `,
  styles: [`
    .auth-container { display: flex; justify-content: center; align-items: center; min-height: 100vh; padding: 20px; }
    .auth-card { background: white; padding: 40px; border-radius: 12px; box-shadow: 0 4px 12px rgba(0,0,0,0.1); width: 100%; max-width: 400px; }
    .auth-card h1 { font-size: 24px; margin-bottom: 4px; }
    .auth-card p { color: #6b7280; }
    .auth-card a { color: #2563eb; text-decoration: none; }
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
      next: () => {
        this.loading = false;
        this.router.navigate(['/dashboard']);
      },
      error: () => {
        this.loading = false;
        this.error = 'Invalid email or password';
      }
    });
  }
}
