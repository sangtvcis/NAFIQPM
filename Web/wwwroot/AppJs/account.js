var account = function () {
    return {
        init: function () {
            this.pageIndex = 1;
            this.FirstLoad = true;
            this.DataSearch = {};
        },
        loadData: function (pageIndex) {
            var self = this;
            if (this.FirstLoad) {
                $("#loading").show();
            }
            let pageSize = 20;
            let keySearch = '';
            $.get("/Admin/Account/ListData", { pageIndex: pageIndex, pageSize: pageSize, keySearch: keySearch }, function (res) {
                if (res.totalPages > 0)
                    $("#gridData").html(res.viewContent);
                else
                    $("#gridData").html('');
                $("#loading").hide();
                if (res.totalPages > 1) {
                    $('#paginationholder').html('<ul id="pagination" class="pagination-sm"></ul>');
                    $('#pagination').twbsPagination({
                        startPage: pageIndex,
                        totalPages: res.totalPages,
                        visiblePages: 5,
                        onPageClick: function (event, page) {
                            pageIndex = page;
                            account.loadData(self.pageIndex);
                        }
                    });
                } else {
                    $('#paginationholder').html('');
                }
            });
        },
        onSearchSuccess: function (res) {
            $('#paginationholder').html('');
            $('#paginationholder').html('<ul id="pagination" class="pagination-sm"></ul>');
            $("#gridData").html(res.viewContent);
            account.DataSearch = {
                Name: $("#Name").val(),
            };
            if (res.totalPages > 1) {
                $('#pagination').twbsPagination({
                    startPage: account.pageIndex,
                    totalPages: res.totalPages,
                    visiblePages: 5,
                    onPageClick: function (event, page) {
                        account.pageIndex = page;
                        account.loadData(page);
                    }
                });
            }
            self.FirstLoad = false;
            $("#loading").hide();
        },
        active: function (id) {
            $("#loading").show();
            var self = this;
            swal({
                title: "Thay đổi trạng thái?",
                text: "",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Có",
                cancelButtonText: "không",
            }, function (isConfirm) {
                if (isConfirm) {
                    AjaxService.POST("/Admin/Account/ChangeStatus", { id: id }, function (res) {
                        self.pageIndex = 1;
                        self.loadData(self.pageIndex);
                        alertmsg.success(res.Messenger);
                    });
                }
                $("#loading").hide();
            });
        },
        loadfrmAdd: function () {
            modal.Render("/Admin/Account/Add", "Thêm mới người dùng", "modal-lg");
        }, 
        onSaveResult: function (res) {
            if (res.isSuccess == true) {
                alertmsg.success(res.messenger);
                account.loadData(1);
                modal.hide();
            } else {
                alertmsg.error(res.messenger);
            }
            $("#loading").hide();
        },
        loadfrmEdit: function (id) {
            modal.Render("/Admin/Account/Edit/" + id, "Cập nhật người dùng", "modal-lg");
        }, 
        loadfrmReset: function(id) {
            modal.Render("/Admin/Account/ResetPassword/" + id, "Reset mật khẩu", "modal-md");
        },
        onResetSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                account.loadData(this.pageIndex);
                modal.hide();
            } else {
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        },
        loadfrmChangePass: function (id) {
            modal.Render("/Admin/Account/ChangePass/" + id, "Đổi mật khẩu", "modal-md");
        },
        onChangePassSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                account.loadData(this.pageIndex);
                modal.hide();
            } else {
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        },
        ondelete: function (id) {
            $("#loading").show();
            var self = this;
            swal({
                title: "Bạn có chắc chắn không?",
                text: "",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Có",
                cancelButtonText: "không",
            }, function (isConfirm) {
                if (isConfirm) {
                    AjaxService.POST("/Admin/Account/Delete", { id: id }, function (res) {
                        self.pageIndex = 1;
                        self.loadData(self.pageIndex);
                        alertmsg.success(res.Messenger);
                    });
                }
                $("#loading").hide();
            });
        },
        onmultidelete: function () {
            var self = this;
            if ($("table tbody").find("input[type=checkbox]:checked").length == 0) {
                alertmsg.error("Bạn cần chọn ít nhất một người dùng cần xóa");
            } else {
                $("#loading").show();
                swal({
                    title: "Bạn có chắc chắn không?",
                    text: "",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "Có",
                    cancelButtonText: "không",
                }, function (isConfirm) {
                    if (isConfirm) {
                        AjaxService.POST("/Admin/Account/DeleteAll", { lstid: $("#hdfID").val() }, function (res) {
                            self.pageIndex = 1;
                            self.loadData(self.pageIndex);
                            alertmsg.success(res.Messenger);
                        });
                    }
                    $("#loading").hide();
                });
            }
        },
        initcheckall: function () {
            var countall = $(".chkelement").length;
            var countchecked = 0;
            $(".chkelement").each(function () {
                if ($(this).find($("input[type=checkbox]")).is(':checked')) {
                    countchecked++;
                }
            });
            if (countall == countchecked) {
                $("#basicForm #chkall").prop('checked', true);
            } else {
                $("#basicForm #chkall").prop('checked', false);
            }
            $("#basicForm #chkall").click(function () {
                if ($(this).is(':checked')) {
                    $(".chkelement").each(function () {
                        $(this).find($("input[type=checkbox]")).prop('checked', true);
                    });
                } else {
                    $(".chkelement").each(function () {
                        $(this).find($("input[type=checkbox]")).prop('checked', false);
                    });
                }
            });
            $(".chkelement").click(function () {
                countchecked = 0;
                $(".chkelement").each(function () {
                    if ($(this).find($("input[type=checkbox]")).is(':checked')) {
                        countchecked++;
                    }
                });
                if (countall == countchecked) {
                    $("#basicForm #chkall").prop('checked', true);
                } else {
                    $("#basicForm #chkall").prop('checked', false);
                }
            });
        }
    };
}();
$(function () { account.init(); });


