var dataTable;

$(document).ready(function () {
    loadDataTable();
});


function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": "/admin/webImage/GetAll",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            //właściwe kolumny z jsona
            { "data": "name", "width": "50%" },
            {
                // renderowanie przycisków
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                        <a href="/Admin/webImage/Upsert/${data}" class="btn btn-success text-white" style= "cursor:pointer; width:100px">
                            <i class="far fa-edit"></i> Edit
                        </a>

                        <a class="btn btn-danger text-white" style= "cursor:pointer; width:100px" onclick=Delete("/Admin/webImage/Delete/${data}")>
                            <i class="far fa-trash-alt"></i> Delete
                        </a>
                        </div>
                    `;

                },
                "width": "40%"
            }
        ],
        "language": {
            "emptyTable": "no data found"
        },
        "width": "100%"
    });
}

function Delete(url) {
    swal({
        title: "Are you sure?",
        text: "Once deleted you will not be able to recover.",
        icon: "warning",
        buttons: true,
        dangerMode: true
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    type: "delete",
                    url: url,
                    success: function (data) {   //data argument is returned by API as JSON
                        if (data.success) {
                            toastr.success(data.message);
                            dataTable.ajax.reload();
                        } else {
                            toastr.error(data.message);
                        }
                    }
                });
            }
        });
}