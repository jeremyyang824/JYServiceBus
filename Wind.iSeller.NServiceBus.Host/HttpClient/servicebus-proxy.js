/**
 * 调用ServiceBus服务
 * 例如：
 * servicebusProxy.ajax.triggerService(
 *   'wind.iSeller.orderManage', 'createOrderCommand', {
 *     命令数据
 *   })
 *   .then(successCallback, failureCallback);
 * );
 */
var servicebusProxy = servicebusProxy || {};
(function($) {
    if (!$) {
        throw 'jquery获取失败！';
    }

    servicebusProxy.json = typeof (JSON) == 'undefined' ? JSON2 : JSON;

    servicebusProxy.ajax = function (userOptions) {
        userOptions = userOptions || {};

        var options = $.extend({}, servicebusProxy.ajax.defaultOpts, userOptions);
        options.success = undefined;
        options.error = undefined;

        return $.Deferred(function ($dfd) {
            $.ajax(options)
                .done(function (result, textStatus, jqXHR) {
                    servicebusProxy.ajax.handleResponse(result, $dfd, jqXHR, userOptions);
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    //jqXHR.status
                    $dfd.reject.apply(this, arguments);
                    userOptions.error && userOptions.error.apply(this, arguments);
                });
        });
    };

    $.extend(servicebusProxy.ajax, {
        defaultOpts: {
            //url: 'ServiceBusExpoHttpHandler.ashx?r=' + Math.random(),
            dataType: 'json',
            type: 'POST',
            contentType: 'application/json; charset=UTF-8'
        },

        handleResponse: function (result, $dfd, jqXHR, userOptions) {
            if(!result || !result.ResponseCode) {
                $dfd && $dfd.reject('error response format!', jqXHR);
                userOptions.error && userOptions.error('error response format!', jqXHR);
                return;
            }

            if (result.ResponseCode.toString() !== '100') {
                //  服务器已经错误
                $dfd && $dfd.reject(result.ErrorInfo, jqXHR);
                userOptions.error && userOptions.error(result, jqXHR);
                return;
            }

            if (result.ResponseContent) {
                var resultData = result.ResponseContent.toJSON();
                $dfd.resolve(resultData);
                userOptions.success && userOptions.success(resultData);
            }
            else {
                $dfd.resolve();
                userOptions.success && userOptions.success();
            }
        },

        buildServiceRequest: function (assemblyName, serviceName, input, success, error, ajaxParams) {
            var requestData = {
                ServiceAssemblyName: assemblyName,
                ServiceCommandName: serviceName
            };
            if (input !== 'undefined' && input !== null) {
                if(input.UserId !== 'undefined' && input.UserId != null) {
                    requestData.userid = input.UserId;
                }
                requestData.RequestMessageContent = servicebusProxy.json.stringify(input);
            }

            var request = $.extend({}, ajaxParams, {
                data: servicebusProxy.json.stringify(requestData),
                success: success,
                error: error
            });
            
            return request;
        },

        triggerService: function(assemblyName, serviceName, input, success, error, ajaxParams) {
            var request = servicebusProxy.ajax.buildServiceRequest(assemblyName, serviceName, input, success, error, ajaxParams);
            request.url = '/ServiceBusTriggerCommand.ashx?r=' + Math.random();
            return servicebusProxy.ajax(request);
        },

        broadcastService: function(assemblyName, serviceName, input, success, error, ajaxParams) {
            var request = servicebusProxy.ajax.buildServiceRequest(assemblyName, serviceName, input, success, error, ajaxParams);
            request.url = '/ServiceBusBroadcastCommand.ashx?broadcast=1&r=' + Math.random();
            return servicebusProxy.ajax(request);
        }
    });

})(jQuery);