var imginfo = "null";
$(function () {
    $("#btnSave").click(function () {
        var eid = $.trim($("#expid").val()); var name = $.trim($("#expname ").val());
        var gender = $("#expgender").val(); var catelogs = GetCategories();
        var birthdate = $("#expbirthdate").val(); var idnumber = $.trim($("#expidentitynumber").val());
        var adacmictitles = $.trim($("#exptechtitles").val()); var eunitproperty = $("#unitproperty").val();
        var unitname = $.trim($("#expunitname").val()); var department = $.trim($("#expdepartment").val());
        var position = $.trim($("#expposition").val()); var telephone = $.trim($("#exptelephone").val());
        var cellphone = $.trim($("#expcellphone").val()); var email = $.trim($("#expemail").val());
        var mailaddress = $.trim($("#expmailaddress ").val()); var fields = $.trim($("#expfileds").val());
        var personalurl = $.trim($("#exppersonalpage").val()); var biref = $.trim($("#expbrief").val());
        var experience = $.trim($("#expexperience").val()); var achievement = $.trim($("#expachievement").val());
        var bestatus = $("#expstatus").val(); var esource = $.trim($("#expsource ").val()); var remark = $.trim($("#expremark ").val());
        if (EmptyCheck(eid) || eid.length != 17) {
            alert("专家编号为空或格式不正确！");
        } else if (EmptyCheck(name)) {
            alert("专家姓名不能为空！");
        } else if (EmptyCheck(catelogs) || catelogs.length == 0) {
            alert("业务分类不能为空！");
        } else if (EmptyCheck(telephone)) {
            alert("办公电话不能为空！");
        } else if (!TelephoneCheck(telephone)) {
            alert("办公电话格式输入不正确！");
        } else if (EmptyCheck(cellphone)) {
            alert("手机号不能为空！");
        } else if (!CellphoneCheck(cellphone)) {
            alert("手机号码格式不正确！");
        } else if (EmptyCheck(email)) {
            alert("邮箱为不能为空！");
        } else if (!EmailCheck(email)) {
            alert("邮箱地址格式不正确！");
        } else if (EmptyCheck(mailaddress)) {
            alert("通信地址不能为空！");
        } else if (EmptyCheck(fields)) {
            alert("从事领域不能为空！");
        } else if (biref.length > 1000) {
            alert("专家简介字数超过最大限度！");
        } else if (experience.length > 1000) {
            alert("专家工作经历字数超过最大限度！");
        } else if (achievement.length > 1000) {
            alert("专家业绩字数超过最大限度！");
        }
        else {
            $.ajax({
                type: "POST",
                url: "/ExpertsManage/SaveExpert?t=new Date().getTime()",
                data: {
                    "eId": eid,
                    "eName": name,
                    "gender": gender,
                    "imgUrl": imginfo,
                    "birthDay": birthdate,
                    "identityNumber": idnumber,
                    "unitProperty": eunitproperty,
                    "unitOne": unitname + " " + department + " " + position,
                    "unitTwo": $.trim($("#expunitnameone")) + " " + $.trim($("#expunitdepartone")) + " " + $.trim($("#expunitpositionone")),
                    "unitThree": $.trim($("#expunitnametwo")) + " " + $.trim($("#expunitdeparttwo")) + " " + $.trim($("#expunitpositiontwo")),
                    "academicTitles": adacmictitles,
                    "field": fields,
                    "email": email,
                    "officePhone": telephone,
                    "cellPhone": cellphone,
                    "postalAddress": mailaddress,
                    "expertSources": esource,
                    "beStatus": bestatus,
                    "personalUrl": personalurl,
                    "Categories": catelogs,
                    "expertIntroduction": biref,
                    "expertworkingExperience": experience,
                    "expertAchievement": achievement,
                    "remark": remark
                },
                dataType: 'json',
                success: function (data) {
                    if (data.status == 'ok') {
                        alert(data.msg);
                        window.location = "/ExpertsManage/Index";
                    }
                    else {
                        alert(data.msg);
                    }
                },
                error: function (xhr) {
                    alert("保存失败，请稍后重试！当前请求的状态为:" + xhr.readyState);
                }
            });
        }
    });
    $("#expbrief").bind("input propertychange", function () {
        $("#brf_count").text(this.value.length);
    });
    $("#expexperience").bind("input propertychange", function () {
        $("#exp_count").text(this.value.length);
    });
    $("#expachievement").bind("input propertychange", function () {
        $("#achi_count").text(this.value.length);
    });
});

KindEditor.ready(function (K) {
    var uploadbutton = K.uploadbutton({
        button: K("#expimginfo")[0],
        fieldName: "expertFile",
        url: "../Scripts/kindeditor/upload/upload_json.ashx?dir=image&type=Icons",
        afterUpload: function (data) {
            if (data.error == 0) {
                var url = K.formatUrl(data.url, "absolute");
                $("#inputimgurl").val(url);
                $("#imgpxinfo").text(data.pixel);
                imginfo = url;
            } else { alert(data.message); }
        }, afterError: function (str) {
            alert("上传过程中出现异常，错误信息：" + str);
        }
    });
    uploadbutton.fileBox.change(function (e) {
        uploadbutton.submit();
    });
});

function GetCategories() {
    var categories = "";
    var cks = $('input:checkbox[name="category"]:checked');
    for (var i = 0; i < cks.length; i++) {
        if (cks[i].checked) {
            categories += cks[i].value + ";";
        }
    }
    return categories;
}

function EmailCheck(data) {
    var EmailReg = /^[a-zA-Z0-9_.-]+@[a-zA-Z0-9]+(\.[a-zA-Z0-9]+)*\.[a-zA-Z0-9]{2,4}$/;
    if (EmailReg.test(data)) {
        return true;
    } else {
        return false;
    }
}

function CellphoneCheck(data) {
    var CellphoneReg = /^1\d{10}$/;
    return CellphoneReg.test(data);
}

function TelephoneCheck(data) {
    var TelephoneReg = /^0\d{2,3}-?\d{7,8}$/;
    if (TelephoneReg.test(data)) {
        return true;
    } else {
        return false;
    }
}

function EmptyCheck(data) {
    if (data == null || data.length == 0) {
        return true;
    } else {
        return false;
    }
}