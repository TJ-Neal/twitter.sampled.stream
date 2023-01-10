import { styled } from "@mui/material";
import { Hashtag } from "../../models/Hashtag.models";

const StyledContainer = styled('div')(() => ({
    display: 'flex',
    justifyContent: 'space-between',
    padding: '0 1em',
    margin: '0 10px'
}));

export interface HashtagProps {
    key: string;
    hashtag: Hashtag;
}

function HashtagDisplay(props: HashtagProps) {
    const {hashtag} = props;

    return (
        <StyledContainer key={props.key}>
            <div>{hashtag.key}</div>
            <div>{hashtag.value}</div>
        </StyledContainer>
    );
}

export default HashtagDisplay;