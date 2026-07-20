import { Routes } from '@angular/router';
import { authGuard } from './shared/guards/auth.guard';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./landing/landing.component').then(m => m.LandingComponent) },
  { path: 'login', loadComponent: () => import('./auth/login.component').then(m => m.LoginComponent) },
  { path: 'register', loadComponent: () => import('./auth/register.component').then(m => m.RegisterComponent) },
  { path: 'forgot-password', loadComponent: () => import('./auth/forgot-password.component').then(m => m.ForgotPasswordComponent) },
  { path: 'onboarding', loadComponent: () => import('./onboarding/onboarding.component').then(m => m.OnboardingComponent), canActivate: [authGuard] },
  { path: 'risk-assessment', loadComponent: () => import('./risk-assessment/risk-assessment.component').then(m => m.RiskAssessmentComponent), canActivate: [authGuard] },
  { path: 'dashboard', loadComponent: () => import('./dashboard/dashboard.component').then(m => m.DashboardComponent), canActivate: [authGuard] },
  { path: 'portfolio', loadComponent: () => import('./portfolio/portfolio.component').then(m => m.PortfolioComponent), canActivate: [authGuard] },
  { path: 'funds', loadComponent: () => import('./funds/fund-list.component').then(m => m.FundListComponent), canActivate: [authGuard] },
  { path: 'funds/compare', loadComponent: () => import('./funds/fund-compare.component').then(m => m.FundCompareComponent), canActivate: [authGuard] },
  { path: 'funds/:id', loadComponent: () => import('./funds/fund-factsheet.component').then(m => m.FundFactsheetComponent), canActivate: [authGuard] },
  { path: 'chat', loadComponent: () => import('./chat/chat.component').then(m => m.ChatComponent), canActivate: [authGuard] },
  { path: 'stress-test', loadComponent: () => import('./stress-test/stress-test.component').then(m => m.StressTestComponent), canActivate: [authGuard] },
  { path: 'reports', loadComponent: () => import('./reports/reports.component').then(m => m.ReportsComponent), canActivate: [authGuard] },
  { path: 'watchlist', loadComponent: () => import('./watchlist/watchlist.component').then(m => m.WatchlistComponent), canActivate: [authGuard] },
  { path: 'admin', loadComponent: () => import('./admin/admin.component').then(m => m.AdminComponent), canActivate: [authGuard] },
  { path: '**', redirectTo: '/login' }
];
