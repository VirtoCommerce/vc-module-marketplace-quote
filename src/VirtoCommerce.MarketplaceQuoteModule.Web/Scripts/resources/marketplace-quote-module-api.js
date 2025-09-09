angular.module('virtoCommerce.marketplaceQuoteModule')
    .factory('virtoCommerce.marketplaceQuoteModule.webApi', ['$resource', function ($resource) {
        return $resource('api/marketplace-quote-module');
    }]);
