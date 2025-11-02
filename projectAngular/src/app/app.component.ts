import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { EmployeeTotal } from './models';
import { TimeService } from './services/time.service';
import { CommonModule } from '@angular/common';
import { TableComponent } from './table/table.component';
import { PiechartComponent } from './piechart/piechart.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, TableComponent, PiechartComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  apiKey: string | null = null;
  rows: EmployeeTotal[] = [];

  constructor(private api: TimeService) {
    this.apiKey = this.getKeyFromUrl();
    if (this.apiKey) {
      this.api.getTimes(this.apiKey).subscribe({
        next: data => this.rows = data,
        error: () => this.rows = []
      });
    }
}
  private getKeyFromUrl(): string | null {
    const url = new URL(window.location.href);
    return url.searchParams.get('code') || new URLSearchParams(url.hash.replace('#','')).get('code');
  }
}
