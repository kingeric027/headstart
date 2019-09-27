(function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined' ? factory(exports, require('@angular/core')) :
    typeof define === 'function' && define.amd ? define('ocm-components', ['exports', '@angular/core'], factory) :
    (global = global || self, factory(global['ocm-components'] = {}, global.ng.core));
}(this, function (exports, core) { 'use strict';

    /**
     * @fileoverview added by tsickle
     * @suppress {checkTypes,constantProperty,extraRequire,missingOverride,missingReturn,unusedPrivateMembers,uselessCode} checked by tsc
     */
    var OcmComponentsComponent = /** @class */ (function () {
        function OcmComponentsComponent() {
        }
        /**
         * @return {?}
         */
        OcmComponentsComponent.prototype.ngOnInit = /**
         * @return {?}
         */
        function () {
            console.log();
        };
        OcmComponentsComponent.decorators = [
            { type: core.Component, args: [{
                        selector: 'lib-ocm-components',
                        template: "\n    <p>\n      ocm-components works!\n    </p>\n  "
                    }] }
        ];
        /** @nocollapse */
        OcmComponentsComponent.ctorParameters = function () { return []; };
        return OcmComponentsComponent;
    }());

    /**
     * @fileoverview added by tsickle
     * @suppress {checkTypes,constantProperty,extraRequire,missingOverride,missingReturn,unusedPrivateMembers,uselessCode} checked by tsc
     */
    var OcmComponentsModule = /** @class */ (function () {
        function OcmComponentsModule() {
        }
        OcmComponentsModule.decorators = [
            { type: core.NgModule, args: [{
                        declarations: [OcmComponentsComponent],
                        imports: [],
                        exports: [OcmComponentsComponent]
                    },] }
        ];
        return OcmComponentsModule;
    }());

    exports.OcmComponentsComponent = OcmComponentsComponent;
    exports.OcmComponentsModule = OcmComponentsModule;

    Object.defineProperty(exports, '__esModule', { value: true });

}));
//# sourceMappingURL=ocm-components.umd.js.map
