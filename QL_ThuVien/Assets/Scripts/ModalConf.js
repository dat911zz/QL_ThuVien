var DVNConfirm = (functionName, objectName, url, id) => {
    Swal.fire({
        icon: 'warning',
        title: 'Cảnh Báo!',
        html: '<div>Bạn có chắc muốn thực hiện hành động này?</div>'
            + '<div>Chi tiết: <span style="font-weight: bold">' + functionName.toLowerCase() + ' ' + objectName.toLowerCase() + ' có mã là <span style="color: red">' + id + '</span></span></div>',
        showCancelButton: true,
        confirmButtonText: functionName,
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: url,
                data: { id: id },
                success: function (r) {
                    if (r == 'ok') {
                        Swal.fire({
                            icon: 'success',
                            title: 'Thành Công',
                            text: 'Đã ' + functionName.toLowerCase() + ' ' + objectName.toLowerCase() + ' có mã là ' + id + ' !',
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