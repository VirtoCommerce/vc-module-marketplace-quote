// Call this to register your module to main application
var moduleName = 'virtoCommerce.marketplaceQuoteModule';

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
    .config(['$stateProvider',
        function ($stateProvider) {
        //    $stateProvider
        //        .state('workspace.MarketplaceQuoteModuleState', {
        //            url: '/marketplace-quote-module',
        //            templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
        //            controller: [
        //                'platformWebApp.bladeNavigationService',
        //                function (bladeNavigationService) {
        //                    var newBlade = {
        //                        id: 'blade1',
        //                        controller: 'VirtoCommerce.MarketplaceQuoteModule.helloWorldController',
        //                        template: 'Modules/$(VirtoCommerce.MarketplaceQuoteModule)/Scripts/blades/hello-world.html',
        //                        isClosingDisabled: true,
        //                    };
        //                    bladeNavigationService.showBlade(newBlade);
        //                }
        //            ]
        //        });
        }
    ])
    .run(['platformWebApp.mainMenuService', '$state',
        'virtoCommerce.stateMachineModule.stateMachineTypes',
        function (mainMenuService, $state,
            stateMachineTypes) {
            //Register module in main menu
            //var menuItem = {
            //    path: 'browse/marketplace-quote-module',
            //    icon: 'fa fa-cube',
            //    title: 'MarketplaceQuoteModule',
            //    priority: 100,
            //    action: function () { $state.go('workspace.MarketplaceQuoteModuleState'); },
            //    permission: 'marketplace-quote-module:access',
            //};
            //mainMenuService.addMenuItem(menuItem);

            // Quote request state machine entity type registration
            stateMachineTypes.addType({
                caption: 'marketplaceQuote.state-machine-entity-types.quote-request',
                value: 'VirtoCommerce.QuoteModule.Core.Models.QuoteRequest'
            });

        }
    ]);
