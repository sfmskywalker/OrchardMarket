///<reference path="jquery.d.ts" />
///<reference path="jqueryui.autocomplete.d.ts" />
$(function () {
    var $userName = $("#UserName");
    $userName.autocomplete({ source: $userName.data("source") });
});
//# sourceMappingURL=AddUser.js.map
