"use strict";

function assert(condition, message) {
	if (!condition) {
		message = message || "Assertion failed";
		if (typeof Error !== "undefined") {
			throw new Error(message);
		}
		throw message; // Fallback
	}
}

$(function () {
	$('form').on('submit', function (e) {
		var $form = $(this);

		if ($form.data('submitted') === true) {
			// Previously submitted - don't submit again.
			e.preventDefault();
		} else {
			// Mark it so that the next submit can be ignored.
			$form.data('submitted', true);
		}
	});
});