import { ChangeDetectionStrategy, Component, effect, inject, input, signal } from '@angular/core';
import { AnnouncementsService } from '../announcements.service';
import { AnnouncementsResponse } from '../announcements.models';

type ViewState =
  | { status: 'loading' }
  | { status: 'error' }
  | { status: 'empty'; response: AnnouncementsResponse }
  | { status: 'list'; response: AnnouncementsResponse };

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-resident-announcements',
  standalone: true,
  templateUrl: './resident-announcements.html',
})
export class ResidentAnnouncementsComponent {
  readonly authorityCode = input.required<string>();
  protected readonly viewState = signal<ViewState>({ status: 'loading' });
  private readonly retryVersion = signal(0);
  private readonly announcementsService = inject(AnnouncementsService);

  constructor() {
    effect((onCleanup) => {
      const authorityCode = this.authorityCode();
      this.retryVersion();
      this.viewState.set({ status: 'loading' });

      const request = this.announcementsService.getForAuthority(authorityCode).subscribe({
        next: (response) => {
          this.viewState.set({
            status: response.announcements.length === 0 ? 'empty' : 'list',
            response,
          });
        },
        error: () => this.viewState.set({ status: 'error' }),
      });

      onCleanup(() => request.unsubscribe());
    });
  }

  protected retry(): void {
    this.retryVersion.update((version) => version + 1);
  }
}