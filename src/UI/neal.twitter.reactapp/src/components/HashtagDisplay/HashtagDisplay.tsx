// Import modules
import { styled } from "@mui/material";

// Import models
import { Hashtag } from "../../models";

/** Container list item element. */
const StyledContainer = styled('li')(() => ({
    display: 'flex',
    justifyContent: 'space-between',
    margin: '0'
}));

/** Div for displaying the Hashtag key (text). */
const ListItemKeyDiv = styled('div')(() => ({
    display: 'list-item',
}));

/** Div for displaying the Hashtag value (count). */
const ListItemValueDiv = styled('div')(() => ({
    display: 'flex',
    flexBasis: 50,
    justifyContent: 'flex-end',
}));

/** Coomponent props for the HashtagDisplay component. */
export interface HashtagDisplayProps {
    /** Hashtag to be displayed */
    hashtag: Hashtag;
    /** Prefix to be used with the hashtag text as the component key */
    keyPrefix: string;
}

/**
 * Represents the visual presentation of a single hashtag with count of occurances.
 * @param props
 * @returns
 */
function HashtagDisplay(props: HashtagDisplayProps) {
    const {hashtag, keyPrefix} = props;

    return (
        /* List item */
        <StyledContainer key={`${keyPrefix}-${hashtag.key}`}>
            {/* Hashtag text */}
            <ListItemKeyDiv>
                {hashtag.key}
            </ListItemKeyDiv>

            {/* Hashtag occurance count */}
            <ListItemValueDiv>
                {hashtag.value}
            </ListItemValueDiv>
        </StyledContainer>
    );
}

/** Default export */
export default HashtagDisplay;