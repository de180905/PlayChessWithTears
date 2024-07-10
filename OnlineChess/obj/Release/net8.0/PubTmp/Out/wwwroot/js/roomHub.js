//establish connection
var connection = new signalR.HubConnectionBuilder().withUrl("/hubs/chessHub").build();
function fullfilled() {
    console.log("hello 0");
    loadMatch();
    console.log("hello1");
}
function failed() {

}
connection.start().then(fullfilled, failed);

//define functions
function startGame() {
    console.log("startgame");
    connection.send("StartGame");
}
function leaveRoom() {
    connection.send("LeaveRoom");
}
function kick() {
    connection.send("KickRoommate");
}
function loadMatch() {
    connection.send("SendMatch");
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
connection.on("GameStarted", (jsonMatch, playerInfo) => {
    PlayerInfo = playerInfo;
    match = JSON.parse(jsonMatch);
    const board = match.ChessBoard;
    console.log(match);
    renderBoard(board);
    initializeTimer();
    updateTimers();
});
connection.on("StartGameFailed", (message) => {
    console.log(message);
})
connection.on("ShowNextMoves", (nextMoves) => {
    const board = match.ChessBoard;
    renderBoard(board);
    const curPos = nextMoves[0].fromPos;
    const selectedPiece = board.pieces[curPos.x][curPos.y];
    const id = "cell" + curPos.x + curPos.y;
    document.getElementById(id).style.backgroundColor = 'yellow';
    nextMoves.forEach((move) => {
        console.log(move);
        const x = move.toPos.x;
        const y = move.toPos.y;
        const id = "cell" + x + y;
        document.getElementById(id).style.backgroundColor = 'yellow';
    });

})
connection.on("MakeMoveSuccessfully", (jsonMatch) => {
    match = JSON.parse(jsonMatch);
    const board = match.ChessBoard;
    console.log(match);
    renderBoard(board);
    updateTimers();
})
connection.on("GameOver", (resultMessage) => {
    // Update modal content with game result
    document.getElementById("gameResult").textContent = resultMessage;

    // Display modal pop-up window
    $("#myModal").modal("show");

    // Disable interactions with original page
    $("body").addClass("modal-open"); // Prevent scrolling
})