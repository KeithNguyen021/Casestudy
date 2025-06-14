$(() => {
    //Display current User
    //Retrieve User
    let currentUser = JSON.parse(sessionStorage.getItem("currentUser"));
    let roleName = sessionStorage.getItem("roleName"); // Get roleId

    //Display user info
    let userName = `${currentUser.firstname} ${currentUser.lastname}`;
    $("#sidebarName").html(userName);


    let userRole = `${roleName}`;
    $("#sidebarRole").html(userRole);



    let userEmail = `${currentUser.email} `;
    $("#sidebarEmail").html(userEmail);

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


    //Retrieving data from API
    const getAll = async (msg) => {
        try {
            // fetch Customer
            response = await fetch(`api/customer`);
            if (response.ok) {
                let payload = await response.json();
                buildEmployeeList(payload);
            } else {
               console.log("Failed to load customers");
            }

        } catch (error) {
           console.log(error.message);
        }
    };

    const buildEmployeeList = (data, usealldata = true) => {
        $("#employeeList").empty();

        let div = $(`
        <div id="heading">
            <div class="list-col">Title</div>
            <div class="list-col">Name</div>
            <div class="list-col">Phone Number</div>
            <div class="list-col">Email</div>
        </div>
    `);
        div.appendTo($("#employeeList"));
        if (usealldata) {
            sessionStorage.setItem("allcustomers", JSON.stringify(data));
        }

        data.forEach(emp => {

            let btn = $(`
            <button class="list-item" id="${emp.id}">
                <div class="list-col" id="employeetitle${emp.id}">${emp.title}</div>
                <div class="list-col" id="employeefirstname${emp.id}">${emp.firstname} ${emp.lastname}</div>
                <div class="list-col" id="empPhonenumber${emp.id}">${emp.phoneno}</div>
                <div class="list-col" id="empEmail${emp.id}">${emp.email}</div>
            </button>
        `);
            btn.appendTo($("#employeeList"));
        });
    };


    $("#employeeList").on('click', (e) => {

        if (!e) e = window.event;
        let id = e.target.parentNode.id;
        if (id === "employeeList" || id === "") {
            id = e.target.id;
        }

        let data = JSON.parse(sessionStorage.getItem("allcustomers"));
        setupForUpdate(id, data);
    });

    if (roleName === "Admin") {
        $("#employeeList").off("click");
    }
    //add button
    $("#addEmployeeButton").on("click", function () {
        setupForAdd();
    });

    //Refresh button
    $("#refreshEmployeeButton").on("click", function () {
        let employees = JSON.parse(sessionStorage.getItem("allcustomers")) || [];
        buildEmployeeList(employees, false); // Reloads the employee list
    });

    const setupForAdd = () => {
        clearModalFields();

        $("#actionbutton").val("Add");
        $("#modaltitle").html("<h4>Add Customer</h4>");
        $("#theModal").modal("toggle");
        $("#deletebutton").hide();
        document.getElementById("deletebutton").style.display = "none";
        $("#passwordBox").show();

    };

    const setupForUpdate = (id, data) => {
        clearModalFields();
        $("#actionbutton").val("Update");
        $("#modaltitle").html("<h4>Update Customer</h4>");
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
                sessionStorage.setItem("customer", JSON.stringify(emp));
            }
        });
    };

    const clearModalFields = () => {
        $("#TextBoxTitle").val("");
        $("#TextBoxFirstName").val("");
        $("#TextBoxSurname").val("");
        $("#TextBoxPhone").val("");
        $("#TextBoxEmail").val("");
        $("#TextBoxPassword").val("");
        $("#TextBoxConfirmPassword").val("");
        sessionStorage.removeItem("customer");
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
                roleId: 3,

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

            let customer = {
                title: $("#TextBoxTitle").val(),
                firstname: $("#TextBoxFirstName").val(),
                lastname: $("#TextBoxSurname").val(),
                email: $("#TextBoxEmail").val(),
                phoneno: $("#TextBoxPhone").val(),
                timer: null,

            };

            employeeResponse = await fetch("api/customer", {
                method: "POST",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(customer)
            });


            //Set the right empID to new user
            let newUserEmail = $("#TextBoxEmail").val();
            let customerFetchResponse = await fetch(`api/customer/${newUserEmail}`);

            let newEmployee = await customerFetchResponse.json();

            let userFetchResponse = await fetch(`api/userlogin/${newUserEmail}`);

            let userToUpdate = await userFetchResponse.json();
            userToUpdate.customerId = newEmployee.id;

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
            let customer = JSON.parse(sessionStorage.getItem("customer"));

            let userResponse = await fetch(`api/userlogin/${customer.email}`);

            let user = await userResponse.json();

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
            let emp = JSON.parse(sessionStorage.getItem("customer"));
            emp.title = $("#TextBoxTitle").val();
            emp.firstname = $("#TextBoxFirstName").val();
            emp.lastname = $("#TextBoxSurname").val();
            emp.phoneno = $("#TextBoxPhone").val();

            let empUpdateResponse = await fetch("api/customer", {
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

    //Delete thingy
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
        let emp = JSON.parse(sessionStorage.getItem("customer"));
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
        },
        messages: {
            TextBoxTitle: { required: "Title is required.", validTitle: "Valid titles: Mr., Ms., Mrs., Dr." },
            TextBoxFirstName: { required: "First name is required.", minlength: "Minimum 2 characters.", maxlength: "Maximum 25 characters." },
            TextBoxSurname: { required: "Last name is required.", minlength: "Minimum 2 characters.", maxlength: "Maximum 25 characters." },
            TextBoxEmail: { required: "Email is required.", email: "Enter a valid email.", maxlength: "Maximum 40 characters." },
            TextBoxPhone: { required: "Phone number is required.", minlength: "Minimum 7 digits.", maxlength: "Maximum 15 digits.", digits: "Only numbers allowed." },
            TextBoxPassword: { required: "Password is required.", minlength: "Minimum 8 characters." },
            TextBoxConfirmPassword: { required: "Please confirm password.", equalTo: "Passwords must match." },
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




