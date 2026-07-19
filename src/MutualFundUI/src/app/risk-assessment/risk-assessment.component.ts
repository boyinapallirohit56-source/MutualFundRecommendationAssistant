import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-risk-assessment',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container" style="max-width:700px; margin-top:40px">
      <div class="page-header">
        <h1>Risk Assessment</h1>
        <p>Answer these questions to determine your risk profile</p>
      </div>

      <!-- Loading -->
      <div *ngIf="loading" class="card text-center">
        <p>Loading questions...</p>
      </div>

      <!-- Questions -->
      <div *ngIf="!loading && !result">
        <div class="progress-bar-container">
          <div class="progress-bar" [style.width.%]="((currentIndex + 1) / questions.length) * 100"></div>
        </div>
        <p style="text-align:right; font-size:13px; color:#6b7280; margin-bottom:12px">
          Question {{ currentIndex + 1 }} of {{ questions.length }}
        </p>

        <div class="card" *ngIf="questions[currentIndex]">
          <h3 style="font-size:17px; margin-bottom:20px">{{ questions[currentIndex].questionText }}</h3>
          <div class="options">
            <label class="option" *ngFor="let option of questions[currentIndex].options"
                   [class.selected]="answers[currentIndex]?.selectedOptionId === option.id"
                   (click)="selectOption(option.id)">
              <span class="radio"></span>
              {{ option.optionText }}
            </label>
          </div>
          <div style="display:flex; justify-content:space-between; margin-top:24px">
            <button class="btn btn-secondary" (click)="prev()" [disabled]="currentIndex === 0">Back</button>
            <button class="btn btn-primary" (click)="next()" *ngIf="currentIndex < questions.length - 1" [disabled]="!answers[currentIndex]">Next</button>
            <button class="btn btn-primary" (click)="submit()" *ngIf="currentIndex === questions.length - 1" [disabled]="!answers[currentIndex] || submitting">
              {{ submitting ? 'Submitting...' : 'Submit' }}
            </button>
          </div>
        </div>
      </div>

      <!-- Result -->
      <div *ngIf="result" class="card text-center">
        <h2 style="color:#2563eb; font-size:48px; margin-bottom:8px">{{ result.normalizedScore }}</h2>
        <p style="font-size:13px; color:#6b7280">Risk Score (out of 100)</p>
        <h3 style="margin-top:16px; font-size:22px">{{ result.riskProfile }}</h3>
        <p style="color:#6b7280; margin-top:8px; max-width:500px; margin-left:auto; margin-right:auto">{{ result.description }}</p>
        <button class="btn btn-primary mt-4" (click)="goToDashboard()">View Recommendations</button>
      </div>
    </div>
  `,
  styles: [`
    .options { display: flex; flex-direction: column; gap: 10px; }
    .option { display: flex; align-items: center; gap: 12px; padding: 14px 16px; border: 1px solid #e5e7eb; border-radius: 8px; cursor: pointer; font-size: 14px; transition: all 0.2s; }
    .option:hover { border-color: #2563eb; background: #f8fafc; }
    .option.selected { border-color: #2563eb; background: #eff6ff; }
    .radio { width: 18px; height: 18px; border: 2px solid #d1d5db; border-radius: 50%; flex-shrink: 0; }
    .option.selected .radio { border-color: #2563eb; background: #2563eb; box-shadow: inset 0 0 0 3px white; }
  `]
})
export class RiskAssessmentComponent implements OnInit {
  questions: any[] = [];
  answers: any[] = [];
  currentIndex = 0;
  loading = true;
  submitting = false;
  result: any = null;

  constructor(private apiService: ApiService, private router: Router) {}

  ngOnInit() {
    this.apiService.getQuestions().subscribe({
      next: (questions) => {
        this.questions = questions;
        this.answers = new Array(questions.length).fill(null);
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  selectOption(optionId: number) {
    this.answers[this.currentIndex] = {
      questionId: this.questions[this.currentIndex].id,
      selectedOptionId: optionId
    };
  }

  next() {
    if (this.currentIndex < this.questions.length - 1) {
      this.currentIndex++;
    }
  }

  prev() {
    if (this.currentIndex > 0) {
      this.currentIndex--;
    }
  }

  submit() {
    this.submitting = true;
    const validAnswers = this.answers.filter(a => a !== null);
    this.apiService.submitAssessment(validAnswers).subscribe({
      next: (res) => {
        this.result = res;
        this.submitting = false;
      },
      error: () => { this.submitting = false; }
    });
  }

  goToDashboard() {
    this.apiService.generateRecommendation().subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: () => this.router.navigate(['/dashboard'])
    });
  }
}
