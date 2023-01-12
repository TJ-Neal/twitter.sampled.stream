// Import modules
import {
    AppBar,
    Box,
    styled,
    Toolbar,
    Typography
} from '@mui/material';

// Import components
import {
    SimpleApi,
    FasterKVApi,
    KafkaReaderApi
} from '../';

// Import files
import { AppMessageStrings as Messages } from './App.constants';

/** Container for the application */
const StyledContainer = styled('div')({
    display: 'flex',
    flex: '1 1 auto',
    flexDirection: 'column'
});

/** Wrapping box for the different API components displayed */
const StyledBox = styled(Box)({
    display: 'flex',
    flex: '1 1 auto',
    flexDirection: 'column',
    margin: '2em',
    maxWidth: '650px',
});

/**
 * Represents the main entry point of the application
 */
function App() {
    return (
        <StyledContainer>
            {/* Header */}
            <AppBar position="static">
                <Toolbar variant="dense">
                    <Typography variant="h5" color="inherit" component="div">
                        {Messages.HEADER_TITLE}
                    </Typography>
                </Toolbar>
            </AppBar>

            {/* Content */}
            <StyledBox>
                <SimpleApi />
                <FasterKVApi />
                <KafkaReaderApi />
            </StyledBox>
        </StyledContainer>
    );
}

/** Default export */
export default App;