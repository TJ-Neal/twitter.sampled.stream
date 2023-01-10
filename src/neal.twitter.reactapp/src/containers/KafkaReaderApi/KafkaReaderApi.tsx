import { useEffect, useState } from 'react';
import HashtagList from '../../components/HashtagList';
import { requestData } from '../../data'
import { Hashtag } from '../../models/Hashtag.models';

function KafkaReaderApi() {
    const [hashtags, setHashtags] = useState<Hashtag[] | undefined>(undefined);

    useEffect(() => {
        if (hashtags === undefined || hashtags.length === 0) {
            requestData('https://localhost:4201/api/tweets/hashtags')
                .then((result: unknown) => {
                    if (result !== undefined) {
                        setHashtags(result as []);
                    }
                })
        }
    }, [hashtags]);

    return (        
        <HashtagList 
            keyPrefix="kafka"
            hashtags={hashtags}
            title="Kafka/Docker API"
            subtitle="- Top 10 Hashtags" />  
    );
}

export default KafkaReaderApi;