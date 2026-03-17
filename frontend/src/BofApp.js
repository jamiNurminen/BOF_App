import * as React from 'react';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';

import Typography from '@mui/material/Typography';
import Stack from '@mui/material/Stack';
import MuiCard from '@mui/material/Card';
import { Select } from '@mui/material';
import { MenuItem } from '@mui/material';
import { DateTimePicker } from '@mui/x-date-pickers/DateTimePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { styled } from '@mui/material/styles';
import AppTheme from './shared-theme/AppTheme';
import ColorModeSelect from './shared-theme/ColorModeSelect';

import { getBanknotes, getExchangeRateCurrencies, convertExchangeRates } from './Services/BofapiService';
import { convertFieldResponseIntoMuiTextFieldProps } from '@mui/x-date-pickers/internals';

const Card = styled(MuiCard)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  alignSelf: 'center',
  width: '100%',
  padding: theme.spacing(4),
  gap: theme.spacing(2),
  margin: 'auto',
  [theme.breakpoints.up('sm')]: {
    maxWidth: '450px',
  },
  boxShadow:
    'hsla(220, 30%, 5%, 0.05) 0px 5px 15px 0px, hsla(220, 25%, 10%, 0.05) 0px 15px 35px -5px',
  ...theme.applyStyles('dark', {
    boxShadow:
      'hsla(220, 30%, 5%, 0.5) 0px 5px 15px 0px, hsla(220, 25%, 10%, 0.08) 0px 15px 35px -5px',
  }),
}));

const SignInContainer = styled(Stack)(({ theme }) => ({
  height: 'calc((1 - var(--template-frame-height, 0)) * 100dvh)',
  minHeight: '100%',
  padding: theme.spacing(2),
  [theme.breakpoints.up('sm')]: {
    padding: theme.spacing(4),
  },
  '&::before': {
    content: '""',
    display: 'block',
    position: 'absolute',
    zIndex: -1,
    inset: 0,
    backgroundImage:
      'radial-gradient(ellipse at 50% 50%, hsl(210, 100%, 97%), hsl(0, 0%, 100%))',
    backgroundRepeat: 'no-repeat',
    ...theme.applyStyles('dark', {
      backgroundImage:
        'radial-gradient(at 50% 50%, hsla(210, 100%, 16%, 0.5), hsl(220, 30%, 5%))',
    }),
  },
}));

const infoBoxStyles = {
  p: 1.5,
  borderRadius: 1,
  border: 1,
  borderColor: 'divider',
};

const valueBoxStyles = (value) => ({
  p: 1.5,
  borderRadius: 1,
  bgcolor: value < 0 ? 'error.light' : 'success.light',
  color: value < 0 ? 'error.contrastText' : 'success.contrastText',
});

const formatDate = (value) => {
  if (!value) {
    return '-';
  }

  return new Date(value).toLocaleDateString('fi-FI');
};

const formatNumber = (value) => {
  return new Intl.NumberFormat('fi-FI').format(value ?? 0);
};

