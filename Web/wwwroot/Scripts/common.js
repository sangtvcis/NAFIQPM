$(".select2").select2().change(function () {
    $(this).valid();
});
$(document).ready(function() {
    $(".ckeditor_vx").each(function() {
        var Field = $(this).attr("name");
        CKEDITOR.replace(Field, {
            language: 'vi',
            filebrowserBrowseUrl: "/Content/Admin/assets/ckfinder/ckfinder.html",
            filebrowserImageBrowseUrl: '/Content/Admin/assets/ckfinder/ckfinder.html?Type=Images',
            filebrowserFlashBrowseUrl: '/Content/Admin/assets/ckfinder/ckfinder.html?Type=Flash',
            height: 350
        });
    });
});
//URL
var URL = {};
URL.addQueryString = function (qry) {
    var curentPage = window.location.href.split('?')[0] + "?" + qry;
    window.history.pushState(null, null, curentPage);
}
URL.getParameterByName = function (name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}
URL.encodeUrl = function (uri) {
    return encodeURIComponent(uri);
}
URL.decodeUrl = function (uri) {
    return decodeURIComponent(uri);
}
URL.getdomain = function () {
    return location.hostname;
}
var redirectToSubdomain = function (subdomain) {
    var url = window.location.origin;
    var domain = url.split('//')[1];
    window.location.href = "http://" + subdomain + "." + domain;
}
//Datetime
var ConvertDateTime = {};
ConvertDateTime.MDYToDMY = function (dt) {
    var arr = dt.split('T')[0].split('-');
    return arr[2] + "-" + arr[1] + "-" + arr[0];
}
ConvertDateTime.VN = function () {
    var d = new Date();
    var currDate = d.getDate();
    var currMonth = d.getMonth() + 1;
    var currYear = d.getFullYear();
    var str = "Hà nội, Ngày " + currDate + " Tháng " + currMonth + " Năm " + currYear;
    return str;
}

InitDatePicker = function () {
    $.get("/Base/GetFormatDate", function (res) {
        if (res == "M/d/yyyy") {
            res = "mm/dd/yyyy";
        }
        if (res == 'd/M/yyyy') {
            res = "dd/mm/yyyy";
        }
        $('.default-date-picker').datepicker({
            format: res,
            autoclose: true
        });
    });
}
var File = {};
File.GetExtension = function (filename) {
    var fileExt = filename.split('.').pop();
    return fileExt;
}
//localstorage
var LocalStorage = {};
LocalStorage.insertLocalStorage = function (key, value) {
    localStorage.setItem(key, value);
}
LocalStorage.getLocalStorage = function (key) { return localStorage.getItem(key) };

LocalStorage.deleteLocalStorage = function () {
    localStorage.clear();
}

//IsCheck
var isCheck = function (source, target) {
    if ((parseInt(source) & parseInt(target)) == parseInt(target)) {
        return true;
    }
    else {
        return false;
    }
}

