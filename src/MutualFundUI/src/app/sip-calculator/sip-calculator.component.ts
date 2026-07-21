import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-sip-calculator',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './sip-calculator.component.html',
  styleUrls: ['./sip-calculator.component.css']
})
export class SipCalculatorComponent {
  activeTab = 'sip';

  // SIP Calculator
  sip = { monthly: 10000, years: 10, returnRate: 12, futureValue: 0, totalInvested: 0, gained: 0, growthX: '', gainPct: '', investedPct: 0, gainedPct: 0 };

  // Goal Planner (Reverse SIP)
  goal = { target: 10000000, years: 20, returnRate: 12, monthlySIP: 0, totalInvested: 0, marketGain: 0, goalName: 'Retirement' };

  // EMI vs SIP
  emi = { monthly: 5000, years: 5, savingsRate: 3.5, mfRate: 12, savingsValue: 0, mfValue: 0, difference: 0, extraPercent: '' };

  constructor() {
    this.calcSIP();
    this.calcGoal();
    this.calcEMI();
  }

  calcSIP() {
    const P = this.sip.monthly;
    const n = this.sip.years * 12;
    const r = this.sip.returnRate / 100 / 12;
    this.sip.futureValue = P * (((Math.pow(1 + r, n) - 1) / r) * (1 + r));
    this.sip.totalInvested = P * n;
    this.sip.gained = this.sip.futureValue - this.sip.totalInvested;
    this.sip.growthX = (this.sip.futureValue / this.sip.totalInvested).toFixed(1);
    this.sip.gainPct = ((this.sip.gained / this.sip.totalInvested) * 100).toFixed(0);
    this.sip.investedPct = (this.sip.totalInvested / this.sip.futureValue) * 100;
    this.sip.gainedPct = (this.sip.gained / this.sip.futureValue) * 100;
  }

  calcGoal() {
    const FV = this.goal.target;
    const n = this.goal.years * 12;
    const r = this.goal.returnRate / 100 / 12;
    this.goal.monthlySIP = FV * r / (((Math.pow(1 + r, n) - 1)) * (1 + r));
    this.goal.totalInvested = this.goal.monthlySIP * n;
    this.goal.marketGain = FV - this.goal.totalInvested;
  }

  setGoal(name: string, target: number, years: number) {
    this.goal.goalName = name;
    this.goal.target = target;
    this.goal.years = years;
    this.calcGoal();
  }

  calcEMI() {
    const P = this.emi.monthly;
    const n = this.emi.years * 12;
    const rSav = this.emi.savingsRate / 100 / 12;
    const rMf = this.emi.mfRate / 100 / 12;
    this.emi.savingsValue = P * (((Math.pow(1 + rSav, n) - 1) / rSav) * (1 + rSav));
    this.emi.mfValue = P * (((Math.pow(1 + rMf, n) - 1) / rMf) * (1 + rMf));
    this.emi.difference = this.emi.mfValue - this.emi.savingsValue;
    this.emi.extraPercent = ((this.emi.difference / this.emi.savingsValue) * 100).toFixed(0);
  }
}
