// Import modules
import { styled, Typography } from "@mui/material";

// Import components
import { HashtagDisplay } from "../";

// Import files
import { HashtagListMessages } from "./HashtagList.constants";

// Import models
import { Hashtag } from "../../models";

/** Container div element. */
const ContainerDiv = styled('div')(() => ({
    borderBottom: '2px solid darkgrey',
    padding: '5px'
}));

/** Span element for display Tweet count. */
const StyledCount = styled('span')(() => ({
    fontWeight: 700,
    fontSize: '1.4em',
    color: 'green',
    paddingLeft: '15px',
}));

/** Ordered list element for display the top selected hashtags. */
const StyledList = styled('ol')(() => ({
    listStylePosition: 'outside',
}));

/** Coomponent props for the HashtagList component. */
export interface HashtagListProps {
    /** Prefix to be used with the hashtag text as the component key */
    keyPrefix: string;
    /** Array of hashtags to be displayed */
    hashtags?: Hashtag[];
    /** Title to display for this group of hashtags */
    title: string;
    /** The count of Tweets that this list of hashtags is from */
    tweetCount: number;
    /** Subtitle to display above hashtag list */
    subtitle: string;
}

/**
 * Represents the visual presentation of a list of hashtags and the count of their occurances.
 * @param props
 * @returns
 */
function HashtagList(props: HashtagListProps) {
    const {
        hashtags,
        keyPrefix,
        subtitle,
        title,
        tweetCount,
    } = props;

    return (
        hashtags
            ?
            <ContainerDiv>
                {/* Title */}
                <Typography variant="h5" component="div" noWrap>
                    {title}
                </Typography>

                {/* Count of Tweets */}
                <Typography variant='h6' component='span' noWrap>
                    {HashtagListMessages.COUNT_LABEL}
                </Typography>
                <StyledCount>
                    {tweetCount}
                </StyledCount>

                {/* Subtitle */}
                <Typography variant='subtitle1' component='div'>
                    {subtitle}
                </Typography>

                {/* List of hashtags */}
                <StyledList>
                    {
                        hashtags.map((tag: Hashtag) =>
                            <HashtagDisplay hashtag={tag} keyPrefix={keyPrefix} />
                        )
                    }
                </StyledList>
            </ContainerDiv>
            : <></>
    );
}

/** Default export */
export default HashtagList;