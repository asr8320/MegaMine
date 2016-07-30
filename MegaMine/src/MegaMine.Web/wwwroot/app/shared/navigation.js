var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var MegaMine;
(function (MegaMine) {
    var Shared;
    (function (Shared) {
        "use strict";
        var Navigation = (function () {
            function Navigation($rootScope, $state, $window, $location, profile, utility, constants) {
                this.$rootScope = $rootScope;
                this.$state = $state;
                this.$window = $window;
                this.$location = $location;
                this.profile = profile;
                this.utility = utility;
                this.constants = constants;
                this.appTitle = "Mega Mine";
                this.isLoading = true;
                this.breadcrumbs = [];
                this.vehicleMenuItems = [];
                this.environmentName = $window.environmentName;
            }
            Navigation.prototype.initialize = function () {
                var self = this;
                self.$rootScope.navigation = self;
                self.$rootScope.$on("$stateChangeStart", function (evt, toState, toParams, fromState, fromParams) {
                    self.isLoading = true;
                    self.$window.document.title = toState.title + " | " + self.appTitle;
                    // checking whether user is authenticated
                    if (self.profile.isAuthenticated === false && toState.name !== "login") {
                        if (self.environmentName.toLowerCase() === self.constants.devEnvironment) {
                            self.profile.get();
                        }
                        else {
                            evt.preventDefault();
                            self.$state.go("login");
                        }
                    }
                });
                self.$rootScope.$on("$stateChangeSuccess", function (evt, toState, toParams, fromState, fromParams) {
                    self.isLoading = false;
                    if (toState.name === "dashboard") {
                        while (self.breadcrumbs.pop()) {
                        }
                    }
                    else {
                        while (self.breadcrumbs.length > 0) {
                            if (self.breadcrumbs[self.breadcrumbs.length - 1].name === toState.previousState) {
                                break;
                            }
                            self.breadcrumbs.pop();
                        }
                    }
                    // adding the breadcrumbs
                    self.breadcrumbs.push({ name: toState.name, title: toState.title, url: self.$location.path() });
                });
                // window resize
                angular.element(self.$window).on("resize", function () {
                    self.$rootScope.$broadcast("window_resize");
                });
            };
            Navigation.prototype.go = function (stateName) {
                this.$state.go(stateName);
            };
            Navigation.prototype.gotoDashboard = function () {
                this.$state.go("dashboard");
            };
            Navigation.prototype.gotoVehicle = function (vehicleId) {
                var self = this;
                var state = "vehicle";
                self.populateVehicleMenu(vehicleId); // populating the vehicle menu items
                if (self.vehicleMenuItems.length > 0) {
                    state += "." + self.vehicleMenuItems[0].state;
                }
                self.$state.go(state, { vehicleid: vehicleId });
            };
            Navigation.prototype.gotoSparePart = function (sparePartId) {
                this.$state.go("sparepart", { sparepartid: sparePartId });
            };
            Navigation.prototype.gotoManufacturer = function (manufacturerId) {
                this.$state.go("manufacturer", { manufacturerid: manufacturerId });
            };
            Navigation.prototype.populateVehicleMenu = function (vehicleId) {
                var self = this;
                self.vehicleMenuItems.splice(0, self.vehicleMenuItems.length);
                if (self.profile.isAuthorized(["Fleet:VehicleServiceView"])) {
                    self.vehicleMenuItems.push(self.getVehicleMenuItem(vehicleId, " Service History", "service", "service"));
                }
                if (self.profile.isAuthorized(["Fleet:VehicleFuelView"])) {
                    self.vehicleMenuItems.push(self.getVehicleMenuItem(vehicleId, " Fuel History", "fuel", "fuel"));
                }
                if (self.profile.isAuthorized(["Fleet:VehicleDriverView"])) {
                    self.vehicleMenuItems.push(self.getVehicleMenuItem(vehicleId, " Driver History", "driver", "driver"));
                }
                if (self.profile.isAuthorized(["Fleet:VehicleTripView"])) {
                    self.vehicleMenuItems.push(self.getVehicleMenuItem(vehicleId, " Trip History", "vehicletrip", "trip"));
                }
            };
            Navigation.prototype.getVehicleMenuItem = function (vehicleId, text, url, iconCss) {
                var self = this;
                var cssClass = "";
                var iconCssClass = "icon-menu icon-" + iconCss;
                var hash = self.utility.routePath("vehicle/" + vehicleId + "/" + url);
                var currentHash = self.$state.href(self.$state.current.name, self.$state.params);
                if (hash === currentHash) {
                    cssClass = "highlight";
                }
                return { text: text, url: hash, state: url, cssClass: cssClass, iconCssClass: iconCssClass };
            };
            Navigation = __decorate([
                MegaMine.service("megamine", "MegaMine.Shared.Navigation"),
                MegaMine.inject("$rootScope", "$state", "$window", "$location", "MegaMine.Shared.Profile", "MegaMine.Shared.Utility", "MegaMine.Shared.Constants")
            ], Navigation);
            return Navigation;
        }());
        Shared.Navigation = Navigation;
    })(Shared = MegaMine.Shared || (MegaMine.Shared = {}));
})(MegaMine || (MegaMine = {}));
//# sourceMappingURL=Navigation.js.map