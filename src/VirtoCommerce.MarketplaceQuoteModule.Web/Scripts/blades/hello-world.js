angular.module('VirtoCommerce.MarketplaceQuoteModule')
    .controller('VirtoCommerce.MarketplaceQuoteModule.helloWorldController', ['$scope', 'VirtoCommerce.MarketplaceQuoteModule.webApi', function ($scope, api) {
        var blade = $scope.blade;
        blade.title = 'MarketplaceQuoteModule';

        blade.refresh = function () {
            api.get(function (data) {
                blade.title = 'MarketplaceQuoteModule.blades.hello-world.title';
                blade.data = data.result;
                blade.isLoading = false;
            });
        };

        blade.refresh();
    }]);
