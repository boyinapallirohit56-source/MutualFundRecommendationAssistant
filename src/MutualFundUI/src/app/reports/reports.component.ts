import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.css']
})
export class ReportsComponent {
  reportHtml: SafeHtml | null = null;
  reportTitle = '';
  loading = false;
  downloading = false;
  error = '';
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient, private sanitizer: DomSanitizer) {}

  viewReport(type: string) {
    this.error = '';
    this.loading = true;
    this.reportTitle = this.getTitle(type);

    const url = type === 'stress-test'
      ? `${this.apiUrl}/reports/stress-test/pdf`
      : `${this.apiUrl}/reports/${type}/pdf`;

    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });

    if (type === 'stress-test') {
      this.http.post(url, null, { headers, responseType: 'text' }).subscribe({
        next: (html) => {
          this.reportHtml = this.sanitizer.bypassSecurityTrustHtml(this.extractBody(html));
          this.loading = false;
        },
        error: (err) => {
          this.error = err.error?.message || 'Failed to generate report. Complete your assessment first.';
          this.loading = false;
        }
      });
    } else {
      this.http.get(url, { headers, responseType: 'text' }).subscribe({
        next: (html) => {
          this.reportHtml = this.sanitizer.bypassSecurityTrustHtml(this.extractBody(html));
          this.loading = false;
        },
        error: (err) => {
          this.error = err.error?.message || 'Failed to generate report. Complete your assessment first.';
          this.loading = false;
        }
      });
    }
  }

  downloadPdf(type: string) {
    this.error = '';
    this.downloading = true;

    const url = type === 'stress-test'
      ? `${this.apiUrl}/reports/stress-test/pdf`
      : `${this.apiUrl}/reports/${type}/pdf`;

    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });

    const request$ = type === 'stress-test'
      ? this.http.post(url, null, { headers, responseType: 'text' })
      : this.http.get(url, { headers, responseType: 'text' });

    request$.subscribe({
      next: (html) => {
        // Open HTML in new window for printing as PDF
        const printWindow = window.open('', '_blank');
        if (printWindow) {
          printWindow.document.write(html);
          printWindow.document.close();
          setTimeout(() => printWindow.print(), 500);
        }
        this.downloading = false;
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to download report.';
        this.downloading = false;
      }
    });
  }

  closeReport() {
    this.reportHtml = null;
  }

  private getTitle(type: string): string {
    switch (type) {
      case 'risk-assessment': return 'Risk Assessment Report';
      case 'recommendation': return 'Recommendation Report';
      case 'portfolio': return 'Portfolio Analysis Report';
      case 'stress-test': return 'Stress Test Report';
      default: return 'Report';
    }
  }

  private extractBody(html: string): string {
    // Extract content between <body> and </body> for modal display
    const bodyMatch = html.match(/<body[^>]*>([\s\S]*)<\/body>/i);
    return bodyMatch ? bodyMatch[1] : html;
  }
}
