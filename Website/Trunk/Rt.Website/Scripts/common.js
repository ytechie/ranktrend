$(document).ready
(
	function()
	{
		$(".header").corner();
		$(".right p").corner();
		$("h3").corner("5px");
		$(".helpHeader").click
		(
			function()
			{
				$(this)
					.toggleClass("showHelpHeader")
					.toggleClass("hideHelpHeader")
					.next(".help")
					.toggle('slow');
			}
		);
		
	}
);