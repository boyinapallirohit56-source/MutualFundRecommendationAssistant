import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.css']
})
export class LandingComponent {
  steps = [
    { title: 'Complete Profile', desc: 'Share your financial situation, goals, and investment timeline' },
    { title: 'Risk Assessment', desc: 'Answer 15 questions to determine your investment risk tolerance' },
    { title: 'AI Analysis', desc: 'Our engine calculates your optimal allocation across asset classes' },
    { title: 'Get Recommendations', desc: 'Receive personalized fund suggestions with AI explanations' }
  ];

  features = [
    { icon: '&#128202;', title: 'Risk Profiling', desc: 'Scientifically designed questionnaire to accurately assess your investment risk appetite' },
    { icon: '&#129302;', title: 'AI Advisor', desc: 'Intelligent chatbot that explains investment concepts and recommendations in plain language' },
    { icon: '&#128200;', title: 'Portfolio Analysis', desc: 'Upload your existing portfolio for diversification scoring and rebalancing insights' },
    { icon: '&#9888;&#65039;', title: 'Stress Testing', desc: 'Simulate market crashes to understand how your portfolio would react under pressure' },
    { icon: '&#128176;', title: 'Tax Optimization', desc: 'Section 80C calculator and ELSS fund recommendations for maximum tax savings' },
    { icon: '&#128202;', title: 'Goal Planning', desc: 'Reverse SIP calculator to determine exactly how much you need to invest monthly' }
  ];
}
