import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-onboarding',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './onboarding.component.html',
  styleUrls: ['./onboarding.component.css']
})
export class OnboardingComponent implements OnInit {
  step = 1;
  saving = false;
  directStep = false; // true if user came directly to a specific step

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

  constructor(private apiService: ApiService, private router: Router, private route: ActivatedRoute) {}

  ngOnInit() {
    // Check if user came directly to a specific step (from dashboard buttons)
    this.route.queryParams.subscribe(params => {
      const stepParam = params['step'];
      if (stepParam) {
        this.step = parseInt(stepParam, 10);
        this.directStep = true;
      }
    });

    // Load existing profile if available
    this.apiService.getProfile().subscribe({
      next: (profile: any) => {
        if (profile) {
          this.profile = profile;
          if (profile.goals) {
            this.selectedGoals = profile.goals.split(',').filter((g: string) => g.trim());
          }
        }
      }
    });
  }

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
        // If user came directly to a step, go back to dashboard
        if (this.directStep) {
          this.router.navigate(['/dashboard']);
        } else {
          this.router.navigate(['/risk-assessment']);
        }
      },
      error: () => {
        this.saving = false;
      }
    });
  }
}
