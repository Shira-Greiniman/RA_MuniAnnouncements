# AI Usage Log

This file documents where and how AI tools were used in this assignment, as required by the task instructions.

## Tools used
- GitHub Copilot

## Log
| Step | What the AI produced | What I verified or changed manually |
|---|---|---|
| Backend scaffold | שלד פרויקט (.NET 10, Swagger, Health) נוצר ב-Copilot. נבדק ידנית: build, test, run. | Retargeted projects from net8.0 to net10.0, verified build/test/run, reviewed Program.cs |
| Models and interfaces | Copilot generated the records (CmsAnnouncement, AuthoritySiteSettings, AnnouncementDto, AnnouncementsResponse) and the service interfaces from my written spec. | Compared field names and types against the assignment JSON; made expiryDate nullable; reviewed XML comments. |
| Mock data | Copilot generated MockCmsClient and MockSiteSettingsRepository from my list of edge cases. | Verified every edge case is present (draft, expired, future, null expiry, disabled category, HTML body, foreign tenant, more items than maxItems) and that all dates are relative to TimeProvider. |
| Announcement normalizer | Copilot implemented AnnouncementNormalizer from my detailed rules. The prompts were run in the wrong order: the implementation was generated BEFORE the tests (not strict TDD). | Read the logic line by line; confirmed Take happens after sorting, time comes from TimeProvider, and category sets are case-insensitive. |
| Unit tests | Copilot wrote 14 tests (xUnit, FakeTimeProvider). One test exposed a real bug: a null CMS body threw ArgumentNullException in HtmlAgilityPack. | Fixed it with a guard and made CmsAnnouncement.Body nullable (string?). Planned manual mutation checks to confirm the tests catch regressions. |

## Notes
- All AI-generated code was reviewed by me before committing; I can explain each part.
- Prompts were written by me in small steps, and each step was verified with dotnet build / dotnet test.
- Known deviation: the normalizer was implemented before its tests, so tests were written against existing code; I compensated with manual mutation checks.