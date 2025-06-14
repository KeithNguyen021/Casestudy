$(() => {
    //Display current User
    //Retrieve User
    let currentUser = JSON.parse(sessionStorage.getItem("currentUser"));
    let roleName = sessionStorage.getItem("roleName");

    //Display user info
    let userName = `${currentUser.firstname} ${currentUser.lastname}`;
    $("#sidebarName").html(userName);


    let userRole = `${roleName}`;
    $("#sidebarRole").html(userRole);



    let userEmail = `${currentUser.email} `;
    $("#sidebarEmail").html(userEmail);

    //Access control
    //Access control
    if (roleName === "Admin") {
        $("#addEmployeeButton").hide();
    }
    else if (roleName === "Technician") {
        $("#managementNav").hide();
    }
    else {
        $("#managementNav, #customerNav").hide();
    }

    //Log out
    $("#signOutButton").click(() => {
        sessionStorage.removeItem("currentUser"); // Delete current user from sessionStorage
        window.location.href = "userlogin.html"; // Redirect to login page
    });


    const getAll = async (msg) => {
        try {
            // get call data
            let response = await fetch(`api/call`);
            if (response.ok) {
                let callload = await response.json();
                sessionStorage.setItem("allcalls", JSON.stringify(callload));
                msg === "" ? $("#status").text("Calls Loaded") : $("#status").text(`${msg} - Calls Loaded`);
            } else if (response.status !== 404) {
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else {
                $("#status").text("no such path on server");
            }

            // get problem data
            response = await fetch(`api/problem`);
            if (response.ok) {
                let probs = await response.json(); // this returns a promise, so we await it
                sessionStorage.setItem("allproblems", JSON.stringify(probs));
            } else if (response.status !== 404) { // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else { // else 404 not found
                $("#status").text("no such path on server");
            } // else

            // get division data
            response = await fetch(`api/customer`);
            if (response.ok) {
                let cus = await response.json(); // this returns a promise, so we await it
                sessionStorage.setItem("allcustomers", JSON.stringify(cus));
            } else if (response.status !== 404) { // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else { // else 404 not found
                $("#status").text("no such path on server");
            } // else

             response = await fetch(`api/employee`);
            if (response.ok) {
                payload = await response.json();
                buildEmployeeList(payload);
                msg === "" ? $("#status").text("Calls Loaded") : $("#status").text(`${msg} - Employees Loaded`);
            } else if (response.status !== 404) {
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else {
                $("#status").text("no such path on server");
            }



            // get division data
            response = await fetch(`api/department`);
            if (response.ok) {
                let divs = await response.json(); // this returns a promise, so we await it
                sessionStorage.setItem("alldepartments", JSON.stringify(divs));
            } else if (response.status !== 404) { // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else { // else 404 not found
                $("#status").text("no such path on server");
            } // else

            //fetch user
            response = await fetch("api/userlogin");
            if (response.ok) {
                let user = await response.json();
                sessionStorage.setItem("allusers", JSON.stringify(user));
            } else {
                console.error("Error loading roles");
            }

        } catch (error) {
            $("#status").text(error.message);
        }
    };

    const loadDepartmentDDL = (empdep) => {
        html = '';
        $('#ddlDepartments').empty();
        let allDeparments = JSON.parse(sessionStorage.getItem('alldepartments'));
        allDeparments.forEach((dep) => { html += `<option value="${dep.id}">${dep.name}</option>` });
        $('#ddlDepartments').append(html);
        $('#ddlDepartments').val(empdep);
    };

    const loadProblemDDL = (probId) => {
        let html = '';
        $('#ddlProblems').empty();
        let allProblems = JSON.parse(sessionStorage.getItem('allproblems'));
        allProblems.forEach((problem) => {html += `<option value="${problem.id}">${problem.description}</option>`;});
        $('#ddlProblems').append(html);
        $('#ddlProblems').val(probId);
    };

    const loadEmployeeDDL = (emp) => {
        html = '';
        $('#ddlEmployees').empty();
        let allEmployees = JSON.parse(sessionStorage.getItem('allemployees'));
        let allUsers = JSON.parse(sessionStorage.getItem("allusers"));

        allEmployees.forEach(emp => {
            let user = allUsers.find(user => user.email === emp.email);

            employee = allEmployees.find(e => e.id === emp.id && user.roleId === 2);
            if (employee) {
                html += `<option value="${employee.id}">${employee.lastname}</option>`
            }
        });
        $('#ddlEmployees').append(html);
        $('#ddlEmployees').val(emp);
    };

    const loadCustomerDDL = (emp) => {
        html = '';
        $('#ddlCustomers').empty();
        let allCustomers = JSON.parse(sessionStorage.getItem('allcustomers'));

        allCustomers.forEach((customer) => { html += `<option value="${customer.id}">${customer.lastname}</option>`; });


        $('#ddlCustomers').append(html);
        $('#ddlCustomers').val(emp);
    };



    const buildEmployeeList = (data, usealldata = true) => {
        $("#employeeList").empty();
        let div = $(`
    <div id="heading">
            <div class="call-col">Date</div>
            <div class="call-col">Technician</div>
            <div class="call-col">Problem</div>
            <div class="call-col">Customer</div>
            <div class="call-col">Est. Days</div>
            <div class="call-col">Actual Days</div>
            <div class="call-col">Status</div>
        </div>`);
        div.appendTo($("#employeeList"));
        usealldata ? sessionStorage.setItem("allemployees", JSON.stringify(data)) : null;
        allCalls = "";
        allCalls = JSON.parse(sessionStorage.getItem("allcalls"));
        allProblems = JSON.parse(sessionStorage.getItem("allproblems"));
        allCustomers = JSON.parse(sessionStorage.getItem("allcustomers"));

        allCalls.forEach(call => {
            employee = data.find(emp => emp.id === call.employeeId);
            problem = allProblems.find(prob => prob.id === call.problemId);
            customer = allCustomers.find(cus => cus.id === call.customerId);

            if (employee) {
                let btn = $(`<button class="call-item" id="${call.id}">`);

                let processDay;
                let status;
                if (call.dateOpened != null && call.dateClosed != null) {
                    const opened = new Date(Date.parse(call.dateOpened));
                    const closed = new Date(Date.parse(call.dateClosed));
                    processDay = Math.round((closed - opened) / (1000 * 60 * 60 * 24));
                    status = processDay > call.expectedProcessingDays ? "Late" : "On time";
                }
                else {
                    processDay = "";
                    status = "In Progress";
                }

                btn.html(`
                <div class="call-col" id="call${call.id}">${formatDate(call.dateOpened).replace("T", " ")}</div>
                <div class="call-col" id="employee${call.id}">${employee.lastname}</div>
                <div class="call-col" id="problem${call.id}">${problem.description}</div>
                <div class="call-col" id="customer${call.id}">${customer.lastname}</div>
                <div class="call-col" id="expectedprocessingDate${call.id}">${call.expectedProcessingDays}</div>
                <div class="call-col" id="processingDate${call.id}">${processDay}</div>
                <div class="call-col" id="status${call.id}">${status}</div>
            `);
                btn.appendTo($("#employeeList"));
            }
        });
    };


    $("#deletebutton").on("click", () => {
        $("#dialog").show();


    }); // deletebutton click

    $("#nobutton").on("click", (e) => {
        $("#dialog").hide();
        $("#dialogbutton").show();
        $("#modalstatus").text("delete canceled");

    });
    $("#yesbutton").on("click", () => {
        $("#dialog").hide();
        $("#dialogbutton").show();
        _delete();
    });

    //List button
    $("#employeeList").on('click', (e) => {
        if (!e) e = window.event;
        let id = e.target.parentNode.id;
        if (id === "employeeList" || id === "") {
            id = e.target.id;
        }

        if (id !== "status" && id !== "heading") {
            let data = JSON.parse(sessionStorage.getItem("allcalls"));
            id === "0" ? setupForAdd() : setupForUpdate(id, data);
        } else {
            return false;
        }
    });

    if (roleName === "Admin") {
        $("#employeeList").off("click");
    }

    //add button
    $("#addEmployeeButton").on("click", function () {
        setupForAdd();
    });

    const setupForAdd = () => {
        clearModalFields();

        const now = new Date();
        const datetime = now.toISOString();

        $("#actionbutton").show();
        $("#actionbutton").val("Add");
        $("#modaltitle").html("<h4>Add Call</h4>");
        $("#theModal").modal("toggle");
        $("#modalstatus").text("Add New Call");
        $("#DateOpenedBox").text(formatDate().replace("T", " "));
        sessionStorage.setItem("dateOpened", formatDate());

        $("#deletebutton").hide();
        $("#closeWhenAdd").hide();


    };

    const setupForUpdate = (id, data) => {
        clearModalFields();
        $("#actionbutton").show();

        $("#actionbutton").val("Update");
        $("#modaltitle").html("<h4>Update Call</h4>");
        $("#deletebutton").show();
        $("#theModal").modal("toggle");
        clearModalFields();

        data.forEach(emp => {
            if (emp.id === parseInt(id)) {
                $("#Notebox").val(emp.notes);
                loadEmployeeDDL(emp.employeeId);
                loadProblemDDL(emp.problemId);
                loadCustomerDDL(emp.customerId);
                $("#DayExpectedBox").val(emp.expectedProcessingDays);
                sessionStorage.setItem("dateOpened", formatDate(emp.dateOpened));
                $("#DateOpenedBox").text(formatDate(emp.dateOpened).replace("T", " "));
                sessionStorage.setItem("call", JSON.stringify(emp));
                $("#modalstatus").text("update data");
                $("#theModal").modal("toggle");
                loadDepartmentDDL(emp.departmentId);
                if (emp.openStatus == false) {
                    $('#Notebox').attr('disabled', true);
                    $('#ddlProblems').attr('disabled', true);
                    $('#ddlEmployees').attr('disabled', true);
                    $('#ddlTechnician').attr('disabled', true);
                    $('#ddlCustomers').attr('disabled', true);
                    $('#checkBoxClose').attr('disabled', true);
                    $('#DayExpectedBox').prop('disabled', true);

                    $('#checkBoxClose').prop('checked', true);
                    $("#DateClosedBox").text(formatDate(emp.dateClosed).replace("T", " "));
                    $("#actionbutton").hide();

                }
            }

        });


    };

    const clearModalFields = () => {
        loadDepartmentDDL(-1);
        loadProblemDDL(-1);
        loadEmployeeDDL(-1);
        loadCustomerDDL(-1);
        $("#closeWhenAdd").show();

        $("#Notebox").val("");
        $("#TextBoxTitle").val("");
        $("#TextBoxFirstName").val("");
        $("#TextBoxSurname").val("");
        $("#TextBoxPhone").val("");
        $("#TextBoxEmail").val("");
        $("#DayExpectedBox").val("");

        $('#checkBoxClose').prop('checked', false);
        $('#Notebox').attr('disabled', false);
        $('#DayExpectedBox').attr('disabled', false);
        $('#ddlProblems').attr('disabled', false);
        $('#ddlCustomers').attr('disabled', false);
        $('#ddlEmployees').attr('disabled', false);
        $('#checkBoxClose').attr('disabled', false);

        $("#DateClosedBox").text("");

        sessionStorage.removeItem("employees");
        sessionStorage.removeItem("staffPicture64");
        $("#imageHolder").html("");
        $("#uploader").val("");
        $("#theModal").modal("toggle");
        let validator = $("#EmployeeModalForm").validate();
        validator.resetForm();




    };

    const add = async () => {
        try {
            let call = {
                employeeId: parseInt($("#ddlEmployees").val()),
                problemId: parseInt($("#ddlProblems").val()),
                customerId: parseInt($("#ddlCustomers").val()),
                dateOpened: formatDate(),
                dateClosed: null,
                openStatus: true,
                notes: $("#Notebox").val(),
                timer: null,
                expectedProcessingDays: parseInt($("#DayExpectedBox").val()),
            };


            let response = await fetch("api/call", {
                method: "POST",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(call)
            });

            if (response.ok) {
                let data = await response.json();
                getAll(data.msg);
            } else if (response.status !== 404) {
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else {
                $("#status").text("no such path on server");
            }
        } catch (error) {
            $("#status").text(error.message);
        }
        $("#theModal").modal("toggle");
    };

    const update = async () => {
        try {
            let emp = JSON.parse(sessionStorage.getItem("call"));
            emp.employeeId = parseInt($("#ddlEmployees").val());
            emp.problemId = parseInt($("#ddlProblems").val());
            emp.customerId = parseInt($("#ddlCustomers").val());
            emp.notes = $("#Notebox").val();
            if ($("#checkBoxClose").prop("checked")) {
                emp.dateClosed = formatDate();
                emp.openStatus = false;
            }
            else {
                emp.openStatus = true;
            }


            let response = await fetch("api/call", {
                method: "PUT",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(emp)
            });

            if (response.ok) {
                let payload = await response.json();
                getAll(payload.msg);
                $("#theModal").modal("toggle");
            } else if (response.status !== 404) {
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else {
                $("#status").text("no such path on server");
            }
        } catch (error) {
            $("#status").text(error.message);
        }
    };



    const _delete = async () => {
        let emp = JSON.parse(sessionStorage.getItem("call"));
        try {
            let response = await fetch(`api/call/${emp.id}`, {
                method: 'DELETE',
                headers: { 'Content-Type': 'application/json; charset=utf-8' }
            });
            if (response.ok) // or check for response.status
            {
                let data = await response.json();
                getAll(data.msg);
            } else {
                $('#status').text(`Status - ${response.status}, Problem on delete server side, see server console`);
            } // else
            $('#theModal').modal('toggle');
        } catch (error) {
            $('#status').text(error.message);
        }
    }; // _delete

    $("#dialog").hide();

    $("#actionbutton").on("click", () => {
        $("#actionbutton").val() === "Update" ? update() : add();
    });

    document.addEventListener("keyup", e => {
        $("#modalstatus").removeClass(); //remove any existing css on div
        if ($("#EmployeeModalForm").valid()) {
            $("#modalstatus").attr("class", "badge bg-success"); //green
            $("#modalstatus").text("data entered is valid");
            $("#actionbutton").prop("disabled", false);
        }
        else {
            $("#modalstatus").attr("class", "badge bg-danger"); //red
            $("#modalstatus").text("fix errors");
            $("#actionbutton").prop("disabled", true);
        }
    });

    $("#LogInModalForm").validate({
        rules: {
            ddlProblems: { required: true, },
            ddlEmployees: { required: true },
            ddlCustomers: { required: true },
            Notebox: { maxlength: 250, required: true,}
        },
        errorElement: "div",
        messages: {
            ddlProblems: {
                required: "Select Problem"
            },
            ddlEmployees: {
                required: "Select Employee"
            },
            ddlCustomers: {
                required: "Select Technician"
            },
            Notebox: {
                required: "Required 1-250 chars.", maxlength: "Required 1-250 chars."
            }
        }
    }); //StudentModalForm.validate

    $.validator.addMethod("validTitle", (value) => { //custome rule
        return (value === "Mr." || value === "Ms." || value === "Mrs." || value === "Dr.");
    }, ""); //.validator.addMethod

    $("#srch").on("keyup", () => {
        let alldata = JSON.parse(sessionStorage.getItem("allemployees"));
        let filtereddata = alldata.filter((emp) => emp.lastname.match(new RegExp($("#srch").val(), 'i')));
        buildEmployeeList(filtereddata, false);
    }); // srch keyup

    $("#checkBoxClose").on("click", () => {
        if ($("#checkBoxClose").is(":checked")) {
            $("#DateClosedBox").text(formatDate().replace("T", " "));
            sessionStorage.setItem("dateClosed", formatDate());
        } else {
            $("#DateClosedBox").text("");
            sessionStorage.setItem("dateClosed", "");
        }
    }); // checkBoxClose

    const formatDate = (date) => {
        let d;
        (date === undefined) ? d = new Date() : d = new Date(Date.parse(date));
        let _day = d.getDate();
        if (_day < 10) { _day = "0" + _day; }
        let _month = d.getMonth() + 1;
        if (_month < 10) { _month = "0" + _month; }
        let _year = d.getFullYear();
        let _hour = d.getHours();
        if (_hour < 10) { _hour = "0" + _hour; }
        let _min = d.getMinutes();
        if (_min < 10) { _min = "0" + _min; }
        return _year + "-" + _month + "-" + _day + "T" + _hour + ":" + _min;
    } // formatDate



    getAll("");
});




