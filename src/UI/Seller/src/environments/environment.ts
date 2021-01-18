// ===== MAKE CHANGES TO CONFIGURATION BETWEEN THESE LINES ONLY =======
// ====================================================================
const brand = Brand.SELF_ESTEEM_BRANDS
const sebEnvironment = Environment.TEST
const useLocalMiddleware = false
const localMiddlewareURL = 'https://localhost:5001'
// ====================================================================
// ======= UNLESS YOU ARE DOING SOMETHING WEIRD =======================

import selfesteembrandstest from '../assets/appConfigs/selfesteembrands-test.json'
import brandweardesignstest from '../assets/appConfigs/brandweardesigns-test.json'
import fastsignstest from '../assets/appConfigs/fastsigns-test.json'
import go2partnerstest from '../assets/appConfigs/go2partners-test.json'
import headstartdemotest from '../assets/appConfigs/headstartdemo-test.json'
import selfesteembrandsstaging from '../assets/appConfigs/selfesteembrands-staging.json'
import selfesteembrandsproduction from '../assets/appConfigs/selfesteembrands-production.json'

const apps = {
  TEST: {
    SELF_ESTEEM_BRANDS: selfesteembrandstest,
    BRAND_WEAR_DESIGNS: brandweardesignstest,
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
import {
  Brand,
  Environment,
  EnvironmentConfig,
} from '@app-seller/models/environment.types'
const target: EnvironmentConfig = apps[sebEnvironment][brand]
target.hostedApp = false
if (useLocalMiddleware) {
  target.middlewareUrl = localMiddlewareURL
}
export const environment = target
