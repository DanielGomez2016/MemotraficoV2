var getDateFromNet = function (dateTime) {
    var date = new Date(parseInt(dateTime.substr(6)));
    var day = ("0" + date.getDate()).slice(-2);
    var month = ("0" + (date.getMonth() + 1)).slice(-2);
    var year = date.getFullYear();
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var seconds = ("0" + date.getSeconds()).slice(-2);
    var miliseconds = date.getMilliseconds();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = ("0" + (hours ? hours : 12)).slice(-2);
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ":" + seconds + " " + ampm;
    var formated = year + "/" + month + "/" + day + " " + strTime;
    return formated;
}


var loadScript = function (filename, filetype) {
    if (filetype == "js") { //if filename is a external JavaScript file
        var fileref = document.createElement('script')
        fileref.setAttribute("type", "text/javascript")
        fileref.setAttribute("src", filename)
    }
    else if (filetype == "css") { //if filename is an external CSS file
        var fileref = document.createElement("link")
        fileref.setAttribute("rel", "stylesheet")
        fileref.setAttribute("type", "text/css")
        fileref.setAttribute("href", filename)
    }
    if (typeof fileref != "undefined")
        document.getElementsByTagName("head")[0].appendChild(fileref)
}

var bindAutcomplete = function (element, url, callback) {
    loadScript("/Scripts/jquery-ui-1.8.24.min.js", "js");
    loadScript("/Content/themes/base/jquery.ui.autocomplete.css", "css");
    loadScript("/Content/themes/base/css", "css");

    // Tratar de enlazar el evento hasta que ya este cargado el script
    var detectAutocompelte = setInterval(function () {

        try {

            $(element).autocomplete({
                source: url,
                select: callback
            });

            clearInterval(detectAutocompelte);

        }
        catch (e) { }

    }, 1000);


}

var paginate = function (table, pager, nPP) {
    var currentPage = 0;
    var numPerPage = nPP;
    var $table = $(table);
    var $pager = $(pager);
    var numRows = $table.find('tbody tr').length;
    var numPages = Math.ceil(numRows / numPerPage);

    $pager.find('li').remove();

    $table.bind('repaginate', function () {
        $table.find('tbody tr').hide().slice(currentPage * numPerPage, (currentPage + 1) * numPerPage).show();
    });

    $table.trigger('repaginate');

    $('<li><a href="#">&laquo;</a></li>').bind('click', function (event) {
        currentPage = hasPrevious(currentPage);
        $table.trigger('repaginate');
        $pager.find('li.page-number').siblings().removeClass('active');
        $pager.find('li#p' + (currentPage + 1)).addClass('active');
    }).appendTo($pager);

    for (var page = 0; page < numPages; page++) {
        $('<li id="p' + (page + 1) + '" class="page-number"><a href="#">' + (page + 1) + '</a></li>').bind('click', { newPage: page }, function (event) {
            currentPage = event.data['newPage'];
            $table.trigger('repaginate');
            $(this).addClass('active').siblings().removeClass('active');
        }).appendTo($pager);
    }

    $('<li><a href="#">&raquo;</a></li>').bind('click', function (event) {
        currentPage = hasNext(currentPage, numPages);
        $table.trigger('repaginate');
        $pager.find('li.page-number').siblings().removeClass('active');
        $pager.find('li#p' + (currentPage + 1)).addClass('active');
    }).appendTo($pager);

    $pager.find('li.page-number:first').addClass('active');
}

var hasNext = function (currentPage, numPages) {
    if (currentPage + 1 < numPages)
        return currentPage + 1;
    else
        return currentPage;
}

var hasPrevious = function (currentPage) {
    if (currentPage - 1 >= 0)
        return currentPage - 1;
    else
        return currentPage;
}

$(function () {
    $(document.body).delegate('select[readonly]', 'change', function (e) {
        $(this).val($(this).attr('data-default'));
        return false;
    });

    $(document.body).delegate('[data-captura]', 'keypress', function (e) {
        var tipo = this.getAttribute('data-captura').toLowerCase();

        switch (tipo) {
            case 'enteros':
                return /\d/.test(String.fromCharCode(e.keyCode));
                break;
            case 'decimales':
                return /\d|\./.test(String.fromCharCode(e.keyCode)) && /^\d+(\.\d*)?$/.test(this.value + String.fromCharCode(e.keyCode));
                break;
        }
    });
});