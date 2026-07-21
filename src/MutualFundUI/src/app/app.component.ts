import { Component, OnInit } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './shared/services/auth.service';
import { ApiService } from './shared/services/api.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, CommonModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
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
