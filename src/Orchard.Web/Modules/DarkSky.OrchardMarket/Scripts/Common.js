(function($) {
  $(function() {
    return $("a.submit").click(function(e) {
      e.preventDefault();
      var $sender = $(this);
      var $form = $(this).parents("form:first");
      var name = $sender.data("name");
      if (name && name.length > 0) {
        $form.append("<input type=\"hidden\" name=\"" + name + "\" value=\"submit\" />");
      }
      return $form.submit();
    });
  });
}).call(jQuery);
