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
