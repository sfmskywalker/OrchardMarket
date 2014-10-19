///<reference path="jquery.d.ts" />
///<reference path="jqueryui.autocomplete.d.ts" />
$(function () {
    var $organizationName = $("#OrganizationName");
    $organizationName.autocomplete({ source: $organizationName.data("source") });
});
//# sourceMappingURL=JoinOrganization.js.map
