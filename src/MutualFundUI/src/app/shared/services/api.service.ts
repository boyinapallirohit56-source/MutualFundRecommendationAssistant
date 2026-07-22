import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // User Profile
  getProfile(): Observable<any> {
    return this.http.get(`${this.apiUrl}/users/profile`);
  }

  saveProfile(profile: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/users/profile`, profile);
  }

  // Risk Assessment
  getQuestions(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/risk-assessment/questions`);
  }

  submitAssessment(answers: any[]): Observable<any> {
    return this.http.post(`${this.apiUrl}/risk-assessment/submit`, { answers });
  }

  getLatestAssessment(): Observable<any> {
    return this.http.get(`${this.apiUrl}/risk-assessment/latest`);
  }

  // Recommendations
  generateRecommendation(): Observable<any> {
    return this.http.post(`${this.apiUrl}/recommendations/generate`, {});
  }

  getLatestRecommendation(): Observable<any> {
    return this.http.get(`${this.apiUrl}/recommendations/latest`);
  }

  // Portfolio
  getPortfolio(): Observable<any> {
    return this.http.get(`${this.apiUrl}/portfolio`);
  }

  addHolding(holding: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/portfolio/holdings`, holding);
  }

  removeHolding(holdingId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/portfolio/holdings/${holdingId}`);
  }

  analyzePortfolio(): Observable<any> {
    return this.http.get(`${this.apiUrl}/portfolio/analyze`);
  }

  uploadPortfolioFile(endpoint: string, formData: FormData): Observable<any> {
    return this.http.post(`${this.apiUrl}/portfolio/${endpoint}`, formData);
  }

  // Funds
  listFunds(category?: string, search?: string): Observable<any[]> {
    let params: any = {};
    if (category) params.category = category;
    if (search) params.search = search;
    return this.http.get<any[]>(`${this.apiUrl}/funds`, { params });
  }

  getFundFactsheet(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/funds/${id}`);
  }

  compareFunds(fundIds: number[]): Observable<any> {
    return this.http.post(`${this.apiUrl}/funds/compare`, { fundIds });
  }

  // AI Chat
  sendChatMessage(message: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/ai/chat`, { message });
  }

  getChatHistory(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/ai/chat/history`);
  }

  // Reports
  getRiskAssessmentReport(): Observable<any> {
    return this.http.get(`${this.apiUrl}/reports/risk-assessment`);
  }

  getRecommendationReport(): Observable<any> {
    return this.http.get(`${this.apiUrl}/reports/recommendation`);
  }

  getPortfolioReport(): Observable<any> {
    return this.http.get(`${this.apiUrl}/reports/portfolio`);
  }

  runStressTest(scenarios?: any[]): Observable<any> {
    return this.http.post(`${this.apiUrl}/reports/stress-test`, scenarios ? { scenarios } : null);
  }

  // Watchlist
  getWatchlist(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/watchlist`);
  }

  addToWatchlist(mutualFundId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/watchlist`, { mutualFundId });
  }

  removeFromWatchlist(itemId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/watchlist/${itemId}`);
  }

  // Notifications
  getNotifications(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/notifications`);
  }

  getNotificationCount(): Observable<any> {
    return this.http.get(`${this.apiUrl}/notifications/count`);
  }

  markNotificationRead(id: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/notifications/${id}/read`, {});
  }

  markAllNotificationsRead(): Observable<any> {
    return this.http.put(`${this.apiUrl}/notifications/read-all`, {});
  }

  // Admin
  getAdminUsers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/admin/users`);
  }

  updateUserStatus(userId: number, isActive: boolean): Observable<any> {
    return this.http.put(`${this.apiUrl}/admin/users/${userId}/status`, { isActive });
  }

  getAdminAnalytics(): Observable<any> {
    return this.http.get(`${this.apiUrl}/admin/analytics`);
  }

  // Auth - Forgot Password
  forgotPassword(email: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/auth/forgot-password`, { email });
  }

  resetPassword(token: string, newPassword: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/auth/reset-password`, { token, newPassword });
  }

  resendVerification(email: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/auth/resend-verification`, { email });
  }

  // Dashboard aggregation
  getDashboardData(): Observable<any> {
    return this.http.get(`${this.apiUrl}/dashboard`);
  }

  // Goals
  getGoals(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/goals`);
  }

  createGoalsBatch(goals: any[]): Observable<any> {
    return this.http.post(`${this.apiUrl}/goals/batch`, goals);
  }

  updateGoalProgress(goalId: number, currentAmount: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/goals/${goalId}/progress`, { currentAmount });
  }
}
