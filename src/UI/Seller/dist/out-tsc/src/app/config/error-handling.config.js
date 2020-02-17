import { __decorate, __extends, __metadata, __param } from "tslib";
import { ErrorHandler, Inject, Injector, Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
/**
 * this error handler class extends angular's ErrorHandler
 * in order to automatically format ordercloud error messages
 * and display them in toastr
 */
var AppErrorHandler = /** @class */ (function (_super) {
    __extends(AppErrorHandler, _super);
    function AppErrorHandler(injector) {
        var _this = _super.call(this) || this;
        _this.injector = injector;
        return _this;
    }
    AppErrorHandler.prototype.handleError = function (ex) {
        this.displayError(ex);
        _super.prototype.handleError.call(this, ex);
    };
    /**
     * use this to display error message
     * but continue exection of code
     */
    AppErrorHandler.prototype.displayError = function (ex) {
        if (ex.rejection) {
            ex = ex.rejection;
        }
        var message = '';
        if (ex && ex.error && ex.error.Errors && ex.error.Errors.length) {
            var e = ex.error.Errors[0];
            if (e.Data && e.Data.WebhookName) {
                // webhook error
                message = e.Data.body;
            }
            else if (e.ErrorCode === 'NotFound') {
                message = e.Data.ObjectType + " " + e.Data.ObjectID + " not found.";
            }
            else {
                message = e.Message;
            }
        }
        else if (ex && ex.error && ex.error['error_description']) {
            message = ex.error['error_description'];
        }
        else if (ex.error) {
            message = ex.error;
        }
        else if (ex.message) {
            message = ex.message;
        }
        else {
            message = 'An error occurred';
        }
        if (typeof message === 'object') {
            message = JSON.stringify(message);
        }
        if (message === 'Token refresh attempt not possible' || message === 'Access token is invalid or expired.') {
            // display user friendly error
            message = 'Your session has expired. Please log in.';
        }
        this.toastrService.error(message, 'Error', { onActivateTick: true });
    };
    Object.defineProperty(AppErrorHandler.prototype, "toastrService", {
        /**
         * Need to get ToastrService from injector rather than constructor injection to avoid cyclic dependency error
         */
        get: function () {
            return this.injector.get(ToastrService);
        },
        enumerable: true,
        configurable: true
    });
    AppErrorHandler = __decorate([
        Injectable({
            providedIn: 'root',
        }),
        __param(0, Inject(Injector)),
        __metadata("design:paramtypes", [Injector])
    ], AppErrorHandler);
    return AppErrorHandler;
}(ErrorHandler));
export { AppErrorHandler };
//# sourceMappingURL=error-handling.config.js.map