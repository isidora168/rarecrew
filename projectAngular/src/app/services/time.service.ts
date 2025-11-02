import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EmployeeTotal, TimeEntry } from '../models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TimeService {
  private http = inject(HttpClient);

  constructor() { }

  getTimes(code: string): Observable<EmployeeTotal[]>{
    const url = `${environment.apiBase}/gettimeentries?code=${encodeURIComponent(code)}`;

    return new Observable<EmployeeTotal[]>(subscriber => {
      const sub = this.http.get<TimeEntry[]>(url).subscribe({
        next: rowsRaw => {
          const rows = rowsRaw ?? [];
          const acc = new Map<string, number>();

          for (const r of rows) {
            if (r.DeletedOn) continue;
            if(r.EmployeeName == null) continue;
            
            const s = this.parseUtc(r.StarTimeUtc);
            const e = this.parseUtc(r.EndTimeUtc);
            if (isNaN(s.getTime()) || isNaN(e.getTime())) continue;

            const start = e < s ? e : s;
            const end   = e < s ? s : e;

            const hrs = (end.getTime() - start.getTime()) / 36e5;
            if (!(hrs >= 0) || !isFinite(hrs)) continue;

            acc.set(r.EmployeeName, (acc.get(r.EmployeeName) ?? 0) + hrs);
          }

          const result: EmployeeTotal[] = Array.from(acc.entries())
            .map(([employee, total]) => ({
              employee,
              totalHours: Math.round(total * 100) / 100
            }))
            .sort((a, b) => b.totalHours - a.totalHours || a.employee.localeCompare(b.employee));

          subscriber.next(result);
          subscriber.complete();
        },
        error: _ => {
          subscriber.next([]);
          subscriber.complete();
        }
      });

      return () => sub.unsubscribe();
    });
  }

    private parseUtc(iso: string) {
    return new Date(iso.endsWith('Z') ? iso : iso + 'Z');
  }
}
