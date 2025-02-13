

const connection = new signalR.HubConnectionBuilder() //  Creates a new SignalR client connection.
    .withUrl("http://localhost:5235/chathub") // Specifies the URL of the SignalR hub
    .configureLogging(signalR.LogLevel.Information)
    .build()

async function start() {
    try {
        await connection.start();
        console.log("signalR connected");
    } catch (err) {
        console.log(err);
        //setTimeout(start, 5000); // Waits 5 seconds before retrying
    }
}

connection.onclose(async () => {
    //await start(); // If the connection closes unexpectedly retry
})

start();


// add html now

document.getElementById("send-button").addEventListener("click", async () => {
    const user = document.getElementById("user-input").value;
    const message = document.getElementById("message-input").value;

    try {
        await connection.invoke("SendMessage", user, message);
    } catch (err) {
        console.error(err);
    }
}); 

// above should give warning
// Warning: No client method with the name 'ReceiveMessage' founds.
// see ChatHub.cs
// we tell the js signalR connection object to call the ReceiveMessage function
// Clients.All.SendAsync("ReceiveMessage", user, message)


function addMessageToList(user, message) {
    const li = document.createElement("li");
    li.textContent = `${user}: ${message}`;
    document.getElementById("message-list").appendChild(li);
}

connection.on("ReceiveMessage", (user, message) => { addMessageToList(user, message) });

// open two copies of the live server and show message from one goes to the other