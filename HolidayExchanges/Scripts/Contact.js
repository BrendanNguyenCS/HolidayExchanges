function ajaxCall(model) {
    var token = $('[name=__RequestVerificationToken]').val();
    $.ajax({
        url: action,
        method: 'POST',
        async: true,
        headers: { '__RequestVerificationToken': token },
        data: { model: model },
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                alert("Thank you for your response. You will be redirected to the main page when you close this window.");
                window.location.href = successRedirect;
            }
        },
        error: function () {
            return false;
        }
    });
}