//checkbox click checkall
var InitClickCheckAllInTable = function (btnCheckAll, fn) {
    var arrValue = [];
    $("table tbody").find("input[type=checkbox]").click(function () {
        arrValue = [];
        if ($("table tbody").find("input[type=checkbox]").length == $("table tbody").find("input[type=checkbox]:checked").length) {
            $("#" + btnCheckAll).prop("checked", true);
        } else {
            $("#" + btnCheckAll).prop("checked", false);
        }
        $("table tbody").find("input[type=checkbox]:checked").each(function () {
            arrValue.push($(this).val());
        });
        fn(arrValue);
    });
    $("#" + btnCheckAll).click(function () {
        arrValue = [];
        if ($(this).is(':checked')) {
            $("table tbody").find("input[type=checkbox]").each(function () {
                $(this).prop("checked", true);
                arrValue.push($(this).val());
            });
        }
        else {
            $("table tbody").find("input[type=checkbox]").each(function () {
                $(this).prop("checked", false);
            });
        }
        fn(arrValue);
    });
}
//Show Modal
var modal = {};
modal.show = function (url) {
    $.get(url, function (result) {
        $("#mymodal").html(result);
        $('#mymodal').modal('show');
    });
}
modal.hide = function () {
    $('#mymodal').modal('hide');
    $("#mymodal").html('');
}
modal.Render = function (url, title, classSize) {
    if (classSize == null || classSize == undefined) {
        classSize = "";
    }
    $("#loading").show();
    $.get(url, function (result) {
        var strHtml = "";
        strHtml += "<div class='modal-dialog " + classSize + "'>" +
            "<div class='modal-content p-0 b-0'>" +
            "<div class='panel panel-black panel-default'>" +
            "<div class='panel-heading'> " +
            "<button type='button' class='close' onclick='modal.hide()' aria-hidden='true'>×</button> " +
            "<h3 class='panel-title'>" + title + "</h3> " +
            "</div> " +
            "<div class='panel-body'> " +
            result +
            "</div> " +
            "</div>" +
            "</div>" +
            "</div>";
        $("#mymodal").html(strHtml);
        $("#loading").hide();
        $('#mymodal').modal({
            backdrop: 'static',
            keyboard: false
        });
    });
}
//Alert Messenger
var alertmsg = {};
alertmsg.error = function (content) {
    $.Notification.notify('error', 'top right', 'Thông báo!', content);
    setTimeout(function () {
        $(".notifyjs-wrapper").hide();
    }, 3000);
    return false;
}
alertmsg.success = function (content) {
    $.Notification.notify('success', 'top right', 'Thông báo!', content);
    setTimeout(function () {
        $(".notifyjs-wrapper").fadeOut();
    }, 3000);
    return true;
}
//Upload File 
$.fn.uploadajax = function (iconloading, fn) {
    //Upload albumn Images
    $(this).on('change', function () {
        $(iconloading).show();
        // Checking whether FormData is available in browser  
        if (window.FormData !== undefined) {
            var fileUpload = $(this).get(0);
            var files = fileUpload.files;
            // Create FormData object  
            var fileData = new FormData();
            // Looping over all files and add it to FormData object  
            for (var i = 0; i < files.length; i++) {
                fileData.append(files[i].name, files[i]);
            }
            // Adding one more key to FormData object  
            fileData.append('username', 'Manas');
            $.ajax({
                url: '/Upload/UploadFiles',
                type: "POST",
                contentType: false, // Not to set any content header  
                processData: false, // Not to process data  
                data: fileData,
                success: function (result) {
                    $(this).val('');
                    fn(result);
                },
                error: function (err) {
                    alert(err.statusText);
                }
            });
        } else {
            alert("FormData is not supported.");
        }
    });
};
//Upload Video 
$.fn.uploadvideo = function (iconloading, fn) {
    //Upload albumn Images
    $(this).on('change', function () {
        $(iconloading).show();
        // Checking whether FormData is available in browser  
        if (window.FormData !== undefined) {
            var fileUpload = $(this).get(0);
            var files = fileUpload.files;
            // Create FormData object  
            var fileData = new FormData();
            // Looping over all files and add it to FormData object  
            for (var i = 0; i < files.length; i++) {
                fileData.append(files[i].name, files[i]);
            }
            $.ajax({
                url: '/Video/Upload',
                type: "POST",
                contentType: false, // Not to set any content header  
                processData: false, // Not to process data  
                data: fileData,
                success: function (result) {
                    $(this).val('');
                    fn(result);
                },
                error: function (err) {
                    alert(err.statusText);
                }
            });
        } else {
            alert("FormData is not supported.");
        }
    });
};
//AddCookies from javascript pass to razor
var AddCookies = function (key, value, href) {
    var data = { key: key, value: value };
    AjaxService.GET("/Base/AddCookies", data, function (res) {
        window.location.href = href;
    });
}
//Chuyển đổi ngôn ngữ - languages
var swaplang = function (langcode) {
    var data = { langcode: langcode };
    AjaxService.GET("/Base/SwapLanguages", data, function (res) {
        location.href = '/';
    });
}
//Loại bỏ tiếng việt có dấu sang không dấu
var UnsignCharacter = function (str) {
    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    return str;
}
//Youtube
var renderIframeYoutube = function (url, width, height) {
    var id = url.split('v=')[1];
    var newurl = 'https://www.youtube.com/embed/' + id;
    return '<iframe width="' + width + '" height="' + height + '" src="' + newurl + '" frameborder="0" allowfullscreen></iframe>';
}
// datepicker + maskinput
$(document).ready(function () {
    $.fn.datepicker.defaults.language = 'vi';
    $('.datepicker-mask').datepicker({
        format: 'dd/mm/yyyy',
        autoclose: true,
        clearBtn: true
    });
    $('.datepicker-mask2').datepicker({
        format: 'dd/mm/yyyy',
        autoclose: true,
        clearBtn: true
    });
    $('.checkdate').change(function () {
        debugger;
        var DenN = $('#DenNgay').val();
        var DenNgay = parseDate(DenN).getTime();
        var TuN = $('#TuNgay').val();
        var TuNgay = parseDate(TuN).getTime();
        $("#show_errordate").html("");
        if (DenNgay < TuNgay) {
            $("#show_errordate").html("Ngày bắt đầu không lớn hơn ngày kết thúc");
            return false;
        }
    });
});
function parseDate(str) {
    var mdy = str.split('/');
    return new Date(mdy[2], mdy[1], mdy[0]);
}