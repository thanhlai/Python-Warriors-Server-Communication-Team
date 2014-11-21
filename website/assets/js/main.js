$(function() {
	$('#content').onepage_scroll({
		sectionContainer: ".row",
		easing: "ease",
		animationTime: 500,
		pagination: true,
		updateURL: false,
		beforeMove: function(index) {updateNav(index);},
		afterMove: function(index) {},
		loop: false,
		keyboard: true,
		responsiveFallback: false,
		direction: "vertical"
	});

	$('.navbar-nav a').click(function() {
		$('#content').jumpTo($(this).data('target') - 1);
		updateNav($(this).data('target'), false);
	});

	function updateNav(index) {
		if(isNumber(index)) {
			$('.navbar-nav .active').removeClass('active');
			$(".navbar-nav a[data-target='" + index + "']").parent('li').addClass('active');
		}
	}

	function isNumber(n) {
		return !isNaN(parseFloat(n)) && isFinite(n);
	}
});