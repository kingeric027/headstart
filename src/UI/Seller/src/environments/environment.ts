import { Brand, Environment, EnvironmentConfig } from './environment.interfaces'

// ===== MAKE CHANGES TO CONFIGURATION BETWEEN THESE LINES ONLY =======
// ====================================================================
const brand = Brand.SELF_ESTEEM_BRANDS
const sebEnvironment = Environment.TEST
const useLocalMiddleware = false
const localMiddlewareURL = 'https://localhost:5001'
// ====================================================================
// ======= UNLESS YOU ARE DOING SOMETHING WEIRD =======================

import { environment as selfesteembrandstest } from './environment.selfesteembrands-test'
import { environment as brandweardesignstest } from './environment.brandweardesigns-test'
import { environment as fastsignstest } from './environment.fastsigns-test'
import { environment as go2partnerstest } from './environment.go2partners-test'
import { environment as headstartdemotest } from './environment.headstartdemo-test'
import { environment as selfesteembrandsstaging } from './environment.selfesteembrands-staging'
import { environment as selfesteembrandsproduction } from './environment.selfesteembrands-production'

const apps = {
  TEST: {
    SELF_ESTEEM_BRANDS: selfesteembrandstest,
    BRANDWEAR_DESIGNS: brandweardesignstest,
    GO2PARTNERS: go2partnerstest,
    HEADSTART_DEMO: headstartdemotest,
    FAST_SIGNS: fastsignstest,
  },
  STAGING: {
    SELF_ESTEEM_BRANDS: selfesteembrandsstaging,
  },
  PRODUCTION: {
    SELF_ESTEEM_BRANDS: selfesteembrandsproduction,
  },
}

// for easier debugging in development mode, ignores zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
import 'zone.js/dist/zone-error'
const target: EnvironmentConfig = apps[sebEnvironment][brand]
target.hostedApp = false
if (useLocalMiddleware) {
  target.middlewareUrl = localMiddlewareURL
}
export const environment = target
