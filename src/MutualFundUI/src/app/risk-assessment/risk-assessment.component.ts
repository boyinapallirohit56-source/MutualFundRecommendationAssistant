import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-risk-assessment',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './risk-assessment.component.html',
  styleUrls: ['./risk-assessment.component.css']
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
