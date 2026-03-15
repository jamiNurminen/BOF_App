import logo from './logo.svg';
import './App.css';
import SignIn from './SignIn';
import { createTheme } from '@mui/material/styles';
import { useState } from 'react';

const lightTheme = createTheme({
  palette: {
    mode: 'light',
  },
});

const darkTheme = createTheme({
  palette: {
    mode: 'dark',
  },
});

function App() {
  return (
    <SignIn />
  );
}

export default App;
