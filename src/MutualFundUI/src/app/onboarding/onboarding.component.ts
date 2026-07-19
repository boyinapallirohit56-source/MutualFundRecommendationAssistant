import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-onboarding',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container" style="max-width:700px; margin-top:40px">
      <div class="page-header">
        <h1>Complete Your Profile</h1>
        <p>Help us understand your financial situation</p>
      </div>

      <div class="progress-bar-container">
        <div class="progress-bar" [style.width.%]="(step / 4) * 100"></div>
      </div>

      <!-- Step 1: Personal -->
      <div class="card" *ngIf="step === 1">
        <h3>Personal Details</h3>
        <div class="grid-2 mt-4">
          <div class="form-group">
            <label>Age</label>
            <input type="number" [(ngModel)]="profile.age" placeholder="Your age">
          </div>
          <div class="form-group">
            <label>Occupation</label>
            <input type="text" [(ngModel)]="profile.occupation" placeholder="Your occupation">
          </div>
          <div class="form-group">
            <label>Location</label>
            <input type="text" [(ngModel)]="profile.location" placeholder="City">
          </div>
          <div class="form-group">
            <label>Marital Status</label>
            <select [(ngModel)]="profile.maritalStatus">
              <option value="">Select</option>
              <option value="Single">Single</option>
              <option value="Married">Married</option>
            </select>
          </div>
          <div class="form-group">
            <label>Dependents</label>
            <input type="number" [(ngModel)]="profile.dependents" placeholder="0">
          </div>
        </div>
        <button class="btn btn-primary mt-4" (click)="step = 2">Next</button>
      </div>

      <!-- Step 2: Financial -->
      <div class="card" *ngIf="step === 2">
        <h3>Financial Details</h3>
        <div class="grid-2 mt-4">
          <div class="form-group">
            <label>Monthly Income (Rs)</label>
            <input type="number" [(ngModel)]="profile.monthlyIncome" placeholder="50000">
          </div>
          <div class="form-group">
            <label>Monthly Expenses (Rs)</label>
            <input type="number" [(ngModel)]="profile.monthlyExpenses" placeholder="30000">
          </div>
          <div class="form-group">
            <label>Savings (Rs)</label>
            <input type="number" [(ngModel)]="profile.savings" placeholder="200000">
          </div>
          <div class="form-group">
            <label>Loans/EMIs (Rs)</label>
            <input type="number" [(ngModel)]="profile.loans" placeholder="0">
          </div>
        </div>
        <div style="display:flex; gap:12px" class="mt-4">
          <button class="btn btn-secondary" (click)="step = 1">Back</button>
          <button class="btn btn-primary" (click)="step = 3">Next</button>
        </div>
      </div>

      <!-- Step 3: Investment -->
      <div class="card" *ngIf="step === 3">
        <h3>Investment Details</h3>
        <div class="grid-2 mt-4">
          <div class="form-group">
            <label>Existing Investments</label>
            <select [(ngModel)]="profile.existingInvestments">
              <option value="">Select</option>
              <option value="None">None</option>
              <option value="FD/RD">FD/RD only</option>
              <option value="Mutual Funds">Mutual Funds</option>
              <option value="Stocks">Direct Stocks</option>
              <option value="Multiple">Multiple types</option>
            </select>
          </div>
          <div class="form-group">
            <label>Monthly SIP Amount (Rs)</label>
            <input type="number" [(ngModel)]="profile.sipAmount" placeholder="5000">
          </div>
          <div class="form-group">
            <label>Investment Duration (Years)</label>
            <input type="number" [(ngModel)]="profile.durationInYears" placeholder="5">
          </div>
        </div>
        <div style="display:flex; gap:12px" class="mt-4">
          <button class="btn btn-secondary" (click)="step = 2">Back</button>
          <button class="btn btn-primary" (click)="step = 4">Next</button>
        </div>
      </div>

      <!-- Step 4: Goals -->
      <div class="card" *ngIf="step === 4">
        <h3>Financial Goals</h3>
        <p style="color:#6b7280; margin-top:8px">Select all that apply</p>
        <div class="goals-grid mt-4">
          <label class="goal-checkbox" *ngFor="let goal of goalOptions">
            <input type="checkbox" [checked]="isGoalSelected(goal)" (change)="toggleGoal(goal)">
            {{ goal }}
          </label>
        </div>
        <div style="display:flex; gap:12px" class="mt-4">
          <button class="btn btn-secondary" (click)="step = 3">Back</button>
          <button class="btn btn-primary" (click)="saveProfile()" [disabled]="saving">
            {{ saving ? 'Saving...' : 'Save & Continue' }}
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    h3 { font-size: 18px; font-weight: 600; }
    .goals-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }
    .goal-checkbox { display: flex; align-items: center; gap: 8px; padding: 10px; border: 1px solid #e5e7eb; border-radius: 8px; cursor: pointer; font-size: 14px; }
    .goal-checkbox:hover { border-color: #2563eb; }
    .goal-checkbox input { width: 16px; height: 16px; }
  `]
})
export class OnboardingComponent {
  step = 1;
  saving = false;

  profile = {
    age: 0,
    occupation: '',
    location: '',
    maritalStatus: '',
    dependents: 0,
    monthlyIncome: 0,
    monthlyExpenses: 0,
    savings: 0,
    loans: 0,
    existingInvestments: '',
    sipAmount: 0,
    durationInYears: 0,
    goals: ''
  };

  goalOptions = ['Wealth Creation', 'Retirement', 'Tax Saving', 'Child Education', 'Home Purchase', 'Emergency Fund'];
  selectedGoals: string[] = [];

  constructor(private apiService: ApiService, private router: Router) {}

  isGoalSelected(goal: string): boolean {
    return this.selectedGoals.includes(goal);
  }

  toggleGoal(goal: string) {
    const index = this.selectedGoals.indexOf(goal);
    if (index > -1) {
      this.selectedGoals.splice(index, 1);
    } else {
      this.selectedGoals.push(goal);
    }
  }

  saveProfile() {
    this.saving = true;
    this.profile.goals = this.selectedGoals.join(',');
    this.apiService.saveProfile(this.profile).subscribe({
      next: () => {
        this.saving = false;
        this.router.navigate(['/risk-assessment']);
      },
      error: () => {
        this.saving = false;
      }
    });
  }
}
