import { login } from './login.js';
import { setupConnection, startConnection } from './chat.js';


async function main() {
    try {
      const email = "test@test.com";
      const password = "123_ABC_xyz";
  
      // Step 1: Login and get the token
      const token = await login(email, password);
  
      if (!token) {
        throw new Error("Failed to retrieve token");
      }
  
      // Step 2: Set up the SignalR connection with the token
      setupConnection(token);
  
      // Step 3: Start the connection
      await startConnection();
  
      console.log("Ready to chat!");
    } catch (error) {
      console.error("Initialization failed:", error);
    }
  }
  
  main();