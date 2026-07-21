import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../shared/services/api.service';

@Component({
  selector: 'app-fund-factsheet',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './fund-factsheet.component.html',
  styleUrls: ['./fund-factsheet.component.css']
})
export class FundFactsheetComponent implements OnInit {
  fund: any = null;

  constructor(
    private apiService: ApiService,
    private route: ActivatedRoute,
    public router: Router
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      const id = +params['id'];
      this.apiService.getFundFactsheet(id).subscribe({
        next: (res) => { this.fund = res; }
      });
    });
  }
}
