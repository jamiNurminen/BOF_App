import { API_URL } from '../config';

/**
 * Get banknotes data for a date range
 * @param {string} startTime - Start date/time (ISO format)
 * @param {string} endTime - End date/time (ISO format)
 * @returns {Promise<Object>} Banknotes data
 */
export const getBanknotes = async (startTime, endTime) => {
  try {
    console.log(API_URL)
    const response = await fetch(
      `${API_URL}/Banknotes?startDate=${encodeURIComponent(startTime)}&endDate=${encodeURIComponent(endTime)}`
    );
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    return await response.json();
  } catch (error) {
    console.error('Error fetching banknotes:', error);
    throw error;
  }
};

/**
 * Get available currencies for exchange rates
 * @returns {Promise<Array<string>>} Array of currency codes
 */
export const getExchangeRateCurrencies = async () => {
  try {
    const response = await fetch(`${API_URL}/ExchangeRates/currencies`);
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    return await response.json();
  } catch (error) {
    console.error('Error fetching exchange rate currencies:', error);
    throw error;
  }
};

/**
 * Convert amount to multiple currencies
 * @param {number} amount - Amount to convert
 * @param {Array<string>} currencies - Array of currency codes
 * @returns {Promise<Object>} Conversion results
 */
export const convertExchangeRates = async (amount, currencies) => {
  try {
    const response = await fetch(
      `${API_URL}/ExchangeRates?currencies=${encodeURIComponent(currencies.join(','))}&amount=${encodeURIComponent(amount)}`
    );
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    return await response.json();
  } catch (error) {
    console.error('Error fetching exchange rates:', error);
    throw error;
  }
};
