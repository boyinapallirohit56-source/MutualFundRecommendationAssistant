import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-stress-test',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './stress-test.component.html',
  styleUrls: ['./stress-test.component.css']
})
export class StressTestComponent {
  result: any = null;
  loading = false;

  constructor(private apiService: ApiService) {}

  runTest() {
    this.loading = true;
    this.apiService.runStressTest().subscribe({
      next: (res) => { this.result = res; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }
}
