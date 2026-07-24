import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  activeTab = 'analytics';
  analytics: any = null;
  users: any[] = [];
  funds: any[] = [];
  riskDistItems: any[] = [];
  goalDistItems: any[] = [];

  // AMFI Sync
  syncing = false;
  syncResult: any = null;

  // Allocation Rules
  allocationRules: any[] = [];
  riskProfiles = ['Conservative', 'Moderate', 'Aggressive', 'Very Aggressive'];
  selectedProfile = 'Conservative';
  currentAllocations: any[] = [];
  savingRules = false;
  rulesSaved = false;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadAnalytics();
  }

  loadAnalytics() {
    this.apiService.getAdminAnalytics().subscribe({
      next: (data) => {
        this.analytics = data;
        this.riskDistItems = Object.entries(data.riskProfileDistribution || {}).map(([key, value]) => ({ key, value }));
        this.goalDistItems = Object.entries(data.goalDistribution || {}).map(([key, value]) => ({ key, value }));
      }
    });
  }

  loadUsers() {
    this.apiService.getAdminUsers().subscribe({
      next: (users) => { this.users = users; }
    });
  }

  loadFunds() {
    this.apiService.listFunds().subscribe({
      next: (funds) => { this.funds = funds; }
    });
  }

  toggleUserStatus(user: any) {
    this.apiService.updateUserStatus(user.id, !user.isActive).subscribe({
      next: () => { user.isActive = !user.isActive; }
    });
  }

  // --- AMFI Sync ---
  syncAmfiData() {
    this.syncing = true;
    this.syncResult = null;
    this.apiService.syncAmfiData().subscribe({
      next: (res) => {
        this.syncing = false;
        this.syncResult = { success: true, message: res.message };
      },
      error: (err) => {
        this.syncing = false;
        this.syncResult = { success: false, message: err.error?.message || 'Sync failed. Check internet connection.' };
      }
    });
  }

  // --- Allocation Rules ---
  loadAllocationRules() {
    this.apiService.getAllocationRules().subscribe({
      next: (rules) => {
        this.allocationRules = rules;
        this.filterByProfile(this.selectedProfile);
      }
    });
  }

  filterByProfile(profile: string) {
    this.selectedProfile = profile;
    this.rulesSaved = false;
    this.currentAllocations = this.allocationRules
      .filter(r => r.riskProfile === profile)
      .map(r => ({ ...r }));
  }

  getTotalPercentage(): number {
    return this.currentAllocations.reduce((sum, r) => sum + (r.percentage || 0), 0);
  }

  saveAllocations() {
    if (this.getTotalPercentage() !== 100) return;
    this.savingRules = true;
    this.rulesSaved = false;
    this.apiService.updateAllocationRules(this.selectedProfile, this.currentAllocations).subscribe({
      next: () => {
        this.savingRules = false;
        this.rulesSaved = true;
        // Update local data
        this.allocationRules = this.allocationRules.map(r => {
          if (r.riskProfile === this.selectedProfile) {
            const updated = this.currentAllocations.find(c => c.assetClass === r.assetClass);
            return updated || r;
          }
          return r;
        });
      },
      error: () => { this.savingRules = false; }
    });
  }

  getAssetColor(assetClass: string): string {
    const colors: any = {
      'Equity': '#6366f1',
      'Debt': '#10b981',
      'Hybrid': '#f59e0b',
      'Gold': '#eab308',
      'Liquid': '#06b6d4',
      'International': '#ec4899'
    };
    return colors[assetClass] || '#94a3b8';
  }
}
