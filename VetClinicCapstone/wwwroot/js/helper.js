//var SavingFunction = function (options) {

//	var title = options.title;
//	var url = options.url;
//	var form = options.form;
//	var redirection = options.redirection;
//	var message = options.message;
//	var title = options.title;

//	Swal.fire({
//		title: title,
//		text: message,
//		icon: 'warning',
//		showCancelButton: true,
//		confirmButtonColor: '#3085d6',
//		cancelButtonColor: '#d33',
//		confirmButtonText: 'Yes',
//		cancelButtonText: 'No'
//	}).then((result) => {
//		if (result.isConfirmed) {
//			// If user clicks 'Yes', perform AJAX request
//			$.ajax({
//				url: url, // Replace with your actual controller and action names
//				type: 'post',
//				data: $(form).serialize(), // Generate GUID on server side
//				success: function (response) {

//					// Handle success response
//					if (response.success) {
//						// Show success message with SweetAlert
//						Swal.fire(
//							response.title,
//							response.message,
//							'success'
//						).then(function () {
//							// Optionally, redirect to another page or reload the page
//							window.location = redirection; // Example: Redirect to the index page
//						});
//					} else {
//						// Show error message with SweetAlert
//						Swal.fire(
//							'Error!',
//							response.message,
//							'error'
//						);
//					}
//				},
//				error: function (error) {
//					// Show error message with SweetAlert
//					Swal.fire(
//						'Error!',
//						error.text,
//						'error'
//					);
//				}
//			});
//		}
//	});
//}


var SavingFunction = function (options) {
    var title = options.title;
    var url = options.url;
    var form = options.form;
    var redirection = options.redirection;
    var message = options.message;
    var callback = options.callback;

    Swal.fire({
        title: title,
        text: message,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            // If user clicks 'Yes', perform AJAX request
            $.ajax({
                url: url, // Replace with your actual controller and action names
                type: 'post',
                data: $(form).serialize(), // Serialize the form data
                success: function (response) {
                    if (response.success) {
                        // Show success message with SweetAlert
                        Swal.fire(
                            response.title,
                            response.message,
                            'success'
                        ).then(function () {
                            // Redirect or perform other actions
                            if (redirection == null) {
                                callback(response);
                            }
                            else {
                                window.location = redirection;
                            }
                             
                            //debugger
                            //if (typeof callback === 'function') {
                               
                            //}
                        });
                    } else {
                        // Show error message with SweetAlert
                        Swal.fire(
                            'Error!',
                            response.message,
                            'error'
                        );
                    }
                },
                error: function (error) {
                    // Show error message with SweetAlert
                    Swal.fire(
                        'Error!',
                        error.responseText || 'An error occurred while processing your request.',
                        'error'
                    );
                }
            });
        }
    });
}