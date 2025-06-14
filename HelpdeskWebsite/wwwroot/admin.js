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
        $("#addEmployeeButton").show();
    }
    else if (roleName === "Technician") {
        $("#managementNav").hide();
    }
    else {
        $("#managementNav, #customerNav, #ticketNav").hide();
    }

    //Log out
    $("#signOutButton").click(() => {
        sessionStorage.removeItem("currentUser"); // Delete current user from sessionStorage
        window.location.href = "userlogin.html"; // Redirect to login page
    });


    //Retrieving data from API
    const getAll = async (msg) => {
        try {

            // departments first
            let response = await fetch(`api/department`);
            if (response.ok) {
                let divs = await response.json();
                sessionStorage.setItem("alldepartments", JSON.stringify(divs));
            } else {
                $("#status").text("Failed to load departments");
                return;  // Stop execution if departments fail
            }
            //fetch roles
            response = await fetch("api/Role");
            if (response.ok) {
                let roles = await response.json();
                sessionStorage.setItem("allroles", JSON.stringify(roles));
            } else {
                console.error("Error loading roles");
            }
            //fetch user
            response = await fetch("api/userlogin");
            if (response.ok) {
                let user = await response.json();
                sessionStorage.setItem("allusers", JSON.stringify(user));
            } else {
                console.error("Error loading roles");
            }


            // fetch employees
            response = await fetch(`api/employee`);
            if (response.ok) {
                let payload = await response.json();
                buildEmployeeList(payload);
                msg === "" ? $("#status").text("Employees Loaded") : $("#status").text(`${msg} - Employees Loaded`);
            } else {
                $("#status").text("Failed to load employees");
            }

        } catch (error) {
            console.log(error.message);
        }
    };


    //Load Depaartment into the drop box
    const loadDepartmentDDL = (empdep) => {
        html = '';
        $('#ddlDepartments').empty();
        let allDeparments = JSON.parse(sessionStorage.getItem('alldepartments'));
        allDeparments.forEach((dep) => { html += `<option value="${dep.id}">${dep.name}</option>` });
        $('#ddlDepartments').append(html);
        $('#ddlDepartments').val(empdep);
    };

    //Load ROles into the drop box
    const loadRoleDDL = (empdrole) => {
        html = '';
        $('#ddlRoles').empty();
        let allRoles = JSON.parse(sessionStorage.getItem('allroles'));
        allRoles
            .filter(role => role.id !== 3)
            .forEach((role) => {
                html += `<option value="${role.id}">${role.roleName}</option>`;
            });
        $('#ddlRoles').append(html);
        $('#ddlRoles').val(empdrole);
    };

    //Build list
    const buildEmployeeList = (data, usealldata = true) => {
        $("#employeeList").empty();

        let div = $(`
        <div id="heading">
            <div class="list-col">Title</div>
            <div class="list-col">Name</div>
            <div class="list-col">Position</div>
            <div class="list-col">Phone Number</div>
            <div class="list-col">Email</div>
            <div class="list-col">Department</div>

        </div>
    `);
        div.appendTo($("#employeeList"));
        allDeparments = JSON.parse(sessionStorage.getItem("alldepartments"));
        let allRoles = JSON.parse(sessionStorage.getItem("allroles")); // Get roles from session
        let allUsers = JSON.parse(sessionStorage.getItem("allusers"));
        if (usealldata) {
            sessionStorage.setItem("allemployees", JSON.stringify(data));
        }

        data.forEach(emp => {
            let department = allDeparments.find(dep => dep.id === emp.departmentId);
            let user = allUsers.find(user => user.email === emp.email);
            let personRole = allRoles.find(role => role.id === user.roleId);
            let btn = $(`
            <button class="list-item" id="${emp.id}">
                <div class="list-col" id="employeetitle${emp.id}">${emp.title}</div>
                <div class="list-col" id="employeefirstname${emp.id}">${emp.firstname} ${emp.lastname}</div>
                <div class="list-col" id="empPosition${emp.id}">${personRole.roleName}</div>
                <div class="list-col" id="empPhonenumber${emp.id}">${emp.phoneno}</div>
                <div class="list-col" id="empEmail${emp.id}">${emp.email}</div>
                <div class="list-col" id="empDepartment${emp.id}">${department.name}</div>
            </button>
        `);
            btn.appendTo($("#employeeList"));
        });
    };




    //list button
    $("#employeeList").on('click', (e) => {
        if (!e) e = window.event;
        let id = e.target.parentNode.id;
        if (id === "employeeList" || id === "") {
            id = e.target.id;
        }

        let data = JSON.parse(sessionStorage.getItem("allemployees"));
        setupForUpdate(id, data);
    });

    //add button
    $("#addEmployeeButton").on("click", function () {
        setupForAdd();
    });

    //Refresh button
    $("#refreshEmployeeButton").on("click", function () {
        let employees = JSON.parse(sessionStorage.getItem("allemployees")) || [];
        buildEmployeeList(employees, false); // Reloads the employee list
    });

    //CHoose modal form
    const setupForAdd = () => {
        clearModalFields();

        $("#actionbutton").val("Add");
        $("#modaltitle").html("<h4>Add User</h4>");
        $("#theModal").modal("toggle");
        document.getElementById("deletebutton").style.display = "none";
        $("#passwordBox").show();

    };

    const setupForUpdate = (id, data) => {
        clearModalFields();
        $("#actionbutton").val("Update");
        $("#modaltitle").html("<h4>Update User</h4>");
        $("#deletebutton").show();
        $("#theModal").modal("toggle");
        document.getElementById("passwordBox").style.display = "none";

        data.forEach(emp => {
            if (emp.id === parseInt(id)) {
                $("#TextBoxTitle").val(emp.title);
                $("#TextBoxFirstName").val(emp.firstname);
                $("#TextBoxSurname").val(emp.lastname);
                $("#TextBoxPhone").val(emp.phoneno);
                $("#TextBoxEmail").val(emp.email);

                sessionStorage.setItem("employee", JSON.stringify(emp));
                loadDepartmentDDL(emp.departmentId);
                let allUsers = JSON.parse(sessionStorage.getItem("allusers"));
                let user = allUsers.find(user => user.email === emp.email);
                loadRoleDDL(user.roleId);


            }
        });
    };

    //Clear modal fields
    const clearModalFields = () => {
        loadDepartmentDDL(-1);
        loadRoleDDL(-1);
        $("#TextBoxTitle").val("");
        $("#TextBoxFirstName").val("");
        $("#TextBoxSurname").val("");
        $("#TextBoxPhone").val("");
        $("#TextBoxEmail").val("");
        $("#TextBoxPassword").val("");
        $("#TextBoxConfirmPassword").val("");
        sessionStorage.removeItem("employees");
        $("#theModal").modal("toggle");
        let validator = $("#EmployeeModalForm").validate();
        validator.resetForm();

    };

    //add function
    const add = async () => {
        try {
            let userlogin = {
                email: $("#TextBoxEmail").val(),
                userPassword: $("#TextBoxPassword").val(),
                roleId: parseInt($("#ddlRoles").val()),

            };

            await fetch("api/userlogin", {
                method: "POST",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(userlogin)
            });

            let useremail = {
                email: $("#TextBoxEmail").val(),
            };

            await fetch("api/useremail", {
                method: "POST",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(useremail)
            });

            let employee = {
                title: $("#TextBoxTitle").val(),
                firstname: $("#TextBoxFirstName").val(),
                lastname: $("#TextBoxSurname").val(),
                email: $("#TextBoxEmail").val(),
                phoneno: $("#TextBoxPhone").val(),
                departmentId: parseInt($("#ddlDepartments").val()),
                timer: null,

            };
           
             employeeResponse = await fetch("api/employee", {
                method: "POST",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(employee)
            });


            //Set the right empID to new user
            let newUserEmail = $("#TextBoxEmail").val();
            let employeeFetchResponse = await fetch(`api/employee/${newUserEmail}`); 

            let newEmployee = await employeeFetchResponse.json(); 

            let userFetchResponse = await fetch(`api/userlogin/${newUserEmail}`);

            let userToUpdate = await userFetchResponse.json();
            userToUpdate.employeeId = newEmployee.id; 

            await fetch("api/userlogin", {
                method: "PUT",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(userToUpdate)
            });
        } catch (error) {
            console.log(error.message);
        }
        $("#theModal").modal("toggle");
        getAll("");

    };

    const update = async () => {
        try {
            // Update UserLogin first
            let employee = JSON.parse(sessionStorage.getItem("employee"));

            let userResponse = await fetch(`api/userlogin/${employee.email}`);

            let user = await userResponse.json();

            user.roleId = parseInt($("#ddlRoles").val());;
            user.email = $("#TextBoxEmail").val(); // This should cascade to related tables

            let userUpdateResponse = await fetch("api/userlogin", {
                method: "PUT",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify({
                    id: user.id,
                    email: user.email,
                    userPassword: user.userPassword,
                    roleId: user.roleId,
                    employeeId: user.employeeId,
                    customerId: user.customerId,
                    timer: user.timer
                })
            });

            if (userUpdateResponse.ok) {
                console.log("UserLogin updated successfully!");
            } else {
                console.error("Error updating UserLogin.");
            }

            // Update Employee (excluding email because it should be cascaded)
            let emp = JSON.parse(sessionStorage.getItem("employee"));
            emp.title = $("#TextBoxTitle").val();
            emp.firstname = $("#TextBoxFirstName").val();
            emp.lastname = $("#TextBoxSurname").val();
            emp.phoneno = $("#TextBoxPhone").val();
            emp.departmentId = parseInt($("#ddlDepartments").val());

            let empUpdateResponse = await fetch("api/employee", {
                method: "PUT",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(emp)
            });

            if (empUpdateResponse.ok) {
                let payload = await empUpdateResponse.json();
                getAll(payload.msg);
                $("#theModal").modal("toggle");
            } else if (empUpdateResponse.status !== 404) {
                let problemJson = await empUpdateResponse.json();
                errorRtn(problemJson, empUpdateResponse.status);
            } else {
                console.log("No such path on server");
            }
        } catch (error) {
            console.log(error.message);
        }
    };

    //Delete stuffs
    $("#deletebutton").on("click", () => {
        $("#dialog").show();
    }); // deletebutton click

    $("#nobutton").on("click", (e) => {
        $("#dialog").hide();
    });
    $("#yesbutton").on("click", () => {
        $("#dialog").hide();
        _delete();
    });

    const _delete = async () => {
        let emp = JSON.parse(sessionStorage.getItem("employee"));
        let email = emp.email; // Retrieve the email associated with the employee

        try {
            let userResponse = await fetch(`api/userlogin/${email}`, {
                method: "DELETE",
                headers: { "Content-Type": "application/json; charset=utf-8" }
            });

            if (!userResponse.ok) {
                console.error(`Failed to delete UserLogin for ${email}`);
                return;
            }

            console.log(`Successfully deleted UserLogin, UserEmail, and Employee for ${email}`);

        } catch (error) {
            $('#status').text(error.message);
            console.error("Error deleting user:", error.message);
        }
        getAll("");
        $('#theModal').modal('toggle');
    };


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

    //Validation
    $("#EmployeeModalForm").validate({
        rules: {
            TextBoxTitle: { required: true, validTitle: true },
            TextBoxFirstName: { required: true, maxlength: 25, minlength: 2 },
            TextBoxSurname: { required: true, maxlength: 25, minlength: 2 },
            TextBoxEmail: { required: true, email: true, maxlength: 40 },
            TextBoxPhone: { required: true, minlength: 7, maxlength: 15, digits: true },
            TextBoxPassword: { required: true, minlength: 8, validPassword: true },
            TextBoxConfirmPassword: { required: true, equalTo: "#TextBoxPassword" },
            ddlRoles: { required: true },
            ddlDepartments: { required: true }
        },
        messages: {
            TextBoxTitle: { required: "Title is required.", validTitle: "Valid titles: Mr., Ms., Mrs., Dr." },
            TextBoxFirstName: { required: "First name is required.", minlength: "Minimum 2 characters.", maxlength: "Maximum 25 characters." },
            TextBoxSurname: { required: "Last name is required.", minlength: "Minimum 2 characters.", maxlength: "Maximum 25 characters." },
            TextBoxEmail: { required: "Email is required.", email: "Enter a valid email.", maxlength: "Maximum 40 characters." },
            TextBoxPhone: { required: "Phone number is required.", minlength: "Minimum 7 digits.", maxlength: "Maximum 15 digits.", digits: "Only numbers allowed." },
            TextBoxPassword: { required: "Password is required.", minlength: "Minimum 8 characters." },
            TextBoxConfirmPassword: { required: "Please confirm password.", equalTo: "Passwords must match." },
            ddlRoles: { required: "Role selection is required." },
            ddlDepartments: { required: "Department selection is required." }
        },
        errorElement: "div"
    });

    // Custom rules
    $.validator.addMethod("validTitle", function (value) {
        return ["Mr.", "Ms.", "Mrs.", "Dr."].includes(value);
    }, "Invalid title.");

    $.validator.addMethod("validPassword", function (value) {
        return /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/.test(value);
    }, "Password must have at least 8 characters, 1 uppercase letter, 1 lowercase letter, and 1 number.");

    //disable the button until input is correct
    function toggleActionButton() {
        if ($("#EmployeeModalForm").valid()) {
            $("#actionbutton").prop("disabled", false);
        } else {
            $("#actionbutton").prop("disabled", true);
        }
    }

    // Attach validation check to form inputs
    $("#EmployeeModalForm input").on("keyup blur", toggleActionButton);

    // Ensure action button starts disabled
    $("#actionbutton").prop("disabled", true);




    $("#srch").on("keyup", () => {
        let alldata = JSON.parse(sessionStorage.getItem("allemployees"));
        let filtereddata = alldata.filter((emp) => emp.lastname.match(new RegExp($("#srch").val(), 'i')));
        buildEmployeeList(filtereddata, false);
    }); // srch keyup 

    getAll("");
});




