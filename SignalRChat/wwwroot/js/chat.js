
let connection = null;

export function setupConnection(token) {
  console.log(`connection token ${token}`)
  connection = new signalR.HubConnectionBuilder()
  .withUrl(`http://localhost:5235/chathub?access_token=${encodeURIComponent(token)}`)
    .configureLogging(signalR.LogLevel.Information)
    .build();

  connection.on("ReceiveMessage", (user, message) => { addMessageToList(user, message) });

  connection.onclose(async () => {
     await startConnection(); // If the connection closes unexpectedly retry
  })

}

export async function startConnection() {
    if (!connection) {
        throw new Error("Connection is not set up. Call setupConnection() first.");
    }
    try {
        await connection.start();
        console.log("signalR connected");
    } catch (err) {
        console.log(err);
        setTimeout(startConnection, 5000); // Waits 5 seconds before retrying
    }
}


document.getElementById("send-button").addEventListener("click", async () => {
    const user = document.getElementById("user-input").value;
    const message = document.getElementById("message-input").value;

    try {
        await connection.invoke("SendMessage", user, message);
    } catch (err) {
        console.error(err);
    }
}); 

function addMessageToList(user, message) {
    const li = document.createElement("li");
    li.textContent = `${user}: ${message}`;
    document.getElementById("message-list").appendChild(li);
}
