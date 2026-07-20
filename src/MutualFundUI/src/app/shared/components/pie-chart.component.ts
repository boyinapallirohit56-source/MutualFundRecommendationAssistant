import { Component, Input, OnChanges, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface ChartData {
  label: string;
  value: number;
  color: string;
}

@Component({
  selector: 'app-pie-chart',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="chart-wrapper">
      <canvas #chartCanvas [width]="size" [height]="size"></canvas>
      <div class="chart-legend" *ngIf="showLegend">
        <div class="legend-item" *ngFor="let item of data">
          <span class="legend-color" [style.background]="item.color"></span>
          <span class="legend-label">{{ item.label }}</span>
          <span class="legend-value">{{ item.value }}%</span>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .chart-wrapper { display: flex; align-items: center; gap: 24px; flex-wrap: wrap; justify-content: center; }
    canvas { max-width: 100%; }
    .chart-legend { display: flex; flex-direction: column; gap: 8px; }
    .legend-item { display: flex; align-items: center; gap: 8px; font-size: 13px; }
    .legend-color { width: 12px; height: 12px; border-radius: 3px; flex-shrink: 0; }
    .legend-label { color: #374151; min-width: 80px; }
    .legend-value { font-weight: 600; color: #111827; }
  `]
})
export class PieChartComponent implements OnChanges, AfterViewInit {
  @Input() data: ChartData[] = [];
  @Input() size: number = 200;
  @Input() showLegend: boolean = true;
  @ViewChild('chartCanvas') canvasRef!: ElementRef<HTMLCanvasElement>;

  private initialized = false;

  ngAfterViewInit() {
    this.initialized = true;
    this.drawChart();
  }

  ngOnChanges() {
    if (this.initialized) {
      this.drawChart();
    }
  }

  private drawChart() {
    if (!this.canvasRef) return;
    const canvas = this.canvasRef.nativeElement;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    const centerX = this.size / 2;
    const centerY = this.size / 2;
    const radius = (this.size / 2) - 10;

    // Clear canvas
    ctx.clearRect(0, 0, this.size, this.size);

    // Filter out zero values
    const filteredData = this.data.filter(d => d.value > 0);
    const total = filteredData.reduce((sum, d) => sum + d.value, 0);

    if (total === 0) return;

    let startAngle = -Math.PI / 2; // Start from top

    for (const item of filteredData) {
      const sliceAngle = (item.value / total) * 2 * Math.PI;

      ctx.beginPath();
      ctx.moveTo(centerX, centerY);
      ctx.arc(centerX, centerY, radius, startAngle, startAngle + sliceAngle);
      ctx.closePath();
      ctx.fillStyle = item.color;
      ctx.fill();

      // White border between slices
      ctx.strokeStyle = '#ffffff';
      ctx.lineWidth = 2;
      ctx.stroke();

      // Draw percentage label if slice is big enough
      if (item.value >= 10) {
        const labelAngle = startAngle + sliceAngle / 2;
        const labelX = centerX + (radius * 0.65) * Math.cos(labelAngle);
        const labelY = centerY + (radius * 0.65) * Math.sin(labelAngle);

        ctx.fillStyle = '#ffffff';
        ctx.font = 'bold 12px Inter, sans-serif';
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        ctx.fillText(`${item.value}%`, labelX, labelY);
      }

      startAngle += sliceAngle;
    }

    // Draw center hole (donut effect)
    ctx.beginPath();
    ctx.arc(centerX, centerY, radius * 0.4, 0, 2 * Math.PI);
    ctx.fillStyle = '#ffffff';
    ctx.fill();
  }
}
