import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, Input, OnChanges, OnDestroy, SimpleChanges, ViewChild } from '@angular/core';
import { EmployeeTotal } from '../models';
import { Chart, ChartConfiguration } from 'chart.js';

@Component({
  selector: 'app-piechart',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './piechart.component.html',
  styleUrl: './piechart.component.css'
})
export class PiechartComponent implements AfterViewInit, OnChanges, OnDestroy{
  @Input() rows: EmployeeTotal[] = [];
  @ViewChild('canvas', { static: true }) canvasRef!: ElementRef<HTMLCanvasElement>;

  private chart?: Chart;

  ngAfterViewInit(): void {
    this.buildOrUpdateChart();
  }

  ngOnChanges(_: SimpleChanges): void {
    this.buildOrUpdateChart();
  }

  ngOnDestroy(): void {
    this.chart?.destroy();
  }

  private buildOrUpdateChart(): void {
    if (!this.canvasRef) return;

    const labels = this.rows.map(r => r.employee || 'N/A');
    const data = this.rows.map(r => r.totalHours || 0);
    const colors = this.generateColors(data.length);

    const config: ChartConfiguration<'pie'> = {
      type: 'pie',
      data: {
        labels,
        datasets: [{
          data,
          backgroundColor: colors,
          borderWidth: 1
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: true, position: 'bottom', align: 'center', 
                    labels: 
                    {
                      boxWidth: 10,         
                      boxHeight: 10,
                      padding: 10           
                    }
                  },
          tooltip: { enabled: true },
          datalabels: {
            color: '#ffffffff',
            anchor: 'center',
            align: 'center',
            clamp: true,
            formatter: (value: number, ctx: any) => {
              const arr = ctx.chart.data.datasets[0].data as number[];
              const total = arr.reduce((a, b) => a + (Number(b) || 0), 0) || 1;
              const pct = (Number(value) / total) * 100;
              return pct >= 3 ? `${pct.toFixed(0)}%` : ''; 
            }
          }
        }
      }
    };

    if (this.chart) {
      this.chart.data.labels = labels;
      this.chart.data.datasets[0].data = data as any;
      (this.chart.data.datasets[0] as any).backgroundColor = colors;
      this.chart.update();
    } else {
      this.chart = new Chart(this.canvasRef.nativeElement.getContext('2d')!, config);
    }
}
 generateColors(n: number): string[] {
    return Array.from({ length: n }, (_, i) => `hsl(${Math.floor((360 / Math.max(1, n)) * i)} 70% 60%)`);
  }
}
