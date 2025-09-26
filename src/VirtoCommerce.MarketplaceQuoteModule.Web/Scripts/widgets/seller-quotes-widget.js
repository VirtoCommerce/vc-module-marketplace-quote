angular.module('virtoCommerce.marketplaceQuoteModule')
    .controller('virtoCommerce.marketplaceQuoteModule.sellerQuotesWidgetController',
        ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.quoteModule.quotes',
        function ($scope, bladeNavigationService, quotesApi) {
            var sellerId = $scope.widget.blade.currentEntity.id;
            
            var searchCriteria = {
                responseGroup: "None",
                sellerId: sellerId,
                skip: 0,
                take: 0
            };
            quotesApi.search(searchCriteria, function (data) {
                $scope.quotesCount = data.totalCount;
            });

            $scope.openBlade = function () {
                var newBlade = {
                    id: 'seller-quotes-list',
                    title: 'marketplaceQuote.blades.seller-quotes-list.title',
                    sellerId: sellerId,
                    controller: 'virtoCommerce.marketplaceQuoteModule.sellerQuotesListController',
                    template: 'Modules/$(VirtoCommerce.MarketplaceQuote)/Scripts/blades/seller-quotes-list.tpl.html',
                    isExpanded: true
                };

                bladeNavigationService.showBlade(newBlade, $scope.widget.blade);
            };
        }
    ]);
