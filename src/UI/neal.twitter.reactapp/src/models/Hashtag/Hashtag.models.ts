/** Represents a key/value (text/count) pair of a Tweet Hashtag with count of occurance */
export interface Hashtag {
    /** Key (text) of the hashtag */
    key: string;
    /** Value (count) of the occurances of the hashtag */
    value: number;
}