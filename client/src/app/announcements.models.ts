export interface Announcement {
  id: string;
  title: string;
  summary: string;
  categoryName: string;
  isUrgent: boolean;
  publishDate: string;
}

export interface AnnouncementsResponse {
  authorityCode: string;
  authorityName: string;
  announcements: Announcement[];
}