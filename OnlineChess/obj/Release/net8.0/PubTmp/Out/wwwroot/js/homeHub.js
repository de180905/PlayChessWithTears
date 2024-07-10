//establish connection
var connection = new signalR.HubConnectionBuilder().withUrl("/hubs/chessHub").build();
function fullfilled() {

}
function failed() {

}
connection.start().then(fullfilled, failed);

//define functions
function createRoomOnClient() {
    connection.send("CreateRoom");
}
function joinRoom() {
    var roomId = document.getElementById("roomId").value;
    // You can perform any necessary actions here to join the room
    // Perform actions to join the room, such as sending a request to the server
    connection.send("JoinRoom", roomId);
}

//listen for chessHub
connection.on("UnauthorizedAccess", (message) => {
    // Redirect to the login page or perform other actions
    alert(message);
});
connection.on("ExceptionHandler", (message) => {
    alert(message);
})
connection.on("RedirectToHome", () => {
    window.location.href = "/";
})
connection.on("RoomUpdated", () => {
    window.location.href = "/Room";
});
connection.on("CreateRoomFailed", (message) => {
    alert(message);
})
connection.on("JoinRoomFailed", (message) => {
    alert(message);
});
