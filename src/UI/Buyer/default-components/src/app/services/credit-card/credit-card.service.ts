import { BuyerCreditCard } from '@ordercloud/angular-sdk';
import { AuthNetCreditCard } from 'marketplace';

export const acceptedPartialCards = {
  Visa: RegExp('^4'), // e.g. 4000000000000000
  MasterCard: RegExp('^5[1-5]|^2[1-7]'), // e.g. 5100000000000000
  Discover: RegExp('^6(?:011|5[0-9]{2})'), // e.g. 6011000000000000
};

export const GetCardType = (cardNumber: string): string => {
  if (!cardNumber) {
    return null;
  }
  for (const type in acceptedPartialCards) {
    if (acceptedPartialCards.hasOwnProperty(type)) {
      if (acceptedPartialCards[type].test(cardNumber)) {
        return type;
      }
    }
  }
  return null;
};

// this function will weed out made up numbers with the luhn algorithm. this does not guarantee that a number exists
export const IsValidPerLuhnAlgorithm = (cardNumber: string) => {
  const map: number[] = [
    0,
    1,
    2,
    3,
    4,
    5,
    6,
    7,
    8,
    9,
    0,
    2,
    4,
    6,
    8,
    1,
    3,
    5,
    7,
    9,
  ];
  let sum = 0;
  const cardNumberArrayString: string[] = cardNumber.split('');
  const cardNumberArray: number[] = cardNumberArrayString.map((num) =>
    parseInt(num, 10)
  );
  const numberLength: number = cardNumber.split('').length - 1;
  for (let i = 0; i <= numberLength; i++) {
    sum += map[cardNumberArray[numberLength - i] + (i & 1) * 10];
  }
  if (sum % 10 !== 0) {
    return false;
  }
  // If we made it this far the credit card number is in a valid format
  return true;
};

// returns true is Visa, MasterCard, or Discover
export const IsValidCardType = (cardNumber: string) => {
  if (!cardNumber) {
    return false;
  }

  for (const type in acceptedPartialCards) {
    if (acceptedPartialCards.hasOwnProperty(type)) {
      if (acceptedPartialCards[type].test(cardNumber)) {
        return true;
      }
    }
  }
  return false;
};

export const IsValidLength = (cardNumber: string) => {
  // ensures that the card is of valid length for Visa, Discover, or MasterCard
  return (
    cardNumber.length === 13 ||
    cardNumber.length === 16 ||
    cardNumber.length === 19
  );
};

export const RemoveSpacesFrom = (cardNumber: string) => {
  return cardNumber.replace(/[^0-9]/g, '');
};
