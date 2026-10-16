import { ChangeDetectionStrategy, Component, ElementRef, effect, inject, input, signal, ViewChild } from '@angular/core';
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
  styleUrl: './resident-announcements.css',
  templateUrl: './resident-announcements.html',
})
export class ResidentAnnouncementsComponent {
  readonly authorityCode = input.required<string>();
  protected readonly viewState = signal<ViewState>({ status: 'loading' });
  private readonly retryVersion = signal(0);
  private readonly announcementsService = inject(AnnouncementsService);

  @ViewChild('feedTitle') private feedTitle?: ElementRef<HTMLHeadingElement>;

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
    this.feedTitle?.nativeElement.focus();
    this.retryVersion.update((version) => version + 1);
  }

  protected toIsoDate(displayDate: string): string {
    const match = /^(\d{2})\/(\d{2})\/(\d{4})$/.exec(displayDate);
    return match ? `${match[3]}-${match[2]}-${match[1]}` : '';
  }
}