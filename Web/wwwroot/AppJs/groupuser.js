var groupuser = function () {
    return {
        init: function () {
            this.pageIndex = 1;
            this.FirstLoad = true;
            this.Search = false;
        },
        loadData: function (pageindex) {
            var self = this;
            if (this.FirstLoad) {
                $("#loading").show();
            }
            $("#loading").show();
            AjaxService.POST("/Admin/GroupUser/ListData?page=" + pageindex, null, function (res) {
                $("#gridData").html(res.viewContent);
                if (res.totalPages > 1) {
                    $('#paginationholder').html('');
                    $('#paginationholder').html('<ul id="pagination" class="pagination-sm"></ul>');
                    $('#pagination').twbsPagination({
                        startPage: self.pageIndex,
                        totalPages: res.totalPages,
                        visiblePages: 5,
                        onPageClick: function (event, page) {
                            $("#page").val(page);
                            self.pageIndex = page;
                            if (self.Search == true) {
                                $("#btnSearch").click();
                            } else {
                                groupuser.loadData(self.pageIndex);
                            }
                        }
                    });
                }
                self.FirstLoad = false;
                $("#loading").hide();
            });
        },
        onSearchSuccess: function (res) {
            groupuser.Search = true;
            $("#gridData").html(res.viewContent); 
            $("#loading").hide();
        },
        loadfrmAdd: function () {
            modal.Render("/Admin/GroupUser/Add", "Thêm mới nhóm người dùng", "modal-lg");
        },
        onSaveResult: function (res) { 
            if (res.isSuccess == true) {
                alertmsg.success(res.messenger);
                groupuser.loadData(1);
                modal.hide();
            } else {
                alertmsg.error(res.messenger);
            }
            $("#loading").hide();
        },
        loadfrmEdit: function (id) {
            modal.Render("/Admin/GroupUser/Edit/" + id, "Cập nhật nhóm người dùng", "modal-lg");
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
                    AjaxService.POST("/Admin/GroupUser/Delete", { id: id }, function (res) {
                        self.pageIndex = 1;
                        self.loadData(self.pageIndex);
                        alertmsg.success(res.messenger);
                    });
                }
                $("#loading").hide();
            });
        },
        onmultidelete: function () {
            var self = this;
            if ($("table tbody").find("input[type=checkbox]:checked").length == 0) {
                alertmsg.error("Bạn cần chọn ít nhất một nhóm cần xóa");
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
                        AjaxService.POST("/Admin/GroupUser/DeleteAll", { lstid: $("#hdfID").val() }, function (res) {
                            self.pageIndex = 1;
                            self.loadData(self.pageIndex);
                            alertmsg.success(res.messenger);
                        });
                    }
                    $("#loading").hide();
                });
            }
        },
        onloadpermission: function (id, name) {
            modal.Render("/Admin/GroupUser/Permission?id=" + id, "Phân quyền nhóm người dùng: " + name, "modal-lg");
        },
        onupdatepermission: function (isUpdateCatNews) {
            var countchecked = 0;
            var countAllcheck = 0;
            $("#lstAdminmenu tbody tr").each(function () {
                countchecked = 0;
                countAllcheck = 0;
                $(this).find('input[type=checkbox]:checked').each(function () {
                    if ($(this).attr('class') != "chkall") {
                        countchecked += 1;
                    }
                });
                $(this).find('input[type=checkbox]').each(function () {
                    if ($(this).attr('class') != "chkall") {
                        countAllcheck += 1;
                    }
                });
                if (countchecked == countAllcheck) {
                    $(this).find("input[class=chkall]").prop("checked", true);
                } else {
                    $(this).find("input[class=chkall]").prop("checked", false);
                }
            });
            var strAction = "";
            $('input[type=checkbox]').click(function () {
                var tr = $(this).parent().parent().parent();
                if ($(this).attr('class') == "chkall") {
                    if ($(this).is(":checked")) {
                        $(tr).find('input[type=checkbox]').each(function () {
                            $(this).prop("checked", true);
                        });
                    } else {
                        $(tr).find('input[type=checkbox]').each(function () {
                            $(this).prop("checked", false);
                        });
                    }
                }
                countchecked = 0;
                $(tr).find('input[type=checkbox]:checked').each(function () {
                    if ($(this).attr('class') != "chkall") {
                        countchecked += 1;
                    }
                });
                countAllcheck = 0;
                $(tr).find('input[type=checkbox]').each(function () {
                    if ($(this).attr('class') != "chkall") {
                        countAllcheck += 1;
                    }
                });
                if (countchecked == countAllcheck) {
                    $(tr).find("input[class=chkall]").prop("checked", true);
                } else {
                    $(tr).find("input[class=chkall]").prop("checked", false);
                }
                var arrAction = [];
                $(tr).find('input[type=checkbox]:checked').each(function () {
                    if ($(this).attr('class') != "chkall") {
                        arrAction.push($(this).val());
                    }
                });

                var adminmenuid = $(tr).find(".hdfAdminMenu").val();

                if (arrAction.length > 0) {
                    strAction = adminmenuid + ":" + arrAction.join();
                } else {
                    strAction = adminmenuid;
                }

                var groupuserId = $("#groupuserId").val();
                var data = { groupuserId: groupuserId, roles: strAction };
                var urlPost = "/Admin/GroupUser/Permission";
                AjaxService.POST(urlPost, data, function (res) {
                    if (res.isSuccess == true) {
                        alertmsg.success(res.messenger);
                    } else {
                        alertmsg.error(res.messenger);
                    }
                });
            });
        },
        onloaduserindgroup: function (id, name) {
            modal.Render("/Admin/GroupUser/AddUserToGroup?id=" + id, "Thêm người dùng vào nhóm: " + name, "modal-lg");
        }
    };
}();
$(function () { groupuser.init(); });


