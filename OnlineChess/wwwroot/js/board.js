class CountdownTimer {
    constructor(duration, callback, timerDisplay) {
        this.duration = duration;
        this.callback = callback;
        this.intervalId = null;
        this.isRunning = false;
        this.remainingTime = duration;
        this.timerDisplay = document.getElementById(timerDisplay);
        this.renderTime();
    }

    start() {
        if (!this.isRunning) {
            this.isRunning = true;
            this.intervalId = setInterval(() => {
                this.remainingTime--;

                if (this.remainingTime <= 0) {
                    this.stop();
                    if (typeof this.callback === 'function') {
                        this.callback();
                    }
                }

                this.renderTime();
            }, 1000);
        }
    }

    stop() {
        clearInterval(this.intervalId);
        this.isRunning = false;
    }

    resume() {
        if (!this.isRunning && this.remainingTime > 0) {
            this.isRunning = true;
            this.intervalId = setInterval(() => {
                this.remainingTime--;

                if (this.remainingTime <= 0) {
                    this.stop();
                    if (typeof this.callback === 'function') {
                        this.callback();
                    }
                }

                this.renderTime();
            }, 1000);
        }
    }

    renderTime() {
        const minutes = Math.floor(this.remainingTime / 60);
        const seconds = this.remainingTime % 60;
        const formattedTime = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
        this.timerDisplay.textContent = formattedTime;
    }
}
let match;
let PlayerInfo = {};
let timer1;
let timer2;
function clearBoard() {
    var board = document.getElementById("chessBoard");

    // Loop until there are no more child elements
    while (board.firstChild) {
        // Remove the first child element
        board.removeChild(board.firstChild);
    }
    return board;
}
function renderBoard(model) {
    clearBoard();
    document.getElementById("chessSection").style.display = "flex";
    // Access the board and fill it with pieces
    const p = document.getElementById("turn");
    p.style.display = "block";
    p.querySelector("span").innerText = match.Turn == 0? "White" : "Black";
    const board = document.getElementById("chessBoard");
    let content;
    const pieces = model.pieces;
    // Loop through the board and create cells
    let start;
    let step;
    if (PlayerInfo.color == 0) {
        start = 0;
        step = 1;
    } else {
        start = 7;
        step = -1;
    }
    for (let i = start; i < 8 && i > -1; i += step) {
        for (let j = start; j < 8 && j > -1; j += step) {
            let piece = pieces[i][j];
            let cell = document.createElement('div');
            cell.classList.add('cell');
            cell.id = 'cell' + i + j;
            if (piece == null) {
                cell.innerHTML = "";
            }
            else {
                content = document.createElement('img');
                imgSrc = piece.Color.toString() + piece.Type.toString();
                content.src = '/images/pieces/' + imgSrc + '.png';
                content.style.width = "85%"
                cell.appendChild(content);
            }
            cell.pos = {
                x: i,
                y: j
            };
            cell.addEventListener('click', handleCellClick);
            // Set the background color of the cell based on its position
            if ((i + j) % 2 === 0) {
                cell.style.backgroundColor = 'brown';
            } else {
                cell.style.backgroundColor = 'green';
            }

            // Append the cell to the board
            board.appendChild(cell);
          
        }
    }
}

function handleCellClick() {
    connection.send("SelectCell", this.pos);
}

function updateTimers() {
    seconds1 = match.Player1.Timer.TimeRemaining;
    seconds2 = match.Player2.Timer.TimeRemaining;
    timer1.remainingTime = convertToSeconds(seconds1);
    timer2.remainingTime = convertToSeconds(seconds2);
    if (match.Player2.Color == match.Turn) {
        timer1.stop();
        timer2.resume();
    } else {
        timer2.stop();
        timer1.resume();
    }
}
function convertToSeconds(timeString) {
    // Split the time string into hours, minutes, and seconds
    var timeParts = timeString.split(':');

    // Convert each part to integer
    var hours = parseInt(timeParts[0], 10);
    var minutes = parseInt(timeParts[1], 10);
    var seconds = parseInt(timeParts[2], 10);

    // Calculate total seconds
    var totalSeconds = hours * 3600 + minutes * 60 + seconds;

    return totalSeconds;
}

function initializeTimer() {
    seconds1 = match.Player1.Timer.TimeRemaining;
    seconds2 = match.Player2.Timer.TimeRemaining;
    timer1 = new CountdownTimer(convertToSeconds(seconds1), () => { }, "timer1");
    timer2 = new CountdownTimer(convertToSeconds(seconds2), () => { }, "timer2");
}
