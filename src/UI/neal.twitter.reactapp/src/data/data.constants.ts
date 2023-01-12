/**
 * Error messages.
 */
export enum ErrorMessages {
    /** Response is null or is incorrect type. */
    INVALID_RESPONSE = 'Received an invalid response.',
    /** Not able to create request object. */
    UNABLE_TO_CREATE_REQUEST_OBJECT = 'Unable to create request object.',
    /** Catch on the json() promise. */
    UNABLE_TO_PARSE_JSON = 'Unable to parse JSON.',
    /** An unknown error occurred. */
    UNKNOWN_REQUEST_ERROR = 'Unknown error occurred during web request.',
}

/**
 * Strings for header configs and values.
 */
export enum HeaderStrings {
    /** Header field for content mime type. */
    CONTENT_TYPE = 'Content-Type',
    /** Header value for include credential. */
    CREDENTIAL_INCLUDE = 'include',
    /** Header field for accept mime type. */
    HEADER_ACCEPT = 'Accept',
    /** Header value for accepting JSON return type. */
    JSON_TYPE = 'application/json',
    /** Header value for setting mode to cross origin. */
    MODE_CORS = 'cors',
}

/**
 * Currently used HTTP methods (verbs).
 */
export enum Methods {
    /** Verb for removing a resource. */
    DELETE = 'DELETE',
    /** Verb for inserting a resource. */
    POST = 'POST',
    /** Verb for retrieving a resource. */
    GET = 'GET',
}

export enum UriStrings {
    /** Delimiter for additional query strings. */
    ADDITIONAL_QUERY_STRING_DELIMITER = '&',
    /** Delimiter for first query string */
    FIRST_QUERY_STRING_DELIMITER = '?',
    /** Query string value for json indented response. */
    JSON_INDENT = 'format=jsonindent',
    /** Delimiter for specifying port. */
    PORT_DELIMITER = ':',
}