export const getZip = (countryCode: string = 'US'): string => {
    switch (countryCode) {
      case 'CA':
        return '^[A-Za-z]\\d[A-Za-z][ -]?\\d[A-Za-z]\\d$'; // CA zip
      case 'US':
        return '^[0-9]{5}(?:-[0-9]{4})?$'; // US zip - five numbers
    }
}