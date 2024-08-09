

$(document).ready(function () {
    var projectbills;
    var handlingbills;

    function ShowReport() {
        $("#projectbills").html("");
        var token = getCookie("token"); console.log(token);
        $.ajax({
            url: "https://localhost:7071/api/Billing/report",
            type: "get",
            headers: {
                "Authorization": "Bearer " + "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjExIiwibmJmIjoxNzEwODY4MjkxLCJleHAiOjE3MTE0NzMwOTEsImlhdCI6MTcxMDg2ODI5MX0.UHr19a99X2EvyyvYBIbog4MJtwFtcxrLotEYyhZP5MQ",
            },
            contentType: "application/json",
            success: function (result, status, xhr) {
                $("#totalamount").append(result.totalAmount); 
                $("#amountofmonth").append(result.amountOfMonth); 
            },
            error: function (xhr, status, error) {
                console.log(xhr)
            }
        });

    }



    ShowProjectBills();
    function ShowProjectBills() {
        $("#projectbills").html("");
        var token = getCookie("token"); console.log(token);
        $.ajax({
            url: "https://localhost:7071/api/Billing/projectbills",
            type: "get",
            headers: {
                "Authorization": "Bearer " + "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjExIiwibmJmIjoxNzEwODY4MjkxLCJleHAiOjE3MTE0NzMwOTEsImlhdCI6MTcxMDg2ODI5MX0.UHr19a99X2EvyyvYBIbog4MJtwFtcxrLotEYyhZP5MQ",
            },
            contentType: "application/json",
            success: function (result, status, xhr) {
                projectbills = result;
                $.each(result, function (index, value) {
                    console.log(value)
                    $("#projectbills").append(`
                        <li class="list-group-item border-0 d-flex justify-content-between ps-0 mb-2 border-radius-lg">
                        <div class="d-flex flex-column">
                        <h6 class="mb-1 text-dark font-weight-bold text-sm">${value.title}</h6>
                        <span class="text-xs">${value.type}</span>
                        </div>
                        <div class="d-flex align-items-center text-sm">
                        ${value.total}
                        <a href="javascript:void(0)" data-id="${value.projectId}" class="viewprojectbills">
                            <button class="btn btn-link text-dark text-sm mb-0 px-0 ms-4" data-bs-toggle="modal" data-bs-target="#staticBackdrop">
                                <i class="fas fa-file-pdf text-lg me-1"></i>
                                View
                            </button>
                        </a>
                        
                        </div>
                    </li>
                  `); 
                });
            },
            error: function (xhr, status, error) {
                console.log(xhr)
            }
        });
        setTimeout(showHandlingbills, 500);
        setTimeout(showListOfBills, 1000);
        setTimeout(ShowReport, 200);

    }
   
    function showHandlingbills(){
      $("#handlingbills").html("");
        $.ajax({
            url: "https://localhost:7071/api/Billing/handlingbills",
            type: "get",
            headers: {
                "Authorization": "Bearer " + "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjExIiwibmJmIjoxNzEwODY4MjkxLCJleHAiOjE3MTE0NzMwOTEsImlhdCI6MTcxMDg2ODI5MX0.UHr19a99X2EvyyvYBIbog4MJtwFtcxrLotEYyhZP5MQ",
            },
            contentType: "application/json",
            success: function (result, status, xhr) {
                handlingbills = result;
                $.each(result, function (index, value) {
                  console.log(value)
                  $("#handlingbills").append(`
                      <li class="list-group-item border-0 d-flex p-4 mb-2 bg-gray-100 border-radius-lg">
                      <div class="d-flex flex-column">
                        <h6 class="mb-3 text-sm">${value.orderInfo}</h6>
                        <span class="mb-2 text-xs">Project Title: <span class="text-dark font-weight-bold ms-sm-2"><a href="javascript:void(0)">${value.project.title}</a></span></span>
                        <span class="text-xs">Amount: <span class="text-dark ms-sm-2 font-weight-bold">${value.amount}</span></span>
                      </div>
                      <div class="ms-auto text-end">
                        <a class="btn btn-link text-danger text-gradient px-3 mb-0" href="javascript:;"><i class="far fa-trash-alt me-2"></i>${value.localMessage}</a>
                        <a class="btn btn-link text-dark px-3 mb-0 handling-bill-details" data-id="${value.orderId}" href="javascript:void(0)" data-bs-toggle="modal" data-bs-target="#staticBackdropp">
                            <i class="fas fa-pencil-alt text-dark me-2" aria-hidden="true"></i>
                            View
                        </a>
                      </div>
                    </li>
                  `); 
                });
            },
            error: function (xhr, status, error) {
                console.log(xhr)
            }
        });
    }

    $(document).on('click', '#btnFilderDate', function(){ 
        showListOfBills();
    })

    function showListOfBills(){
        $("#listOfbills").html("");
        var fromDate;
        var todate;
        if( $("#fromDate").val() == false && $("#toDate").val() == false){
            fromDate = new Date(2020, 2, 27, 12, 30);
            todate =  new Date(2024, 2, 27, 12, 30);
        }else{
            fromDate = $("#fromDate").val();
            todate = $("#toDate").val();
        }
        let requestData = {
            fromDate: fromDate,
            todate: todate
        };
        $.ajax({
            url: "https://localhost:7071/api/Billing/GetBillByDate",
            type: "post",
            headers: {
                "Authorization": "Bearer " + "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjExIiwibmJmIjoxNzEwODY4MjkxLCJleHAiOjE3MTE0NzMwOTEsImlhdCI6MTcxMDg2ODI5MX0.UHr19a99X2EvyyvYBIbog4MJtwFtcxrLotEYyhZP5MQ",
            },
            data: JSON.stringify(requestData),
            contentType: "application/json",
            success: function (result, status, xhr) {
                $("#listOfbills").append($("<ul>"));
                $.each(result, function (index, value) {
                    appendElement = $("#listOfbills ul").last();
                    if(value.errorCode === "0" || value.errorCode === "00"){
                        appendElement.append(`
                        <li class="list-group-item border-0 d-flex justify-content-between ps-0 mb-2 border-radius-lg">
                            <div class="d-flex align-items-center">
                            <button class="btn btn-icon-only btn-rounded btn-outline-success mb-0 me-3 btn-sm d-flex align-items-center justify-content-center"><i class="fas fa-arrow-up"></i></button>
                            <div class="d-flex flex-column">
                                <h6 class="mb-1 text-dark text-sm">${value.orderInfo}</h6>
                                <span class="text-xs">${value.dateOfDonation}</span>
                            </div>
                            </div>
                            <div class="d-flex align-items-center text-success text-gradient text-sm font-weight-bold">
                                ${value.amount}
                            </div>
                        </li>
                    `); 
                    }else{
                        appendElement.append(`
                        <li class="list-group-item border-0 d-flex justify-content-between ps-0 mb-2 border-radius-lg">
                            <div class="d-flex align-items-center">
                            <button class="btn btn-icon-only btn-rounded btn-outline-danger mb-0 me-3 btn-sm d-flex align-items-center justify-content-center"><i class="fas fa-exclamation"></i></button>
                            <div class="d-flex flex-column">
                                <h6 class="mb-1 text-dark text-sm">${value.orderInfo}</h6>
                                <span class="text-xs">${value.dateOfDonation}</span>
                            </div>
                            </div>
                            <div class="d-flex align-items-center text-danger text-gradient text-sm font-weight-bold">
                                ${value.amount}
                            </div>
                        </li>
                    `); 
                    }
                    
                });
            },
            error: function (xhr, status, error) {
                console.log(xhr)
            }
        });
    }

    function getCookie(name) {
      // Split cookies by semicolon
      var cookies = document.cookie.split(';');
      
      // Loop through each cookie
      for (var i = 0; i < cookies.length; i++) {
          var cookie = cookies[i];
          // Trim leading and trailing spaces
          cookie = cookie.trim();
          // Check if this cookie is the one we're looking for
          if (cookie.startsWith(name + '=')) {
              // Return the cookie value
              return cookie.substring(name.length + 1);
          }
      }
      // Return null if cookie not found
      return null;
    }

    $("#project-bill-detail").html("");
    $(document).on('click', '.viewprojectbills', function(){
        let id = $(this).attr('data-id');
        $.ajax({
            url: "https://localhost:7071/api/Billing/projectbill/" + id,
            type: "get",
            headers: {
                "Authorization": "Bearer " + "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjExIiwibmJmIjoxNzEwODY4MjkxLCJleHAiOjE3MTE0NzMwOTEsImlhdCI6MTcxMDg2ODI5MX0.UHr19a99X2EvyyvYBIbog4MJtwFtcxrLotEYyhZP5MQ",
            },
            contentType: "application/json",
            success: function (result, status, xhr) {
                $.each(result.orders, function (index, value) {
                    console.log(value)
                    $("#project-bill-detail").append(`
                        <li class="list-group-item border-0 d-flex justify-content-between ps-0 mb-2 border-radius-lg">
                            <div class="d-flex align-items-center">
                            <button class="btn btn-icon-only btn-rounded btn-outline-success mb-0 me-3 btn-sm d-flex align-items-center justify-content-center"><i class="fas fa-arrow-up"></i></button>
                            <div class="d-flex flex-column">
                                <h6 class="mb-1 text-dark text-sm">${value.orderInfo}</h6>
                                <span class="text-xs">${value.dateOfDonation}</span>
                            </div>
                            </div>
                            <div class="d-flex align-items-center text-success text-gradient text-sm font-weight-bold">
                                +${value.amount}
                            </div>
                        </li>
                  `); 
                });
            },
            error: function (xhr, status, error) {
                console.log(xhr)
            }
        });
    });

    $(document).on('click', '.handling-bill-details', function(){
        let id = $(this).attr('data-id');
        var bill = handlingbills.find(obj => obj.orderId == id);
        $("#handling-bill-details").html("");
        $("#handling-bill-details").append(`
            <table class="table">
                <thead>
                <thead> <th></th> <th></th> </thead>
                <tbody>
                <tr> <td>OrderId</td> <td>${bill.orderId}</td> </tr>
                <tr> <td>ProjectId</td> <td>${bill.project.title}</td> </tr>
                <tr> <td>PaymentMethod</td> <td>${bill.paymentMethod}</td> </tr>
                <tr> <td>BankCode</td> <td>${bill.bankCode}</td> </tr>
                <tr> <td>Amount</td> <td>${bill.amount}</td> </tr>
                <tr> <td>OrderInfo</td> <td>${bill.orderInfo}</td> </tr>
                <tr> <td>LocalMessage</td> <td>${bill.localMessage}</td> </tr>
                <tr> <td>DateOfDonation</td> <td>${bill.dateOfDonation}</td> </tr>
                </tbody>
            </table>
          `); 
    });


    $('#inputSearchProject').on('input', function () {
        var searchText = $(this).val().toLowerCase();
        var filteredStudents = projectbills.filter(function (project) {
            return project.title.toLowerCase().indexOf(searchText) !== -1 ||
            project.title.toLowerCase().indexOf(searchText) !== -1;
        });
        // showStudentList(filteredStudents);
        $("#projectbills").html("");
        $.each(filteredStudents, function (index, value) {
            console.log(value)
            $("#projectbills").append(`
                <li class="list-group-item border-0 d-flex justify-content-between ps-0 mb-2 border-radius-lg">
                <div class="d-flex flex-column">
                <h6 class="mb-1 text-dark font-weight-bold text-sm">${value.title}</h6>
                <span class="text-xs">${value.type}</span>
                </div>
                <div class="d-flex align-items-center text-sm">
                ${value.total}
                <a href="javascript:void(0)" data-id="${value.projectId}" class="viewprojectbills">
                    <button class="btn btn-link text-dark text-sm mb-0 px-0 ms-4" data-bs-toggle="modal" data-bs-target="#staticBackdrop">
                        <i class="fas fa-file-pdf text-lg me-1"></i>
                        View
                    </button>
                </a>
                
                </div>
            </li>
          `); 
        });
    });

    
});
//end ready


