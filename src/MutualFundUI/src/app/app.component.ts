import { Component } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './shared/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, CommonModule],
  template: `
    <nav class="navbar" *ngIf="authService.isLoggedIn()">
      <a class="navbar-brand" routerLink="/dashboard">MF Advisor</a>
      <div class="navbar-links">
        <a routerLink="/dashboard">Dashboard</a>
        <a routerLink="/onboarding">Profile</a>
        <a routerLink="/risk-assessment">Assessment</a>
        <a (click)="logout()" style="cursor:pointer;color:#dc2626">Logout</a>
      </div>
    </nav>
    <router-outlet></router-outlet>
  `
})
export class AppComponent {
  constructor(public authService: AuthService) {}

  logout() {
    this.authService.logout();
  }
}
