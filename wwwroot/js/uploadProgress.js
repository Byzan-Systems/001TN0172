"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/uploadProgressHub").build();

connection.on("ReceiveProgressUpdate", function (progress) {
    document.getElementById("progressBar").style.width = progress + "%";
    document.getElementById("progressBar").innerText = progress + "%";
    var progressContainer = document.getElementById("progressContainer");
    var Progress = document.getElementById("Progress");
    var progressBar1 = document.getElementById("progressBar");
    //if (progress >= 100) {
    //    progressContainer.style.display = 'none'; // Hide the progress container
    //    Progress.style.display = 'none'; // Hide the progress container
    //    progressBar1.style.display = 'none'; // Hide the progress container
    //} else {
    //    progressContainer.style.display = 'block'; // Show the progress container
    //    Progress.style.display = 'block'; // Show the progress container
    //    progressBar1.style.display = 'block'; // Show the progress container
    //}
});

connection.start().then(function () {
    console.log("SignalR Connected.");
}).catch(function (err) {
    return console.error(err.toString());
});
