import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ApiService } from '../shared/services/api.service';
import { AuthService } from '../shared/services/auth.service';

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
  directStep = false;
  userName = '';

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
    investmentType: 'SIP',
    sipAmount: 0,
    sipFrequency: 'Monthly',
    sipDate: 5,
    lumpSumAmount: 0,
    hasSWP: false,
    swpAmount: 0,
    durationInYears: 0,
    goals: ''
  };

  goalOptions = ['Wealth Creation', 'Retirement', 'Tax Saving', 'Child Education', 'Home Purchase', 'Emergency Fund'];
  selectedGoals: string[] = [];
  goalTargets: { [key: string]: number } = {};
  goalYears: { [key: string]: number } = {};

  constructor(private apiService: ApiService, private router: Router, private route: ActivatedRoute, private authService: AuthService) {}

  ngOnInit() {
    const user = this.authService.getUser();
    this.userName = user?.name || '';
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
    // Validation: expenses cannot exceed income
    if (this.profile.monthlyExpenses > this.profile.monthlyIncome && this.profile.monthlyIncome > 0) {
      alert('Monthly Expenses cannot be greater than Monthly Income. Please correct your entries.');
      this.saving = false;
      return;
    }
    // Validation: loans + expenses should not exceed income
    if ((this.profile.monthlyExpenses + this.profile.loans) > this.profile.monthlyIncome && this.profile.monthlyIncome > 0) {
      alert('Monthly Expenses + Loans/EMIs exceed your Monthly Income. Please review your entries.');
      this.saving = false;
      return;
    }

    this.saving = true;
    this.profile.goals = this.selectedGoals.join(',');

    // Send profile with both camelCase and PascalCase property names
    // to ensure the backend receives the values regardless of JSON serializer config
    const profilePayload: any = {
      ...this.profile,
      // Explicit duplicate keys for SIP/SWP fields (covers all serializer behaviors)
      SIPAmount: this.profile.sipAmount,
      SIPFrequency: this.profile.sipFrequency,
      SIPDate: this.profile.sipDate,
      HasSWP: this.profile.hasSWP,
      SWPAmount: this.profile.swpAmount
    };

    this.apiService.saveProfile(profilePayload).subscribe({
      next: () => {
        // Save goals with target amounts to backend
        const goalsToSave = this.selectedGoals
          .filter(g => this.goalTargets[g] && this.goalTargets[g] > 0)
          .map(g => ({
            name: g,
            targetAmount: this.goalTargets[g] || 0,
            targetYears: this.goalYears[g] || 5,
            monthlySIP: this.profile.sipAmount || 0
          }));

        if (goalsToSave.length > 0) {
          this.apiService.createGoalsBatch(goalsToSave).subscribe({
            next: () => this.navigateAfterSave(),
            error: () => this.navigateAfterSave()
          });
        } else {
          this.navigateAfterSave();
        }
      },
      error: () => {
        this.saving = false;
      }
    });
  }

  private navigateAfterSave() {
    this.saving = false;
    if (this.directStep) {
      this.router.navigate(['/dashboard']);
    } else {
      this.router.navigate(['/risk-assessment']);
    }
  }
}
