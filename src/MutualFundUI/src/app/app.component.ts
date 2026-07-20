import { Component, OnInit } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './shared/services/auth.service';
import { ApiService } from './shared/services/api.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, CommonModule],
  template: `
    <nav class="navbar" *ngIf="authService.isLoggedIn()">
      <a class="navbar-brand" routerLink="/dashboard">MF Advisor</a>
      <div class="navbar-links">
        <a routerLink="/dashboard">Dashboard</a>
        <a routerLink="/portfolio">Portfolio</a>
        <a routerLink="/funds">Funds</a>
        <a routerLink="/watchlist">Watchlist</a>
        <a routerLink="/chat">AI Chat</a>
        <a routerLink="/sip-calculator">Calculators</a>
        <a routerLink="/what-if">What If</a>
        <a routerLink="/tax-saving">Tax Saving</a>
        <a routerLink="/financial-health">Health Score</a>
        <a routerLink="/stress-test">Stress Test</a>
        <a routerLink="/reports">Reports</a>
        <a routerLink="/admin" *ngIf="isAdmin()">Admin</a>

        <!-- Dark Mode Toggle -->
        <span class="dark-toggle" (click)="toggleDarkMode()">{{ isDarkMode ? '&#9728;' : '&#127769;' }}</span>

        <!-- Notification Bell -->
        <div class="notification-wrapper" (click)="toggleNotifications()">
          <span class="bell-icon">&#128276;</span>
          <span class="notif-badge" *ngIf="unreadCount > 0">{{ unreadCount }}</span>

          <!-- Dropdown -->
          <div class="notif-dropdown" *ngIf="showNotifications">
            <div class="notif-header">
              <strong>Notifications</strong>
              <span class="mark-all" (click)="markAllRead($event)" *ngIf="unreadCount > 0">Mark all read</span>
            </div>
            <div class="notif-list">
              <div class="notif-item" *ngFor="let n of notifications" [class.unread]="!n.isRead" (click)="markRead(n)">
                <div class="notif-title">{{ n.title }}</div>
                <div class="notif-msg">{{ n.message }}</div>
                <div class="notif-time">{{ n.createdAt | date:'short' }}</div>
              </div>
              <div class="notif-empty" *ngIf="!notifications.length">No notifications yet</div>
            </div>
          </div>
        </div>

        <a (click)="logout()" style="cursor:pointer;color:#dc2626">Logout</a>
      </div>
    </nav>
    <router-outlet></router-outlet>
  `,
  styles: [`
    .notification-wrapper { position: relative; cursor: pointer; display: flex; align-items: center; }
    .bell-icon { font-size: 18px; }
    .notif-badge { position: absolute; top: -6px; right: -8px; background: #dc2626; color: white; font-size: 10px; width: 16px; height: 16px; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-weight: 600; }
    .notif-dropdown { position: absolute; top: 30px; right: 0; width: 320px; background: white; border-radius: 12px; box-shadow: 0 4px 20px rgba(0,0,0,0.15); z-index: 1000; overflow: hidden; }
    .notif-header { display: flex; justify-content: space-between; align-items: center; padding: 12px 16px; border-bottom: 1px solid #f3f4f6; }
    .notif-header strong { font-size: 14px; }
    .mark-all { font-size: 12px; color: #2563eb; cursor: pointer; }
    .notif-list { max-height: 300px; overflow-y: auto; }
    .notif-item { padding: 12px 16px; border-bottom: 1px solid #f9fafb; cursor: pointer; }
    .notif-item:hover { background: #f9fafb; }
    .notif-item.unread { border-left: 3px solid #2563eb; }
    .notif-title { font-size: 13px; font-weight: 600; color: #374151; }
    .notif-msg { font-size: 12px; color: #6b7280; margin-top: 2px; }
    .notif-time { font-size: 11px; color: #9ca3af; margin-top: 4px; }
    .notif-empty { padding: 24px; text-align: center; color: #6b7280; font-size: 13px; }
  `]
})
export class AppComponent implements OnInit {
  showNotifications = false;
  notifications: any[] = [];
  unreadCount = 0;
  isDarkMode = false;

  constructor(public authService: AuthService, private apiService: ApiService) {}

  ngOnInit() {
    if (this.authService.isLoggedIn()) {
      this.loadNotifications();
    }
    // Restore dark mode preference
    this.isDarkMode = localStorage.getItem('darkMode') === 'true';
    if (this.isDarkMode) document.body.classList.add('dark-mode');
  }

  loadNotifications() {
    this.apiService.getNotifications().subscribe({
      next: (res) => { this.notifications = res; }
    });
    this.apiService.getNotificationCount().subscribe({
      next: (res) => { this.unreadCount = res.unread; }
    });
  }

  toggleNotifications() {
    this.showNotifications = !this.showNotifications;
    if (this.showNotifications) {
      this.loadNotifications();
    }
  }

  markRead(notification: any) {
    if (!notification.isRead) {
      this.apiService.markNotificationRead(notification.id).subscribe({
        next: () => {
          notification.isRead = true;
          this.unreadCount = Math.max(0, this.unreadCount - 1);
        }
      });
    }
  }

  markAllRead(event: Event) {
    event.stopPropagation();
    this.apiService.markAllNotificationsRead().subscribe({
      next: () => {
        this.notifications.forEach(n => n.isRead = true);
        this.unreadCount = 0;
      }
    });
  }

  logout() {
    this.authService.logout();
  }

  toggleDarkMode() {
    this.isDarkMode = !this.isDarkMode;
    document.body.classList.toggle('dark-mode');
    localStorage.setItem('darkMode', this.isDarkMode.toString());
  }

  isAdmin(): boolean {
    const user = this.authService.getUser();
    return user?.role === 'Admin';
  }
}
