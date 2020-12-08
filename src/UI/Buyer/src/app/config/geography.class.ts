export interface CountryDefinition {
  label: string
  abbreviation: string
}

export interface StateDefinition {
  label: string
  abbreviation: string
  country: string
}

// @dynamic
export class GeographyConfig {
  static states: StateDefinition[] = [
    { label: 'Alabama', abbreviation: 'AL', country: 'US' },
    { label: 'Alaska', abbreviation: 'AK', country: 'US' },
    { label: 'Arizona', abbreviation: 'AZ', country: 'US' },
    { label: 'Arkansas', abbreviation: 'AR', country: 'US' },
    { label: 'California', abbreviation: 'CA', country: 'US' },
    { label: 'Colorado', abbreviation: 'CO', country: 'US' },
    { label: 'Connecticut', abbreviation: 'CT', country: 'US' },
    { label: 'Delaware', abbreviation: 'DE', country: 'US' },
    { label: 'District of Columbia', abbreviation: 'DC', country: 'US' },
    { label: 'Florida', abbreviation: 'FL', country: 'US' },
    { label: 'Georgia', abbreviation: 'GA', country: 'US' },
    { label: 'Hawaii', abbreviation: 'HI', country: 'US' },
    { label: 'Idaho', abbreviation: 'ID', country: 'US' },
    { label: 'Illinois', abbreviation: 'IL', country: 'US' },
    { label: 'Indiana', abbreviation: 'IN', country: 'US' },
    { label: 'Iowa', abbreviation: 'IA', country: 'US' },
    { label: 'Kansas', abbreviation: 'KS', country: 'US' },
    { label: 'Kentucky', abbreviation: 'KY', country: 'US' },
    { label: 'Louisiana', abbreviation: 'LA', country: 'US' },
    { label: 'Maine', abbreviation: 'ME', country: 'US' },
    { label: 'Maryland', abbreviation: 'MD', country: 'US' },
    { label: 'Massachusetts', abbreviation: 'MA', country: 'US' },
    { label: 'Michigan', abbreviation: 'MI', country: 'US' },
    { label: 'Minnesota', abbreviation: 'MN', country: 'US' },
    { label: 'Mississippi', abbreviation: 'MS', country: 'US' },
    { label: 'Missouri', abbreviation: 'MO', country: 'US' },
    { label: 'Montana', abbreviation: 'MT', country: 'US' },
    { label: 'Nebraska', abbreviation: 'NE', country: 'US' },
    { label: 'Nevada', abbreviation: 'NV', country: 'US' },
    { label: 'New Hampshire', abbreviation: 'NH', country: 'US' },
    { label: 'New Jersey', abbreviation: 'NJ', country: 'US' },
    { label: 'New Mexico', abbreviation: 'NM', country: 'US' },
    { label: 'New York', abbreviation: 'NY', country: 'US' },
    { label: 'North Carolina', abbreviation: 'NC', country: 'US' },
    { label: 'North Dakota', abbreviation: 'ND', country: 'US' },
    { label: 'Ohio', abbreviation: 'OH', country: 'US' },
    { label: 'Oklahoma', abbreviation: 'OK', country: 'US' },
    { label: 'Oregon', abbreviation: 'OR', country: 'US' },
    { label: 'Pennsylvania', abbreviation: 'PA', country: 'US' },
    { label: 'Rhode Island', abbreviation: 'RI', country: 'US' },
    { label: 'South Carolina', abbreviation: 'SC', country: 'US' },
    { label: 'South Dakota', abbreviation: 'SD', country: 'US' },
    { label: 'Tennessee', abbreviation: 'TN', country: 'US' },
    { label: 'Texas', abbreviation: 'TX', country: 'US' },
    { label: 'Utah', abbreviation: 'UT', country: 'US' },
    { label: 'Vermont', abbreviation: 'VT', country: 'US' },
    { label: 'Virginia', abbreviation: 'VA', country: 'US' },
    { label: 'Washington', abbreviation: 'WA', country: 'US' },
    { label: 'West Virginia', abbreviation: 'WV', country: 'US' },
    { label: 'Wisconsin', abbreviation: 'WI', country: 'US' },
    { label: 'Wyoming', abbreviation: 'WY', country: 'US' },
    {
      label: 'Armed Forces Americas (AA)',
      abbreviation: 'AA',
      country: 'US',
    },
    {
      label: 'Armed Forces Africa/Canada/Europe/Middle East (AE)',
      abbreviation: 'AE',
      country: 'US',
    },
    { label: 'Armed Forces Pacific (AP)', abbreviation: 'AP', country: 'US' },
    { label: 'American Samoa', abbreviation: 'AS', country: 'US' },
    {
      label: 'Federated States of Micronesia',
      abbreviation: 'FM',
      country: 'US',
    },
    { label: 'Guam', abbreviation: 'GU', country: 'US' },
    { label: 'Marshall Islands', abbreviation: 'MH', country: 'US' },
    { label: 'Northern Mariana Islands', abbreviation: 'MP', country: 'US' },
    { label: 'Palau', abbreviation: 'PW', country: 'US' },
    { label: 'Puerto Rico', abbreviation: 'PR', country: 'US' },
    { label: 'Virgin Islands', abbreviation: 'VI', country: 'US' },
    { label: 'Alberta', abbreviation: 'AB', country: 'CA' },
    { label: 'British Columbia', abbreviation: 'BC', country: 'CA' },
    { label: 'Manitoba', abbreviation: 'MB', country: 'CA' },
    { label: 'New Brunswick', abbreviation: 'NB', country: 'CA' },
    { label: 'Newfoundland and Labrador', abbreviation: 'NL', country: 'CA' },
    { label: 'Northwest Territories', abbreviation: 'NT', country: 'CA' },
    { label: 'Nova Scotia', abbreviation: 'NS', country: 'CA' },
    { label: 'Nunavut', abbreviation: 'NU', country: 'CA' },
    { label: 'Ontario', abbreviation: 'ON', country: 'CA' },
    { label: 'Prince Edward Island', abbreviation: 'PE', country: 'CA' },
    { label: 'Quebec', abbreviation: 'QC', country: 'CA' },
    { label: 'Saskatchewan', abbreviation: 'SK', country: 'CA' },
    { label: 'Yukon', abbreviation: 'YT', country: 'CA' },
  ]

  static getStates(countryCode: string): StateDefinition[] {
    return this.states.filter((state) => state.country === countryCode)
  }

  static getCountries(): CountryDefinition[] {
    return [
      { label: 'United States of America', abbreviation: 'US' },
      { label: 'Canada', abbreviation: 'CA' },
    ]
  }
}