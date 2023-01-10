import { Typography } from '@mui/material';
import { useEffect, useState } from 'react';
import HashtagList from '../../components/HashtagList';
import { requestData } from '../../data'
import { Hashtag } from '../../models/Hashtag.models';

function FasterFVApi() {
    const [hashtags, setHashtags] = useState<Hashtag[] | undefined>(undefined);
    const [tweetCount, setTweetCount] = useState<number | undefined>(undefined);

    useEffect(() => {
        if (hashtags === undefined || hashtags.length === 0) {
            requestData('https://localhost:4101/api/tweets/hashtags')
                .then((result: unknown) => {
                    if (result !== undefined) {
                        setHashtags(result as Hashtag[]);
                    }
                })
        }
    }, [hashtags]);

    useEffect(() => {
        if (tweetCount === 0) {
            requestData('https://localhost:4101/api/tweets/count')
                .then((result: unknown) => {
                    if (result !== undefined) {
                        setTweetCount(result as number);
                    }
                })
        }
    }, [tweetCount])

    return (            
        <>
            <Typography variant='h1'>{tweetCount}</Typography>
                <HashtagList
                    keyPrefix="fasterkv"
                    hashtags={hashtags}
                    title="FasterKV API"
                    subtitle="- Top 10 Hashtags" />
            <Typography variant="h4">{tweetCount}</Typography>         
        </>
    );
}

export default FasterFVApi;