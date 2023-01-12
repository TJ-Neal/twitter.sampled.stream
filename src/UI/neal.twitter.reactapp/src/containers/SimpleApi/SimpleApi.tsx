// Import modules
import { useEffect, useState } from 'react';

// Import components
import { HashtagList } from '../../components/';

// Import models
import { Hashtag } from '../../models';

// Import utilities
import { requestData } from '../../data'

/**
 * Represents the information provided by the Simple Tweet API.
 * @returns 
 */
function SimpleApi() {
    // Configure state
    const [hashtags, setHashtags] = useState<Hashtag[] | undefined>(undefined);
    const [tweetCount, setTweetCount] = useState<number | undefined>(undefined);
    const [timer, setTimer] = useState<NodeJS.Timer | undefined>(undefined);

    // Get data on initial run
    useEffect(() => {
        GetData();
    }, []);

    // Start timer that refreshes the data displayed every 30 seconds
    useEffect(() => {
        if (timer === undefined) {
            const intervalTimer = setInterval(() => {
                GetData();
            }, 30000);

            setTimer(intervalTimer);
        }        

        // Clear timer on unmount
        return () => {
            clearInterval(timer);
        }
    }, [timer, hashtags, tweetCount]);

    // Function to retrieve data from the API
    const GetData = () => {
        requestData('http://localhost:4000/api/tweets/hashtags')
            .then((result: unknown) => {
                if (result !== undefined) {
                    setHashtags(result as Hashtag[]);
                }
            });
        requestData('http://localhost:4000/api/tweets/count')
            .then((result: unknown) => {
                if (result !== undefined) {
                    setTweetCount(result as number);
                }
            });
    }

    return (
        <>
            {/* Hashtag List */}
            <HashtagList
                keyPrefix="simple"
                tweetCount={tweetCount ?? 0}
                hashtags={hashtags}
                title="Simple In-Memory API"
                subtitle="Top 10 Hashtags" />
        </>
    );
}

/** Default export */
export default SimpleApi;