import { Brand, Environment, EnvironmentConfig } from './environment.interfaces'

// ===== MAKE CHANGES TO CONFIGURATION BETWEEN THESE LINES ONLY =======
// ====================================================================
const brand = Brand.WAXING_THE_CITY
const sebEnvironment = Environment.TEST
const useLocalMiddleware = false
const localMiddlewareURL = 'https://localhost:5001'
// ====================================================================
// ======= UNLESS YOU ARE DOING SOMETHING WEIRD =======================

import { environment as waxingthecitytest } from './environment.waxingthecity-test'
import { environment as brandweardesignstest } from './environment.brandweardesigns-test'
import { environment as fastsignstest } from './environment.fastsigns-test'
import { environment as go2partnerstest } from './environment.go2partners-test'
import { environment as headstartdemotest } from './environment.headstartdemo-test'
import { environment as anytimefitnessstaging } from './environment.anytimefitness-staging'
import { environment as basecampfitnessstaging } from './environment.basecampfitness-staging'
import { environment as thebarmethodstaging } from './environment.thebarmethod-staging'
import { environment as waxingthecitystaging } from './environment.waxingthecity-staging'
import { environment as anytimefitnessproduction } from './environment.anytimefitness-production'
import { environment as basecampfitnessproduction } from './environment.basecampfitness-production'
import { environment as thebarmethodproduction } from './environment.thebarmethod-production'
import { environment as waxingthecityproduction } from './environment.waxingthecity-production'
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

const target: EnvironmentConfig = apps[sebEnvironment][brand]
target.hostedApp = false
target.instrumentationKey = ''
if (useLocalMiddleware) {
  target.middlewareUrl = localMiddlewareURL
}
export const environment = target
