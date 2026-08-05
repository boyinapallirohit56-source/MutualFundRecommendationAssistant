import { Component, OnInit, HostListener } from '@angular/core';
import { RouterOutlet, RouterLink, Router, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './shared/services/auth.service';
import { ApiService } from './shared/services/api.service';
import { FloatingChatComponent } from './shared/components/floating-chat/floating-chat.component';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, CommonModule, FloatingChatComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  showNotifications = false;
  showMore = false;
  notifications: any[] = [];
  unreadCount = 0;
  isDarkMode = false;
  isAuthPage = false;

  // Routes where navbar should be hidden
  private authRoutes = ['/', '/login', '/register', '/forgot-password'];

  constructor(
    public authService: AuthService,
    private apiService: ApiService,
    private router: Router
  ) {}

  ngOnInit() {
    // Track route changes to hide navbar on auth pages
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      this.isAuthPage = this.authRoutes.includes(event.urlAfterRedirects || event.url);
    });

    // Check initial route
    this.isAuthPage = this.authRoutes.includes(this.router.url);

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

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event) {
    const target = event.target as HTMLElement;
    // Close "More" dropdown if click is outside
    if (!target.closest('.more-dropdown-wrapper')) {
      this.showMore = false;
    }
    // Close notification dropdown if click is outside
    if (!target.closest('.notification-wrapper')) {
      this.showNotifications = false;
    }
  }
}
