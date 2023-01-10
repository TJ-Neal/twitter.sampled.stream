import { styled, Typography } from "@mui/material";
import { Hashtag } from "../../models/Hashtag.models";
import HashtagDisplay from "../HashtagDisplay/HashtagDisplay";

const ContainerDiv = styled('div')(() => ({
    borderBottom: '2px solid darkgrey',
    padding: '5px'
}));

export interface HashtagListProps {
    keyPrefix: string;
    hashtags?: Hashtag[];
    title: string;
    subtitle: string;
}

function HashtagList(props: HashtagListProps) {
    const {
        hashtags,
        keyPrefix,
        subtitle, 
        title
    } = props;

    return (
        hashtags
            ? 
                <ContainerDiv>
                    <Typography variant='h6' component='span'>{title} </Typography><Typography variant='subtitle1' component='span'>{subtitle}</Typography>
                    <ol>
                        {
                            hashtags.map((tag: Hashtag) => (<li><HashtagDisplay key={`${keyPrefix}-${tag.key}`} hashtag={tag} /></li>))
                        }
                    </ol>
                </ContainerDiv>
            : <></>
    );
}

export default HashtagList;