$(document).on('click', '.viewDetail', function(){
    let id = $(this).attr('data-id');
    $.ajax({
        url: `https://localhost:5000/api/Director/GetDirector/${id}`,
        type: "get",
        // data: JSON.stringify(requestData),
        //processData: false,
        contentType: "application/json",
        success: function (result, status, xhr) {
            $('#table-detail').html('');
            if(result){
                $('#table-detail').html(generateHtmlDirectorDetail(result));
            }
        },
        error: function (xhr, status, error) {
            console.log(xhr)
        }
    });
});



// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



//get By Id
$(document).ready(function () {
    $("#GetButton").click(function (e) {
        $("table tbody").html("");
        $.ajax({
            url: "https://localhost:7029/api/Reservation/" + $("#Id").val(),
            type: "get",
            contentType: "application/json",
            success: function (result, status, xhr) {
                //$("#resultDiv").show();
                //if (typeof result !== 'undefined') {
                //    var str = "<tr><td>" + result["id"] + "</td><td>" + result["name"] + "</td><td>" + result["startLocation"] + "</td><td>" + result["endLocation"] + "</td></tr>";
                //    $("table tbody").append(str);
                //}
                //else $("table tbody").append("<td colspan=\"4\">No Reservation</td>");
                $('#tbody').html('');
                    if (result) {
                        $.each(result, function (index, value) {
                            console.log(value)
                            $("tbody").append($("<tr>"));
                            appendElement = $("tbody tr").last();
                            appendElement.append($("<td>").attr("data-id", value["id"]).attr("class", "viewDetail").html(value["id"])); 
                            appendElement.append($("<td>").html(value["name"]));
                            appendElement.append($("<td>").html(value["description"]));
                            appendElement.append($("<td>").html(value["dob"]));
                            appendElement.append($("<td>").html("<a href=\"UpdateReservation?id=" + value["nationality"] + "\"><img src=\"icon/edit.png\" /></a>"));
                            appendElement.append($("<td>").html("<img class=\"delete\" src=\"icon/close.png\" />"));
                        });
                    }
                    $("#resultDiv").show();
            },
            error: function (xhr, status, error) {
                console.log(xhr)
            }
        });
    });
});


