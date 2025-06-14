$(() => {

    //Clear modal fields
    const clearModalFields = () => {
        $("#TextBoxUserName").val("");
        $("#TextBoxPassword").val("");
        let validator = $("#loginFormContainer").validate();
        validator.resetForm();

    };

    //Validate user input
    $("#loginFormContainer").validate({
        rules: {
            TextBoxUserName: {
                minlength: 6,
                maxlength: 20,
                required: true
            },
            TextBoxPassword: {
                minlength: 6,
                maxlength: 20,
                required: true
            }
        },
        messages: {
            TextBoxUserName: {
                required: "Username is required.",
                minlength: "Username must be at least 6 characters.",
                maxlength: "Username must be at most 20 characters."
            },
            TextBoxPassword: {
                required: "Password is required.",
                minlength: "Password must be at least 6 characters.",
                maxlength: "Password must be at most 20 characters."
            }
        },
        errorElement: "div",
        errorPlacement: function (error, element) {
            error.addClass("text-danger");
            error.insertAfter(element);
        }
    });

    //Login and signup button
    $("#loginbutton").on("click", (e) => {
        clearModalFields();
        document.getElementById("loginFormContainer").style.display = "block";
        document.getElementById("signUpFormContainer").style.display = "none";
    });

    $("#signupbutton").on("click", (e) => {
        clearModalFields();
        $("#theModalsignup").modal("toggle");
    });

    // Login function
    $("#loginbuttonmodal").on("click", async (e) => {
        e.preventDefault();

        let user = {
            email: $("#TextBoxUserName").val(),
            userPassword: $("#TextBoxPassword").val(),
        };

        let response = await fetch("api/UserLogin/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(user)
        });

        if (!response.ok) { 
            let errorMsg = await response.text();
            console.error("Login failed:", errorMsg);
            $("#loginError").text("Invalid email or password").css("color", "red").show(); // Show error message
            return; // Stop execution if login fails
        }

        let loginData = await response.json();
        let roleId = loginData.roleId; // 1 = Admin, 2 = Technician, 3 = Customer
        let userData;

        if (roleId === 1 || roleId === 2) {
            // Admin or Technician Fetch Employee data
            let employeeResponse = await fetch(`api/Employee/${user.email}`, {
                method: "GET",
                headers: { "Content-Type": "application/json" }
            });

            if (employeeResponse.ok) {
                userData = await employeeResponse.json();
            }
        } else if (roleId === 3) {
            // Customer Fetch Customer data
            let customerResponse = await fetch(`api/Customer/${user.email}`, {
                method: "GET",
                headers: { "Content-Type": "application/json" }
            });

            if (customerResponse.ok) {
                userData = await customerResponse.json();
            }
        }

        let roleResponse = await fetch(`api/Role/${roleId}`, {
            method: "GET",
            headers: { "Content-Type": "application/json" }
        });

        let roleData = await roleResponse.json();
        let roleName = roleData.roleName; // Get role name from API

        sessionStorage.setItem("currentUser", JSON.stringify(userData));
        sessionStorage.setItem("roleName", roleName);

        window.location.href = "adminhome.html";



    });


    //sign up function
    $("#signuputtonmodal").on("click", async (e) => {
        e.preventDefault();
        let newuser = {
            email: $("#TextBoxUserName1").val(),
            userPassword: $("#TextBoxPassword1").val(),
        }
        let response = await fetch("api/UserLogin/register", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(newuser)
        });
        let result;
        if (response.ok) {
            $("#modalstatussignup").text("Register Successful!").css("color", "green");

        } else {
            result = await response.text();
            $("#modalstatussignup").text("Failed To Register. Please Enter Another Usernamw!").css("color", "red");
        }
    });

    //close button
    $(".btn-close").on("click", function () {
        clearModalFields();
    });



});