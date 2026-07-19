import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container" style="margin-top:24px; max-width:800px">
      <div class="page-header">
        <h1>AI Investment Assistant</h1>
        <p>Ask me anything about mutual funds and investing</p>
      </div>

      <div class="chat-container">
        <!-- Messages -->
        <div class="chat-messages">
          <div *ngIf="!messages.length" class="welcome-msg">
            <h3>Hi! I'm your investment assistant.</h3>
            <p>You can ask me about:</p>
            <div class="suggestion-chips">
              <button class="chip" (click)="askSuggestion('What is SIP and how does it work?')">What is SIP?</button>
              <button class="chip" (click)="askSuggestion('Explain expense ratio')">Expense Ratio</button>
              <button class="chip" (click)="askSuggestion('What are the different risk profiles?')">Risk Profiles</button>
              <button class="chip" (click)="askSuggestion('What is NAV?')">What is NAV?</button>
              <button class="chip" (click)="askSuggestion('Explain equity funds')">Equity Funds</button>
              <button class="chip" (click)="askSuggestion('What is ELSS and how does it save tax?')">Tax Saving (ELSS)</button>
            </div>
          </div>

          <div *ngFor="let msg of messages" class="message" [class.user-msg]="msg.role === 'user'" [class.assistant-msg]="msg.role === 'assistant'">
            <div class="msg-role">{{ msg.role === 'user' ? 'You' : 'AI Assistant' }}</div>
            <div class="msg-content">{{ msg.content }}</div>
          </div>

          <div *ngIf="loading" class="message assistant-msg">
            <div class="msg-role">AI Assistant</div>
            <div class="msg-content typing">Thinking...</div>
          </div>
        </div>

        <!-- Input -->
        <div class="chat-input">
          <input type="text" [(ngModel)]="userMessage" placeholder="Ask about mutual funds, SIP, NAV, risk..."
                 (keyup.enter)="sendMessage()" [disabled]="loading">
          <button class="btn btn-primary" (click)="sendMessage()" [disabled]="loading || !userMessage.trim()">Send</button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .chat-container { background: white; border-radius: 12px; box-shadow: 0 2px 8px rgba(0,0,0,0.08); overflow: hidden; display: flex; flex-direction: column; height: 600px; }
    .chat-messages { flex: 1; overflow-y: auto; padding: 24px; }
    .welcome-msg { text-align: center; padding: 40px 20px; }
    .welcome-msg h3 { font-size: 18px; margin-bottom: 8px; }
    .welcome-msg p { color: #6b7280; margin-bottom: 16px; }
    .suggestion-chips { display: flex; flex-wrap: wrap; gap: 8px; justify-content: center; }
    .chip { padding: 8px 14px; border: 1px solid #e5e7eb; border-radius: 20px; background: white; font-size: 13px; cursor: pointer; transition: all 0.2s; }
    .chip:hover { border-color: #2563eb; background: #eff6ff; color: #2563eb; }
    .message { margin-bottom: 16px; max-width: 80%; }
    .user-msg { margin-left: auto; }
    .assistant-msg { margin-right: auto; }
    .msg-role { font-size: 11px; color: #6b7280; margin-bottom: 4px; }
    .user-msg .msg-role { text-align: right; }
    .msg-content { padding: 12px 16px; border-radius: 12px; font-size: 14px; line-height: 1.6; white-space: pre-wrap; }
    .user-msg .msg-content { background: #2563eb; color: white; border-bottom-right-radius: 4px; }
    .assistant-msg .msg-content { background: #f3f4f6; color: #333; border-bottom-left-radius: 4px; }
    .typing { color: #6b7280; font-style: italic; }
    .chat-input { display: flex; gap: 8px; padding: 16px; border-top: 1px solid #e5e7eb; }
    .chat-input input { flex: 1; padding: 12px 16px; border: 1px solid #d1d5db; border-radius: 8px; font-size: 14px; }
    .chat-input input:focus { outline: none; border-color: #2563eb; }
  `]
})
export class ChatComponent implements OnInit {
  messages: any[] = [];
  userMessage = '';
  loading = false;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.apiService.getChatHistory().subscribe({
      next: (history) => { this.messages = history; }
    });
  }

  sendMessage() {
    if (!this.userMessage.trim() || this.loading) return;

    const msg = this.userMessage.trim();
    this.messages.push({ role: 'user', content: msg });
    this.userMessage = '';
    this.loading = true;

    this.apiService.sendChatMessage(msg).subscribe({
      next: (res) => {
        this.messages.push({ role: 'assistant', content: res.reply });
        this.loading = false;
      },
      error: () => {
        this.messages.push({ role: 'assistant', content: 'Sorry, something went wrong. Please try again.' });
        this.loading = false;
      }
    });
  }

  askSuggestion(question: string) {
    this.userMessage = question;
    this.sendMessage();
  }
}