//get all
ShowAllReservation();
function ShowAllReservation() {
    $("table tbody").html("");
    $.ajax({
        url: "https://localhost:7029/api/Reservation",
        type: "get",
        contentType: "application/json",
        success: function (result, status, xhr) {
            $.each(result, function (index, value) {
                console.log(value)
                $("tbody").append($("<tr>"));
                appendElement = $("tbody tr").last();
                appendElement.append($("<td>").attr("id", "UpdateReservation?id=" + value["id"]).html(value["id"]));  
                appendElement.append($("<td>").html(value["name"]));
                appendElement.append($("<td>").html(value["startLocation"]));
                appendElement.append($("<td>").html(value["endLocation"]));
                appendElement.append($("<td>").html("<a href=\"UpdateReservation?id=" + value["id"] + "\"><img src=\"icon/edit.png\" /></a>"));
                appendElement.append($("<td>").html("<img class=\"delete\" src=\"icon/close.png\" />"));
            });
        },
        error: function (xhr, status, error) {
            console.log(xhr)
        }
    });
}

//add
$(document).ready(function () {
    $("#AddButton").click(function (e) {

        //data = new FormData();
        //data.append("Name", $("#Name").val());
        //data.append("StartLocation", $("#StartLocation").val());
        //data.append("EndLocation", $("#EndLocation").val());
        let requestData = {
            Name: $("#Name").val(),
            StartLocation: $("#StartLocation").val(),
            EndLocation: $("#EndLocation").val()
        };
        $.ajax({
            url: "https://localhost:7029/api/Reservation",
            type: "post",
            data: JSON.stringify(requestData),
            //processData: false,
            contentType: "application/json",
            success: function (result, status, xhr) {
                //var str = "<tr><td>" + result["id"] + "</td><td>" + result["name"] + "</td><td>" + result["startLocation"] + "</td><td>" + result["endLocation"] + "</td></tr>";
                //$("table tbody").append(str);
                $("tbody").append($("<tr>"));
                appendElement = $("tbody tr").last();
                appendElement.append($("<td>").attr("id", "UpdateReservation?id=" + value["id"]).html(value["id"]));
                appendElement.append($("<td>").html(value["name"]));
                appendElement.append($("<td>").html(value["startLocation"]));
                appendElement.append($("<td>").html(value["endLocation"]));
                appendElement.append($("<td>").html("<a href=\"UpdateReservation?id=" + value["id"] + "\"><img src=\"icon/edit.png\" /></a>"));
                appendElement.append($("<td>").html("<img class=\"delete\" src=\"icon/close.png\" />"));

                $("#resultDiv").show();
            },
            error: function (xhr, status, error) {
                console.log(xhr)
            }
        });
    });
});

