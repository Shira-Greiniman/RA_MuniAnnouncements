import { ChangeDetectionStrategy, Component } from '@angular/core';
import { ResidentAnnouncementsComponent } from './resident-announcements/resident-announcements';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ResidentAnnouncementsComponent],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
  standalone: true,
})
export class App {}
