angular.module('VirtoCommerce.MarketplaceQuoteModule')
    .factory('VirtoCommerce.MarketplaceQuoteModule.webApi', ['$resource', function ($resource) {
        return $resource('api/marketplace-quote-module');
    }]);