// update
$("#UpdateButton").click(function (e) {
    let params = (new URL(document.location)).searchParams;
    let id = params.get("id");
    data = new FormData();
    data.append("Id", $("#Id").val());
    data.append("Name", $("#Name").val());
    data.append("StartLocation", $("#StartLocation").val());
    data.append("EndLocation", $("#EndLocation").val());
    $.ajax({
        url: "https://localhost:7029/api/Reservation",
        type: "put",
        data: data,
        processData: false,
        contentType: false,
        success: function (result, status, xhr) {
            //var str = "<tr><td>" + result["id"] + "</td><td>" + result["name"] + "</td><td>" + result["startLocation"] + "</td><td>" + result["endLocation"] + "</td></tr>";
            //$("table tbody").append(str);
            //$("#resultDiv").show();
            appendElement = $(this).parents("tr");
            appendElement.append($("<td>").attr("id", "UpdateReservation?id=" + value["id"]).html(value["id"]));
            appendElement.append($("<td>").html(value["name"]));
            appendElement.append($("<td>").html(value["startLocation"]));
            appendElement.append($("<td>").html(value["endLocation"]));
            appendElement.append($("<td>").html("<a href=\"UpdateReservation?id=" + value["id"] + "\"><img src=\"icon/edit.png\" /></a>"));
            appendElement.append($("<td>").html("<img class=\"delete\" src=\"icon/close.png\" />"));
        },
        error: function (xhr, status, error) {
            console.log(xhr)
        }
    });
});


//delete
$("table").on("click", "img.delete", function () {
    var reservationId = $(this).parents("tr").find("td:nth-child(1)").text();
    $.ajax({
        url: "https://localhost:7029/api/Reservation/" + reservationId,
        type: "delete",
        contentType: "application/json",
        success: function (result, status, xhr) {
            ShowAllReservation();
            //$(this).closest("tr").remove();
        },
        error: function (xhr, status, error) {
            console.log(xhr)
        }
    });
});