/* eslint-disable prettier/prettier */
import { ClientFunction } from 'testcafe'
import testConfig from '../../testConfig'
import {
    adminClientSetup,
    buyerTestSetup,
    baseTestCleanup,
} from '../../helpers/test-setup'
import buyerHeaderPage from '../../pages/buyer/buyer-header-page'
import productDetailPage from '../../pages/buyer/product-detail-page'
import shoppingCartPage from '../../pages/buyer/shopping-cart-page'
import checkoutPage from '../../pages/buyer/checkout-page'
import loadingHelper from '../../helpers/loading-helper'
import homepage from '../../pages/buyer/homepage'

const getLocation = ClientFunction(() => document.location.href)

fixture`Homepage Tests`
    .meta('TestRun', '1')
    .before(async ctx => {
        ctx.adminClientAuth = await adminClientSetup()
    })
    .beforeEach(async t => {
        t.ctx.testUser = await buyerTestSetup(t.fixtureCtx.adminClientAuth)
    })
    .afterEach(async t => {
        await baseTestCleanup(
            t.ctx.testUser.ID,
            '0005',
            t.fixtureCtx.adminClientAuth
        )
    })
    .page(testConfig.buyerAppUrl)

test('Can I click brand hyperlink and be brought to homepage? | 0000', async t => {
    await homepage.clickAccountButton();
    await homepage.clickMyCreditCardsLink();
    await homepage.clickHomepageLogo();
    // Expect that you're brought to home
    await t.expect(getLocation()).contains('home');
    await t.expect(homepage.featuredProductsH3.exists).ok()
})
test('Can I navigate to products list page? | 0000', async t => {
    await homepage.clickProductsLink();
    // Expect that you're brought to product list page
    await t.expect(getLocation()).contains('products');
    await t.expect(homepage.refineByH2.exists).ok()
})
// TODO: Reinstate this test once an ID has been added
// test('Can I navigate to suppliers list page? | 0000', async t => {
//     await homepage.clickSuppliersLink();
//     // Expect that you're brought to product list page
//     await t.expect(getLocation()).contains('suppliers');
//     // TODO: Add in expect h1 once ID is added to element
//     // await t.expect(homepage.supplierTitleH1.exists).ok()
// })
