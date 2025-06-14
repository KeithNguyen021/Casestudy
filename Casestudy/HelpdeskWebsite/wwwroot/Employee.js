$(() => {
    const getAll = async (msg) => {
        try {
            $("#employeeList").text("Finding Employee Information...");
            let response = await fetch(`api/employee`);
            if (response.ok) {
                let payload = await response.json();
                buildEmployeeList(payload);
                msg === "" ? $("#status").text("Employee Loaded") : $("#status").text(`${msg} - Employees Loaded`);
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


    const buildEmployeeList = (data, usealldata = true) => {
        $("#employeeList").empty();
        let div = $(`<div class="list-group-item row d-flex" id="status">Employee Info</div>
            <div class= "list-group-item row d-flex text-center" id="heading">
                <div class="col-4 h4">Title a</div>
                <div class="col-4 h4">First</div>
                <div class="col-4 h4">Last</div>
            </div>`);
        div.appendTo($("#employeeList"));
        usealldata ? sessionStorage.setItem("allemployees", JSON.stringify(data)) : null;

        let btn = $(`<button class="list-group-item row d-flex" id="0">Add Employee</button>`);
        btn.appendTo($("#employeeList"));

        data.forEach(emp => {
            let btn = $(`<button class="list-group-item row d-flex" id="${emp.id}">`);
            btn.html(`
                <div class="col-4" id="employeetitle${emp.id}">${emp.title}</div>
                <div class="col-4" id="employeefirstname${emp.id}">${emp.firstname}</div>
                <div class="col-4" id="employeelastname${emp.id}">${emp.lastname}</div>
            `);
            btn.appendTo($("#employeeList"));
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

    $("#employeeList").on('click', (e) => {
        if (!e) e = window.event;
        let id = e.target.parentNode.id;
        if (id === "employeeList" || id === "") {
            id = e.target.id;
        }

        if (id !== "status" && id !== "heading") {
            let data = JSON.parse(sessionStorage.getItem("allemployees"));
            id === "0" ? setupForAdd() : setupForUpdate(id, data);
        } else {
            return false;
        }
    });

    $("input:file").on("change", () => {
        try {
            const reader = new FileReader();
            const file = $("#uploader")[0].files[0];
            $("#uploadstatus").text("");
            file ? reader.readAsBinaryString(file) : null;
            reader.onload = (readerEvt) => {
                // get binary data then convert to encoded string
                const binaryString = reader.result;
                const encodedString = btoa(binaryString);
                // replace the picture in session storage
                let employee = JSON.parse(sessionStorage.getItem("employee"));
                employee.staffPicture64 = encodedString;
                sessionStorage.setItem("employee", JSON.stringify(employee));
                $("#uploadstatus").text("retrieved local pic")
            };
        } catch (error) {
            $("#uploadstatus").text("pic upload failed")
        }
    }); // input file change

    const setupForAdd = () => {
        clearModalFields();

        $("#actionbutton").val("add");
        $("#modaltitle").html("<h4>add employee</h4>");
        $("#theModal").modal("toggle");
        $("#modalstatus").text("add new employee");
        $("#deletebutton").hide();


    };

    const setupForUpdate = (id, data) => {
        clearModalFields();
        $("#actionbutton").val("update");
        $("#modaltitle").html("<h4>update employee</h4>");
        $("#deletebutton").show();
        $("#theModal").modal("toggle");
        clearModalFields();

        data.forEach(emp => {
            if (emp.id === parseInt(id)) {
                $("#TextBoxTitle").val(emp.title);
                $("#TextBoxFirstName").val(emp.firstname);
                $("#TextBoxSurname").val(emp.lastname);
                $("#TextBoxPhone").val(emp.phoneno);
                $("#TextBoxEmail").val(emp.email);
                $("#imageHolder").html(`<img height="120" width="110" src="data:img/png;base64,${emp.staffPicture64}" />`);

                sessionStorage.setItem("employee", JSON.stringify(emp));
                $("#modalstatus").text("update data");
                $("#theModal").modal("toggle");
                loadDepartmentDDL(emp.departmentId);

            }
        });
    };

    const clearModalFields = () => {
        loadDepartmentDDL(-1);
        $("#TextBoxTitle").val("");
        $("#TextBoxFirstName").val("");
        $("#TextBoxSurname").val("");
        $("#TextBoxPhone").val("");
        $("#TextBoxEmail").val("");
        sessionStorage.removeItem("employees");
        sessionStorage.removeItem("staffPicture64");
        $("#uploadstatus").text("");
        $("#imageHolder").html("");
        $("#uploader").val("");
        $("#theModal").modal("toggle");
        let validator = $("#EmployeeModalForm").validate();
        validator.resetForm();
    };

    const add = async () => {
        try {
            let employee = {
                title: $("#TextBoxTitle").val(),
                firstname: $("#TextBoxFirstName").val(),
                lastname: $("#TextBoxSurname").val(),
                email: $("#TextBoxEmail").val(),
                phoneno: $("#TextBoxPhone").val(),
                departmentId: parseInt($("#ddlDepartments").val()),
                timer: null,
                picture64: null
            };


            let response = await fetch("api/employee", {
                method: "POST",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(employee)
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
            let emp = JSON.parse(sessionStorage.getItem("employee"));
            emp.title = $("#TextBoxTitle").val();
            emp.firstname = $("#TextBoxFirstName").val();
            emp.lastname = $("#TextBoxSurname").val();
            emp.email = $("#TextBoxEmail").val();
            emp.phoneno = $("#TextBoxPhone").val();
            emp.departmentId = parseInt($("#ddlDepartments").val());


            let response = await fetch("api/employee", {
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
        let emp = JSON.parse(sessionStorage.getItem("allemployees"));
        try {
            let response = await fetch(`api/employee/${emp.id}`, {
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
        $("#actionbutton").val() === "update" ? update() : add();
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

    $("#EmployeeModalForm").validate({
        rules: {
            TextBoxTitle: { maxlength: 4, required: true, validTitle: true },
            TextBoxFirstName: { maxlength: 25, required: true },
            TextBoxSurname: { maxlength: 25, required: true },
            TextBoxEmail: { maxlength: 40, required: true, email: true },
            TextBoxPhone: { maxlength: 15, required: true }
        },
        errorElement: "div",
        messages: {
            TextBoxTitle: {
                required: "required 1-4 chars.", maxlength: "required 1-4 chars.", validTitle: "Mr. Ms. Mrs. or Dr."
            },
            TextBoxFirstName: {
                required: "required 1-25 chars.", maxlength: "required 1-25 chars."
            },
            TextBoxSurname: {
                required: "required 1-25 chars.", maxlength: "required 1-25 chars."
            },
            TextBoxPhone: {
                required: "required 1-15 chars.", maxlength: "required 1-15 chars."
            },
            TextBoxEmail: {
                required: "required 1-40 chars.", maxlength: "required 1-40 chars.", email: "need valid email format"
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

    getAll("");
});




