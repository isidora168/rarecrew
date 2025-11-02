import { Component, Input } from '@angular/core';
import { EmployeeTotal } from '../models';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-table',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './table.component.html',
  styleUrl: './table.component.css'
})
export class TableComponent {
  @Input() rows: EmployeeTotal[] = [];

  formatHours(v: number) { return v.toFixed(); } 
  trackByName = (_: number, r: EmployeeTotal) => r.employee;
}
