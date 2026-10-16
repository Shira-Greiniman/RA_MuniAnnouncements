using RA_MuniAnnouncements.Api.Models;

namespace RA_MuniAnnouncements.Api.Clients;

/// <summary>Provides date-relative mock CMS announcements for supported authorities.</summary>
public sealed class MockCmsClient(TimeProvider timeProvider) : ICmsClient
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<CmsAnnouncement>> GetAnnouncementsAsync(string authorityCode, CancellationToken ct)
    {
        await Task.Delay(10, ct);

        var now = timeProvider.GetUtcNow();

        return authorityCode switch
        {
            "6500" =>
            [
                new("a-1001", "6500", "שדרוג כביש החוף", "עבודות לשיפור התשתיות יתקיימו בשעות הלילה.", "roads", "Published", now.AddDays(-10), now.AddDays(5)),
                new("a-1002", "6500", "הסדרי תנועה ברחוב הנשיא", "החל ממחר יחולו שינויים בהסדרי התנועה.", "roads", "Published", now.AddDays(-1), now.AddDays(6)),
                new("a-1003", "6500", "הפסקת מים יזומה", "ביום שלישי תופסק אספקת המים במספר רחובות.", "water", "Published", now.AddDays(-12), now.AddDays(2)),
                new("a-1004", "6500", "אירוע קיץ בפארק", "הצטרפו להפנינג קיץ לכל המשפחה בפארק העירוני.", "events", "Published", now.AddDays(-2), null),
                new("a-1005", "6500", "תכנון עבודות בכביש", "טיוטת הודעה שטרם אושרה לפרסום.", "roads", "Draft", now.AddDays(-1), now.AddDays(8)),
                new("a-1006", "6500", "עבודות ברחוב הרצל הסתיימו", "העבודות הסתיימו והכביש נפתח לתנועה.", "roads", "Published", now.AddDays(-20), now.AddDays(-1)),
                new("a-1007", "6500", "עדכון עתידי בתחבורה", "הודעה שתפורסם במועד מאוחר יותר.", "roads", "Published", now.AddDays(2), now.AddDays(9)),
                new("a-1008", "6500", "פרסום מכרז עירוני", "פרטים על מכרז עירוני חדש.", "tenders", "Published", now.AddDays(-1), now.AddDays(20)),
                new("a-1009", "6500", "הנחיות חשובות למבקרים", """
                    <article><section><h2>מידע למבקרים</h2><p>הכניסה לאתר תתאפשר לאורך כל שעות היום. יש להישמע להנחיות הצוות ולשמור על המעברים פנויים. מידע נוסף על הפעילות, דרכי הגעה, שעות פתיחה ושירותים זמינים יימסר במקום. ניתן לפנות למוקד העירוני לקבלת פרטים ולסיוע בכל שאלה.</p><div>עדכונים: מים &amp; תשתיות&nbsp;ותחבורה.</div><script>document.cookie = "tracking=true";</script></section></article>
                    """, "events", "Published", now.AddDays(-3), now.AddDays(7)),
                new("a-1010", "6500", "פעילות קהילתית בשכונה", "תושבות ותושבים מוזמנים למפגש קהילתי.", "events", "Published", now.AddDays(-4), now.AddDays(4)),
                new("a-1011", "6500", "תיקוני מדרכה ברחוב הגפן", "העבודות יתבצעו במהלך השבוע.", "roads", "Published", now.AddDays(-6), now.AddDays(3)),
                new("a-1012", "6500", "הרצאה בספרייה העירונית", "הציבור מוזמן להרצאה פתוחה.", "events", "Published", now.AddDays(-7), now.AddDays(10)),
                new("a-1013", "5000", "הודעה מרשות אחרת", "פריט זה שייך לרשות אחרת.", "roads", "Published", now.AddDays(-1), now.AddDays(5))
            ],
            "5000" =>
            [
                new("a-2001", "5000", "שינויים בתנועה בדרך השלום", "הסדרי התנועה יעודכנו במהלך העבודות.", "roads", "Published", now.AddDays(-1), now.AddDays(5)),
                new("a-2002", "5000", "עבודות לילה ברחוב אבן גבירול", "ייתכנו שינויים זמניים בתנועה.", "roads", "Published", now.AddDays(-2), now.AddDays(6)),
                new("a-2003", "5000", "תחנת אוטובוס מועתקת", "התחנה תועתק באופן זמני.", "roads", "Published", now.AddDays(-3), now.AddDays(7)),
                new("a-2004", "5000", "פתיחת נתיב חדש", "נתיב חדש ייפתח לתנועה.", "roads", "Published", now.AddDays(-4), now.AddDays(8)),
                new("a-2005", "5000", "עדכון באספקת המים", "הודעה על עבודות מים בעיר.", "water", "Published", now.AddDays(-1), now.AddDays(5))
            ],
            _ => Array.Empty<CmsAnnouncement>()
        };
    }
}