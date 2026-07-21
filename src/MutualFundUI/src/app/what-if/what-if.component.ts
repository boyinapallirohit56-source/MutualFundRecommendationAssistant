import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-what-if',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './what-if.component.html',
  styleUrls: ['./what-if.component.css']
})
export class WhatIfComponent {
  monthly = 10000;
  yearsAgo = 5;
  returnRate = 12;
  futureValue = 0;
  totalInvested = 0;
  gained = 0;
  futureIfStartNow = 0;

  constructor() { this.calculate(); }

  calculate() {
    const P = this.monthly;
    const n = this.yearsAgo * 12;
    const r = this.returnRate / 100 / 12;
    this.futureValue = P * (((Math.pow(1 + r, n) - 1) / r) * (1 + r));
    this.totalInvested = P * n;
    this.gained = this.futureValue - this.totalInvested;
    this.futureIfStartNow = this.futureValue; // Same calculation for future
  }

  setPreset(monthly: number, years: number) {
    this.monthly = monthly;
    this.yearsAgo = years;
    this.calculate();
  }
}
