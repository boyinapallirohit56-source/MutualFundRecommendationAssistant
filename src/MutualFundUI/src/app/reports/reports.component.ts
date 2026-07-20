import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../shared/services/api.service';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container" style="margin-top:24px; max-width:800px">
      <div class="page-header">
        <h1>Reports</h1>
        <p>View and download your investment reports</p>
      </div>

      <div class="reports-grid">
        <!-- Risk Assessment Report -->
        <div class="report-card">
          <div class="report-icon">&#128202;</div>
          <h3>Risk Assessment Report</h3>
          <p>Detailed breakdown of your questionnaire responses and risk score calculation</p>
          <div class="report-actions">
            <button class="btn btn-secondary" (click)="viewReport('risk-assessment')">View</button>
            <button class="btn btn-primary" (click)="downloadPdf('risk-assessment')">Download PDF</button>
          </div>
        </div>

        <!-- Recommendation Report -->
        <div class="report-card">
          <div class="report-icon">&#128200;</div>
          <h3>Recommendation Report</h3>
          <p>Your personalized mutual fund allocation with fund suggestions and AI explanation</p>
          <div class="report-actions">
            <button class="btn btn-secondary" (click)="viewReport('recommendation')">View</button>
            <button class="btn btn-primary" (click)="downloadPdf('recommendation')">Download PDF</button>
          </div>
        </div>

        <!-- Portfolio Report -->
        <div class="report-card">
          <div class="report-icon">&#128188;</div>
          <h3>Portfolio Analysis Report</h3>
          <p>Complete portfolio health check with diversification, risk alignment, and insights</p>
          <div class="report-actions">
            <button class="btn btn-secondary" (click)="viewReport('portfolio')">View</button>
            <button class="btn btn-primary" (click)="downloadPdf('portfolio')">Download PDF</button>
          </div>
        </div>

        <!-- Stress Test Report -->
        <div class="report-card">
          <div class="report-icon">&#9888;&#65039;</div>
          <h3>Stress Test Report</h3>
          <p>Scenario analysis showing portfolio impact under different market conditions</p>
          <div class="report-actions">
            <button class="btn btn-secondary" (click)="runStressTest()">Run & View</button>
            <button class="btn btn-primary" (click)="downloadStressTestPdf()">Download PDF</button>
          </div>
        </div>
      </div>

      <!-- Report Viewer -->
      <div class="card" *ngIf="reportData" style="margin-top:24px">
        <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:16px">
          <h3 style="font-size:16px">{{ reportTitle }}</h3>
          <button class="btn btn-secondary" (click)="reportData = null">Close</button>
        </div>
        <pre class="report-content">{{ reportData | json }}</pre>
      </div>
    </div>
  `,
  styles: [`
    .reports-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
    .report-card { background: white; border-radius: 12px; padding: 24px; box-shadow: 0 2px 8px rgba(0,0,0,0.08); }
    .report-icon { font-size: 32px; margin-bottom: 12px; }
    .report-card h3 { font-size: 16px; font-weight: 600; margin-bottom: 8px; }
    .report-card p { font-size: 13px; color: #6b7280; margin-bottom: 16px; line-height: 1.5; }
    .report-actions { display: flex; gap: 8px; }
    .report-content { background: #f9fafb; padding: 16px; border-radius: 8px; font-size: 12px; max-height: 400px; overflow-y: auto; white-space: pre-wrap; word-wrap: break-word; }
  `]
})
export class ReportsComponent {
  reportData: any = null;
  reportTitle = '';
  private apiUrl = environment.apiUrl;

  constructor(private apiService: ApiService) {}

  viewReport(type: string) {
    this.reportTitle = type.replace('-', ' ').replace(/\b\w/g, l => l.toUpperCase()) + ' Report';

    switch (type) {
      case 'risk-assessment':
        this.apiService.getRiskAssessmentReport().subscribe({ next: (res) => this.reportData = res });
        break;
      case 'recommendation':
        this.apiService.getRecommendationReport().subscribe({ next: (res) => this.reportData = res });
        break;
      case 'portfolio':
        this.apiService.getPortfolioReport().subscribe({ next: (res) => this.reportData = res });
        break;
    }
  }

  downloadPdf(type: string) {
    const token = localStorage.getItem('token');
    const url = `${this.apiUrl}/reports/${type}/pdf`;
    // Open PDF in new tab — user can Ctrl+P to save as PDF
    window.open(url + '?access_token=' + token, '_blank');
  }

  runStressTest() {
    this.reportTitle = 'Stress Test Report';
    this.apiService.runStressTest().subscribe({ next: (res) => this.reportData = res });
  }

  downloadStressTestPdf() {
    const token = localStorage.getItem('token');
    const url = `${this.apiUrl}/reports/stress-test/pdf`;
    window.open(url + '?access_token=' + token, '_blank');
  }
}
