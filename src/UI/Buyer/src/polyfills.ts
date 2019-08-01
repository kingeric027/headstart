/**
 * This file includes polyfills needed by Angular and is loaded before the app.
 * You can add your own extra polyfills to this file.
 *
 * This file is divided into 2 sections:
 *   1. Browser polyfills. These are applied before loading ZoneJS and are sorted by browsers.
 *   2. Application imports. Files imported after ZoneJS that should be loaded before your main
 *      file.
 *
 * The current setup is for so-called "evergreen" browsers; the last versions of browsers that
 * automatically update themselves. This includes Safari >= 10, Chrome >= 55 (including Opera),
 * Edge >= 13 on the desktop, and iOS 10 and Chrome on mobile.
 *
 * Learn more in https://angular.io/guide/browser-support
 */

/***************************************************************************************************
 * BROWSER POLYFILLS
 */

import 'core-js/es7/array';

/** IE10 and IE11 requires the following for NgClass support on SVG elements */
// import 'classlist.js';  // Run `npm install --save classlist.js`.

/** IE10 and IE11 requires the following for the Reflect API. */
import 'core-js/es6/reflect';

/**
 * Required to support Web Animations `@angular/platform-browser/animations`.
 * Needed for: All but Chrome, Firefox and Opera. http://caniuse.com/#feat=web-animation
 **/
// import 'web-animations-js';  // Run `npm install --save web-animations-js`.

/***************************************************************************************************
 * Zone JS is required by default for Angular itself.
 */
import 'zone.js/dist/zone'; // Included with Angular CLI.

/***************************************************************************************************
 * APPLICATION IMPORTS
 */

/**
 * polyfill for childNode.remove() to work in IE.
 *
 * see https://developer.mozilla.org/en-US/docs/Web/API/ChildNode/remove
 *
 * from:https://github.com/jserz/js_piece/blob/master/DOM/ChildNode/remove()/remove().md
 */
((arr) => {
  arr.forEach((item) => {
    if (item.hasOwnProperty('remove')) {
      return;
    }
    Object.defineProperty(item, 'remove', {
      configurable: true,
      enumerable: true,
      writable: true,
      value: function remove() {
        if (this.parentNode !== null) {
          this.parentNode.removeChild(this);
        }
      },
    });
  });
})([Element.prototype, CharacterData.prototype, DocumentType.prototype]);

// Needed for WebComponents. https://github.com/stackblitz/core/issues/475
(function() {
  'use strict';
  (function() {
    if (
      // @ts-ignore
      void 0 === window.Reflect ||
      void 0 === window.customElements ||
      // @ts-ignore:
      window.customElements.polyfillWrapFlushCallback
    )
      return;
    const a = HTMLElement;
    // @ts-ignore:
    (window.HTMLElement = function HTMLElement() {
      return Reflect.construct(a, [], this.constructor);
    }),
      (HTMLElement.prototype = a.prototype),
      (HTMLElement.prototype.constructor = HTMLElement),
      Object.setPrototypeOf(HTMLElement, a);
  })();
})();
