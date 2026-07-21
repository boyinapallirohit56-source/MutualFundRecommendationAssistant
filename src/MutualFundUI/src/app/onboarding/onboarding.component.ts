import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-onboarding',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './onboarding.component.html',
  styleUrls: ['./onboarding.component.css']
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
