namespace Neal.Twitter.Core.Entities.Configuration;

public class Pagination
{
    public int DefaultPageSize { get; } = 50;

    public int MaximumPageSize { get; } = 1000;

    public int PageSize { get; set; }

    public int Page { get; set; }

    public Pagination(int? page, int? pageSize)
    {
        this.Page = page is null or <= 0
                ? 1                                     // Too small, set to first page
                : page.Value;

        this.PageSize = pageSize is null or <= 0
                ? this.DefaultPageSize                  // Too small, set to default
                : pageSize.Value > this.MaximumPageSize
                    ? this.MaximumPageSize              // Too big, set to max
                    : pageSize.Value;                   // Just right
    }
}