/* eslint-disable @typescript-eslint/no-unsafe-assignment */

// ===== MAKE CHANGES TO CONFIGURATION BETWEEN THESE LINES ONLY =======
// ====================================================================
const brand = Brand.WAXING_THE_CITY
const sebEnvironment = Environment.STAGING
const useLocalMiddleware = false
const localMiddlewareURL = 'https://localhost:44304'
// ====================================================================
// ======= UNLESS YOU ARE DOING SOMETHING WEIRD =======================

import waxingthecitytest from '../assets/appConfigs/waxingthecity-test.json'
import brandweardesignstest from '../assets/appConfigs/fastsigns-test.json'
import fastsignstest from '../assets/appConfigs/waxingthecity-test.json'
import go2partnerstest from '../assets/appConfigs/go2partners-test.json'
import headstartdemotest from '../assets/appConfigs/headstartdemo-test.json'
import anytimefitnessstaging from '../assets/appConfigs/anytimefitness-staging.json'
import basecampfitnessstaging from '../assets/appConfigs/basecampfitness-staging.json'
import thebarmethodstaging from '../assets/appConfigs/thebarmethod-staging.json'
import waxingthecitystaging from '../assets/appConfigs/waxingthecity-staging.json'
import anytimefitnessproduction from '../assets/appConfigs/anytimefitness-production.json'
import basecampfitnessproduction from '../assets/appConfigs/basecampfitness-production.json'
import thebarmethodproduction from '../assets/appConfigs/thebarmethod-production.json'
import waxingthecityproduction from '../assets/appConfigs/waxingthecity-production.json'

const apps = {
  TEST: {
    WAXING_THE_CITY: waxingthecitytest,
    BRANDWEAR_DESIGNS: brandweardesignstest,
    GO2PARTNERS: go2partnerstest,
    HEADSTART_DEMO: headstartdemotest,
    FAST_SIGNS: fastsignstest,
  },
  STAGING: {
    ANYTIME_FITNESS: anytimefitnessstaging,
    BASECAMP_FITNESS: basecampfitnessstaging,
    THE_BAR_METHOD: thebarmethodstaging,
    WAXING_THE_CITY: waxingthecitystaging,
  },
  PRODUCTION: {
    ANYTIME_FITNESS: anytimefitnessproduction,
    BASECAMP_FITNESS: basecampfitnessproduction,
    THE_BAR_METHOD: thebarmethodproduction,
    WAXING_THE_CITY: waxingthecityproduction,
  },
}

// for easier debugging in development mode, ignores zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
import 'zone.js/dist/zone-error'
import {
  Brand,
  Environment,
  EnvironmentConfig,
} from 'src/app/models/environment.types'

const target = apps[sebEnvironment][brand] as EnvironmentConfig
target.hostedApp = false
target.instrumentationKey = ''
if (useLocalMiddleware) {
  target.middlewareUrl = localMiddlewareURL
}
export const environment = target
