var chkPerms = $(".perm");
for (var i = 0; i < chkPerms.length; i++) {
    chkPerms[i].setAttribute('disabled', 'disabled');
}


$('#Target,#DbObject').on('change', () => {
    target = $('#Target :selected').val();
    dbObject = $('#DbObject :selected').val();
    listPerm = "";
    listStates = "";
    console.log('Run load: ' + target + ' + ' + dbObject);
    $.ajax({
        url: "/Admin/CP_V2/GetPermsOfObject",
        async: false,
        type: "GET",
        data: { target: target, dbObject: dbObject },
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            //do your thing on success
            listPerm = data;
        }
    });
    $.ajax({
        url: "/Admin/CP_V2/GetPermStatesOfObject",
        async: false,
        type: "GET",
        data: { target: target, dbObject: dbObject },
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            //do your thing on success
            listStates = data;
        }
    });
    GetTypeOfObject(dbObject);
    var listPermState = listPerm.map(function (x, i) {
        return { "name": x, "state": listStates[i] }
    }.bind(this));

    listPermState = listPermState.filter(function (item) {
        return item.name !== 'REFERENCES'
    })
    var enableList = $('.container-checkbox').find('.cbo-perm:visible');
    chkPerms.removeAttr('disabled');
    if (listPermState == '') {
        for (var i = 0; i < enableList.length; i++) {
            enableList[i].checked = false;
        }
    }
    else {
        //console.log(target + ' - ' + dbObject);
        //console.log(listPermState);
        //console.log(enableList);
        for (var i = 0; i < listPermState.length; i++) {
            //console.log(i + ' : ' + listPermState[i].name + ' - ' + listPermState[i].state + ' - ' + (listPermState[i]?.state == "GRANT" || listPermState[i]?.state == 'GRANT_WITH_GRANT_OPTION'));
            //console.log($('.perm[name=' + listPermState[i].name + ']')[0]);

            //console.log('Check' + ':' + (listPermState[i] == enableList.find('.perm')[i].name));
            //console.log(enableList.find('.perm').html());

            if ($('.perm[name=' + listPermState[i].name + ']')[0] != null) {
                var check = 0;
                for (var j = 0; j < enableList.length; j++) {
                    if (listPermState[i].name == enableList[j].children[1].getAttribute('name')) {
                        check = 1;
                        /*console.log('ok');*/
                    }
                }
                if (check == 1) {
                    /*console.log(listPermState[i].name + ' | ' + enableList.find('input[name*=' + listPermState[i].name + ']').childNodes[1].textContent  + ' | ' + enableList[i].children['' + listPermState[i].name + '']);*/
                    /*$('[name=' + listPermState[i].name + ']').checked = true;*/
                    /*console.log('P1 ' + listPermState[i].state);*/
                    if (listPermState[i].state == "GRANT" || listPermState[i].state == "GRANT_WITH_GRANT_OPTION") {
                        $('.perm[name=' + listPermState[i].name + ']')[0].checked = true;
                        $('.perm[name=' + listPermState[i].name + ']')[0].removeAttribute('disabled');
                        /*console.log('P2');*/

                    }
                    else {
                        $('.perm[name=' + listPermState[i].name + ']')[0].checked = false;
                        $('.perm[name=' + listPermState[i].name + ']')[0].removeAttribute('disabled');
                    }
                }
                else {
                    $('.perm[name=' + listPermState[i].name + ']')[0].checked = false;
                    $('.perm[name=' + listPermState[i].name + ']')[0].removeAttribute('disabled');
                }
            }
        }
    }
      
})
function CheckPerm(perm) {

}
function GetTypeOfObject(dbObject) {
    typeOfObj = "";
    $.ajax({
        url: "/Admin/CP_V2/GetTypeOfObject",
        async: false,
        type: "GET",
        data: { dbObject: dbObject },
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            //do your thing on success
            typeOfObj = data;
            console.log(data);
        }
    });
    $('.cbo-perm').hide();
    target = $('#Target :selected').val();
    dbObject = $('#DbObject :selected').val();
    if (target != '' && dbObject != '') {
        if (typeOfObj == 1 || typeOfObj == 3) {
            $('#Execute').show();
        }
        else {
            $('#Select').show();
            $('#Insert').show();
            $('#Update').show();
            $('#Delete').show();
        }
    }   
}
var SubmitConfrim = () => {
    target = $('#Target :selected').val();
    dbObject = $('#DbObject :selected').val();
    var form = $("#Form");
    var formCollection = form.serialize();
    
    if (target == '' || dbObject == '') {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi!',
            text: 'Vui lòng chọn đầy đủ thông tin!',
            showConfirmButton: true,
            timer: 2000
        })
    }
    else {
        Swal.fire({
            icon: 'warning',
            title: 'Cảnh Báo!',
            html: '<div>Bạn có chắc muốn thực hiện hành động này?</div>',
            showCancelButton: true,
            confirmButtonText: 'Chắc chắn',
            cancelButtonText: 'Hủy'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    type: "POST",
                    url: '/Admin/CP_V2/ManagePermissions',
                    data: formCollection,
                    success: function (r) {
                        if (r == 'ok') {
                            Swal.fire({
                                icon: 'success',
                                title: 'Thành Công',
                                text: 'Đã thay đổi quyền cho ' + target + ' đối với quyền ' + dbObject + '!',
                                showConfirmButton: true,
                                timer: 2000
                            }).then((result) => {
                                location.reload();
                            });
                        }
                        else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Thất Bại',
                                html: '<span style="font-weight: bold">Có lỗi đã xảy ra, vui lòng thử lại! <br/>Chi tiết: </span><span style="color: red;">' + r + '</span>',
                                showConfirmButton: true,
                                timer: 3500
                            })
                        }
                    }
                });
            }
        })
    }
};