export default function BofApp(props) {
  const [startDate, setStartDate] = React.useState(null);
  const [endDate, setEndDate] = React.useState(null);
  const [banknotesData, setBanknotesData] = React.useState(null);
  const [showBreakdown, setShowBreakdown] = React.useState(false);
  const [currencies, setCurrencies] = React.useState([]);
  const [selectedCurrency, setSelectedCurrency] = React.useState('');
  const [exchangeRate, setExchangeRate] = React.useState(null);
  const [convertedAmount, setConvertedAmount] = React.useState(null);

  React.useEffect(() => {
    try {
      const fetchCurrencies = async () => {
        const data = await getExchangeRateCurrencies();
        setCurrencies(data);
        console.log('Currencies:', data);
      }
      fetchCurrencies();
    } catch (error) {
      console.error('Error fetching currencies:', error);
    }
  }, []);

  const handleSubmit = async () => {
    if (!startDate || !endDate) {
      alert('Valitse sekä alkupäivä että loppupäivä.');
      return;
    }

    if (startDate.isAfter(endDate)) {
      alert('Alkupäivä täytyy olla ennen loppupäivää.');
      return;
    }

    // Fetch banknotes data for the selected date range
    let responseData;

    try {
      responseData = await getBanknotes(startDate.toISOString(), endDate.toISOString());
      setBanknotesData(responseData);
      setShowBreakdown(false);
    } catch (error) {
      console.error('Error fetching banknotes data:', error);
    }

    try {
      if (!selectedCurrency || !responseData.quantityChange) {
        return;
      }
      let amountChange = responseData.amountChange;
      let isNegative = amountChange < 0;
      if (isNegative) {
        // flip the amount to the positive
        amountChange = -amountChange;
      }

      const exchangeRateData = await convertExchangeRates(amountChange, [selectedCurrency]);
      console.log('Raw Exchange Rate Data:', exchangeRateData);

      let convertedAmount = exchangeRateData[0].amount;

      if (isNegative) {
        convertedAmount = -convertedAmount;
      }

      console.log('Exchange Rate Data:', exchangeRateData);

      setExchangeRate(exchangeRateData[0].exchangeRate);
      setConvertedAmount(convertedAmount);
    } catch (error) {
      console.error('Error fetching exchange rates:', error);
    }
  }

  const handleCurrencyChange = (event) => {
    setBanknotesData(null);
    setSelectedCurrency(event.target.value);
  }

  return (
    <AppTheme {...props}>
      <CssBaseline enableColorScheme />
      <LocalizationProvider dateAdapter={AdapterDayjs}>
        <Box
          sx={{
            display: 'flex',
            flexDirection: { xs: 'column', lg: 'row' },
            alignItems: { xs: 'stretch', lg: 'flex-start' },
            justifyContent: 'center',
            gap: 3,
            px: 2,
            pb: 4,
          }}
        >
          <SignInContainer
            direction="column"
            justifyContent="center"
            sx={{
              flex: { xs: '1 1 auto', lg: '0 0 500px' },
              maxWidth: { xs: '100%', lg: '500px' },
              minHeight: { xs: 'auto', lg: '100dvh' },
            }}
          >
            <ColorModeSelect sx={{ position: 'fixed', top: '1rem', right: '1rem' }} />
            <Card variant="outlined">
            <Typography
              component="h1"
              variant="h4"
              sx={{ width: '100%', fontSize: 'clamp(2rem, 10vw, 2.15rem)' }}
            >
              Suomen Pankki - App
            </Typography>
            <Typography variant="body1" sx={{ mb: 2 }}>
              Valitse päivämäärä ja halutessasi valuutta, niin näet eurosetelien liikkeellelaskun määrän kyseiselle aikavälille.
            </Typography>
            <Box spacing={2} sx={{ display: 'flex', flexDirection: 'row', flexWrap: 'nowrap', gap: 5 }}>
              <Box >
                <Typography component="label" variant="body2" sx={{ display: 'block', mb: 1 }}>
                  Alkupäivä
                </Typography>
                <DateTimePicker 
                  value={startDate}
                  required
                  onChange={(newValue) => setStartDate(newValue)}
                /> 
              </Box>
              <Box>
                <Typography component="label" variant="body2" sx={{ display: 'block', mb: 1 }}>
                  Loppupäivä
                </Typography>
                 <DateTimePicker 
                  value={endDate}
                  required
                  onChange={(newValue) => setEndDate(newValue)}
                />
              </Box>
            </Box>
            <Typography component="label" variant="body2" sx={{ display: 'block' }}>
              Valuuttamuunnos (valinnainen)
            </Typography>
            <Select
              labelId="demo-simple-select-label"
              id="demo-simple-select"
              value={selectedCurrency}
              label="Age"
              onChange={handleCurrencyChange}
            >
              {currencies.map((currency) => (
                <MenuItem key={currency.currency} value={currency.currency}>{currency.currencyNameFi}</MenuItem>
              ))}
            </Select>
            <Button variant="contained" onClick={handleSubmit} sx={{ mt: 2 }}>Hae Data</Button>
          </Card>
        </SignInContainer>

        {banknotesData && (
          <>
            <Box
              sx={{
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                alignSelf: 'center',
                color: 'text.secondary',
              }}
            >
              <Typography
                variant="h3"
                sx={{ display: { xs: 'none', lg: 'block' }, lineHeight: 1 }}
              >
                →
              </Typography>
              <Typography
                variant="h3"
                sx={{ display: { xs: 'block', lg: 'none' }, lineHeight: 1 }}
              >
                ↓
              </Typography>
            </Box>

            <Box
              sx={{
                flex: { xs: '1 1 auto', lg: '0 0 900px' },
                width: '100%',
                maxWidth: '900px',
                alignSelf: { xs: 'stretch', lg: 'center' },
                pt: { xs: 0, lg: 4 },
              }}
            >
              <MuiCard
                variant="outlined"
                sx={{ width: '100%', maxWidth: '900px', p: 3, display: 'flex', flexDirection: 'column', gap: 2 }}
              >
                <Typography component="h2" variant="h5">
                  Eurosetelien liikkeellelasku 
                </Typography>

                <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 1.5 }}>
                  <Box sx={infoBoxStyles}>
                    <Typography variant="body2">Alkupäivä</Typography>
                    <Typography variant="h6">{formatDate(banknotesData.startDate)}</Typography>
                  </Box>
                  <Box sx={infoBoxStyles}>
                    <Typography variant="body2">Loppupäivä</Typography>
                    <Typography variant="h6">{formatDate(banknotesData.endDate)}</Typography>
                  </Box>
                </Box>

                <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 1.5 }}>
                  <Box sx={valueBoxStyles(banknotesData.quantityChange)}>
                    <Typography variant="body2">Määrän muutos</Typography>
                    <Typography variant="h6">{formatNumber(banknotesData.quantityChange)}</Typography>
                  </Box>
                  <Box sx={valueBoxStyles(banknotesData.amountChange)}>
                    <Typography variant="body2">Summan muutos (EUR)</Typography>
                    <Typography variant="h6">{formatNumber(banknotesData.amountChange)} €</Typography>
                  </Box>
                  {selectedCurrency && exchangeRate && (
                    <Box sx={valueBoxStyles(convertedAmount)}>
                      <Typography variant="body2">Summan muutos ({selectedCurrency})</Typography>
                      <Typography variant="h6">{formatNumber(convertedAmount)} {selectedCurrency}</Typography>
                    </Box>
                    )}
                </Box>

                <Box>
                  <Button variant="outlined" onClick={() => setShowBreakdown((prev) => !prev)}>
                    {showBreakdown ? 'Piilota erittely' : 'Näytä erittely'}
                  </Button>
                </Box>

                {showBreakdown && (
                  <Stack spacing={1}>
                    {banknotesData.breakdown?.map((row) => (
                      <Box
                        key={row.denomination}
                        sx={{
                          display: 'grid',
                          gridTemplateColumns: { xs: '1fr', md: '100px 1fr 1fr 1fr' },
                          gap: 1,
                        }}
                      >
                        <Box sx={infoBoxStyles}>
                          <Typography variant="body2">Nimellisarvo</Typography>
                          <Typography variant="h6">{row.denomination} €</Typography>
                        </Box>
                        <Box sx={valueBoxStyles(row.quantity)}>
                          <Typography variant="body2">Määrä</Typography>
                          <Typography variant="h6">{formatNumber(row.quantity)}</Typography>
                        </Box>
                        <Box sx={valueBoxStyles(row.amount)}>
                          <Typography variant="body2">Summa (EUR)</Typography>
                          <Typography variant="h6">{formatNumber(row.amount)} €</Typography>
                        </Box>
                        {selectedCurrency && exchangeRate && (
                         <Box sx={valueBoxStyles(row.amount * exchangeRate)}>
                          <Typography variant="body2">Summa ({selectedCurrency})</Typography>
                          <Typography variant="h6">{formatNumber(row.amount * exchangeRate)} {selectedCurrency}</Typography>
                        </Box> )}
                      </Box>
                    ))}
                  </Stack>
                )}
              </MuiCard>
            </Box>
          </>
        )}
      </Box>
      </LocalizationProvider>
    </AppTheme>
  );
}
