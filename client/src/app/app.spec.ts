import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { AnnouncementsService } from './announcements.service';
import { App } from './app';

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [
        {
          provide: AnnouncementsService,
          useValue: {
            getForAuthority: () => of({
              authorityCode: '6500',
              authorityName: 'עיריית חדרה',
              announcements: [],
            }),
          },
        },
      ],
    })
      .compileComponents();
  });

  it('should render the resident announcements component', async () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('app-resident-announcements')).toBeTruthy();
    expect(compiled.textContent).toContain('אין הודעות חדשות');
  });
});
