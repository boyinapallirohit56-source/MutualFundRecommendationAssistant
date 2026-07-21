import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../shared/services/api.service';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.css']
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
