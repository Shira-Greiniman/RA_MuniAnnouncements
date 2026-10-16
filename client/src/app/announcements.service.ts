import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { AnnouncementsResponse } from './announcements.models';

@Injectable({ providedIn: 'root' })
export class AnnouncementsService {
  private readonly http = inject(HttpClient);

  getForAuthority(authorityCode: string): Observable<AnnouncementsResponse> {
    const baseUrl = environment.apiBaseUrl.replace(/\/$/, '');
    const url = `${baseUrl}/announcements/${encodeURIComponent(authorityCode)}`;
    return this.http.get<AnnouncementsResponse>(url);
  }
}