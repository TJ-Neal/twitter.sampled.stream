using System.Net;

namespace Neal.Twitter.Application.Utilities;

public static class RetryUtilities
{
    private const int EXPONENT = 2;

    private const int DELAY = 1;

    public static int MaxRetries { get; } = 10;

    public static bool ExceptionIsTransient(Exception exception)
    {
        if (exception is WebException webException)
        {
            switch (webException.Status)
            {
                case WebExceptionStatus.ConnectionClosed:
                case WebExceptionStatus.Timeout:
                case WebExceptionStatus.RequestCanceled:
                case WebExceptionStatus.UnknownError:
                    return true;
            }
        }

        return false;
    }

    public static TimeSpan GetDelay(int retries) => TimeSpan.FromSeconds(retries * EXPONENT * DELAY);
}