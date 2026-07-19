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
}
