angular.module('virtoCommerce.marketplaceQuoteModule')
    .factory('virtoCommerce.marketplaceQuoteModule.webApi', ['$resource', function ($resource) {
        return $resource('api/vcmp', null, {
            getQuoteRequestConditionPrototype: { method: 'GET', url: 'api/vcmp/quote/condition/prototype' },
        });
    }]);
