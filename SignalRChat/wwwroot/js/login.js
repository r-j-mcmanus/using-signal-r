
const login_url = "http://localhost:5235/login"

export async function login(email, password) {
    try {
      const response = await fetch(login_url,
        {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify({   
                "email": email,
                "password": password
            }),
        }
      );
      if (!response.ok) {
        throw new Error(`Response status: ${response.status}`);
      }
  
      const json = await response.json();
      console.log(json);
      return json["accessToken"];
    } catch (error) {
      console.error(error.message);
    }
}
