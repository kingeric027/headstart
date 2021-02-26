import { Injectable } from '@angular/core'
import {
  CountryDefinition,
  StateDefinition,
} from '@app-seller/models/currency-geography.types'

@Injectable({
  providedIn: 'root',
})
export class AppGeographyService {
  states: StateDefinition[] = [
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
    { label: 'Northern Mariana Islands', abbreviation: 'HS', country: 'US' },
    { label: 'Palau', abbreviation: 'PW', country: 'US' },
    { label: 'Puerto Rico', abbreviation: 'PR', country: 'US' },
    { label: 'Virgin Islands', abbreviation: 'VI', country: 'US' },
    { label: 'Drenthe', abbreviation: 'Drenthe', country: 'NL' },
    { label: 'Flevoland', abbreviation: 'Flevoland', country: 'NL' },
    { label: 'Friesland', abbreviation: 'Friesland', country: 'NL' },
    { label: 'Gelderland', abbreviation: 'Gelderland', country: 'NL' },
    { label: 'Groningen', abbreviation: 'Groningen', country: 'NL' },
    { label: 'Limburg', abbreviation: 'Limburg', country: 'NL' },
    { label: 'Noord-Brabant', abbreviation: 'Noord-Brabant', country: 'NL' },
    { label: 'Noord-Holland', abbreviation: 'Noord-Holland', country: 'NL' },
    { label: 'Overijssel', abbreviation: 'Overijssel', country: 'NL' },
    { label: 'Utrecht', abbreviation: 'Utrecht', country: 'NL' },
    { label: 'Zeeland', abbreviation: 'Zeeland', country: 'NL' },
    { label: 'Zuid-Holland', abbreviation: 'Zuid-Holland', country: 'NL' },
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

  getStates(countryCode): StateDefinition[] {
    return this.states.filter((state) => state.country === countryCode)
  }

  getCountries(): CountryDefinition[] {
    return [
      { label: 'United States of America', abbreviation: 'US' },
      { label: 'Afghanistan', abbreviation: 'AF' },
      { label: 'Åland Islands', abbreviation: 'AX' },
      { label: 'Albania', abbreviation: 'AL' },
      { label: 'Algeria', abbreviation: 'DZ' },
      { label: 'American Samoa', abbreviation: 'AS' },
      { label: 'Andorra', abbreviation: 'AD' },
      { label: 'Angola', abbreviation: 'AO' },
      { label: 'Anguilla', abbreviation: 'AI' },
      { label: 'Antarctica', abbreviation: 'AQ' },
      { label: 'Antigua and Barbuda', abbreviation: 'AG' },
      { label: 'Argentina', abbreviation: 'AR' },
      { label: 'Armenia', abbreviation: 'AM' },
      { label: 'Aruba', abbreviation: 'AW' },
      { label: 'Australia', abbreviation: 'AU' },
      { label: 'Austria', abbreviation: 'AT' },
      { label: 'Azerbaijan', abbreviation: 'AZ' },
      { label: 'Bahamas', abbreviation: 'BS' },
      { label: 'Bahrain', abbreviation: 'BH' },
      { label: 'Bangladesh', abbreviation: 'BD' },
      { label: 'Barbados', abbreviation: 'BB' },
      { label: 'Belarus', abbreviation: 'BY' },
      { label: 'Belgium', abbreviation: 'BE' },
      { label: 'Belize', abbreviation: 'BZ' },
      { label: 'Benin', abbreviation: 'BJ' },
      { label: 'Bermuda', abbreviation: 'BM' },
      { label: 'Bhutan', abbreviation: 'BT' },
      { label: 'Bolivia', abbreviation: 'BO' },
      { label: 'Bosnia and Herzegovina', abbreviation: 'BA' },
      { label: 'Botswana', abbreviation: 'BW' },
      { label: 'Bouvet Island', abbreviation: 'BV' },
      { label: 'Brazil', abbreviation: 'BR' },
      { label: 'British Indian Ocean Territory', abbreviation: 'IO' },
      { label: 'Brunei Darussalam', abbreviation: 'BN' },
      { label: 'Bulgaria', abbreviation: 'BG' },
      { label: 'Burkina Faso', abbreviation: 'BF' },
      { label: 'Burundi', abbreviation: 'BI' },
      { label: 'Cambodia', abbreviation: 'KH' },
      { label: 'Cameroon', abbreviation: 'CM' },
      { label: 'Canada', abbreviation: 'CA' },
      { label: 'Cape Verde', abbreviation: 'CV' },
      { label: 'Cayman Islands', abbreviation: 'KY' },
      { label: 'Central African Republic', abbreviation: 'CF' },
      { label: 'Chad', abbreviation: 'TD' },
      { label: 'Chile', abbreviation: 'CL' },
      { label: 'China', abbreviation: 'CN' },
      { label: 'Christmas Island Australia', abbreviation: 'CX' },
      { label: 'Cocos Keeling Islands', abbreviation: 'CC' },
      { label: 'Colombia', abbreviation: 'CO' },
      { label: 'Comoros', abbreviation: 'KM' },
      { label: 'Congo', abbreviation: 'CG' },
      { label: 'Congo, D.R.', abbreviation: 'CD' },
      { label: 'Cook Islands', abbreviation: 'CK' },
      { label: 'Costa Rica', abbreviation: 'CR' },
      { label: "Cote D'Ivoire Ivory Coast", abbreviation: 'CI' },
      { label: 'Croatia Hrvatska', abbreviation: 'HR' },
      { label: 'Cuba', abbreviation: 'CU' },
      { label: 'Cyprus', abbreviation: 'CY' },
      { label: 'Czech Republic', abbreviation: 'CZ' },
      { label: 'Denmark', abbreviation: 'DK' },
      { label: 'Djibouti', abbreviation: 'DJ' },
      { label: 'Dominica', abbreviation: 'DM' },
      { label: 'Dominican Republic', abbreviation: 'DO' },
      { label: 'Ecuador', abbreviation: 'EC' },
      { label: 'Egypt', abbreviation: 'EG' },
      { label: 'El Salvador', abbreviation: 'SV' },
      { label: 'Equatorial Guinea', abbreviation: 'GQ' },
      { label: 'Eritrea', abbreviation: 'ER' },
      { label: 'Estonia', abbreviation: 'EE' },
      { label: 'Ethiopia', abbreviation: 'ET' },
      { label: 'Faeroe Islands', abbreviation: 'FO' },
      { label: 'Falkland Islands Malvinas', abbreviation: 'FK' },
      { label: 'Fiji', abbreviation: 'FJ' },
      { label: 'Finland', abbreviation: 'FI' },
      { label: 'France', abbreviation: 'FR' },
      { label: 'France, Metropolitan', abbreviation: 'FX' },
      { label: 'French Guiana', abbreviation: 'GF' },
      { label: 'French Polynesia', abbreviation: 'PF' },
      { label: 'French Southern Territories', abbreviation: 'TF' },
      { label: 'Gabon', abbreviation: 'GA' },
      { label: 'Gambia', abbreviation: 'GM' },
      { label: 'Georgia', abbreviation: 'GE' },
      { label: 'Germany', abbreviation: 'DE' },
      { label: 'Ghana', abbreviation: 'GH' },
      { label: 'Gibraltar', abbreviation: 'GI' },
      { label: 'Greece', abbreviation: 'GR' },
      { label: 'Greenland', abbreviation: 'GL' },
      { label: 'Grenada', abbreviation: 'GD' },
      { label: 'Guadeloupe', abbreviation: 'GP' },
      { label: 'Guam', abbreviation: 'GU' },
      { label: 'Guatemala', abbreviation: 'GT' },
      { label: 'Guinea', abbreviation: 'GN' },
      { label: 'Guinea Bissau', abbreviation: 'GW' },
      { label: 'Guyana', abbreviation: 'GY' },
      { label: 'Haiti', abbreviation: 'HT' },
      { label: 'Heard and McDonald Is.', abbreviation: 'HM' },
      { label: 'Honduras', abbreviation: 'HN' },
      { label: 'Hong Kong', abbreviation: 'HK' },
      { label: 'Hungary', abbreviation: 'HU' },
      { label: 'Iceland', abbreviation: 'IS' },
      { label: 'India', abbreviation: 'IN' },
      { label: 'Indonesia', abbreviation: 'ID' },
      { label: 'Iran', abbreviation: 'IR' },
      { label: 'Iraq', abbreviation: 'IQ' },
      { label: 'Isle of Man', abbreviation: 'IM' },
      { label: 'Ireland', abbreviation: 'IE' },
      { label: 'Israel', abbreviation: 'IL' },
      { label: 'Italy', abbreviation: 'IT' },
      { label: 'Jamaica', abbreviation: 'JM' },
      { label: 'Japan', abbreviation: 'JP' },
      { label: 'Jersey', abbreviation: 'JE' },
      { label: 'Jordan', abbreviation: 'JO' },
      { label: 'Kazakhstan', abbreviation: 'KZ' },
      { label: 'Kenya', abbreviation: 'KE' },
      { label: 'Kiribati', abbreviation: 'KI' },
      { label: 'Korea North', abbreviation: 'KP' },
      { label: 'Korea South', abbreviation: 'KR' },
      { label: 'Kuwait', abbreviation: 'KW' },
      { label: 'Kyrgyzstan', abbreviation: 'KG' },
      { label: 'Lao P.Dem.R.', abbreviation: 'LA' },
      { label: 'Latvia', abbreviation: 'LV' },
      { label: 'Lebanon', abbreviation: 'LB' },
      { label: 'Lesotho', abbreviation: 'LS' },
      { label: 'Liberia', abbreviation: 'LR' },
      { label: 'Libyan Arab Jamahiriya', abbreviation: 'LY' },
      { label: 'Liechtenstein', abbreviation: 'LI' },
      { label: 'Lithuania', abbreviation: 'LT' },
      { label: 'Luxembourg', abbreviation: 'LU' },
      { label: 'Macau', abbreviation: 'MO' },
      { label: 'Macedonia', abbreviation: 'MK' },
      { label: 'Madagascar', abbreviation: 'MG' },
      { label: 'Malawi', abbreviation: 'MW' },
      { label: 'Malaysia', abbreviation: 'MY' },
      { label: 'Maldives', abbreviation: 'MV' },
      { label: 'Mali', abbreviation: 'ML' },
      { label: 'Malta', abbreviation: 'MT' },
      { label: 'Marshall Islands', abbreviation: 'MH' },
      { label: 'Martinique', abbreviation: 'MQ' },
      { label: 'Mauritania', abbreviation: 'MR' },
      { label: 'Mauritius', abbreviation: 'MU' },
      { label: 'Mayotte', abbreviation: 'YT' },
      { label: 'Mexico', abbreviation: 'MX' },
      { label: 'Micronesia', abbreviation: 'FM' },
      { label: 'Moldova', abbreviation: 'MD' },
      { label: 'Monaco', abbreviation: 'MC' },
      { label: 'Mongolia', abbreviation: 'MN' },
      { label: 'Montenegro', abbreviation: 'ME' },
      { label: 'Montserrat', abbreviation: 'MS' },
      { label: 'Morocco', abbreviation: 'MA' },
      { label: 'Mozambique', abbreviation: 'MZ' },
      { label: 'Myanmar', abbreviation: 'MM' },
      { label: 'Namibia', abbreviation: 'NA' },
      { label: 'Nauru', abbreviation: 'NR' },
      { label: 'Nepal', abbreviation: 'NP' },
      { label: 'Netherlands', abbreviation: 'NL' },
      { label: 'Netherlands Antilles', abbreviation: 'AN' },
      { label: 'New Caledonia', abbreviation: 'NC' },
      { label: 'New Zealand', abbreviation: 'NZ' },
      { label: 'Nicaragua', abbreviation: 'NI' },
      { label: 'Niger', abbreviation: 'NE' },
      { label: 'Nigeria', abbreviation: 'NG' },
      { label: 'Niue', abbreviation: 'NU' },
      { label: 'Norfolk Island', abbreviation: 'NF' },
      { label: 'Northern Mariana Islands', abbreviation: 'HS' },
      { label: 'Norway', abbreviation: 'NO' },
      { label: 'Oman', abbreviation: 'OM' },
      { label: 'Pakistan', abbreviation: 'PK' },
      { label: 'Palau', abbreviation: 'PW' },
      { label: 'Palestinian Territory, Occupied', abbreviation: 'PS' },
      { label: 'Panama', abbreviation: 'PA' },
      { label: 'Papua New Guinea', abbreviation: 'PG' },
      { label: 'Paraguay', abbreviation: 'PY' },
      { label: 'Peru', abbreviation: 'PE' },
      { label: 'Philippines', abbreviation: 'PH' },
      { label: 'Pitcairn', abbreviation: 'PN' },
      { label: 'Poland', abbreviation: 'PL' },
      { label: 'Portugal', abbreviation: 'PT' },
      { label: 'Puerto Rico', abbreviation: 'PR' },
      { label: 'Qatar', abbreviation: 'QA' },
      { label: 'Reunion', abbreviation: 'RE' },
      { label: 'Romania', abbreviation: 'RO' },
      { label: 'Russian Federation', abbreviation: 'RU' },
      { label: 'Rwanda', abbreviation: 'RW' },
      { label: 'Saint Helena', abbreviation: 'SH' },
      { label: 'Saint Kitts and Nevis', abbreviation: 'KN' },
      { label: 'Saint Lucia', abbreviation: 'LC' },
      { label: 'Saint Pierre and Miquelon', abbreviation: 'PM' },
      { label: 'Saint Vincent and the Grenadines', abbreviation: 'VC' },
      { label: 'Samoa', abbreviation: 'WS' },
      { label: 'San Marino', abbreviation: 'SM' },
      { label: 'Sao Tome and Principe', abbreviation: 'ST' },
      { label: 'Saudi Arabia', abbreviation: 'SA' },
      { label: 'Senegal', abbreviation: 'SN' },
      { label: 'Serbia', abbreviation: 'RS' },
      { label: 'Seychelles', abbreviation: 'SC' },
      { label: 'Sierra Leone', abbreviation: 'SL' },
      { label: 'Singapore', abbreviation: 'SG' },
      { label: 'Slovakia', abbreviation: 'SK' },
      { label: 'Slovenia', abbreviation: 'SI' },
      { label: 'Solomon Islands', abbreviation: 'SB' },
      { label: 'Somalia', abbreviation: 'SO' },
      { label: 'South Africa', abbreviation: 'ZA' },
      { label: 'S. Georgia &amp; S. Sandwich Is.', abbreviation: 'GS' },
      { label: 'Spain', abbreviation: 'ES' },
      { label: 'Sri Lanka', abbreviation: 'LK' },
      { label: 'Sudan', abbreviation: 'SD' },
      { label: 'Suriname', abbreviation: 'SR' },
      { label: 'Svalbard &amp; Jan Mayen Is.', abbreviation: 'SJ' },
      { label: 'Swaziland', abbreviation: 'SZ' },
      { label: 'Sweden', abbreviation: 'SE' },
      { label: 'Switzerland', abbreviation: 'CH' },
      { label: 'Syrian Arab Rep.', abbreviation: 'SY' },
      { label: 'Taiwan', abbreviation: 'TW' },
      { label: 'Tajikistan', abbreviation: 'TJ' },
      { label: 'Tanzania', abbreviation: 'TZ' },
      { label: 'Thailand', abbreviation: 'TH' },
      { label: 'Timor-Leste', abbreviation: 'TG' },
      { label: 'Togo', abbreviation: 'TG' },
      { label: 'Tokelau', abbreviation: 'TK' },
      { label: 'Tonga', abbreviation: 'TO' },
      { label: 'Trinidad and Tobago', abbreviation: 'TT' },
      { label: 'Tunisia', abbreviation: 'TN' },
      { label: 'Turkey', abbreviation: 'TR' },
      { label: 'Turkmenistan', abbreviation: 'TM' },
      { label: 'Turks and Caicos Islands', abbreviation: 'TC' },
      { label: 'Tuvalu', abbreviation: 'TU' },
      { label: 'Uganda', abbreviation: 'UG' },
      { label: 'Ukraine', abbreviation: 'UA' },
      { label: 'United Kingdom', abbreviation: 'GB' },
      { label: 'United Arab Emirates', abbreviation: 'AE' },
      { label: 'US Minor Outlying Is.', abbreviation: 'UM' },
      { label: 'Uruguay', abbreviation: 'UY' },
      { label: 'Uzbekistan', abbreviation: 'UZ' },
      { label: 'Vanuatu', abbreviation: 'VU' },
      { label: 'Vatican City State', abbreviation: 'VC' },
      { label: 'Venezuela', abbreviation: 'VE' },
      { label: 'Viet Nam', abbreviation: 'VN' },
      { label: 'Virgin Islands British', abbreviation: 'VG' },
      { label: 'Virgin Islands US', abbreviation: 'VI' },
      { label: 'Wallis and Futuna Islnds', abbreviation: 'WF' },
      { label: 'Western Sahara', abbreviation: 'EH' },
      { label: 'Yemen', abbreviation: 'YE' },
      { label: 'Yugoslavia', abbreviation: 'YU' },
      { label: 'Zambia', abbreviation: 'ZM' },
      { label: 'Zimbabwe', abbreviation: 'ZW' },
    ]
  }
}
