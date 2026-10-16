import { ChangeDetectionStrategy, Component } from '@angular/core';
import { ResidentAnnouncementsComponent } from './resident-announcements/resident-announcements';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ResidentAnnouncementsComponent],
  selector: 'app-root',
  standalone: true,
  template: `<app-resident-announcements [authorityCode]="'6500'"></app-resident-announcements>`,
})
export class App {}
