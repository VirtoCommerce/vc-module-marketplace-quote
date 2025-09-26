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
    .run(['$http', '$compile',
        'platformWebApp.mainMenuService', '$state',
        'platformWebApp.widgetService',
        'virtoCommerce.marketplaceQuoteModule.webApi',
        'virtoCommerce.coreModule.common.dynamicExpressionService', 
        'virtoCommerce.stateMachineModule.stateMachineTypes',
        function ($http, $compile,
            mainMenuService, $state,
            widgetService,
            webApi,
            dynamicExpressionService,
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

            // Vendor details: Quote requests widget
            var sellerQuotesWidget = {
                controller: 'virtoCommerce.marketplaceQuoteModule.sellerQuotesWidgetController',
                template: 'Modules/$(VirtoCommerce.MarketplaceQuote)/Scripts/widgets/seller-quotes-widget.tpl.html'
            };
            widgetService.registerWidget(sellerQuotesWidget, 'sellerDetails');

            // Quote request state machine entity type registration
            stateMachineTypes.addType({
                caption: 'marketplaceQuote.state-machine-entity-types.quote-request',
                value: 'VirtoCommerce.QuoteModule.Core.Models.QuoteRequest',
                getConditionTreeCallback: webApi.getQuoteRequestConditionPrototype
            });

            // State machine transition's condition template registration
            dynamicExpressionService.registerExpression({
                id: 'QuoteRequestCondition',
                displayName: 'Quote request condition ...',
                templateURL: 'QuoteRequestCondition.html',
                newChildLabel: 'Add condition'
            });

            $http.get('Modules/$(VirtoCommerce.MarketplaceQuote)/Scripts/dynamicConditions/templates.html').then(function (response) {
                $compile(response.data);
            });

        }
    ]);
