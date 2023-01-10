import {
    AppBar,
    Box,
    Toolbar,
    Typography
} from '@mui/material';
import { styled } from "@mui/material/styles";
import SimpleApi from '../SimpleApi';
import FasterKVApi from '../FassterKVApi';
import KafkaReaderApi from '../KafkaReaderApi';

import './App.css';

const StyledContainer = styled('div')({
    display: 'flex',
    flex: '1 1 auto',
    flexDirection: 'column'
});

const StyledBox = styled(Box)({
    display: 'flex',
    flex: '1 1 auto',
    flexDirection: 'column',
    margin: '2em'
});

function App() {
    return (
        <StyledContainer>
            <AppBar position="static">
                <Toolbar variant="dense">
                    <Typography variant="h5" color="inherit" component="div">
                        Twitter Sampled Volume Stream Demonstration Site
                    </Typography>
                </Toolbar>
            </AppBar>
            <StyledBox>
                <SimpleApi />                
                <FasterKVApi />
                <KafkaReaderApi />
            </StyledBox>
        </StyledContainer>
    );
}

export default App;