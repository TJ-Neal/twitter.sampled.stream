// Import project items
import {
    ErrorMessages,
    HeaderStrings,
    Methods,
    UriStrings,
} from './data.constants';

interface IErrorMessageResponse {
    exceptionMessage?: string;
    message?: string;
}

/**
 * Simple web request.
 */
export const request = (
    uri: string,
    options?: RequestInit,
    signal?: AbortSignal,
    noFail = false,
): Promise<Response> => baseFetch(configuredUri(uri), options, signal, noFail);

/**
 * This function will serve up will serve up data and extract JSON results.
 *
 * @param {promise} providedRequest Promise returned from an isomorphic fetch.
 */
export function requestData<T>(
    providedRequest: Promise<Response> | string,
    signal?: AbortSignal,
): Promise<T> | Promise<unknown> {
    // If request is a string instead of promise, wrap it in a request promise.
    const requestObject: Promise<Response> = typeof providedRequest === 'string'
        ? request(providedRequest, undefined, signal)
        : providedRequest;

    if (requestObject !== undefined) {
        return response(requestObject);
    }

    // requestObject creation failed.
    return new Promise((res, rej) => rej(ErrorMessages.UNABLE_TO_CREATE_REQUEST_OBJECT));
}

/**
 * Helper method to extract a web response error from the json response.
 *
 * @param error Response that contains an error to extract.
 */
export function getErrorFromResponse(error: { res: Response; toString: () => string }): string {
    if (error && error.res && error.res instanceof Response && error.res.json) {
        error
            .res
            .json()
            .then((responseJson: IErrorMessageResponse) => {
                if (responseJson) {
                    if (responseJson.exceptionMessage) {
                        return responseJson.exceptionMessage;
                    } else if (responseJson.message) {
                        return responseJson.message;
                    }
                }

                return;
            })
            .catch(() => ErrorMessages.UNABLE_TO_PARSE_JSON);
    } else if (error) {
        return error.toString();
    }

    return ErrorMessages.UNKNOWN_REQUEST_ERROR;
}

/**
 * Helper method to determine proper base URL
 */
export const getBaseUrl = (): string => {
    const { protocol, hostname, port } = window.location;

    return `${protocol}//${hostname}${port ? UriStrings.PORT_DELIMITER + port : ''}`;
};

/**
 * Configure the URI with env settings (currently only jsonindent)
 * @param uri
 */
const configuredUri = (uri: string): string => {
    let fetchUrl = uri;

    const delimiter = !fetchUrl.includes(UriStrings.FIRST_QUERY_STRING_DELIMITER)
        ? UriStrings.FIRST_QUERY_STRING_DELIMITER
        : UriStrings.ADDITIONAL_QUERY_STRING_DELIMITER;

    fetchUrl = `${fetchUrl}${delimiter}`;

    return fetchUrl;
};

/**
 * Helper method for performing a HTTP fetch on a URI.
 *
 * @param uri The URI to perform a fetch upon.
 * @param options Options to pass through to the HTTP fetch.
 * @param noFail Whether or not to allow bad requests to cause a failure.
 */
const baseFetch = async (uri: string, options: RequestInit = {}, signal?: AbortSignal, noFail = false): Promise<Response> => {
    const headers = options && options.headers ? options.headers as Headers : new Headers();
    const { method } = options;

    if (headers) {
        if (!headers.get(HeaderStrings.CONTENT_TYPE) && method && method !== Methods.DELETE) {
            headers.append(HeaderStrings.CONTENT_TYPE, HeaderStrings.JSON_TYPE);
        }

        if (!headers.get(HeaderStrings.HEADER_ACCEPT)) {
            headers.append(HeaderStrings.HEADER_ACCEPT, HeaderStrings.JSON_TYPE);
        }
    }

    const res = await fetch(
        uri,
        {
            ...options,
            credentials: HeaderStrings.CREDENTIAL_INCLUDE,
            headers,
            mode: HeaderStrings.MODE_CORS,
            signal,
        });

    return checkStatus(res, noFail);
};

interface IErrorResponse extends Response {
    error: boolean;
}

/**
 * Check the status of the web response.
 *
 * @param response Web response.
 * @param noFail Whether or not to allow bad status to cause a failure.
 */
const checkStatus = (response: Response, noFail = false): Response | IErrorResponse => {
    if (response.status >= 200 && response.status < 300) {
        return response;
    } else if (noFail) {
        return { ...response, error: true };
    }

    const error = new Error(response.statusText);

    error.stack = response.statusText;

    throw error;
};

/**
 * Helper method to get response as json result.
 * @param {promise} response Promise returned from fetch.
 */
function response<T>(
    response: Promise<Response>
): Promise<T> | Promise<unknown> {
    return new Promise((resolve, reject) => {
        response
            .then((response: Response): void => {
                if (response) {
                    response
                        .json()
                        .then((jsonResults: T) => {
                            resolve(jsonResults);
                        })
                        .catch(() => { reject(ErrorMessages.UNABLE_TO_PARSE_JSON); });
                } else {
                    reject(ErrorMessages.INVALID_RESPONSE);
                }
            }).catch((ex: unknown) => {
                reject(ex);
            });
    });
}