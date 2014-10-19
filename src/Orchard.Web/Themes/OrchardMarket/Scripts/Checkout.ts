/// <reference path="type/jquery.d.ts" />
$(".payment-providers").on("change", "input[type='radio']", function (e) {
    var providerId: string = $(this).val();
    var providerElement = $("[data-provider-id='" + providerId + "']");
    var providerName: string = providerElement.data("provider-name");
    $(".payment-provider-logo").hide();
    providerElement.show();
    $("#checkoutButton").show();
});

$(function () {
    $(".payment-providers input[type='radio']").removeAttr("checked");
});