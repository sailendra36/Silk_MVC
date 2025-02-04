var dataTable;
$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else if (url.includes("completed")) {
        loadDataTable("completed");
    }
    else if (url.includes("pending")) {
        loadDataTable("pending");
    }
    else if (url.includes("approved")) {
        loadDataTable("approved");
    }
    else {
        loadDataTable("all");
    }
    
});

function loadDataTable(status) {
        dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/order/getall?status=' + status,
            //dataSrc: function (json) {
            //    console.log(json); // Log the response to inspect it
            //    return json.data; // Ensure this matches your JSON structure
                //dataSrc: 'objOrderHeader'
        },
        "columns": [
            { data: 'id', width: "5%" },
            { data: 'name', width: "20%" },
            { data: 'phoneNumber', width: "20%" },
            { data: 'applicationUser.email', width: "20%" },
            { data: 'orderStatus', width: "10%" },
            { data: 'orderTotal', width: "10%" },
            {
                data: 'id',
                render: function (data) {
                    return `<div class="w-75 btn-group" role = "group">
                    <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Details </a>
                    </div>`
                },
                width: "10%"
            }
        ]
    });
}

