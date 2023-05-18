$(document).on('paste', '.upload-image', function (e) {
    var items = e.originalEvent.clipboardData.items;
    for (var i = 0; i < items.length; i++) {
        var item = items[i];
        if (item.type.indexOf("image") != -1) {
            var data = new FormData();
            data.append('Images_Bin', item.getAsFile());
            requestFile('/images/create', 'post', data);
        }
    }
});

function requestFile(requestUrl, methodType, data) {
    return $.ajax({
        url: requestUrl,
        type: methodType,
        cache: false,
        contentType: false,
        processData: false,
        data: data,
        success: function (o) {
            $('#results').prepend('<img src="/images/show/' + o + '">');
            return true;
        },
        error: function (xhr, status, err) {
            return false;
        }
    });
};