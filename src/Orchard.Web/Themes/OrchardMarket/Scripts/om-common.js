(function($) {
    $(function() {
        $("a.submit").on("click", function(e) {
            e.preventDefault();
            $(this).parents("form:first").submit();
        });
    });
})(jQuery);