(function() {

  $(function() {
    return $("a.submit").click(function(e) {
      e.preventDefault();
      return $(this).parents("form:first").submit();
    });
  });

}).call(this);
