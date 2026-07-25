import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-portfolio',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './portfolio.component.html',
  styleUrls: ['./portfolio.component.css']
})
export class PortfolioComponent implements OnInit {
  portfolio: any = null;
  analysis: any = null;
  loading = true;
  adding = false;
  inputMode = 'manual';
  funds: any[] = [];

  // File upload
  isDragging = false;
  selectedFile: File | null = null;
  uploading = false;
  uploadResult: { success: boolean; message: string } | null = null;

  newHolding: any = {
    mutualFundId: null,
    fundName: '',
    units: 0,
    purchaseNAV: 0,
    investedAmount: 0,
    purchaseDate: ''
  };

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadPortfolio();
    this.apiService.listFunds().subscribe({ next: (f) => this.funds = f });
  }

  loadPortfolio() {
    this.apiService.getPortfolio().subscribe({
      next: (res) => { this.portfolio = res; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  addHolding() {
    this.adding = true;
    this.apiService.addHolding(this.newHolding).subscribe({
      next: () => {
        this.adding = false;
        this.newHolding = { mutualFundId: null, fundName: '', units: 0, purchaseNAV: 0, investedAmount: 0, purchaseDate: '' };
        this.loadPortfolio();
      },
      error: () => { this.adding = false; }
    });
  }

  removeHolding(id: number) {
    this.apiService.removeHolding(id).subscribe({ next: () => this.loadPortfolio() });
  }

  onFundSelect() {
    const fund = this.funds.find(f => f.id === this.newHolding.mutualFundId);
    if (fund) {
      this.newHolding.fundName = fund.name;
      this.newHolding.purchaseNAV = fund.nav || 0;
    }
  }

  analyzePortfolio() {
    this.apiService.analyzePortfolio().subscribe({
      next: (res) => { this.analysis = res; }
    });
  }

  // File Upload Methods
  onDragOver(event: DragEvent) {
    event.preventDefault();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    this.isDragging = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    this.isDragging = false;
    const files = event.dataTransfer?.files;
    if (files && files.length > 0) {
      this.selectedFile = files[0];
    }
  }

  onFileSelected(event: any) {
    const files = event.target.files;
    if (files && files.length > 0) {
      this.selectedFile = files[0];
    }
  }

  uploadFile() {
    if (!this.selectedFile) return;
    this.uploading = true;
    this.uploadResult = null;

    const formData = new FormData();
    formData.append('file', this.selectedFile);

    const endpoint = this.selectedFile.name.endsWith('.csv') ? 'upload/csv' : 'upload/excel';

    this.apiService.uploadPortfolioFile(endpoint, formData).subscribe({
      next: (res: any) => {
        this.uploading = false;
        this.uploadResult = { success: true, message: res.message || 'Holdings imported successfully!' };
        this.selectedFile = null;
        this.loadPortfolio();
      },
      error: (err: any) => {
        this.uploading = false;
        this.uploadResult = { success: false, message: err.error?.message || 'Upload failed. Check file format.' };
      }
    });
  }
}
