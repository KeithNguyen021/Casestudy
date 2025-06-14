$(() => {
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

    $("#welcomeMessage").html(`<p>Welcome, ${currentUser.firstname} ${currentUser.lastname}!</p>`);


    async function loadTickets() {

        try {
            let response = await fetch(`/api/call/${currentUser.id}`);
            let calls = await response.json();
            response = await fetch(`api/problem`);
            let probs = await response.json(); 
            sessionStorage.setItem("allproblems", JSON.stringify(probs));
            allProblems = JSON.parse(sessionStorage.getItem("allproblems"));

            let ticketHTML = "";
            for (let call of calls) {
                problem = allProblems.find(prob => prob.id === call.problemId);
                let processDay;
                let status;
                if (call.dateOpened != null && call.dateClosed != null) {
                    const opened = new Date(Date.parse(call.dateOpened));
                    const closed = new Date(Date.parse(call.dateClosed));
                    processDay = Math.round((closed - opened) / (1000 * 60 * 60 * 24));
                    status = processDay > call.expectedProcessingDays ? "Late" : "On time";
                }
                else {
                    status = "In Progress";
                }

                ticketHTML += `
                    <li>
                        <strong>Problem:</strong> ${problem.description} <br>                        
                        <strong>Status:</strong> ${status} <br>
                        <strong>Date Opened:</strong> ${formatDate(call.dateOpened).replace("T", " ")} <br>
                        <strong>Date Closed:</strong> ${call.dateClosed ? formatDate(call.dateClosed).replace("T", " ") : ""} <br>
                        <strong>Notes:</strong> ${call.notes}
                    </li><hr>`;
            }

            $("#ticketList").html(ticketHTML);
        } catch (error) {
            console.error("Error fetching tickets:", error);
        }
    }


    //Access control
    if (roleName === "Admin") {
        $("#addEmployeeButton,#ticketListContainer").hide();
    }
    else if (roleName === "Technician") {
        $("#managementNav, #ticketListContainer").hide();
    }
    else {
        $("#managementNav, #customerNav, #ticketNav, #reportNav").hide();
        loadTickets();
    }

    //Log out
    $("#signOutButton").click(() => {
        sessionStorage.removeItem("currentUser"); // Delete current user from sessionStorage
        window.location.href = "userlogin.html"; // Redirect to login page
    });
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

});