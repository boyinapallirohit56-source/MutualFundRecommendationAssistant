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
}
