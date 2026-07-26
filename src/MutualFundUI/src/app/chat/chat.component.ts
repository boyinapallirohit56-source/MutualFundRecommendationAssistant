import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
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
    this.messages.push({ role: 'user', content: msg, timestamp: new Date() });
    this.userMessage = '';
    this.loading = true;

    this.apiService.sendChatMessage(msg).subscribe({
      next: (res) => {
        this.messages.push({ role: 'assistant', content: res.reply, timestamp: new Date() });
        this.loading = false;
      },
      error: () => {
        this.messages.push({ role: 'assistant', content: "I couldn't generate a response. Please try asking another mutual fund or investment-related question.", timestamp: new Date() });
        this.loading = false;
      }
    });
  }

  askSuggestion(question: string) {
    this.userMessage = question;
    this.sendMessage();
  }

  getTimestamp(): string {
    const now = new Date();
    return `Today • ${now.toLocaleTimeString('en-IN', { hour: 'numeric', minute: '2-digit', hour12: true })}`;
  }
}
