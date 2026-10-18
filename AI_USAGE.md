# AI Usage Log

This file documents where and how AI tools were used in this assignment, as required by the task instructions.

## Tools used
- GitHub Copilot

## Log
| Step | What the AI produced | What I verified or changed manually |
|---|---|---|
| Backend scaffold | Copilot generated the .NET 10 project scaffold with Swagger and health checks. | Retargeted projects from net8.0 to net10.0; reviewed Program.cs and verified the API build and run. |
| Models and interfaces | Copilot generated the records (CmsAnnouncement, AuthoritySiteSettings, AnnouncementDto, AnnouncementsResponse) and the service interfaces from my written spec. | Compared field names and types against the assignment JSON; made expiryDate nullable; reviewed XML comments. |
| Mock data | Copilot generated MockCmsClient and MockSiteSettingsRepository from my list of edge cases. | Verified every edge case is present (draft, expired, future, null expiry, disabled category, HTML body, foreign tenant, more items than maxItems) and that all dates are relative to TimeProvider. |
| Announcement normalizer | Copilot implemented AnnouncementNormalizer from my detailed rules. The prompts were run in the wrong order: the implementation was generated BEFORE the tests (not strict TDD). | Read the logic line by line; confirmed Take happens after sorting, time comes from TimeProvider, and category sets are case-insensitive. |
| Unit tests | Copilot generated 14 xUnit tests using FakeTimeProvider. One test exposed a bug: a null CMS body threw ArgumentNullException in HtmlAgilityPack. | Added a guard and made CmsAnnouncement.Body nullable (string?). Mutation checks are planned but have not been run. |
| Summary bug fix | I wrote a new test first (Normalize_SeparatesTextFromAdjacentHtmlElements). It failed and proved that text from adjacent HTML tags was joined without a space ("ABC" instead of "A B C"). Copilot then fixed CreateSummary. | Confirmed the test went from red to green, confirmed the 14 existing tests were not modified, and checked that the diff touched only CreateSummary. |
| Service and controller | Copilot generated AnnouncementService (parallel CMS and settings calls, tenant guard, AuthorityNotFoundException), AnnouncementsController (regex validation, ProblemDetails for 400/404/500), CORS for the Angular dev origin, and 2 service tests. | Reviewed the code and ran the full test suite. Manually checked GET /api/announcements/6500 in Swagger: urgent item first, maxItems respected, items of another tenant filtered out, summary cleaned and truncated. |
| Angular client and submission documents | Copilot scaffolded the standalone Angular client, implemented the resident announcements UI, added a custom favicon, and prepared the README and companion submission notes. | Reviewed RTL, accessibility, responsiveness, and API wiring; verified `ng build`, the Angular test with `--isolate=false`, and the companion DOCX structure. |

## Notes
- All AI-generated code was reviewed by me before committing; I can explain each part.
- Prompts were written by me in small steps. Backend changes were verified with `dotnet build` and `dotnet test`; Angular changes were verified with `ng build` and `ng test --watch=false --isolate=false`.