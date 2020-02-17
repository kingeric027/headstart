import { __read, __spread } from "tslib";
export function getPsHeight(additionalClass) {
    if (additionalClass === void 0) { additionalClass = ''; }
    /* The height of perfect scroll containers is typically dependent on certain items
    that are always in the layout which are given the class name 'base-layout-item' in
    addition to items that are specific to calculating the height of that specific perfect
    scroll container.*/
    var baseLayoutItems = Array.from(document.getElementsByClassName('base-layout-item'));
    var additionalItems = additionalClass ? Array.from(document.getElementsByClassName(additionalClass)) : [];
    var totalHeight = 0;
    __spread(baseLayoutItems, additionalItems).forEach(function (div) {
        // div does contain the property 'offsetHeight, but typescript throws error
        totalHeight += div.offsetHeight;
    });
    return window.innerHeight - totalHeight;
}
export function getScreenSizeBreakPoint() {
    var map = {
        xs: 575,
        sm: 767,
        md: 991,
        lg: 1199,
    };
    var innerWidth = window.innerWidth;
    if (innerWidth < map.xs) {
        return 'xs';
    }
    else if (innerWidth > map.xs && innerWidth < map.sm) {
        return 'sm';
    }
    else if (innerWidth > map.sm && innerWidth < map.md) {
        return 'md';
    }
    else if (innerWidth > map.md && innerWidth < map.lg) {
        return 'lg';
    }
    else if (innerWidth > map.lg) {
        return 'xl';
    }
}
//# sourceMappingURL=dom.helper.js.map