$ ->
	$("a.submit").click (e) -> 
		e.preventDefault()
		$(this).parents("form:first").submit()